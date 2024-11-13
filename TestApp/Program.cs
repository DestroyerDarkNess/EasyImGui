
using EasyImGui;
using Hexa.NET.ImGui;
using RenderSpy.Globals;
using System;
using System.Drawing;
using System.Numerics;
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
                    var colors = style.Colors;

                    colors[(int)ImGuiCol.Text] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
                    colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
                    colors[(int)ImGuiCol.WindowBg] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
                    colors[(int)ImGuiCol.ChildBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
                    colors[(int)ImGuiCol.PopupBg] = new Vector4(0.19f, 0.19f, 0.19f, 0.92f);
                    colors[(int)ImGuiCol.Border] = new Vector4(0.19f, 0.19f, 0.19f, 0.29f);
                    colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.24f);
                    colors[(int)ImGuiCol.FrameBg] = new Vector4(0.05f, 0.05f, 0.05f, 0.54f);
                    colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.19f, 0.19f, 0.19f, 0.54f);
                    colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
                    colors[(int)ImGuiCol.TitleBg] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
                    colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.06f, 0.06f, 0.06f, 1.00f);
                    colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.06f, 0.06f, 0.06f, 1.00f);
                    colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
                    colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.05f, 0.05f, 0.05f, 0.54f);
                    colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.34f, 0.34f, 0.34f, 0.54f);
                    colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.40f, 0.40f, 0.40f, 0.54f);
                    colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.56f, 0.56f, 0.56f, 0.54f);
                    colors[(int)ImGuiCol.CheckMark] = new Vector4(0.33f, 0.67f, 0.86f, 1.00f);
                    colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.34f, 0.34f, 0.34f, 0.54f);
                    colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.56f, 0.56f, 0.56f, 0.54f);
                    colors[(int)ImGuiCol.Button] = new Vector4(0.05f, 0.05f, 0.05f, 0.54f);
                    colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.19f, 0.19f, 0.19f, 0.54f);
                    colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
                    colors[(int)ImGuiCol.Header] = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
                    colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.00f, 0.00f, 0.00f, 0.36f);
                    colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.20f, 0.22f, 0.23f, 0.33f);
                    colors[(int)ImGuiCol.Separator] = new Vector4(0.28f, 0.28f, 0.28f, 0.29f);
                    colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.44f, 0.44f, 0.44f, 0.29f);
                    colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.40f, 0.44f, 0.47f, 1.00f);
                    colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.28f, 0.28f, 0.28f, 0.29f);
                    colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.44f, 0.44f, 0.44f, 0.29f);
                    colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.40f, 0.44f, 0.47f, 1.00f);
                    colors[(int)ImGuiCol.Tab] = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
                    colors[(int)ImGuiCol.TabHovered] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
                    colors[(int)ImGuiCol.TabSelected] = new Vector4(0.20f, 0.20f, 0.20f, 0.36f);
                    colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
                    colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
                    colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.33f, 0.67f, 0.86f, 1.00f);
                    colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
                    colors[(int)ImGuiCol.PlotLines] = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
                    colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
                    colors[(int)ImGuiCol.PlotHistogram] = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
                    colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
                    colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
                    colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
                    colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.28f, 0.28f, 0.28f, 0.29f);
                    colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
                    colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
                    colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
                    colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.33f, 0.67f, 0.86f, 1.00f);
                    colors[(int)ImGuiCol.NavCursor] = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
                    colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 0.00f, 0.00f, 0.70f);
                    colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(1.00f, 0.00f, 0.00f, 0.20f);
                    colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.10f, 0.10f, 0.10f, 0.00f);

                    style.WindowPadding = new Vector2(8.00f, 8.00f);
                    style.FramePadding = new Vector2(5.00f, 2.00f);
                    style.CellPadding = new Vector2(6.00f, 6.00f);
                    style.ItemSpacing = new Vector2(6.00f, 6.00f);
                    style.ItemInnerSpacing = new Vector2(6.00f, 6.00f);
                    style.TouchExtraPadding = new Vector2(0.00f, 0.00f);
                    style.IndentSpacing = 25;
                    style.ScrollbarSize = 15;
                    style.GrabMinSize = 10;
                    style.WindowBorderSize = 1;
                    style.ChildBorderSize = 1;
                    style.PopupBorderSize = 1;
                    style.FrameBorderSize = 1;
                    style.TabBorderSize = 1;
                    style.WindowRounding = 7;
                    style.ChildRounding = 4;
                    style.FrameRounding = 3;
                    style.PopupRounding = 4;
                    style.ScrollbarRounding = 9;
                    style.GrabRounding = 3;
                    style.LogSliderDeadzone = 4;
                    style.TabRounding = 4;

                    // When viewports are enabled we tweak WindowRounding/WindowBg so platform windows can look identical to regular ones.
                    if ((OverlayWindow.ImguiManager.IO.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
                    {
                        style.WindowRounding = 0.0f;
                        style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
                    }

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
