using DearImguiSharp;
using EasyImGui.Core;
using GameOverlay.PInvoke;
using RenderSpy.ImGui;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyImGui
{
    public class Test
    {

        public static void DrawCircle(int x, int y, int radius, RawColorBGRA color, float thickness = 1.0f)
        {
            if (Test.GOverlay.LineSurfaces.Count != 0)
            {
                // Calculate circle points using Bresenham's circle algorithm
                List<RawVector2> circlePoints = GetCirclePoints(x, y, radius);

                // Draw lines using existing Draw functionality (assuming Draw is available)
                foreach (var pointPair in SplitCirclePoints(circlePoints))
                {
                    Draw(pointPair[0], pointPair[1], color, thickness);
                }
            }
        }

        private static List<RawVector2> GetCirclePoints(int xCenter, int yCenter, int radius)
        {
            List<RawVector2> circlePoints = new List<RawVector2>();
            int x = 0;
            int y = radius;
            int decisionOver2 = 1 - radius;

            while (y >= x)
            {
                circlePoints.Add(new RawVector2(xCenter + x, yCenter + y));
                circlePoints.Add(new RawVector2(xCenter - x, yCenter + y));
                circlePoints.Add(new RawVector2(xCenter + x, yCenter - y));
                circlePoints.Add(new RawVector2(xCenter - x, yCenter - y));
                circlePoints.Add(new RawVector2(xCenter + y, yCenter + x));
                circlePoints.Add(new RawVector2(xCenter - y, yCenter + x));
                circlePoints.Add(new RawVector2(xCenter + y, yCenter - x));
                circlePoints.Add(new RawVector2(xCenter - y, yCenter - x));

                x++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * x + 1;
                }
                else
                {
                    y--;
                    decisionOver2 += 2 * (x - y) + 1;
                }
            }

            return circlePoints;
        }

        private static IEnumerable<RawVector2[]> SplitCirclePoints(List<RawVector2> points)
        {
            for (int i = 0; i < points.Count; i += 2)
            {
                yield return new RawVector2[] { points[i], points[(i + 1) % points.Count] };
            }
        }

        private static void Draw(RawVector2 Raw1, RawVector2 Raw2, RawColorBGRA Color, float thickness)
        {
            if (Test.GOverlay.LineSurfaces.Count != 0)
            {
                Test.GOverlay.LineSurfaces[0].Draw(new[] {
            Raw1,
            Raw2,
        }, Color);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            public Vector4 Position;
            public SharpDX.ColorBGRA Color;
        }

        

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public static Overlay GOverlay;



       


        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

        public static bool IsKeyDown(Keys VK)
        {
            return (GetAsyncKeyState(VK) & 0x8000) != 0;
        }

        // Yah U Ah
        static void Main(string[] args)
        {
            GameOverlay.TimerService.EnableHighPrecisionTimers();
            var example = new Example();

            Overlay OverlayWindow = new Overlay() { EnableDrag = false, ResizableBorders = true, NoActiveWindow = true, Fix_WM_NCLBUTTONDBLCLK = false };
            GOverlay = OverlayWindow;

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

                    Process GameProc = Process.GetProcessesByName("CodeSmart").FirstOrDefault();
                    bool invalidate = false;
                    OverlayWindow.Imgui.Render += delegate {

                        System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(10, System.Drawing.Color.Red);
                        RawColorBGRA fovcol = new RawColorBGRA(drawingColor.B , drawingColor.G , drawingColor.R , drawingColor.A );

                        DrawCircle((int)OverlayWindow.Right / 2, (int)OverlayWindow.Bottom / 2, (int)(140 + 1f), fovcol);


                        if (OverlayWindow.FontSurfaces.Count != 0) { OverlayWindow.FontSurfaces[0].DrawText(null, "https://github.com/DestroyerDarkNess/RenderSpy" + Environment.NewLine + "Discord: Destroyer#8328", 0, 0, SharpDX.Color.Red); }

                        //                  if (OverlayWindow.LineSurfaces.Count != 0) { OverlayWindow.LineSurfaces[0].Draw(new[] {
                        //new RawVector2(100, 100),  // Superior izquierda
                        //new RawVector2(20, 500),  // Superior derecha
                        //}, Color.Red); }

                        OverlayWindow.FitTo(GameProc.MainWindowHandle, true);
                        OverlayWindow.PlaceAbove(GameProc.MainWindowHandle);

                        if (IsKeyDown( Keys.Insert)) { DrawImguiMenu = !DrawImguiMenu; }

                        OverlayWindow.Interactive(DrawImguiMenu);

                        if (DrawImguiMenu == false) { return true; }

                        //Debug.WriteLine(((SetWindowStyleW.WindowStyles)OverlayPreserveStyles).ToString());

                        if (OverlayWindow.Imgui.Imgui_Ini == true && OverlayWindow.Imgui.IO != null)
                        {

                            // Overlay Dragger
                            //bool IsFocusOnMainImguiWindow = (Form.ActiveForm == OverlayWindow); // Old : DearImguiSharp.ImGui.IsWindowFocused((int)DearImguiSharp.ImGuiFocusedFlags.RootWindow);
                            //if (IsFocusOnMainImguiWindow == true) { InputHook.Universal(OverlayWindow.Imgui.IO); }
                            //OverlayWindow.EnableDrag =  (DearImguiSharp.ImGui.IsAnyItemActive() == true) ? false : IsFocusOnMainImguiWindow;



                            //DearImguiSharp.ImGui.SetNextWindowPos(new ImVec2 { X = 500, Y = 50 }, 0, new ImVec2 { X = 0, Y = 0 });
                            //DearImguiSharp.ImGui.SetNextWindowSize(new ImVec2() { X = OverlayWindow.ClientSize.Width -100, Y = OverlayWindow.ClientSize.Height -100}, 0);

                            DearImguiSharp.ImGui.Begin(OverlayWindow.Text, ref DrawImguiMenu, 0); // (int)ImGuiWindowFlags.NoResize | (int)ImGuiWindowFlags.NoMove | (int)ImGuiWindowFlags.NoCollapse | (int)ImGuiWindowFlags.NoBringToFrontOnFocus

                            if (DearImguiSharp.ImGui.Button("Message", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                            {
                               
                                //SetParent(example._window.Handle , OverlayWindow.Handle   );
                                //Task.Run(() =>
                                //{
                                //    MessageBox.Show("Hello World!");
                                //});

                            }

                            if (DearImguiSharp.ImGui.Button("Close Me", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                            {
                                DrawImguiMenu = false;
                            }
                              

                            ImGui.Text("Hola ImGui!" );
                            ImGui.TextColored(new ImVec4() { W= 14, X = 14, Y = 14, Z = 14 }, "Texto verde");
                            ImGui.TextWrapped("Este texto se ajusta al ancho disponible");

                            // Líneas
                            ImGui.Separator();
                            ImGui.NewLine();
                            ImGui.ImDrawListAddPolyline(ImGui.GetWindowDrawList(), new ImVec2() { X = 100, Y = 100 },1 , 5, 5 , 4.5f);

                            // Círculos
                            ImGui.ImDrawListAddCircle(ImGui.GetWindowDrawList(), new ImVec2() { X = 100, Y = 100 }, 50, 5, 4 , 4.5f);
                           
                            DearImguiSharp.ImGui.End();
                        }


                        return true; 
                    
                    };

                }
            };

            //Task.Run(() =>
            //{
            //    example.Run();
            //});

            Application.Run(OverlayWindow);

        }


    }
}
