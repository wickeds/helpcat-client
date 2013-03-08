using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace helpcat.Dialogs
{
    [Flags]
    public enum TaskDialogButtons : int
    {
        OK = 0x0001,
        Yes = 0x0002,
        No = 0x0004,
        Cancel = 0x0008,
        Retry = 0x0010,
        Close = 0x0020
    }

    [Flags]
    public enum TaskDialogIcon
    {
        None = 0x0,
        Warning = 0xFFFF,
        Error = 0xFFFE,
        Information = 0xFFFD,
        Shield = 0xFFFC,
    }

    [Flags]
    public enum TaskDialogResult : int
    {
        None = 0x0,
        OK = 0x1,
        Cancel = 0x2,
        Retry = 0x4,
        Yes = 0x6,
        No = 0x7,
        Close = 0x8
    }

    public static class TaskDialogEx
    {
        [DllImport("comctl32.dll", CharSet = CharSet.Unicode, EntryPoint = "TaskDialog", PreserveSig = false)]
        private static extern void TaskDialogPInvoke(IntPtr hWndParent, IntPtr hInstance, String pszWindowTitle, String pszMainInstruction, String pszContent, int dwCommonButtons, IntPtr pszIcon, out int pnButton);

        public static TaskDialogResult Show(IntPtr parentHandle, string windowTitle, string mainInstruction, string content, TaskDialogButtons buttons, TaskDialogIcon icon)
        {
            int re = (int)TaskDialogResult.None;
            TaskDialogPInvoke(parentHandle, IntPtr.Zero, windowTitle, mainInstruction, content, (int)buttons, new IntPtr((int)icon), out re);
            return (TaskDialogResult)re;
        }

        public static TaskDialogResult ShowEx(Window parentWindow, string windowTitle, string mainInstruction, string content, TaskDialogButtons buttons, TaskDialogIcon icon)
        {
            WindowInteropHelper helper = new WindowInteropHelper(parentWindow);
            return Show(helper.Handle, windowTitle, mainInstruction, content, buttons, icon);
        }
    }
}
