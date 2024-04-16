using DearImguiSharp;
using EasyImGui;
using EasyImGui.Core;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImguiSharp
{
    class Program
    {
        static void Main(string[] args)
        {

            Overlay OverlayWindow = new Overlay() { EnableDrag = true, ResizableBorders = true,  Fix_WM_NCLBUTTONDBLCLK = true };

            //SingleImguiWindow.ImGuiWindowFlagsEx = DearImguiSharp.ImGuiWindowFlags.NoTitleBar;
            OverlayWindow.OnImGuiReady += (object sender, bool Status) =>
            {

                if (Status)
                {
                    OverlayWindow.Imgui.ConfigContex += delegate {

                        //DearImguiSharp.ImGui.StyleColorsLight(null);

                        var style = ImGui.GetStyle();

                        ImVec4[] styleColors = style.Colors;
                        styleColors[(int)ImGuiCol.CheckMark].W = 1.0f;
                        styleColors[(int)ImGuiCol.CheckMark].X = 1.0f;
                        styleColors[(int)ImGuiCol.CheckMark].Y = 1.0f;
                        styleColors[(int)ImGuiCol.CheckMark].Z = 1.0f;

                        styleColors[(int)ImGuiCol.FrameBg].W = 0.0f;
                        styleColors[(int)ImGuiCol.FrameBg].X = 0.0f;
                        styleColors[(int)ImGuiCol.FrameBg].Y = 0.0f;
                        styleColors[(int)ImGuiCol.FrameBg].Z = 0.0f;

                        style.WindowRounding = 5.0f;
                        style.FrameRounding = 5.0f;
                        style.FrameBorderSize = 1.0f;



                        //OverlayWindow.Imgui.IO.ConfigFlags |= (int)ImGuiConfigFlags.ViewportsEnable;

                        return true;
                    };


                    bool DrawImguiMenu = true;

                    SharpDX.Direct3D9.Device device = OverlayWindow.D3DDevice;

                    OverlayWindow.Imgui.Render += delegate {


                        if (DrawImguiMenu == false) { OverlayWindow.Close();  return true; }

                        if (OverlayWindow.Imgui.Imgui_Ini == true && OverlayWindow.Imgui.IO != null)
                        {

                            // Overlay Dragger
                            bool IsFocusOnMainImguiWindow = (Form.ActiveForm == OverlayWindow); // Old : DearImguiSharp.ImGui.IsWindowFocused((int)DearImguiSharp.ImGuiFocusedFlags.RootWindow);
                            if (IsFocusOnMainImguiWindow == true) { InputHook.Universal(OverlayWindow.Imgui.IO); }
                            OverlayWindow.EnableDrag = (DearImguiSharp.ImGui.IsAnyItemActive() == true) ? false : IsFocusOnMainImguiWindow;

                            DearImguiSharp.ImGui.SetNextWindowPos(new ImVec2 { X = 0, Y = 0 }, 0, new ImVec2 { X = 0, Y = 0 });
                            DearImguiSharp.ImGui.SetNextWindowSize(new ImVec2() { X = OverlayWindow.ClientSize.Width , Y = OverlayWindow.ClientSize.Height  }, 0);

                            DearImguiSharp.ImGui.Begin(OverlayWindow.Text, ref DrawImguiMenu, 0); // (int)ImGuiWindowFlags.NoResize | (int)ImGuiWindowFlags.NoMove | (int)ImGuiWindowFlags.NoCollapse | (int)ImGuiWindowFlags.NoBringToFrontOnFocus

                            if (DearImguiSharp.ImGui.Button("Message", new DearImguiSharp.ImVec2() { X = OverlayWindow.ClientSize.Width - 15, Y = 20 }))
                            {
                                Task.Run(() =>
                                {
                                    MessageBox.Show("Hello World!");
                                });
                            }

                            if (DearImguiSharp.ImGui.Button("Exit", new DearImguiSharp.ImVec2() { X = OverlayWindow.ClientSize.Width - 15, Y = 20 }))
                                OverlayWindow.Close();

                        }

                        return true;
                    };

                }
            };

            try { Application.Run(OverlayWindow); } catch { Environment.Exit(0); }
          

        }
    }
}
