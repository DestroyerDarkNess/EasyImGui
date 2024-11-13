
using EasyImGui;
using Hexa.NET.ImGui;
using RenderSpy.Globals;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Overlay OverlayWindow = new Overlay() { EnableDrag = false, ResizableBorders = true, ShowInTaskbar = false })
            {
                OverlayWindow.ImguiManager.ConfigContex += delegate
                {

                    var style = ImGui.GetStyle();

                    style.Colors[(int)ImGuiCol.CheckMark].W = 1.0f;
                    style.Colors[(int)ImGuiCol.CheckMark].X = 1.0f;
                    style.Colors[(int)ImGuiCol.CheckMark].Y = 1.0f;
                    style.Colors[(int)ImGuiCol.CheckMark].Z = 1.0f;

                    style.Colors[(int)ImGuiCol.FrameBg].W = 0.0f;
                    style.Colors[(int)ImGuiCol.FrameBg].X = 0.0f;
                    style.Colors[(int)ImGuiCol.FrameBg].Y = 0.0f;
                    style.Colors[(int)ImGuiCol.FrameBg].Z = 0.0f;

                    style.WindowRounding = 5.0f;
                    style.FrameRounding = 5.0f;
                    style.FrameBorderSize = 1.0f;

                    //OverlayWindow.ImguiManager.IO.ConfigFlags &= ~Hexa.NET.ImGui.ImGuiConfigFlags.ViewportsEnable;

                    OverlayWindow.Size = new Size(800, 600);

                    return true;
                };

                OverlayWindow.OnImGuiReady += (object sender, bool Status) =>
                {
                    if (Status)
                    {

                        bool DrawImguiMenu = true;

                        SharpDX.Direct3D9.Device device = OverlayWindow.D3DDevice;

                        OverlayWindow.ImguiManager.Render += delegate
                        {
                            DrawImguiMenu = true;

                            // your Imgui Logic Here...
                            Hexa.NET.ImGui.ImGui.ShowDemoWindow(ref DrawImguiMenu);

                            // Enables or disables interaction with the overlay. 
                            OverlayWindow.Interactive(DrawImguiMenu);

                            // If you use OverlayWindow.EnableDrag = true, you will need to create the logic to interact with the menu, here is an example:
                            //bool IsFocusOnMainImguiWindow = (Form.ActiveForm == OverlayWindow);
                            //if (IsFocusOnMainImguiWindow == true) { Universal(OverlayWindow.ImguiManager.IO); }
                            //OverlayWindow.EnableDrag = (ImGui.IsAnyItemActive() == true) ? false : IsFocusOnMainImguiWindow;

                            return true;
                        };

                    }
                };

                try { Application.Run(OverlayWindow); } catch (Exception Ex) { MessageBox.Show(Ex.Message); Environment.Exit(0); }
            }

        }

        public static bool Universal(Hexa.NET.ImGui.ImGuiIOPtr IO)
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
            catch (Exception Ex) { Console.WriteLine(Ex.Message); return false; }
        }
    }
}
