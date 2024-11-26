
using EasyImGui;
using EasyImGui.Core;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Widgets;
using SharpDX.Direct3D9;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace TestApp
{
    internal class Program
    {

        public enum OverlayMode
        {
            Normal = 0, // Normal mode, without the features of an overlay.
            InGame = 1, // This mode is the classic overlay, totally external but when interacting with the imgui window the game will lose focus.
            InGameEmbed = 2 // This mode requires a WndProc hook to the game process, and its behavior causes the game to not lose focus from the window.
        }


        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_NOACTIVATE = 0x00080000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        public static OverlayMode overlayMode = OverlayMode.Normal; // Use this to change the overlay mode

        static void Main(string[] args)
        {
            bool result = Diagnostic.RunDiagnostic();

            if (result)
            {
                Console.WriteLine("All diagnostics passed. The system is ready.");
            }
            else
            {
                Console.WriteLine("Some diagnostics failed. Please resolve the missing libraries, Press any key to continue.");
                Console.ReadKey();
            }

            Process GameProc = Process.GetProcessesByName("hl").FirstOrDefault(); //InGameEmbed Tested on Counter Striker 1.6, Battlefield 4, notepad XD.

            if (GameProc == null && (overlayMode == OverlayMode.InGame || overlayMode == OverlayMode.InGameEmbed))
            {
                System.Windows.Forms.MessageBox.Show("Game not running.");
                return;
            }

            bool UseCustomD3dDevice = true;

            using (Overlay OverlayWindow = new Overlay() { EnableDrag = false, ResizableBorders = true, ShowInTaskbar = false, AutoInitialize = !UseCustomD3dDevice })
            {

                OverlayWindow.ImguiManager.Architecture = Runtimes.Architecture.Auto;

                OverlayWindow.PresentParams = new SharpDX.Direct3D9.PresentParameters
                {
                    Windowed = true,
                    SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard,
                    BackBufferFormat = Format.X8R8G8B8,
                    PresentationInterval = PresentInterval.Immediate,
                    EnableAutoDepthStencil = false,
                    AutoDepthStencilFormat = Format.Unknown,
                    MultiSampleType = SharpDX.Direct3D9.MultisampleType.None,
                    MultiSampleQuality = 0
                };

                // Using Custom D3D Device
                if (UseCustomD3dDevice)
                {
                    OverlayWindow.Load += (sender, e) =>
                    {
                        OverlayWindow.D3DDevice = new DeviceEx(new Direct3DEx(), 0, DeviceType.Hardware, OverlayWindow.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded, OverlayWindow.PresentParams);
                        OverlayWindow.InitializeD3D();
                    };
                }

                if (overlayMode == OverlayMode.InGame || overlayMode == OverlayMode.InGameEmbed)
                {
                    OverlayWindow.Opacity = 0.8;
                    OverlayWindow.NoActivateWindow = true;
                    OverlayWindow.AdditionalExStyle |= WS_EX_TOPMOST | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                    OverlayWindow.Text = GameProc.MainWindowTitle;
                    OverlayWindow.GameWindowHandle = GameProc.MainWindowHandle;
                }

                ImFontPtr HaloFont = null;
                WidgetDemo widgetDemo = null;
                bool UseCustomImguiCursor = true;
                InputImguiEmu inputEmulation = null;

                OverlayWindow.ImguiManager.ConfigContex += delegate
                {
                    // setup fonts.
                    //if (System.IO.File.Exists("assets/halo.ttf"))
                    //{
                    //    ImGuiFontBuilder builder = new ImGuiFontBuilder();
                    //    builder.AddFontFromFileTTF("assets/halo.ttf", 12);
                    //    HaloFont = builder.Build();

                    //    Console.WriteLine("assets/halo.ttf Loaded!");
                    //}

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
                    colors[(int)ImGuiCol.TitleBg] = new Vector4(0.06f, 0.06f, 0.06f, 1.00f);
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
                    style.TabBorderSize = 1;
                    style.ChildRounding = 4;
                    style.PopupRounding = 4;
                    style.ScrollbarRounding = 9;
                    style.GrabRounding = 3;
                    style.LogSliderDeadzone = 4;
                    style.TabRounding = 4;
                    //style.WindowRounding = 5.0f;
                    //style.FrameRounding = 5.0f;
                    //style.FrameBorderSize = 1.0f;

                    // When viewports are enabled we tweak WindowRounding/WindowBg so platform windows can look identical to regular ones.
                    if ((OverlayWindow.ImguiManager.IO.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
                    {
                        style.WindowRounding = 0.0f;
                        style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
                    }



                    if (overlayMode == OverlayMode.InGame || overlayMode == OverlayMode.InGameEmbed)
                    {
                        OverlayWindow.ImguiManager.IO.ConfigFlags &= ~Hexa.NET.ImGui.ImGuiConfigFlags.ViewportsEnable;
                        if (UseCustomImguiCursor) OverlayWindow.ImguiManager.IO.ConfigFlags &= ~ImGuiConfigFlags.NoMouseCursorChange;
                    }

                    if (overlayMode == OverlayMode.Normal)
                    {
                        //SET Overlay Location and Size
                        OverlayWindow.Location = new System.Drawing.Point(0, 0);
                        OverlayWindow.Size = new System.Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    }

                    widgetDemo = new WidgetDemo();
                    //ImGuiGC.Init(); ???

                    // Disable Debug Mode
                    OverlayWindow.ImguiManager.IO.ConfigDebugIsDebuggerPresent = false;
                    OverlayWindow.ImguiManager.IO.ConfigErrorRecoveryEnableAssert = false;

                    inputEmulation = new InputImguiEmu(OverlayWindow.ImguiManager.IO);

                    return true;
                };

                bool SetChildWindow = false;
                OverlayWindow.OnImGuiReady += (object sender, bool Status) =>
                {
                    if (Status)
                    {

                        bool DrawImguiMenu = true;

                        SharpDX.Direct3D9.Device device = OverlayWindow.D3DDevice;

                        // For Normal Mode
                        //OverlayWindow.Keyboard += (key, isKeyDown) =>
                        //{
                        //    if (key == Keys.Insert && isKeyDown)
                        //    {
                        //        DrawImguiMenu = !DrawImguiMenu;
                        //    }
                        //};


                        DateTime lastToggleTime = DateTime.MinValue;
                        bool isInsertKeyDown = false;
                        bool wasInsertKeyDown = false;

                        OverlayWindow.ImguiManager.Render += delegate
                        {
                            if ((overlayMode == OverlayMode.InGame || overlayMode == OverlayMode.InGameEmbed) && GameProc.HasExited) Environment.Exit(0);

                            if (overlayMode == OverlayMode.InGameEmbed && !SetChildWindow)
                            {
                                SetChildWindow = true;
                                OverlayWindow.MakeOverlayChild(OverlayWindow.Handle, OverlayWindow.GameWindowHandle);
                            }

                            if (overlayMode == OverlayMode.InGame || overlayMode == OverlayMode.InGameEmbed)
                            {
                                OverlayWindow.Location = new System.Drawing.Point(0, 0);
                                OverlayWindow.FitTo(GameProc.MainWindowHandle, true);
                                OverlayWindow.PlaceAbove(GameProc.MainWindowHandle);
                            }

                            if (overlayMode == OverlayMode.InGameEmbed && UseCustomImguiCursor)
                            {
                                OverlayWindow.ImguiManager.IO.MouseDrawCursor = DrawImguiMenu;
                            }

                            if (overlayMode == OverlayMode.InGameEmbed && inputEmulation != null)
                            {
                                OverlayWindow.ImguiManager.IO.MouseDrawCursor = DrawImguiMenu;
                                inputEmulation.UpdateMouseState();
                                inputEmulation.UpdateKeyboardState();
                            }


                            isInsertKeyDown = inputEmulation.IsKeyDown(Keys.Insert);

                            if (isInsertKeyDown && !wasInsertKeyDown && (DateTime.Now - lastToggleTime).TotalMilliseconds > 100)
                            {
                                DrawImguiMenu = !DrawImguiMenu;
                                lastToggleTime = DateTime.Now;
                            }

                            wasInsertKeyDown = isInsertKeyDown;

                            // your Imgui Logic Here...
                            // DrawImguiMenu = true;

                            Hexa.NET.ImGui.ImGui.ShowDemoWindow();

                            if (DrawImguiMenu)
                            {
                                WidgetManager.Draw();

                                ImGui.Begin("Hello World", ref DrawImguiMenu);

                                //if (!HaloFont.IsNull) ImGui.PushFont(HaloFont);

                                ImGui.Text("Thank you for using EasyImgui");

                                //if (!HaloFont.IsNull) ImGui.PopFont();

                                widgetDemo.DrawContent();

                                ImGui.End();
                            }

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

                try { Application.Run(OverlayWindow); } catch (Exception Ex) { System.Windows.Forms.MessageBox.Show(Ex.Message); Environment.Exit(0); }
            }

        }


    }
}
