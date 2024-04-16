<h1 align="center">EasyImGui</h1>
<p align="center">
  <a href="https://github.com/DestroyerDarkNess/RenderSpy/blob/master/LICENSE">
    <img src="https://img.shields.io/github/license/Rebzzel/kiero.svg?style=flat-square"/>
  </a>
   <img src="https://img.shields.io/badge/platform-Windows-0078d7.svg"/>
  <br>
  Use ImGui with Winforms / .NET Framework.
  <br>
  💠 Please leave a Star to the repository ✅ if you liked it. ✨
</p>

- Install via NuGet Package :
  ```
  dotnet add package EasyImGui --version 1.0.2
  ```

# Example Projects

| Sample | Description       |
|----------|---------------|
| [EasyColorPicker](https://github.com/DestroyerDarkNess/EasyColorPicker)| A color picker made with EasyImGui |
| [EasyImGui.Samples](https://github.com/DestroyerDarkNess/EasyImGui/tree/main/Samples/EasyImGui.Samples) | Examples in C# and VB |

# Features


EasyImGui allows you to Create your GUI with ImGui, without having to configure anything, you don't need any additional knowledge.

You just need to install EasyImGui from NuGet and that's it! .

 *Note:*

**EasyImGui uses DearImGuiSharp, so any question related to ImGui should be asked in its official Reposttory:** [https://github.com/Sewer56/DearImguiSharp](https://github.com/Sewer56/DearImguiSharp)

If within your ImGui code you launch controls that block the Main UI, such as OpenFileDialog or MessageBox.Show, it will cause your application to fail, this is easy to solve, Simply do everything Asynchronously, Without Dialogs ```".ShowDialog()"```

## Example

### DX9 Overlay + Imgui Window.

```C
       Overlay OverlayWindow = new Overlay() { EnableDrag = false, ResizableBorders = true, NoActiveWindow = true,  Fix_WM_NCLBUTTONDBLCLK = true};

  //OverlayWindow.WindowStyles = WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST;

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

          OverlayWindow.Imgui.Render += delegate {


              if (OverlayWindow.FontSurfaces.Count != 0) { OverlayWindow.FontSurfaces[0].DrawText(null, "https://github.com/DestroyerDarkNess/RenderSpy" + Environment.NewLine + "Discord: Destroyer#8328", 0, 0, SharpDX.Color.Red); }

              //                  if (OverlayWindow.LineSurfaces.Count != 0) { OverlayWindow.LineSurfaces[0].Draw(new[] {
              //new RawVector2(100, 100),  // Superior izquierda
              //new RawVector2(20, 500),  // Superior derecha
              //}, Color.Red); }

              OverlayWindow.FitTo(GameProc.MainWindowHandle, true);
              OverlayWindow.PlaceAbove(GameProc.MainWindowHandle);

              if (DrawImguiMenu == false && OverlayWindow.Visible == true) { return true; }

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
                     
                      Task.Run(() =>
                      {
                          MessageBox.Show("Hello World!");
                      });

                  }

                  if (DearImguiSharp.ImGui.Button("Close Me", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                      DrawImguiMenu = false;

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

  Application.Run(OverlayWindow);
```

[![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/colored.png)](#table-of-contents)

### Preview

![Captura de pantalla 2024-01-16 194047](https://github.com/DestroyerDarkNess/EasyImGui/assets/32405118/1ce3f2e7-7480-4832-a459-a1397493a1a3)

![Captura de pantalla 2024-01-16 194153](https://github.com/DestroyerDarkNess/EasyImGui/assets/32405118/90fe71ea-9ec4-4f10-befe-2f95d83fac51)

![image](https://github.com/DestroyerDarkNess/EasyImGui/assets/32405118/9ba38227-e8c1-4e79-a949-55f5e83a8daf)

### Credits

[Sewer56](https://github.com/Sewer56/) : For [DearImguiSharp](https://github.com/Sewer56/DearImguiSharp) library.

[Veldrid Discord Server](https://discord.gg/s5EvvWJ) : Thanks to the community for the help, especially to [zaafar](https://github.com/zaafar/) and his [ClickableTransparentOverlay](https://github.com/zaafar/ClickableTransparentOverlay) project .

### License
```
MIT License

Copyright (c) 2019-2023 DestroyerDarkNess

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
