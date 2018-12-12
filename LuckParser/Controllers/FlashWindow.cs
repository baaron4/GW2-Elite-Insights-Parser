using System;
using System.Runtime.InteropServices;

namespace LuckParser.Controllers
{
    public static class FlashWindow
    {/*
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FlashwInfo pwfi);

        [StructLayout(LayoutKind.Sequential)]
        private struct FlashwInfo
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint CbSize;
            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr Hwnd;
            /// <summary>
            /// The Flash Status.
            /// </summary>
            public uint DwFlags;
            /// <summary>
            /// The number of times to Flash the window.
            /// </summary>
            public uint UCount;
            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint DwTimeout;
        }

        /// <summary>
        /// Stop flashing. The system restores the window to its original state.
        /// </summary>
        private const uint FlashWStop = 0;

        /// <summary>
        /// Flash the window caption.
        /// </summary>
        public const uint FlashWCaption = 1;

        /// <summary>
        /// Flash the taskbar button.
        /// </summary>
        public const uint FlashWTray = 2;

        /// <summary>
        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FlashWCaption | FlashWTray flags.
        /// </summary>
        private const uint FlashHAll = 3;

        /// <summary>
        /// Flash continuously, until the FlashWStop flag is set.
        /// </summary>
        public const uint FlashHTimer = 4;

        /// <summary>
        /// Flash continuously until the window comes to the foreground.
        /// </summary>
        private const uint FlashWTimerNoCfg = 12;


        /// <summary>
        /// Flash the specified Window (Form) until it receives focus.
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <returns></returns>
        public static bool Flash(System.Windows.Forms.Form form)
        {
            // Make sure we're running under Windows 2000 or later
            if (Win2000OrLater)
            {
                FlashwInfo fi = CreateFlashHInfo(form.Handle, FlashHAll | FlashWTimerNoCfg, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        private static FlashwInfo CreateFlashHInfo(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FlashwInfo fi = new FlashwInfo();
            fi.CbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.Hwnd = handle;
            fi.DwFlags = flags;
            fi.UCount = count;
            fi.DwTimeout = timeout;
            return fi;
        }

        /// <summary>
        /// Flash the specified Window (form) for the specified number of times
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <param name="count">The number of times to Flash.</param>
        /// <returns></returns>
        public static bool Flash(System.Windows.Forms.Form form, uint count)
        {
            if (Win2000OrLater)
            {
                FlashwInfo fi = CreateFlashHInfo(form.Handle, FlashHAll, count, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Start Flashing the specified Window (form)
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <returns></returns>
        public static bool Start(System.Windows.Forms.Form form)
        {
            if (Win2000OrLater)
            {
                FlashwInfo fi = CreateFlashHInfo(form.Handle, FlashHAll, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Stop Flashing the specified Window (form)
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool Stop(System.Windows.Forms.Form form)
        {
            if (Win2000OrLater)
            {
                FlashwInfo fi = CreateFlashHInfo(form.Handle, FlashWStop, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// A boolean value indicating whether the application is running on Windows 2000 or later.
        /// </summary>
        private static bool Win2000OrLater
        {
            get { return Environment.OSVersion.Version.Major >= 5; }
        }*/
    }
}
