using EasyImGui.Core;
using EasyImGui.Core.PInvoke;
using RenderSpy.Overlay;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EasyImGui
{
    public class Overlay : D3D9Window, IDisposable
    {

        #region " Properties "

        public ImguiManager ImguiManager = new ImguiManager();

        #endregion

        #region " Events "

        public delegate void OnImguiReadyType(object sender, bool Status);
        public event OnImguiReadyType OnImGuiReady = null;

        #endregion

        #region " Constructor "

        public Overlay()
        {
            this.OnD3DReady += new OnDXCreated(OnDeviceReady);
        }

        #endregion

        private void OnDeviceReady(object sender, bool Status)
        {
            if (Status == true)
            {
                while (this.Visible == false) Application.DoEvents();
                bool ImguiResult = ImguiManager.Initialize(this.Handle);
                OnImGuiReady?.Invoke(this, ImguiResult);
            }
        }

        #region " WndProc "

        public delegate void KeyboardEventHandler(Keys key, bool isKeyDown);
        public event KeyboardEventHandler Keyboard = null;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_ACTIVATE = 0x0006;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public IntPtr GameWindowHandle = IntPtr.Zero;

        protected override void WndProc(ref System.Windows.Forms.Message message)
        {

            if (GameWindowHandle != IntPtr.Zero)
            {
                switch (message.Msg)
                {
                    case WM_KEYDOWN:
                        Keyboard?.Invoke((Keys)message.WParam, true);
                        break;

                    case WM_KEYUP:
                        Keyboard?.Invoke((Keys)message.WParam, false);
                        break;

                    case WM_MOUSEACTIVATE:
                        message.Result = (IntPtr)3; // MA_NOACTIVATE
                        return;

                    case WM_SETFOCUS:
                        return;

                    case WM_ACTIVATE:
                        SetForegroundWindow(GameWindowHandle);
                        return;
                }
            }

            if (ImguiManager.Imgui_Alive)
            {
                try
                {
                    unsafe
                    {
                        Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32.WndProcHandler(
                            message.HWnd,
                            (uint)message.Msg,
                            new UIntPtr(message.WParam.ToPointer()),
                            message.LParam
                        );
                    }
                }
                catch (Exception ex)
                {
                    Helpers.WriteException(ex);
                }
            }

            try { base.WndProc(ref message); } catch (Exception ex) { Helpers.WriteException(ex); }

        }

        #endregion

        #region " Methods "

        /// <summary>
        /// Adapts to another window in the postion and size.
        /// </summary>
        /// <param name="windowHandle">The target window handle.</param>
        /// <param name="attachToClientArea">A Boolean determining whether to fit to the client area of the target window.</param>
        public void FitTo(IntPtr windowHandle, bool attachToClientArea = false)
        {
            WindowBounds rect;
            bool result = attachToClientArea ? WindowHelper.GetWindowClientBounds(windowHandle, out rect) : WindowHelper.GetWindowBounds(windowHandle, out rect);

            if (result)
            {
                int X = this.Location.X;
                int Y = this.Location.Y;
                int x = rect.Left;
                int y = rect.Top;
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (X != x
                    || Y != y
                    || Width != width
                    || Height != height)
                {
                    this.Location = new Point(x, y);
                    this.Size = new Size(width, height);
                }
            }
        }

        /// <summary>
        /// Places the OverlayWindow above the target window according to the windows z-order.
        /// </summary>
        /// <param name="windowHandle">The target window handle.</param>
        public void PlaceAbove(IntPtr windowHandle)
        {
            var windowAboveParentWindow = User32.GetWindow(windowHandle, WindowCommand.Previous);

            if (windowAboveParentWindow != this.Handle)
            {
                User32.SetWindowPos(
                    this.Handle,
                    windowAboveParentWindow,
                    0, 0, 0, 0,
                    SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize | SwpFlags.AsyncWindowPos);
            }
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        public void Interactive(bool status)
        {
            IntPtr hwnd = this.Handle;

            int extendedStyle = (int)RenderSpy.Globals.WinApi.GetWindowLongPtr(hwnd, GWL_EXSTYLE);

            if (status)
            {
                RenderSpy.Globals.WinApi.SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)(extendedStyle & ~WS_EX_TRANSPARENT));
            }
            else
            {
                RenderSpy.Globals.WinApi.SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)(extendedStyle | WS_EX_TRANSPARENT));
            }
        }


        #endregion

        #region IDisposable implementation with finalizer

        private bool isDisposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    ImguiManager.Dispose();
                }

            }

            isDisposed = true;

            base.Dispose(disposing);
        }

        #endregion


    }
}
