using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace Carnac.Logic
{
    public static class ReplaceKey
    {
        static readonly Dictionary<Keys, string> ShiftReplacements = new Dictionary<Keys, string>
        {
            {Keys.D0, ")"},
            {Keys.D1, "!"},
            {Keys.D2, "@"},
            {Keys.D3, "#"},
            {Keys.D4, "$"},
            {Keys.D5, "%"},
            {Keys.D6, "^"},
            {Keys.D7, "&"},
            {Keys.D8, "*"},
            {Keys.D9, "("},
            {Keys.OemOpenBrackets, "{"},
            {Keys.Oem6, "}"},
            {Keys.OemMinus, "_"},
            {Keys.Oemplus, "+"},
            {Keys.OemBackslash, "|"},
            {Keys.Oem5, "|"},
            {Keys.OemQuestion, "?"},
            {Keys.OemPeriod, ">"},
            {Keys.Oemcomma, "<"},
            {Keys.Oem1, ":"},
            {Keys.Oem7, "\""},
            {Keys.Oemtilde, "~"},
            {Keys.Insert, "ins"},
            {Keys.Delete, "del"}
        };

        static readonly Dictionary<Keys, string> Replacements = new Dictionary<Keys, string>
        {
            {Keys.Space, " "},
            {Keys.D0, "0"},
            {Keys.D1, "1"},
            {Keys.D2, "2"},
            {Keys.D3, "3"},
            {Keys.D4, "4"},
            {Keys.D5, "5"},
            {Keys.D6, "6"},
            {Keys.D7, "7"},
            {Keys.D8, "8"},
            {Keys.D9, "9"},
            {Keys.NumPad0, "0"},
            {Keys.NumPad1, "1"},
            {Keys.NumPad2, "2"},
            {Keys.NumPad3, "3"},
            {Keys.NumPad4, "4"},
            {Keys.NumPad5, "5"},
            {Keys.NumPad6, "6"},
            {Keys.NumPad7, "7"},
            {Keys.NumPad8, "8"},
            {Keys.NumPad9, "9"},
            {Keys.OemOpenBrackets, "["},
            {Keys.Oem6, "]"},
            {Keys.OemMinus, "-"},
            {Keys.Oemplus, "="},
            {Keys.Oem5, "\\"},
            {Keys.OemBackslash, "\\"},
            {Keys.OemQuestion, "/"},
            {Keys.OemPeriod, "."},
            {Keys.Oemcomma, ","},
            {Keys.Oem1, ";"},
            {Keys.Oem7, "'"},
            {Keys.Oemtilde, "`"},
            {Keys.Decimal, "."},
            {Keys.Divide, " / "},
            {Keys.Multiply, " * "},
            {Keys.Subtract, " - "},
            {Keys.Add, " + "},
            {Keys.LShiftKey, "Shift"},
            {Keys.RShiftKey, "Shift"},
            {Keys.LWin, "Win"},
            {Keys.RWin, "Win"},
        };

        // kept to continue to support keymaps parsing
        public static Keys? ToKey(string keyText)
        {
            foreach (var shiftReplacement in ShiftReplacements)
            {
                if (shiftReplacement.Value.Equals(keyText, StringComparison.CurrentCultureIgnoreCase))
                    return shiftReplacement.Key;
            }
            Keys parsedKey;
            if (Enum.TryParse(keyText, true, out parsedKey))
                return parsedKey;

            foreach (var replacement in Replacements)
            {
                if (replacement.Value.Equals(keyText, StringComparison.CurrentCultureIgnoreCase))
                    return replacement.Key;
            }
            return null;
        }

        // new implementation of sanatize to support locals
        // https://stackoverflow.com/questions/318777/c-sharp-how-to-translate-virtual-keycode-to-char
        public static string KeyCodeToUnicode(Keys key, bool lowerOnly = false)
        {
            byte[] keyboardState = new byte[255];
            if (!lowerOnly)
            {
                bool keyboardStateStatus = GetKeyboardState(keyboardState);
                if (!keyboardStateStatus)
                {
                    return "";
                }
            }

            uint virtualKeyCode = (uint)key;
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            switch(key)
            {
                case Keys.Back:
                    return "⌫ ";
                case Keys.Enter:
                    return "↩ ";
                case Keys.Tab:
                    return " ⇆ ";
                case Keys.CapsLock:
                    return "Caps";
                case Keys.Escape:
                    return "Esc";
                case Keys.Left:
                    return "⯇ ";
                case Keys.Right:
                    return "⯈ ";
                case Keys.Up:
                    return "⯅ ";
                case Keys.Down:
                    return "⯆ ";
                case Keys.PrintScreen:
                    return "PrntScr";
                case Keys.Scroll:
                    return "ScrollLock";
                case Keys.Pause:
                    return "Pause";
                case Keys.Insert:
                    return "Insert";
                case Keys.Home:
                    return "Home";
                case Keys.PageUp:
                    return "PageUp";
                case Keys.Delete:
                    return "Delete";
                case Keys.End:
                    return "End";
                case Keys.PageDown:
                    return "PageDown";
                case Keys.F1:
                    return "F1";
                case Keys.F2:
                    return "F2";
                case Keys.F3:
                    return "F3";
                case Keys.F4:
                    return "F4";
                case Keys.F5:
                    return "F5";
                case Keys.F6:
                    return "F6";
                case Keys.F7:
                    return "F7";
                case Keys.F8:
                    return "F8";
                case Keys.F9:
                    return "F9";
                case Keys.F10:
                    return "F10";
                case Keys.F11:
                    return "F11";
                case Keys.F12:
                    return "F12";
                case Keys.VolumeDown:
                case Keys.VolumeUp:
                    return "Volume";
            }

            return result.ToString();
        }

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        public static string Sanitise(this Keys key)
        {
            return KeyCodeToUnicode(key);
        }

        public static string SanitiseLower(this Keys key)
        {
            return KeyCodeToUnicode(key, true);
        }
    }
}