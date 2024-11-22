using Hexa.NET.ImGui;
using RenderSpy.Globals;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public class InputImguiEmu
    {
        #region PInvoke

        [DllImport("user32.dll")]
        private static extern int ToUnicode(
   uint wVirtKey,
   uint wScanCode,
   byte[] lpKeyState,
   [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
    StringBuilder pwszBuff,
   int cchBuff,
   uint wFlags);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        #endregion

        public Dictionary<Keys, ImGuiKey> VirtualKeyToImGuiKeyMap = new Dictionary<Keys, ImGuiKey>
        {
            { Keys.D0, ImGuiKey.Key0 },
            { Keys.D1, ImGuiKey.Key1 },
            { Keys.D2, ImGuiKey.Key2 },
            { Keys.D3, ImGuiKey.Key3 },
            { Keys.D4, ImGuiKey.Key4 },
            { Keys.D5, ImGuiKey.Key5 },
            { Keys.D6, ImGuiKey.Key6 },
            { Keys.D7, ImGuiKey.Key7 },
            { Keys.D8, ImGuiKey.Key8 },
            { Keys.D9, ImGuiKey.Key9 },
            { Keys.NumPad0, ImGuiKey.Key0 },
            { Keys.NumPad1, ImGuiKey.Key1 },
            { Keys.NumPad2, ImGuiKey.Key2 },
            { Keys.NumPad3, ImGuiKey.Key3 },
            { Keys.NumPad4, ImGuiKey.Key4 },
            { Keys.NumPad5, ImGuiKey.Key5 },
            { Keys.NumPad6, ImGuiKey.Key6 },
            { Keys.NumPad7, ImGuiKey.Key7 },
            { Keys.NumPad8, ImGuiKey.Key8 },
            { Keys.NumPad9, ImGuiKey.Key9 },
            { Keys.A, ImGuiKey.A },
            { Keys.B, ImGuiKey.B },
            { Keys.C, ImGuiKey.C },
            { Keys.D, ImGuiKey.D },
            { Keys.E, ImGuiKey.E },
            { Keys.F, ImGuiKey.F },
            { Keys.G, ImGuiKey.G },
            { Keys.H, ImGuiKey.H },
            { Keys.I, ImGuiKey.I },
            { Keys.J, ImGuiKey.J },
            { Keys.K, ImGuiKey.K },
            { Keys.L, ImGuiKey.L },
            { Keys.M, ImGuiKey.M },
            { Keys.N, ImGuiKey.N },
            { Keys.O, ImGuiKey.O },
            { Keys.P, ImGuiKey.P },
            { Keys.Q, ImGuiKey.Q },
            { Keys.R, ImGuiKey.R },
            { Keys.S, ImGuiKey.S },
            { Keys.T, ImGuiKey.T },
            { Keys.U, ImGuiKey.U },
            { Keys.V, ImGuiKey.V },
            { Keys.W, ImGuiKey.W },
            { Keys.X, ImGuiKey.X },
            { Keys.Y, ImGuiKey.Y },
            { Keys.Z, ImGuiKey.Z },
            { Keys.Enter, ImGuiKey.Enter },
            { Keys.Escape, ImGuiKey.Escape },
            { Keys.Back, ImGuiKey.Backspace },
            { Keys.Delete, ImGuiKey.Delete },
            { Keys.Space, ImGuiKey.Space },
            { Keys.Tab, ImGuiKey.Tab },
            { Keys.Left, ImGuiKey.LeftArrow },
            { Keys.Right, ImGuiKey.RightArrow },
            { Keys.Up, ImGuiKey.UpArrow },
            { Keys.Down, ImGuiKey.DownArrow },
            { Keys.F1, ImGuiKey.F1 },
            { Keys.F2, ImGuiKey.F2 },
            { Keys.F3, ImGuiKey.F3 },
            { Keys.F4, ImGuiKey.F4 },
            { Keys.F5, ImGuiKey.F5 },
            { Keys.F6, ImGuiKey.F6 },
            { Keys.F7, ImGuiKey.F7 },
            { Keys.F8, ImGuiKey.F8 },
            { Keys.F9, ImGuiKey.F9 },
            { Keys.F10, ImGuiKey.F10 },
            { Keys.F11, ImGuiKey.F11 },
            { Keys.F12, ImGuiKey.F12 },
            { Keys.OemPeriod, ImGuiKey.Period },      // .
            { Keys.Oemcomma, ImGuiKey.Comma },        // ,
            { Keys.OemSemicolon, ImGuiKey.Semicolon },// ;
            { Keys.OemQuotes, ImGuiKey.Apostrophe },       // '
            { Keys.OemQuestion, ImGuiKey.Slash },     // /
            { Keys.OemPipe, ImGuiKey.Backslash },     // \
            { Keys.OemCloseBrackets, ImGuiKey.RightBracket }, // ]
            { Keys.OemOpenBrackets, ImGuiKey.LeftBracket },   // [
            { Keys.OemMinus, ImGuiKey.Minus },        // -
            { Keys.Oemplus, ImGuiKey.Equal },         // =
            { Keys.Oemtilde, ImGuiKey.GraveAccent },  // `
            { Keys.OemBackslash, ImGuiKey.Backslash } // \
        };

        private Dictionary<ImGuiKey, DateTime> keyLastPressed = new Dictionary<ImGuiKey, DateTime>();
        public TimeSpan keyRepeatDelay = TimeSpan.FromMilliseconds(150);

        private Hexa.NET.ImGui.ImGuiIOPtr IO;

        public InputImguiEmu(Hexa.NET.ImGui.ImGuiIOPtr Imgui_IO)
        {
            this.IO = Imgui_IO;

            foreach (var key in VirtualKeyToImGuiKeyMap.Values)
            {
                keyLastPressed[key] = DateTime.MinValue;
            }
        }

        public void UpdateKeyboardState()
        {
            foreach (KeyValuePair<Keys, ImGuiKey> key in VirtualKeyToImGuiKeyMap)
            {
                try
                {
                    int keyState = WinApi.GetAsyncKeyState(key.Key);
                    bool isKeyDown = (keyState & 0x8000) != 0;

                    if (isKeyDown)
                    {
                        DateTime lastPressed = keyLastPressed[key.Value];
                        if ((DateTime.Now - lastPressed) >= keyRepeatDelay)
                        {
                            IO.AddKeyEvent(key.Value, true);

                            char c = ConvertKeyToChar(key.Key);
                            if (c != '\0')
                            {
                                IO.AddInputCharacter(c);
                            }

                            keyLastPressed[key.Value] = DateTime.Now;
                        }
                    }
                    else
                    {
                        IO.AddKeyEvent(key.Value, false);
                    }
                }
                catch { continue; }
            }

            UpdateModifierKey(IO, ImGuiKey.ModShift, Keys.ShiftKey);
            UpdateModifierKey(IO, ImGuiKey.ModCtrl, Keys.ControlKey);
            UpdateModifierKey(IO, ImGuiKey.ModAlt, Keys.Menu);
            UpdateModifierKey(IO, ImGuiKey.ModSuper, Keys.LWin);
        }

        public bool UpdateMouseState()
        {
            try
            {

                int LButton = WinApi.GetAsyncKeyState(Keys.LButton); IO.MouseDown[0] = (LButton != 0);

                int RButton = WinApi.GetAsyncKeyState(Keys.RButton); IO.MouseDown[1] = (RButton != 0);

                int MButton = WinApi.GetAsyncKeyState(Keys.MButton); IO.MouseDown[2] = (MButton != 0);

                int XButton1 = WinApi.GetAsyncKeyState(Keys.XButton1); IO.MouseDown[3] = (XButton1 != 0);

                int XButton2 = WinApi.GetAsyncKeyState(Keys.XButton2); IO.MouseDown[4] = (XButton2 != 0);

                return true;

            }
            catch { return false; }
        }

        public bool IsKeyDown(Keys VK)
        {
            return (GetAsyncKeyState(VK) & 0x8000) != 0;
        }

        #region " Private Methods "

        private char ConvertKeyToChar(Keys key)
        {
            var output = new StringBuilder(2);
            byte[] keyboardState = new byte[256];

            if (!GetKeyboardState(keyboardState))
                return '\0';

            uint virtualKey = (uint)key;
            uint scanCode = MapVirtualKey(virtualKey, 0);

            int result = ToUnicode(virtualKey, scanCode, keyboardState, output, output.Capacity, 0);
            if (result > 0)
            {
                return output[0];
            }

            return '\0';
        }

        private void UpdateModifierKey(ImGuiIOPtr IO, ImGuiKey imguiKey, Keys virtualKey)
        {
            try
            {
                int keyState = WinApi.GetAsyncKeyState(virtualKey);
                bool isKeyDown = (keyState & 0x8000) != 0;
                IO.AddKeyEvent(imguiKey, isKeyDown);
            }
            catch { return; }
        }

        #endregion

    }
}
