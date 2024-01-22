using DearImguiSharp;
using EasyImGui.Core;
using EasyImGui.Helpers;
using RenderSpy.Globals;
using RenderSpy.Overlay;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EasyImGui.Core.ImGuiHook;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EasyImGui
{
    public class ImGuiForm : D3D9Window, IDisposable
    {

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        public event RenderUI_EventHandler Render = delegate {
            return true; 
        };

        private InputHook InputHooks = null;
        public Core.ImGuiHook Imgui = null;

        public ImGuiWindowFlags ImGuiWindowFlagsEx = ImGuiWindowFlags.None;
        public ImGuiConfigFlags ImGuiConfigFlagsEx = ImGuiConfigFlags.None;

        public ImGuiForm() : base(true, true)
        {
            this.AutoReset = true;
            this.Fix_WM_NCLBUTTONDBLCLK = false;
            this.Text = "EasyImgui";
            this.VisibleChanged += this.ImGuiForm_VisibleChanged;
        }

        public ImGuiForm(bool Transparent = true, bool Clickable = true) : base(Transparent, Clickable)
        {
            this.AutoReset = true;
            this.Fix_WM_NCLBUTTONDBLCLK = false;
        }

        private bool DrawMenu = true;
        public bool DragWindow = true;

        private void ImGuiForm_VisibleChanged(object sender, EventArgs e)
        {
            DrawMenu = this.Visible;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Text = "EasyImgui";
            this.VisibleChanged += this.ImGuiForm_VisibleChanged;
            
            if (D3DDevice == null)
            {
                try
                {
                    this.CreateDevice();

                    if (D3DDevice != null)
                    {

                        Imgui = new Core.ImGuiHook();

                        if (Imgui.LoadCImgui() == true)
                        {
                            Models.Graphichook GH = Imgui.Attach(this.Handle);

                            if (GH.IsAttached == true)
                            {

                                Imgui.ConfigContexEvent += delegate {

                                    if (ImGuiConfigFlagsEx != ImGuiConfigFlags.None)
                                    Imgui.IO.ConfigFlags = (int)ImGuiConfigFlagsEx;

                                    return true;
                                };

                                Imgui.RenderEvent += delegate {

                                    DearImguiSharp.ImGui.SetNextWindowPos(new ImVec2 { X = 0, Y = 0 }, 0, new ImVec2 { X = 0, Y = 0 });
                                    DearImguiSharp.ImGui.SetNextWindowSize(new ImVec2() { X = this.ClientSize.Width, Y = this.ClientSize.Height }, 0);

                                    DearImguiSharp.ImGui.Begin(this.Text, ref DrawMenu, (int)ImGuiWindowFlags.NoResize | (int)ImGuiWindowFlags.NoMove | (int)ImGuiWindowFlags.NoCollapse | (int)ImGuiWindowFlags.NoBringToFrontOnFocus | (int)ImGuiWindowFlagsEx);

                                    Render();


                                    DearImguiSharp.ImGui.End();

                                    return true;
                                }; ;
                              
                            }

                        }

                        RenderLoop.Run(this, () =>
                        {

                            try
                            {
                                if (this.IsAlive == false)
                                    return;

                                D3DDevice.Clear(ClearFlags.Target, this.ClearColor, 1.0f, 0);
                                D3DDevice.BeginScene();

                                this.Surface();

                                if (Imgui.Imgui_Ini == true && Imgui.IO != null)
                                {

                                    if (Form.ActiveForm == this)
                                        InputHook.Universal(Imgui.IO);

                                    this.EnableDrag = (DragWindow == true && DearImguiSharp.ImGui.IsAnyItemActive() == false) ? true : false;

                                }


                                D3DDevice.EndScene();
                                D3DDevice.Present();
                            }
                            catch (SharpDXException ex)
                            {
                                bool IsPathAvailable = this.Patch(ex);
                                if (IsPathAvailable == false)
                                {
                                    throw ex;
                                }

                            }

                        });


                    }
                    else
                    {
                        throw new Exception(this.D3dError.Message);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "D3D Error");
                }
            }

            if (HideRenderWindow == true)
            {
                ShowWindow(this.Handle, 0);
            }
            base.OnLoad(e);
        }


        protected override void WndProc(ref Message message)
        {
            if (Imgui?.Imgui_Ini == true && this.DrawMenu == true)
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


        #region IDisposable implementation with finalizer
        private bool isDisposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    //if (_wplayer != null) _wplayer.Dispose();
                }
            }
            isDisposed = true;
        }
        #endregion


    }
}
