using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Interop;

namespace helpcat
{
    static class Natives
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        enum DWMWINDOWATTRIBUTE
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_LAST
        };

        public static void EnableGlass(IntPtr hWnd, int left, int right, int top, int bottom)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
            {
                MARGINS m = new MARGINS();
                m.leftWidth = left;
                m.rightWidth = right;
                m.topHeight = top;
                m.bottomHeight = bottom;

                DwmExtendFrameIntoClientArea(hWnd, ref m);
            }
        }

        public static void EnableShadow(IntPtr hWnd)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
            {
                MARGINS m = new MARGINS();
                m.leftWidth = -1;
                m.rightWidth = -1;
                m.topHeight = -1;
                m.bottomHeight = -1;

                int attr = (int)DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY;
                DwmSetWindowAttribute(hWnd, attr, ref attr, 4);
                DwmExtendFrameIntoClientArea(hWnd, ref m);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public static bool IsForegroundWindow(Window window)
        {
            WindowInteropHelper interop = new WindowInteropHelper(window);
            return GetForegroundWindow() == interop.Handle;
        }

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        public static void SetExplorerStyle(Control control)
        {
            SetWindowTheme(control.Handle, "Explorer", null);
        }

        [DllImport("uxtheme.dll")]
        public static extern int SetWindowThemeAttribute(IntPtr hWnd, WindowThemeAttributeType wtype, ref WTA_OPTIONS attributes, uint size);

        public enum WindowThemeAttributeType : uint
        {
            /// <summary>Non-client area window attributes will be set.</summary>
            WTA_NONCLIENT = 1,
        }

        public struct WTA_OPTIONS
        {
            public uint Flags;
            public uint Mask;
        }
        public static uint WTNCA_NODRAWCAPTION = 0x00000001;
        public static uint WTNCA_NODRAWICON = 0x00000002;


        public static void SetNoDrawCaptionNoDrawIcon(Form form)
        {
            WTA_OPTIONS wta = new WTA_OPTIONS() { Flags = WTNCA_NODRAWCAPTION | WTNCA_NODRAWICON, Mask = WTNCA_NODRAWCAPTION | WTNCA_NODRAWICON };
            SetWindowThemeAttribute(form.Handle, WindowThemeAttributeType.WTA_NONCLIENT, ref wta, (uint)Marshal.SizeOf(typeof(WTA_OPTIONS)));
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        public const int SWP_DEFERERASE = 0x2000;
        public const int SWP_DRAWFRAME = 0x0020;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOOWNERZORDER = 0x0200;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_NOREPOSITION = 0x0200;
        public const int SWP_NOSENDCHANGING = 0x0400;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_SHOWWINDOW = 0x0040;

        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;

        public static void BringToFront(this Window window)
        {
            WindowInteropHelper interop = new WindowInteropHelper(window);
            SetWindowPos(interop.Handle, new IntPtr(HWND_NOTOPMOST), 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0040);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;

        public static void MakeUnfocusable(IntPtr hWnd)
        {
            int style = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, style | WS_EX_NOACTIVATE);
        }
    }
}
