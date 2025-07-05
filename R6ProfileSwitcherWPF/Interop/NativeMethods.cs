﻿using System;
using System.Runtime.InteropServices;

namespace R6ProfileSwitcherWPF.Interop
{
    internal static class NativeMethods
    {
        internal const uint INPUT_MOUSE = 0;
        internal const uint INPUT_KEYBOARD = 1;
        internal const uint INPUT_HARDWARE = 2;

        internal const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        internal const uint KEYEVENTF_KEYUP = 0x0002;
        internal const uint KEYEVENTF_SCANCODE = 0x0008;
        internal const uint KEYEVENTF_UNICODE = 0x0004;

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(
            uint nInputs,
            INPUT[] pInputs,
            int cbSize
        );

        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }
    }
}
