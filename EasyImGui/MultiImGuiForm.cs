using DearImguiSharp;
using EasyImGui.Core;
using RenderSpy.Overlay;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EasyImGui.Core.ImGuiHook;

namespace EasyImGui
{
    public class MultiImGuiForm : D3D9Window, IDisposable
    {

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public event RenderUI_EventHandler OnReady = delegate {
            return true;
        };

        public event RenderUI_EventHandler Render = delegate {
            return true;
        };

        public bool Draw = true;
        public Core.ImGuiHook Imgui = null;

        public MultiImGuiForm() : base(false, true)
        {
            AutoReset = true;
            HideRenderWindow = true; 
            EnableDrag = false;
        }

        public MultiImGuiForm(bool Transparent = true, bool Clickable = true) : base(Transparent, Clickable)
        {
          
        }

        private void ImGuiForm_VisibleChanged(object sender, EventArgs e)
        {
            Draw = this.Visible;
        }


        protected override void OnLoad(EventArgs e)
        {
            this.Text = "EasyImgui";
            this.VisibleChanged += this.ImGuiForm_VisibleChanged;
            base.OnLoad(e);
        }


        protected override void OnShown(EventArgs e)
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

                                if (this.HideRenderWindow == true)
                                {

                                    ShowWindow(this.Handle, 0);
                                }

                                Imgui.IO.ConfigFlags |= (int)ImGuiConfigFlags.ViewportsEnable;

                                return true;
                            };

                            Imgui.RenderEvent += delegate { return Render(); };
                           
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

            OnReady();
            base.OnShown(e);
        }


    }
}
