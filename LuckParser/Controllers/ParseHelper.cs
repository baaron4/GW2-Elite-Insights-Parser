using System;
using System.IO;

namespace LuckParser.Controllers
{
    class ParseHelper
    {
        public static void safeSkip(MemoryStream stream,long bytes_to_skip)
        {

            while (bytes_to_skip > 0)
            {
                int dummyByte = stream.ReadByte();
                long bytes_actually_skipped = 1;
                if (bytes_actually_skipped > 0)
                {
                    bytes_to_skip -= bytes_actually_skipped;
                }
                else if (bytes_actually_skipped == 0)
                {
                    if (stream.ReadByte() == -1)
                    {
                        break;
                    }
                    else
                    {
                        bytes_to_skip--;
                    }
                }
            }

            return;
        }
        public static ushort getShort(MemoryStream stream)
        {
            byte[] bytes = new byte[2];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                //stream.Position++;
            }
            // return Short.toUnsignedInt(ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).getShort());
            return BitConverter.ToUInt16(bytes, 0);
        }
        public static int getInt(MemoryStream stream)
        {
            byte[] bytes = new byte[4];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                // stream.Position++;
            }
            //return ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).getInt();
            return BitConverter.ToInt32(bytes, 0);
        }
        public static long getLong(MemoryStream stream)
        {
            byte[] bytes = new byte[8];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                // stream.Position++;
            }

            // return ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).getLong();
            return BitConverter.ToInt64(bytes, 0);
        }
        public static string getString(MemoryStream stream, int length)
        {
            byte[] bytes = new byte[length];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                // stream.Position++;
            }

            string s = new String(System.Text.Encoding.UTF8.GetString(bytes).ToCharArray()).TrimEnd();
            if (s != null)
            {
                return s;
            }
            return "UNKNOWN";
        }
    }
}
