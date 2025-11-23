using System.Buffers;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser;

// https://github.com/scandum/fluxsort/blob/main/LICENSE
// UNLICENSE
/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/


#pragma warning disable CA1000 // static on generic type

public static unsafe class StableSort<T>
{
    // This Selector exists to be able to clear the pool after sorting large arrays of reference types, since the pooled buffers would keep the objects alive otherwise.
    //NOTE(Rennorb): This custom pool impl might not be worth it;
    // This could be replaced with a bool = true for reference types that is then passed and stored into PoolReturners where it would clear all arrays when returning them to the default pool.
    // In measuring that approach i found it to be marginally slower than the external clearing i implemented.
    // The current version is still a bit "better", but i was surprised with how close the two options are performance wise.
    // Changing this to just use the boolean would get rid of the whole ClearableSharedArrayPool implementation since it only exists for the sorting of large event (AbstractBuffEvent) arrays.
    public static readonly ArrayPool<T> Pool = typeof(T).IsValueType ? ArrayPool<T>.Shared : ClearableSharedArrayPool<T>.Shared;

    // quadsort 1.2.1.3 - Igor van den Hoven ivdhoven@gmail.com


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int branchless_swap(Span<T> array, int offset, Func<T, T, int> cmp)
    {
        var x = offset;
        var swap = cmp(array[offset], array[offset + 1]) > 0 ? array[x++] : array[offset + 1];
        array[offset] = array[x];
        array[offset + 1] = swap;
        return x - offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void head_branchless_merge(Span<T> dest, ref int ptd, Span<T> froml, ref int ptl, Span<T> fromr, ref int ptr, Func<T, T, int> cmp)
    {
        dest[ptd++] = cmp(froml[ptl], fromr[ptr]) <= 0 ? froml[ptl++] : fromr[ptr++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void tail_branchless_merge(Span<T> dest, ref int tpd, Span<T> froml, ref int tpl, Span<T> fromr, ref int tpr, Func<T, T, int> cmp)
    {
        dest[tpd--] = cmp(froml[tpl], fromr[tpr]) > 0 ? froml[tpl--] : fromr[tpr--];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void parity_merge_two(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        int ptl = 0, ptr = 2, // array
            pts = 0; // swap
        head_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        swap[pts] = cmp(array[ptl], array[ptr]) <= 0 ? array[ptl] : array[ptr];
        
        ptl = 1; ptr = 3; pts = 3;
        tail_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        swap[pts] = cmp(array[ptl], array[ptr]) > 0 ? array[ptl] : array[ptr];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void parity_merge_four(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        int ptl = 0, ptr = 4, //array
            pts = 0; // swap
        head_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        head_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        head_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        swap[pts] = cmp(array[ptl], array[ptr]) <= 0 ? array[ptl] : array[ptr];
        
        ptl = 3; ptr = 7; pts = 7;
        tail_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        tail_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        tail_branchless_merge(swap, ref pts, array, ref ptl, array, ref ptr, cmp);
        swap[pts] = cmp(array[ptl], array[ptr]) > 0 ? array[ptl] : array[ptr];
    }
       

    // the next seven functions are used for sorting 0 to 31 elements

    static void parity_swap_four(Span<T> array, Func<T, T, int> cmp)
    {
        T tmp;

        branchless_swap(array, 0, cmp);
        branchless_swap(array, 2, cmp);

        if (cmp(array[1], array[2]) > 0)
        {
            tmp = array[1]; array[1] = array[2]; array[2] = tmp;

            branchless_swap(array, 0, cmp);
            branchless_swap(array, 2, cmp);
            branchless_swap(array, 1, cmp);
        }
    }

    static void parity_swap_five(Span<T> array, Func<T, T, int> cmp)
    {
        branchless_swap(array, 0, cmp);
        branchless_swap(array, 2, cmp);
        var x = branchless_swap(array, 1, cmp);
        var y = branchless_swap(array, 3, cmp);

        if (x + y != 0)
        {
            branchless_swap(array, 0, cmp);
            branchless_swap(array, 2, cmp);
            branchless_swap(array, 1, cmp);
            branchless_swap(array, 3, cmp);
            branchless_swap(array, 0, cmp);
            branchless_swap(array, 2, cmp);
        }
    }

    static void parity_swap_six(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
      int pta = 0, ptl, ptr;
      int x;

      branchless_swap(array, pta, cmp); pta++;
      branchless_swap(array, pta, cmp); pta += 3;
      branchless_swap(array, pta, cmp); pta--;
      branchless_swap(array, pta, cmp); pta = 0;

      if (cmp(array[pta + 2], array[pta + 3]) <= 0)
      {
        branchless_swap(array, pta, cmp); pta += 4;
        branchless_swap(array, pta, cmp);
        return;
      }

      x = cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; swap[0] = array[pta + x]; swap[1] = array[pta + (x == 0 ? 1 : 0)]; swap[2] = array[pta + 2]; pta += 4;
      x = cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; swap[4] = array[pta + x]; swap[5] = array[pta + (x == 0 ? 1 : 0)]; swap[3] = array[pta - 1];

      pta = 0; ptl = 0; ptr = 3;

      head_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
      head_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
      head_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);

      pta = 5; ptl = 2; ptr = 5;

      tail_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
      tail_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
      array[pta] = cmp(swap[ptl], swap[ptr]) > 0 ? swap[ptl] : swap[ptr];
    }

    static void parity_swap_seven(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        int pta = 0, ptl, ptr;
        int x, y;

        branchless_swap(array, pta, cmp); pta += 2;
        branchless_swap(array, pta, cmp); pta += 2;
        branchless_swap(array, pta, cmp); pta -= 3;
        y = branchless_swap(array, pta, cmp); pta += 2;
        y += branchless_swap(array, pta, cmp); pta += 2;
        y += branchless_swap(array, pta, cmp); pta -= 1;

        if (y == 0) { return; }

        branchless_swap(array, pta, cmp); pta = 0;

        x = cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; swap[0] = array[pta + x]; swap[1] = array[pta + (x == 0 ? 1 : 0)]; swap[2] = array[pta + 2]; pta += 3;
        x = cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; swap[3] = array[pta + x]; swap[4] = array[pta + (x == 0 ? 1 : 0)]; pta += 2;
        x = cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; swap[5] = array[pta + x]; swap[6] = array[pta + (x == 0 ? 1 : 0)];

        pta = 0; ptl = 0; ptr = 3;

        head_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
        head_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
        head_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);

        pta = 6; ptl = 2; ptr = 6;

        tail_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
        tail_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
        tail_branchless_merge(array, ref pta, swap, ref ptl, swap, ref ptr, cmp);
        array[pta] = cmp(swap[ptl], swap[ptr]) > 0 ? swap[ptl] : swap[ptr];
    }

    static void tiny_sort(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
      switch (array.Length)
      {
        case 0:
        case 1:
          return;
            case 2:
                branchless_swap(array, 0, cmp);
                return;
            case 3:
                branchless_swap(array, 0, cmp);
                branchless_swap(array, 1, cmp);
                branchless_swap(array, 0, cmp);
                return;
        case 4:
          parity_swap_four(array, cmp);
          return;
        case 5:
          parity_swap_five(array, cmp);
          return;
        case 6:
          parity_swap_six(array, swap, cmp);
                return;
        case 7:
          parity_swap_seven(array, swap, cmp);
          return;
      }
    }

    // left must be equal or one smaller than right

    static void parity_merge(Span<T> dest, Span<T> from, int left, int right, Func<T, T, int> cmp)
    {
        int ptl, ptr, tpl, tpr, tpd, ptd;
        ptl = 0; // from
        ptr = left; // from
        tpl = ptr - 1;
        tpr = tpl + right;

        ptd = 0; // dest
        tpd = left + right - 1; // dest

        if (left < right)
        {
            dest[ptd++] = cmp(from[ptl], from[ptr]) <= 0 ? from[ptl++] : from[ptr++];
        }
        dest[ptd++] = cmp(from[ptl], from[ptr]) <= 0 ? from[ptl++] : from[ptr++];

        {
            while (--left != 0)
            {
                head_branchless_merge(dest, ref ptd, from, ref ptl, from, ref ptr, cmp);
                tail_branchless_merge(dest, ref tpd, from, ref tpl, from, ref tpr, cmp);
            }
        }
        dest[tpd] = cmp(from[tpl], from[tpr]) > 0 ? from[tpl] : from[tpr];
    }

    static void tail_swap(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        var nmemb = array.Length;
        if (nmemb < 8)
        {
            tiny_sort(array, swap, cmp);
            return;
        }
        int quad1, quad2, quad3, quad4, half1, half2;

        half1 = nmemb / 2;
        quad1 = half1 / 2;
        quad2 = half1 - quad1;
        half2 = nmemb - half1;
        quad3 = half2 / 2;
        quad4 = half2 - quad3;

        int pta = 0;

        tail_swap(array[..quad1], swap, cmp); pta += quad1;
        tail_swap(array.Slice(pta, quad2), swap, cmp); pta += quad2;
        tail_swap(array.Slice(pta, quad3), swap, cmp); pta += quad3;
        tail_swap(array.Slice(pta, quad4), swap, cmp);
        if (cmp(array[quad1 - 1], array[quad1]) <= 0 && cmp(array[half1 - 1], array[half1]) <= 0 && cmp(array[pta - 1], array[pta]) <= 0)
        {
            return;
        }
        parity_merge(swap, array, quad1, quad2, cmp);
        parity_merge(swap[half1..], array[half1..], quad3, quad4, cmp);
        parity_merge(array, swap, half1, half2, cmp);
    }

    // the next three functions create sorted blocks of 32 elements

    static void quad_reversal(Span<T> array, int pta, int ptz)
    {
        int ptb, pty;
        T tmp1, tmp2;

        int loop = (ptz - pta) / 2;

        ptb = loop;
        pty = ptz - loop;

        if (loop % 2 == 0)
        {
            tmp2 = array[ptb]; array[ptb--] = array[pty]; array[pty++] = tmp2; loop--;
        }

        loop /= 2;

        do
        {
            tmp1 = array[pta]; array[pta++] = array[ptz]; array[ptz--] = tmp1;
            tmp2 = array[ptb]; array[ptb--] = array[pty]; array[pty++] = tmp2;
        }
        while (loop-- != 0);
    }

    static void quad_swap_merge(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        parity_merge_two(array, swap, cmp);
        parity_merge_two(array[4..], swap[4..], cmp);
        parity_merge_four(swap, array, cmp);
    }

    static int quad_swap(Span<T> array, Func<T, T, int> cmp)
    {
        using var swap = new ArrayPoolReturner<T>(32, StableSort<T>.Pool);
        T tmp;
        int count, nmemb = array.Length;
        int pta, pts;
        int v1, v2, v3, v4;
        pta = 0;

        count = nmemb / 8;

        while (count-- != 0)
        {
            v1 = cmp(array[pta    ], array[pta + 1]) > 0 ? 1 : 0;
            v2 = cmp(array[pta + 2], array[pta + 3]) > 0 ? 1 : 0;
            v3 = cmp(array[pta + 4], array[pta + 5]) > 0 ? 1 : 0;
            v4 = cmp(array[pta + 6], array[pta + 7]) > 0 ? 1 : 0;

            switch (v1 + v2 * 2 + v3 * 4 + v4 * 8)
            {
                case 0:
                    if (cmp(array[pta + 1], array[pta + 2]) <= 0 && cmp(array[pta + 3], array[pta + 4]) <= 0 && cmp(array[pta + 5], array[pta + 6]) <= 0)
                    {
                        goto ordered;
                    }
                    quad_swap_merge(array[pta..], swap, cmp);
                    break;

                case 15:
                    if (cmp(array[pta + 1], array[pta + 2]) > 0 && cmp(array[pta + 3], array[pta + 4]) > 0 && cmp(array[pta + 5], array[pta + 6]) > 0)
                    {
                        pts = pta;
                        goto reversed;
                    }
                    goto default;

                default:
                //not_ordered:
                    tmp = array[pta + (v1 == 0 ? 1 : 0)]; array[pta] = array[pta + v1]; array[pta + 1] = tmp; pta += 2;
                    tmp = array[pta + (v2 == 0 ? 1 : 0)]; array[pta] = array[pta + v2]; array[pta + 1] = tmp; pta += 2;
                    tmp = array[pta + (v3 == 0 ? 1 : 0)]; array[pta] = array[pta + v3]; array[pta + 1] = tmp; pta += 2;
                    tmp = array[pta + (v4 == 0 ? 1 : 0)]; array[pta] = array[pta + v4]; array[pta + 1] = tmp; pta -= 6;

                    quad_swap_merge(array[pta..], swap, cmp);
                    break;
            }
            pta += 8;

            continue;

            ordered:

            pta += 8;

            if (count-- != 0)
            {
                v1 = cmp(array[pta    ], array[pta + 1]) > 0 ? 1 : 0;
                v2 = cmp(array[pta + 2], array[pta + 3]) > 0 ? 1 : 0;
                v3 = cmp(array[pta + 4], array[pta + 5]) > 0 ? 1 : 0;
                v4 = cmp(array[pta + 6], array[pta + 7]) > 0 ? 1 : 0;

                if ((v1 | v2 | v3 | v4) != 0)
                {
                    if (v1 + v2 + v3 + v4 == 4 && cmp(array[pta + 1], array[pta + 2]) > 0 && cmp(array[pta + 3], array[pta + 4]) > 0 && cmp(array[pta + 5], array[pta + 6]) > 0)
                    {
                        pts = pta;
                        goto reversed;
                    }


                    //goto not_ordered;
                    {
                        tmp = array[pta + (v1 == 0 ? 1 : 0)]; array[pta] = array[pta + v1]; array[pta + 1] = tmp; pta += 2;
                        tmp = array[pta + (v2 == 0 ? 1 : 0)]; array[pta] = array[pta + v2]; array[pta + 1] = tmp; pta += 2;
                        tmp = array[pta + (v3 == 0 ? 1 : 0)]; array[pta] = array[pta + v3]; array[pta + 1] = tmp; pta += 2;
                        tmp = array[pta + (v4 == 0 ? 1 : 0)]; array[pta] = array[pta + v4]; array[pta + 1] = tmp; pta -= 6;

                        quad_swap_merge(array[pta..], swap, cmp);
                        pta += 8;

                        continue;
                    }
                }
                if (cmp(array[pta + 1], array[pta + 2]) <= 0 && cmp(array[pta + 3], array[pta + 4]) <= 0 && cmp(array[pta + 5], array[pta + 6]) <= 0)
                {
                    goto ordered;
                }
                quad_swap_merge(array[pta..], swap, cmp);
                pta += 8;
                continue;
            }
            break;

            reversed:

            pta += 8;

            if (count-- != 0)
            {
                v1 = cmp(array[pta    ], array[pta + 1]) <= 0 ? 1 : 0;
                v2 = cmp(array[pta + 2], array[pta + 3]) <= 0 ? 1 : 0;
                v3 = cmp(array[pta + 4], array[pta + 5]) <= 0 ? 1 : 0;
                v4 = cmp(array[pta + 6], array[pta + 7]) <= 0 ? 1 : 0;
                if ((v1 | v2 | v3 | v4) != 0)
                {
                    // not reversed
                }
                else
                {
                    if (cmp(array[pta - 1], array[pta]) > 0 && cmp(array[pta + 1], array[pta + 2]) > 0 && cmp(array[pta + 3], array[pta + 4]) > 0 && cmp(array[pta + 5], array[pta + 6]) > 0)
                    {
                        goto reversed;
                    }
                }
                quad_reversal(array, pts, pta - 1);

                if (v1 + v2 + v3 + v4 == 4 && cmp(array[pta + 1], array[pta + 2]) <= 0 && cmp(array[pta + 3], array[pta + 4]) <= 0 && cmp(array[pta + 5], array[pta + 6]) <= 0)
                {
                    goto ordered;
                }
                if (v1 + v2 + v3 + v4 == 0 && cmp(array[pta + 1], array[pta + 2])  > 0 && cmp(array[pta + 3], array[pta + 4])  > 0 && cmp(array[pta + 5], array[pta + 6])  > 0)
                {
                    pts = pta;
                    goto reversed;
                }

                tmp = array[pta + v1]; array[pta] = array[pta + (v1 == 0 ? 1 : 0)]; array[pta + 1] = tmp; pta += 2;
                tmp = array[pta + v2]; array[pta] = array[pta + (v2 == 0 ? 1 : 0)]; array[pta + 1] = tmp; pta += 2;
                tmp = array[pta + v3]; array[pta] = array[pta + (v3 == 0 ? 1 : 0)]; array[pta + 1] = tmp; pta += 2;
                tmp = array[pta + v4]; array[pta] = array[pta + (v4 == 0 ? 1 : 0)]; array[pta + 1] = tmp; pta -= 6;

                if (cmp(array[pta + 1], array[pta + 2]) > 0 || cmp(array[pta + 3], array[pta + 4]) > 0 || cmp(array[pta + 5], array[pta + 6]) > 0)
                {
                    quad_swap_merge(array[pta..], swap, cmp);
                }
                pta += 8;
                continue;
            }

            switch (nmemb % 8)
            {
              case 7: if (cmp(array[pta + 5], array[pta + 6]) <= 0) { break; }  goto case 6;
              case 6: if (cmp(array[pta + 4], array[pta + 5]) <= 0) { break; }  goto case 5;
              case 5: if (cmp(array[pta + 3], array[pta + 4]) <= 0) { break; }  goto case 4;
              case 4: if (cmp(array[pta + 2], array[pta + 3]) <= 0) { break; }  goto case 3;
              case 3: if (cmp(array[pta + 1], array[pta + 2]) <= 0) { break; }  goto case 2;
              case 2: if (cmp(array[pta    ], array[pta + 1]) <= 0) { break; }  goto case 1;
              case 1: if (cmp(array[pta - 1], array[pta    ]) <= 0) { break; }  goto case 0;
              case 0:
                quad_reversal(array, pts, pta + nmemb % 8 - 1);

                if (pts == 0)
                {
                    return 1;
                }
                goto reverse_end;
            }
            quad_reversal(array, pts, pta - 1);
            break;
        }
        tail_swap(array.Slice(pta, nmemb % 8), swap, cmp);

        reverse_end:

        pta = 0;

        for (count = nmemb / 32 ; count-- != 0; pta += 32)
        {
            if (cmp(array[pta + 7], array[pta + 8]) <= 0 && cmp(array[pta + 15], array[pta + 16]) <= 0 && cmp(array[pta + 23], array[pta + 24]) <= 0)
            {
                continue;
            }
            parity_merge(swap, array[pta..], 8, 8, cmp);
            parity_merge(swap[16..], array[(pta + 16)..], 8, 8, cmp);
            parity_merge(array[pta..], swap, 16, 16, cmp);
        }

        if (nmemb % 32 > 8)
        {
            tail_merge(array[pta..], swap[..32], nmemb % 32, 8, cmp);
        }
        return 0;
    }

    // The next six functions are quad merge support routines

    static void cross_merge(Span<T> dest, int ptd, Span<T> from, int ptl, int left, int right, Func<T, T, int> cmp)
    {
        int tpl, ptr, tpr, //from 
            tpd; //dest
        int loop;
        //ptl = from;
        ptr = ptl + left;
        tpl = ptr - 1;
        tpr = tpl + right;

        if (left + 1 >= right && right >= left && left >= 32)
        {
            if (cmp(from[ptl + 15], from[ptr]) > 0 && cmp(from[ptl], from[ptr + 15]) <= 0 && cmp(from[tpl], from[tpr - 15]) > 0 && cmp(from[tpl - 15], from[tpr]) <= 0)
            {
                parity_merge(dest[ptd..], from[ptl..], left, right, cmp);
                return;
            }
        }
        //ptd = dest;
        tpd = ptd + left + right - 1;

        while (true)
        {
            if (tpl - ptl > 8)
            {
                ptl8_ptr: if (cmp(from[ptl + 7], from[ptr]) <= 0)
                {
                    from.Slice(ptl, 8).CopyTo(dest[ptd..]);
                    ptd += 8; ptl += 8;

                    if (tpl - ptl > 8) { goto ptl8_ptr; }
                    continue;
                }

                tpl8_tpr: if (cmp(from[tpl - 7], from[tpr]) > 0)
                {
                    tpd -= 7; tpl -= 7;
                    from.Slice(tpl--, 8).CopyTo(dest[(tpd--)..]);

                    if (tpl - ptl > 8) { goto tpl8_tpr; }
                    continue;
                }
            }

            if (tpr - ptr > 8)
            {
                ptl_ptr8: if (cmp(from[ptl], from[ptr + 7]) > 0)
                {
                    from.Slice(ptr, 8).CopyTo(dest[ptd..]);
                    ptd += 8; ptr += 8;

                    if (tpr - ptr > 8) { goto ptl_ptr8; }
                    continue;
                }

                tpl_tpr8: if (cmp(from[tpl], from[tpr - 7]) <= 0)
                {
                    tpd -= 7; tpr -= 7;
                    from.Slice(tpr--, 8).CopyTo(dest[(tpd--)..]);

                    if (tpr - ptr > 8) { goto tpl_tpr8; }
                    continue;
                }
            }

            if (tpd - ptd < 16)
            {
                break;
            }

            {
                loop = 8;
                do
                {
                    head_branchless_merge(dest, ref ptd, from, ref ptl, from, ref ptr, cmp);
                    tail_branchless_merge(dest, ref tpd, from, ref tpl, from, ref tpr, cmp);
                }
                while (--loop != 0);
            }
        }

        while (ptl <= tpl && ptr <= tpr)
        {
            dest[ptd++] = cmp(from[ptl], from[ptr]) <= 0 ? from[ptl++] : from[ptr++];
        }
        while (ptl <= tpl)
        {
            dest[ptd++] = from[ptl++];
        }
        while (ptr <= tpr)
        {
            dest[ptd++] = from[ptr++];
        }
    }

    static void quad_merge_block(Span<T> array, Span<T> swap, int block, Func<T, T, int> cmp)
    {
        int pt1, pt2, pt3;
        int block_x_2 = block * 2;

        pt1 = block;
        pt2 = pt1 + block;
        pt3 = pt2 + block;

        switch ((cmp(array[pt1 -1], array[pt1]) <= 0 ? 1 : 0) | (cmp(array[pt3 - 1], array[pt3]) <= 0 ? 2 : 0))
        {
            case 0:
                cross_merge(swap, 0, array, 0, block, block, cmp);
                cross_merge(swap, block_x_2, array, pt2, block, block, cmp);
                break;
            case 1:
                array[..block_x_2].CopyTo(swap);
                cross_merge(swap, block_x_2, array, pt2, block, block, cmp);
                break;
            case 2:
                cross_merge(swap, 0, array, 0, block, block, cmp);
                array.Slice(pt2, block_x_2).CopyTo(swap[block_x_2..]);
                break;
            case 3:
                if (cmp(array[pt2 - 1], array[pt2]) <= 0) { return; }
                array[..(block_x_2 * 2)].CopyTo(swap);
                break;
        }
        cross_merge(array, 0, swap, 0, block_x_2, block_x_2, cmp);
    }

    static int quad_merge(Span<T> array, Span<T> swap, int block, Func<T, T, int> cmp)
    {
        int pta, pte, swap_size = swap.Length, nmemb = array.Length;
        pte = nmemb;
        block *= 4;

        while (block <= nmemb && block <= swap_size)
        {
            pta = 0;

            do
            {
                quad_merge_block(array[pta..], swap, block / 4, cmp);
                pta += block;
            }
            while (pta + block <= pte);

            tail_merge(array[pta..], swap, pte - pta, block / 4, cmp);
            block *= 4;
        }

        tail_merge(array, swap, nmemb, block / 4, cmp);

        return block / 2;
    }

    static void partial_forward_merge(Span<T> array, Span<T> swap, int swap_size, int nmemb, int block, Func<T, T, int> cmp)
    {
        int ptl, tpl, //swap
            pta, ptr, tpr; //array
        int x;

        if (nmemb == block)
        {
            return;
        }

        pta = 0;
        ptr = block;
        tpr = nmemb - 1;

        if (cmp(array[ptr - 1], array[ptr]) <= 0)
        {
            return;
        }
        array[..block].CopyTo(swap);

        ptl = 0;
        tpl = block - 1;
        while (ptl < tpl - 1 && ptr < tpr - 1)
        {
            ptr2: if (cmp(swap[ptl], array[ptr + 1]) > 0)
            {
                array[pta++] = array[ptr++]; array[pta++] = array[ptr++];

                if (ptr < tpr - 1) { goto ptr2; }
                break;
            }
            if (cmp(swap[ptl + 1], array[ptr]) <= 0)
            {
                array[pta++] = swap[ptl++]; array[pta++] = swap[ptl++];
                if (ptl < tpl - 1) { goto ptl2; }
                break;
            }
            goto cross_swap;

            ptl2: if (cmp(swap[ptl + 1], array[ptr]) <= 0)
            {
                array[pta++] = swap[ptl++]; array[pta++] = swap[ptl++];

                if (ptl < tpl - 1) { goto ptl2; }
                break;
            }

            if (cmp(swap[ptl], array[ptr + 1]) > 0)
            {
                array[pta++] = array[ptr++]; array[pta++] = array[ptr++];

                if (ptr < tpr - 1) { goto ptr2; }
                break;
            }

            cross_swap:

            x = cmp(swap[ptl], array[ptr]) <= 0 ? 1 : 0; array[pta + x] = array[ptr++]; array[pta + (x == 0 ? 1 : 0)] = swap[ptl++]; pta += 2;
            head_branchless_merge(array, ref pta, swap, ref ptl, array, ref ptr, cmp);
        }

        while (ptl <= tpl && ptr <= tpr)
        {
            array[pta++] = cmp(swap[ptl], array[ptr]) <= 0 ? swap[ptl++] : array[ptr++];
        }

        while (ptl <= tpl)
        {
            array[pta++] = swap[ptl++];
        }
    }

    static void partial_backward_merge(Span<T> array, Span<T> swap, int nmemb, int block, Func<T, T, int> cmp)
    {
        int tpl, tpa, // array
            tpr; // swap
        int right, loop, x;
        if (nmemb == block)
        {
            return;
        }

        tpl = block - 1;
        tpa = nmemb - 1;

        if (cmp(array[tpl], array[tpl + 1]) <= 0)
        {
            return;
        }

        right = nmemb - block;

        if (nmemb <= swap.Length && right >= 64)
        {
            cross_merge(swap, 0, array, 0, block, right, cmp);

            swap[..nmemb].CopyTo(array);

            return;
        }

        array.Slice(block, right).CopyTo(swap);

        tpr = right - 1; // swap
        while (tpl > 16 && tpr > 16)
        {
            tpl_tpr16: if (cmp(array[tpl], swap[tpr - 15]) <= 0)
            {
                loop = 16; do { array[tpa--] = swap[tpr--]; } while (--loop != 0);

                if (tpr > 16) { goto tpl_tpr16; } break;
            }

            tpl16_tpr: if (cmp(array[tpl - 15], swap[tpr]) > 0)
            {
                loop = 16; do { array[tpa--] = array[tpl--]; } while (--loop != 0);
      
                if (tpl > 16) { goto tpl16_tpr; } break;
            }

            loop = 8;
            do
            {
                if (cmp(array[tpl], swap[tpr - 1]) <= 0)
                {
                    array[tpa--] = swap[tpr--]; array[tpa--] = swap[tpr--];
                }
                else if (cmp(array[tpl - 1], swap[tpr]) > 0)
                {
                    array[tpa--] = array[tpl--]; array[tpa--] = array[tpl--];
                }
                else
                {
                    x = cmp(array[tpl], swap[tpr]) <= 0 ? 1 : 0; tpa--; array[tpa + x] = swap[tpr--]; array[tpa + (x == 0 ? 1 : 0)] = array[tpl--]; tpa--;
                    tail_branchless_merge(array, ref tpa, array, ref tpl, swap, ref tpr, cmp);
                }
            }
            while (--loop != 0);
        }

        while (tpr > 1 && tpl > 1)
        {
            tpr2: if (cmp(array[tpl], swap[tpr - 1]) <= 0)
            {
                array[tpa--] = swap[tpr--]; array[tpa--] = swap[tpr--];
      
                if (tpr > 1) { goto tpr2; }
                break;
            }

            if (cmp(array[tpl - 1], swap[tpr]) > 0)
            {
                array[tpa--] = array[tpl--]; array[tpa--] = array[tpl--];

                if (tpl > 1) { goto tpl2; }
                break;
            }
            goto cross_swap;

            tpl2: if (cmp(array[tpl - 1], swap[tpr]) > 0)
            {
                array[tpa--] = array[tpl--]; array[tpa--] = array[tpl--];

                if (tpl > 1) { goto tpl2; }
                break;
            }

            if (cmp(array[tpl], swap[tpr - 1]) <= 0)
            {
                array[tpa--] = swap[tpr--]; array[tpa--] = swap[tpr--];
      
                if (tpr > 1) { goto tpr2; }
                break;
            }
        
            cross_swap:
            x = cmp(array[tpl], swap[tpr]) <= 0 ? 1 : 0; tpa--; array[tpa + x] = swap[tpr--]; array[tpa + (x == 0 ? 1 : 0)] = array[tpl--]; tpa--;
            tail_branchless_merge(array, ref tpa, array, ref tpl, swap, ref tpr, cmp);
        }

        while (tpr >= 0 && tpl >= 0)
        {
            array[tpa--] = cmp(array[tpl], swap[tpr]) > 0 ? array[tpl--] : swap[tpr--];
        }

        while (tpr >= 0)
        {
            array[tpa--] = swap[tpr--];
        }
    }

    static void tail_merge(Span<T> array, Span<T> swap, int nmemb, int block, Func<T, T, int> cmp)
    {
      int pta, pte; // array
      pte = nmemb;

      while (block < nmemb && block <= swap.Length)
      {
        for (pta = 0; pta + block < pte ; pta += block * 2)
        {
          if (pta + block * 2 < pte)
          {
            partial_backward_merge(array[pta..], swap, block * 2, block, cmp);
            continue;
          }
          partial_backward_merge(array[pta..], swap, pte - pta, block, cmp);
          break;
        }
        block *= 2;
      }
    }

    // the next four functions provide in-place rotate merge support

    static void trinity_rotation(Span<T> array, Span<T> swap, int swap_size, int nmemb, int left)
    {
        T temp;
        int bridge, right = nmemb - left;

        if (swap_size > 65536)
        {
            swap_size = 65536;
        }

        if (left < right)
        {
            if (left <= swap_size)
            {
                array[..left].CopyTo(swap);
                array.Slice(left, right).CopyTo(array);
                swap[..left].CopyTo(array[right..]);
            }
            else
            {
                int pta, ptb, ptc, ptd; // array

                pta = 0;
                ptb = pta + left;

                bridge = right - left;

                if (bridge <= swap_size && bridge > 3)
                {
                    ptc = pta + right;
                    ptd = ptc + left;

                    array.Slice(ptb, bridge).CopyTo(swap);

                    while (left-- != 0)
                    {
                        array[--ptc] = array[--ptd]; array[ptd] = array[--ptb];
                    }
                    swap[..bridge].CopyTo(array[pta..]);
                }
                else
                {
                    ptc = ptb;
                    ptd = ptc + right;

                    bridge = left / 2;

                    while (bridge-- != 0)
                    {
                        temp = array[--ptb]; array[ptb] = array[pta]; array[pta++] = array[ptc]; array[ptc++] = array[--ptd]; array[ptd] = temp;
                    }
                    bridge = (ptd - ptc) / 2;
                    while (bridge-- != 0)
                    {
                        temp = array[ptc]; array[ptc++] = array[--ptd]; array[ptd] = array[pta]; array[pta++] = temp;
                    }

                    bridge = (ptd - pta) / 2;

                    while (bridge-- != 0)
                    {
                        temp = array[pta]; array[pta++] = array[--ptd]; array[ptd] = temp;
                    }
                }
            }
        }
        else if (right < left)
        {
            if (right <= swap_size)
            {
                array.Slice(left, right).CopyTo(swap);
                array[..left].CopyTo(array[right..]);
                swap[..right].CopyTo(array);
            }
            else
            {
                int pta, ptb, ptc, ptd; // array

                pta = 0;
                ptb = pta + left;

                bridge = left - right;

                if (bridge <= swap_size && bridge > 3)
                {
                    ptc = pta + right;
                    ptd = ptc + left;

                    array.Slice(ptc, bridge).CopyTo(swap);

                    while (right-- != 0)
                    {
                        array[ptc++] = array[pta]; array[pta++] = array[ptb++];
                    }
                    swap[..bridge].CopyTo(array[(ptd - bridge)..]);
                }
                else
                {
                    ptc = ptb;
                    ptd = ptc + right;

                    bridge = right / 2;

                    while (bridge-- != 0)
                    {
                        temp = array[--ptb]; array[ptb] = array[pta]; array[pta++] = array[ptc]; array[ptc++] = array[--ptd]; array[ptd] = temp;
                    }
                    bridge = (ptb - pta) / 2;
                    while (bridge-- != 0)
                    {
                        temp = array[--ptb]; array[ptb] = array[pta]; array[pta++] = array[--ptd]; array[ptd] = temp;
                    }

                    bridge = (ptd - pta) / 2;

                    while (bridge-- != 0)
                    {
                        temp = array[pta]; array[pta++] = array[--ptd]; array[ptd] = temp;
                    }
                }
            }
        }
        else
        {
            int pta, ptb; // array
            pta = 0;
            ptb = pta + left;
            while (left-- != 0)
            {
                temp = array[pta]; array[pta++] = array[ptb]; array[ptb++] = temp;
            }
        }
    }

    static int monobound_binary_first(Span<T> array, Span<T> value, int top, Func<T, T, int> cmp)
    {
        int end; //array
        int mid;
        end = top;
        while (top > 1)
        {
            mid = top / 2;
            if (cmp(value[0], array[end - mid]) <= 0)
            {
                end -= mid;
            }
            top -= mid;
        }

        if (cmp(value[0], array[end - 1]) <= 0)
        {
            end--;
        }
        return end;
    }

    static void rotate_merge_block(Span<T> array, Span<T> swap, int lblock, int right, Func<T, T, int> cmp)
    {
        int left, rblock, swap_size = swap.Length;
        bool unbalanced;
        if (cmp(array[lblock - 1], array[lblock]) <= 0)
        {
            return;
        }

        rblock = lblock / 2;
        lblock -= rblock;

        left = monobound_binary_first(array[(lblock + rblock)..], array[lblock..], right, cmp);
        right -= left;

        // [ lblock ] [ rblock ] [ left ] [ right ]

        if (left != 0)
        {
            if (lblock + left <= swap_size)
            {
                array[..lblock].CopyTo(swap);
                array.Slice(lblock + rblock, left).CopyTo(swap[lblock..]);
                array.Slice(lblock, rblock).CopyTo(array[(lblock + left)..]);

                cross_merge(array, 0, swap, 0, lblock, left, cmp);
            }
            else
            {
                trinity_rotation(array[lblock..], swap, swap_size, rblock + left, rblock);
                unbalanced = (left * 2 < lblock) | (lblock * 2 < left);

                if (unbalanced && left <= swap_size)
                {
                    partial_backward_merge(array, swap, lblock + left, lblock, cmp);
                }
                else if (unbalanced && lblock <= swap_size)
                {
                    partial_forward_merge(array, swap, swap_size, lblock + left, lblock, cmp);
                }
                else
                {
                    rotate_merge_block(array, swap, lblock, left, cmp);
                }
            }
        }

        if (right != 0)
        {
            unbalanced = (right*  2 < rblock) | (rblock*  2 < right);

            if ((unbalanced && right <= swap_size) || right + rblock <= swap_size)
            {
                partial_backward_merge(array[(lblock + left)..], swap, rblock + right, rblock, cmp);
            }
            else if (unbalanced && rblock <= swap_size)
            {
                partial_forward_merge(array[(lblock + left)..], swap, swap_size, rblock + right, rblock, cmp);
            }
            else
            {
                rotate_merge_block(array[(lblock + left)..], swap, rblock, right, cmp);
            }
        }
    }

    static void rotate_merge(Span<T> array, Span<T> swap, int block, Func<T, T, int> cmp)
    {
        int pta, pte; // array
        int nmemb = array.Length, swap_size = swap.Length;

        pte = nmemb;

        if (nmemb <= block * 2 && unchecked((ulong)nmemb - (ulong)block) <= (ulong)swap_size) //TODO(Rennorb) @correctness
        {
            partial_backward_merge(array, swap, nmemb, block, cmp);

            return;
        }

        while (block < nmemb)
        {
            for (pta = 0; pta + block < pte ; pta += block * 2)
            {
                if (pta + block * 2 < pte)
                {
                    rotate_merge_block(array[pta..], swap, block, block, cmp);

                    continue;
                }
                rotate_merge_block(array[pta..], swap, block, pte - pta - block, cmp);

                break;
            }
            block *= 2;
      }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //┌─────────────────────────────────────────────────────────────────────────┐//
    //│    ██████┐ ██┐   ██┐ █████┐ ██████┐ ███████┐ ██████┐ ██████┐ ████████┐  │//
    //│   ██┌───██┐██│   ██│██┌──██┐██┌──██┐██┌────┘██┌───██┐██┌──██┐└──██┌──┘  │//
    //│   ██│   ██│██│   ██│███████│██│  ██│███████┐██│   ██│██████┌┘   ██│     │//
    //│   ██│▄▄ ██│██│   ██│██┌──██│██│  ██│└────██│██│   ██│██┌──██┐   ██│     │//
    //│   └██████┌┘└██████┌┘██│  ██│██████┌┘███████│└██████┌┘██│  ██│   ██│     │//
    //│    └──▀▀─┘  └─────┘ └─┘  └─┘└─────┘ └──────┘ └─────┘ └─┘  └─┘   └─┘     │//
    //└─────────────────────────────────────────────────────────────────────────┘//
    ///////////////////////////////////////////////////////////////////////////////

    static void quadsort(Span<T> array, Func<T, T, int> cmp)
    {
        if (true || array.Length < 32)
        {
            using var swap = new ArrayPoolReturner<T>(Math.Max(array.Length, 32), StableSort<T>.Pool);
            tail_swap(array, swap, cmp);
        }
        else if (quad_swap(array, cmp) == 0)
        {
            int block, nmemb = array.Length, swap_size = nmemb;

            if (nmemb > 4194304) { for (swap_size = 4194304 ; swap_size * 8 <= nmemb ; swap_size *= 4) {} }

            using var swap = new ArrayPoolReturner<T>(swap_size, StableSort<T>.Pool);
            if (swap.Length == 0) //TODO(Rennorb) 
            {
                using var sswap = new ArrayPoolReturner<T>(512, StableSort<T>.Pool);
                block = quad_merge(array, sswap, 32, cmp);
                rotate_merge(array, sswap, block, cmp);
                return;
            }

            block = quad_merge(array, swap, 32, cmp);
            rotate_merge(array, swap, block, cmp);
        }
    }

    static void quadsort_swap(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        if (array.Length <= 96)
        {
            tail_swap(array, swap, cmp);
        }
        else if (quad_swap(array, cmp) == 0)
        {
            int block = quad_merge(array, swap, 32, cmp);

            rotate_merge(array, swap, block, cmp);
        }
    }

    // Determine whether to use mergesort or quicksort

    static void flux_analyze(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        int loop, asum, bsum, csum, dsum;
        int astreaks, bstreaks, cstreaks, dstreaks, nmemb = array.Length;
        int quad1, quad2, quad3, quad4, half1, half2;
        long cnt, abalance, bbalance, cbalance, dbalance;
        int pta, ptb, ptc, ptd; // array

        half1 = nmemb / 2;
        quad1 = half1 / 2;
        quad2 = half1 - quad1;
        half2 = nmemb - half1;
        quad3 = half2 / 2;
        quad4 = half2 - quad3;

        pta = 0;
        ptb = quad1;
        ptc = half1;
        ptd = half1 + quad3;

        astreaks = bstreaks = cstreaks = dstreaks = 0;
        abalance = bbalance = cbalance = dbalance = 0;

        if (quad1 < quad2) { bbalance += cmp(array[ptb], array[ptb + 1]) > 0 ? 1 : 0; ptb++; }
        if (quad1 < quad3) { cbalance += cmp(array[ptc], array[ptc + 1]) > 0 ? 1 : 0; ptc++; }
        if (quad1 < quad4) { dbalance += cmp(array[ptd], array[ptd + 1]) > 0 ? 1 : 0; ptd++; }

        for (cnt = nmemb ; cnt > 132 ; cnt -= 128)
        {
            for (asum = bsum = csum = dsum = 0, loop = 32 ; loop != 0; loop--)
            {
                asum += cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; pta++;
                bsum += cmp(array[ptb], array[ptb + 1]) > 0 ? 1 : 0; ptb++;
                csum += cmp(array[ptc], array[ptc + 1]) > 0 ? 1 : 0; ptc++;
                dsum += cmp(array[ptd], array[ptd + 1]) > 0 ? 1 : 0; ptd++;
            }
            abalance += asum; astreaks += asum = (asum == 0 ? 1 : 0) | (asum == 32 ? 1 : 0);
            bbalance += bsum; bstreaks += bsum = (bsum == 0 ? 1 : 0) | (bsum == 32 ? 1 : 0);
            cbalance += csum; cstreaks += csum = (csum == 0 ? 1 : 0) | (csum == 32 ? 1 : 0);
            dbalance += dsum; dstreaks += dsum = (dsum == 0 ? 1 : 0) | (dsum == 32 ? 1 : 0);

            if (cnt > 516 && asum + bsum + csum + dsum == 0)
            {
                abalance += 48; pta += 96;
                bbalance += 48; ptb += 96;
                cbalance += 48; ptc += 96;
                dbalance += 48; ptd += 96;
                cnt -= 384;
            }
        }

        for ( ; cnt > 7 ; cnt -= 4)
        {
            abalance += cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; pta++;
            bbalance += cmp(array[ptb], array[ptb + 1]) > 0 ? 1 : 0; ptb++;
            cbalance += cmp(array[ptc], array[ptc + 1]) > 0 ? 1 : 0; ptc++;
            dbalance += cmp(array[ptd], array[ptd + 1]) > 0 ? 1 : 0; ptd++;
        }

        cnt = abalance + bbalance + cbalance + dbalance;

        if (cnt == 0)
        {
            if (cmp(array[pta], array[pta + 1]) <= 0 && cmp(array[ptb], array[ptb + 1]) <= 0 && cmp(array[ptc], array[ptc + 1]) <= 0)
            {
                return;
            }
        }

        asum = (quad1 - abalance) == 1 ? 1 : 0;
        bsum = (quad2 - bbalance) == 1 ? 1 : 0;
        csum = (quad3 - cbalance) == 1 ? 1 : 0;
        dsum = (quad4 - dbalance) == 1 ? 1 : 0;

        if ((asum | bsum | csum | dsum) != 0)
        {
            int span1 = ((asum | (bsum << 1)) == 3 ? 1 : 0) * (cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0);
            int span2 = ((bsum | (csum << 1)) == 3 ? 1 : 0) * (cmp(array[ptb], array[ptb + 1]) > 0 ? 1 : 0);
            int span3 = ((csum | (dsum << 1)) == 3 ? 1 : 0) * (cmp(array[ptc], array[ptc + 1]) > 0 ? 1 : 0);

            switch (span1 | span2 * 2 | span3 * 4)
            {
                case 0: break;
                case 1: quad_reversal(array,         0, ptb); abalance = bbalance = 0; break;
                case 2: quad_reversal(array, (pta + 1), ptc); bbalance = cbalance = 0; break;
                case 3: quad_reversal(array,         0, ptc); abalance = bbalance = cbalance = 0; break;
                case 4: quad_reversal(array, (ptb + 1), ptd); cbalance = dbalance = 0; break;
                case 5: quad_reversal(array,         0, ptb);
                        quad_reversal(array, (ptb + 1), ptd); abalance = bbalance = cbalance = dbalance = 0; break;
                case 6: quad_reversal(array, (pta + 1), ptd); bbalance = cbalance = dbalance = 0; break;
                case 7: quad_reversal(array,         0, ptd); return;
            }
            if (asum != 0 && abalance != 0) { quad_reversal(array,         0, pta); abalance = 0; }
            if (bsum != 0 && bbalance != 0) { quad_reversal(array, (pta + 1), ptb); bbalance = 0; }
            if (csum != 0 && cbalance != 0) { quad_reversal(array, (ptb + 1), ptc); cbalance = 0; }
            if (dsum != 0 && dbalance != 0) { quad_reversal(array, (ptc + 1), ptd); dbalance = 0; }
        }

        cnt = nmemb / 512; // switch to quadsort if at least 25% ordered
        asum = astreaks > cnt ? 1 : 0;
        bsum = bstreaks > cnt ? 1 : 0;
        csum = cstreaks > cnt ? 1 : 0;
        dsum = dstreaks > cnt ? 1 : 0;

        if (quad1 > 262144)
        {
            asum = bsum = csum = dsum = 1;
        }

        switch (asum + bsum * 2 + csum * 4 + dsum * 8)
        {
            case 0:
                flux_partition(array, swap, array, nmemb, nmemb, cmp);
                return;
            case 1:
                if (abalance != 0) { quadsort_swap(array[..quad1], swap, cmp); }
                flux_partition(array[(pta + 1)..], swap, array[(pta + 1)..], quad2 + half2, quad2 + half2, cmp);
                break;
            case 2:
                flux_partition(array, swap, array, quad1, quad1, cmp);
                if (bbalance != 0) { quadsort_swap(array.Slice(pta + 1, quad2), swap, cmp); }
                flux_partition(array[(ptb + 1)..], swap, array[(ptb + 1)..], half2, half2, cmp);
                break;
            case 3:
                if (abalance != 0) { quadsort_swap(array[..quad1], swap, cmp); }
                if (bbalance != 0) { quadsort_swap(array.Slice(pta + 1, quad2), swap, cmp); }
                flux_partition(array[(ptb + 1)..], swap, array[(ptb + 1)..], half2, half2, cmp);
                break;
            case 4:
                flux_partition(array, swap, array, half1, half1, cmp);
                if (cbalance != 0) { quadsort_swap(array.Slice(ptb + 1, quad3), swap, cmp); }
                flux_partition(array[(ptc + 1)..], swap, array[(ptc + 1)..], quad4, quad4, cmp);
                break;
            case 8:
                flux_partition(array, swap, array, half1 + quad3, half1 + quad3, cmp);
                if (dbalance != 0) { quadsort_swap(array.Slice(ptc + 1, quad4), swap, cmp); }
                break;
            case 9:
                if (abalance != 0) { quadsort_swap(array[..quad1], swap, cmp); }
                flux_partition(array[(pta + 1)..], swap, array[(pta + 1)..], quad2 + quad3, quad2 + quad3, cmp);
                if (dbalance != 0) { quadsort_swap(array.Slice(ptc + 1, quad4), swap, cmp); }
                break;
            case 12:
                flux_partition(array, swap, array, half1, half1, cmp);
                if (cbalance != 0) { quadsort_swap(array.Slice(ptb + 1, quad3), swap, cmp); }
                if (dbalance != 0) { quadsort_swap(array.Slice(ptc + 1, quad4), swap, cmp); }
                break;
            case 5:
            case 6:
            case 7:
            case 10:
            case 11:
            case 13:
            case 14:
            case 15:
                if (asum != 0)
                {
                    if (abalance != 0) { quadsort_swap(array[..quad1], swap, cmp); }
                }
                else { flux_partition(array, swap, array, quad1, quad1, cmp); }

                if (bsum != 0)
                {
                    if (bbalance != 0) { quadsort_swap(array.Slice(pta + 1, quad2), swap, cmp); }
                }
                else { flux_partition(array[(pta + 1)..], swap, array[(pta + 1)..], quad2, quad2, cmp); }

                if (csum != 0)
                {
                    if (cbalance != 0) { quadsort_swap(array.Slice(ptb + 1, quad3), swap, cmp); }
                }
                else { flux_partition(array[(ptb + 1)..], swap, array[(ptb + 1)..], quad3, quad3, cmp); }

                if (dsum != 0)
                {
                    if (dbalance != 0) { quadsort_swap(array.Slice(ptc + 1, quad4), swap, cmp); }
                }
                else { flux_partition(array[(ptc + 1)..], swap, array[(ptc + 1)..], quad4, quad4, cmp); }
                break;
        }

        if (cmp(array[pta], array[pta + 1]) <= 0)
        {
            if (cmp(array[ptc], array[ptc + 1]) <= 0)
            {
                if (cmp(array[ptb], array[ptb + 1]) <= 0)
                {
                    return;
                }
                array[..nmemb].CopyTo(swap);
            }
            else
            {
                cross_merge(swap, half1, array, half1, quad3, quad4, cmp);
                array[..half1].CopyTo(swap);
            }
        }
        else
        {
            if (cmp(array[ptc], array[ptc + 1]) <= 0)
            {
                array.Slice(half1, half2).CopyTo(swap[half1..]);
                cross_merge(swap, 0, array, 0, quad1, quad2, cmp);
            }
            else
            {
                cross_merge(swap, half1, array, ptb + 1, quad3, quad4, cmp);
                cross_merge(swap, 0, array, 0, quad1, quad2, cmp);
            }
        }
        cross_merge(array, 0, swap, 0, half1, half2, cmp);
    }

    // The next 4 functions are used for pivot selection

    static T binary_median(Span<T> pta, Span<T> ptb, int len, Func<T, T, int> cmp)
    {
        int ia = 0, ib = 0;
        while ((len /= 2) != 0)
        {
            if (cmp(pta[ia + len], ptb[ib + len]) <= 0) { ia += len; } else { ib += len; }
        }
        return cmp(pta[ia], ptb[ib]) > 0 ? pta[ia] : ptb[ib];
    }

    static void trim_four(Span<T> array, Func<T, T, int> cmp)
    {
        T swap;
        int pta = 0, x;

        x = cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; swap = array[pta + (x == 0 ? 1 : 0)]; array[pta] = array[pta + x]; array[pta + 1] = swap; pta += 2;
        x = cmp(array[pta], array[pta + 1]) > 0 ? 1 : 0; swap = array[pta + (x == 0 ? 1 : 0)]; array[pta] = array[pta + x]; array[pta + 1] = swap; pta -= 2;

        x = (cmp(array[pta], array[pta + 2]) <= 0) ? 2 : 0; array[pta + 2] = array[pta + x]; pta++;
        x = (cmp(array[pta], array[pta + 2])  > 0) ? 2 : 0; array[pta] = array[pta + x];
    }

    static T median_of_nine(Span<T> array, int nmemb, Func<T, T, int> cmp)
    {
        using var swap = new ArrayPoolReturner<T>(9, StableSort<T>.Pool);
        int pta;
        int x, y, z;

        z = nmemb / 9;

        pta = 0;

        for (x = 0 ; x < 9 ; x++)
        {
            swap[x] = array[pta];

            pta += z;
        }

        trim_four(swap, cmp);
        trim_four(swap[4..], cmp);

        swap[0] = swap[5];
        swap[3] = swap[8];

        trim_four(swap, cmp);

        swap[0] = swap[6];

        x = cmp(swap[0], swap[1]) > 0 ? 1 : 0;
        y = cmp(swap[0], swap[2]) > 0 ? 1 : 0;
        z = cmp(swap[1], swap[2]) > 0 ? 1 : 0;

        return swap[(x == y ? 1 : 0) + (y ^ z)];
    }

    static T median_of_cbrt(Span<T> array, Span<T> swap, Span<T> ptx, int nmemb, ref int generic, Func<T, T, int> cmp)
    {
        int pta;
        Span<T> pts;
        int cnt, div, cbrt;

        for (cbrt = 32 ; nmemb > cbrt * cbrt * cbrt ; cbrt *= 2) {}

        div = nmemb / cbrt;

        pta = (int)((ulong)&div / 16 % (ulong)div); // for a non-deterministic offset
        pts = ptx == array ? swap : array;

        for (cnt = 0 ; cnt < cbrt ; cnt++)
        {
            pts[cnt] = ptx[pta];

            pta += div;
        }
        cbrt /= 2;

        quadsort_swap(pts[..cbrt], pts.Slice(cbrt * 2, cbrt), cmp);
        quadsort_swap(pts.Slice(cbrt, cbrt), pts.Slice(cbrt * 2, cbrt), cmp);

        generic = (cmp(pts[cbrt * 2 - 1], pts[0]) <= 0) & (cmp(pts[cbrt - 1], pts[0]) <= 0) ? 1 : 0;

        return binary_median(pts, pts[cbrt..], cbrt, cmp);
    }

    // As per suggestion by Marshall Lochbaum to improve generic data handling by mimicking dual-pivot quicksort

    static void flux_reverse_partition(Span<T> array, Span<T> swap, Span<T> ptx, int piv /* in swap */, int nmemb, Func<T, T, int> cmp)
    {
        int a_size, s_size;

        {
            int cnt;
            int pta = 0, pts = 0, pty = 0;
            T pivt = swap[piv];

            for (cnt = nmemb / 8 ; cnt != 0; cnt--)
            {
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            }

            for (cnt = nmemb % 8 ; cnt != 0; cnt--)
            {
                if(cmp(pivt, ptx[pty]) > 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            }
            a_size = pta;
            s_size = pts;
        }

        swap[..s_size].CopyTo(array[a_size..]);

        if (s_size <= a_size / 16 || a_size <= 96)
        {
            quadsort_swap(array[..a_size], swap[..a_size], cmp);
            return;
        }
        flux_partition(array, swap, array, piv, a_size, cmp);
    }

    static int flux_default_partition(Span<T> array, Span<T> swap, Span<T> ptx, int piv, int nmemb, Func<T, T, int> cmp)
    {
        int run = 0, a = 0, m = 0;

        int pta = 0, //array
            pts = 0, // swap
            pty = 0; // ptx
        T pivt = swap[piv];

        for (a = 8 ; a <= nmemb ; a += 8)
        {
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }

            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }

            if (pta == 0 || pts == 0) { run = a; }
        }

        for (a = nmemb % 8 ; a != 0; a--)
        {
            if(cmp(ptx[pty], pivt) <= 0) { array[pta++] = ptx[pty++]; } else { swap[pts++] = ptx[pty++]; }
        }
        m = pta;

        if (run <= nmemb / 4)
        {
            return m;
        }

        if (m == nmemb)
        {
            return m;
        }

        a = nmemb - m;

        swap[..a].CopyTo(array[m..]);

        quadsort_swap(array.Slice(m, a), swap[..a], cmp);
        quadsort_swap(array[..m], swap[..m], cmp);

        return 0;
    }

    static void flux_partition(Span<T> array, Span<T> swap, Span<T> ptx, int piv /* in swap */, int nmemb, Func<T, T, int> cmp)
    {
        int a_size = 0, s_size;
        int generic = 0;

        while (true)
        {
            --piv;

            if (nmemb <= 2048)
            {
                swap[piv] = median_of_nine(ptx, nmemb, cmp);
            }
            else
            {
                swap[piv] = median_of_cbrt(array, swap, ptx, nmemb, ref generic, cmp);

                if (generic != 0)
                {
                    if (ptx == swap) //TODO(Rennorb) 
                    {
                        swap[..nmemb].CopyTo(array);
                    }
                    quadsort_swap(array[..nmemb], swap[..nmemb], cmp);
                    return;
                }
            }

            if (a_size != 0 && cmp(swap[piv + 1], swap[piv]) <= 0)
            {
                flux_reverse_partition(array, swap, array, piv, nmemb, cmp);
                return;
            }
            a_size = flux_default_partition(array, swap, ptx, piv, nmemb, cmp);
            s_size = nmemb - a_size;

            if (a_size <= s_size / 32 || s_size <= 96)
            {
                if (a_size == 0)
                {
                    return;
                }
                if (s_size == 0)
                {
                    flux_reverse_partition(array, swap, array, piv, a_size, cmp);
                    return;
                }
                swap[..s_size].CopyTo(array[a_size..]);
                quadsort_swap(array.Slice(a_size, s_size), swap[..s_size], cmp);
            }
            else
            {
                flux_partition(array[a_size..], swap, swap, piv, s_size, cmp);
            }

            if (s_size <= a_size / 32 || a_size <= 96)
            {
                if (a_size <= 96)
                {
                    quadsort_swap(array[..a_size], swap[..a_size], cmp);
                }
                else
                {
                    flux_reverse_partition(array, swap, array, piv, a_size, cmp);
                }
                return;
            }
            nmemb = a_size;
            ptx = array;
        }
    }

    /// The cmp function must be a complete compare operation. (int)(A - B) is not enough (that might overflow and violate transitivity).
    public static void fluxsort(Span<T> array, Func<T, T, int> cmp)
    {
        if (true || array.Length <= 132)
        {
            quadsort(array, cmp);
        }
        else
        {
            using var mem = new ArrayPoolReturner<T>(array.Length, StableSort<T>.Pool);
            var swap = mem.AsSpan();
            if (swap == null) //TODO(Rennorb) 
            {
                quadsort(array, cmp);
                return;
            }
            flux_analyze(array, swap, cmp);
        }
    }
    static void fluxsort_swap(Span<T> array, Span<T> swap, Func<T, T, int> cmp)
    {
        if (array.Length <= 132)
        {
            quadsort_swap(array, swap, cmp);
        }
        else
        {
            flux_analyze(array, swap, cmp);
        }
    }
}
#pragma warning restore CA1000 // static on generic type
