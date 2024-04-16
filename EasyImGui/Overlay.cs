using DearImguiSharp;
using EasyImGui.Core;
using GameOverlay.PInvoke;
using GameOverlay.Windows;
using RenderSpy.Overlay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EasyImGui
{
    public  class Overlay : D3D9Window, IDisposable
    {

        public ImGuiHook Imgui = null;
      
        public delegate void OnImguiReadyType(object sender, bool Status);
        public event OnImguiReadyType OnImGuiReady = null;

        public Overlay() {
            this.OnD3DReady += new OnDXCreated(OnDeviceReady);
        }

        private void OnDeviceReady(object sender, bool Status)
        {
             if (Status == true)
            {
                Imgui = new ImGuiHook();
                bool ImguiResult = true;
                if (Imgui.LoadCImgui() == true)
                {
                    Models.GraphicHook GH = Imgui.Attach(this.Handle);
                    if (GH.IsAttached == true)
                    {
                        ImguiResult = true;
                    }
                }
                OnImGuiReady?.Invoke(this, ImguiResult);
            }
        }

        /// <summary>
		/// Adapts to another window in the postion and size.
		/// </summary>
		/// <param name="windowHandle">The target window handle.</param>
		/// <param name="attachToClientArea">A Boolean determining whether to fit to the client area of the target window.</param>
		public void FitTo(IntPtr windowHandle, bool attachToClientArea = false)
        {
            WindowBounds rect;
            bool result = attachToClientArea ? WindowHelper.GetWindowClientBounds(windowHandle, out  rect) : WindowHelper.GetWindowBounds(windowHandle, out rect);

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

        private const int WS_EX_TOPMOST = 0x8;
        private const int WS_EX_NOACTIVATE = 0x8000000;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        public void Interactive(bool status)
        {
            IntPtr hwnd = this.Handle;
            int extendedStyle = (int)RenderSpy.Globals.WinApi.GetWindowLongPtr(hwnd, GWL_EXSTYLE);

            if (status)
            {
                RenderSpy.Globals.WinApi.SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)(extendedStyle & ~(WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE | WS_EX_TOPMOST)));
            }
            else
            {
                RenderSpy.Globals.WinApi.SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)(extendedStyle | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE | WS_EX_TOPMOST));
            }

        }

        protected override void WndProc(ref System.Windows.Forms.Message message)
        {
            if (Imgui != null && Imgui?.Imgui_Ini == true)
            {
                try
                {
                    uint uintMsg = (uint)message.Msg;
                    IntPtr intptrWParam = (IntPtr)message.WParam;

                    DearImguiSharp.ImGui.ImplWin32_WndProcHandlerEx(message.HWnd, uintMsg, intptrWParam, message.LParam);
                }
                catch (Exception Ex) { Helpers.ExceptionHandler.HandleException(Ex); return; }

            }
            base.WndProc(ref message);
        }

        #region " IDisposable implementation with finalizer "

        private bool isDisposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    this.Imgui.Dispose();
                    this.D3DDevice.Dispose();
                }
            }
            isDisposed = true;
        }

        #endregion

    }
}
