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
  dotnet add package EasyImGui --version 1.0.0
  ```

# Example Projects

| Sample | Description       |
|----------|---------------|
| [EasyImGui.Samples](https://github.com/DestroyerDarkNess/EasyImGui/tree/main/Samples/EasyImGui.Samples) | Examples in C# and VB |

# Features


EasyImGui allows you to Create your GUI with ImGui, without having to configure anything, you don't need any additional knowledge.

You just need to install EasyImGui from NuGet and that's it! .

**EasyImGui has two Imgui Views:**

1) **ImGuiForm:** A Single View, It does not support multi-Windows, and to work, it must be Launched with:
  ```Application.Run(SingleImguiWindow);``` .

2) **MultiImGuiForm:** Allows Multiple Imgui Windows, It can be launched normally with ```.Show()```, It has no restrictions and using WinAPIS, you could embed the windows in Controls Panel or in other Forms.

 *Note:*

If within your ImGui code you launch controls that block the Main UI, such as OpenFileDialog or MessageBox.Show, it will cause your application to fail, this is easy to solve, Simply do everything Asynchronously, Without Dialogs ```".ShowDialog()"```

## Example

### Single Imgui Window.

```C
            Thread AsyncThread = new Thread(() =>
            {
                ImGuiForm SingleImguiWindow = new ImGuiForm(true, true);
                //SingleImguiWindow.ImGuiWindowFlagsEx = DearImguiSharp.ImGuiWindowFlags.NoTitleBar;

                SingleImguiWindow.Render += delegate
                {

                    if (DearImguiSharp.ImGui.Button("Message", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                    {
                        Task.Run(() =>
                        {
                            MessageBox.Show("Hello World!");
                        });
                    }

                    if (DearImguiSharp.ImGui.Button("Close Me", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                        SingleImguiWindow.Visible = false;

                    return true;
                };

                Application.Run(SingleImguiWindow);
            });

            AsyncThread.SetApartmentState(ApartmentState.STA);
            AsyncThread.Start();
```

### Multi Imgui Window.

```C
            bool ShowImguiDemo = false;
            MultiImGuiForm MultiForm = new MultiImGuiForm();
            MultiForm.Render += delegate
            {
                if (!MultiForm.Draw)
                    return false;

                DearImguiSharp.ImGui.Begin("Another Window in C#", ref MultiForm.Draw, (int)DearImguiSharp.ImGuiWindowFlags.None);
                DearImguiSharp.ImGui.Text("Hello from another window!");

                if (DearImguiSharp.ImGui.Button("ImguiDemo", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                    ShowImguiDemo = !ShowImguiDemo;

                if (ShowImguiDemo)
                    DearImguiSharp.ImGui.ShowDemoWindow(ref ShowImguiDemo);

                DearImguiSharp.ImGui.End();
                return true;
            };

            MultiForm.Show();
```
[![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/colored.png)](#table-of-contents)

### Preview

![Captura de pantalla 2024-01-16 194047](https://github.com/DestroyerDarkNess/EasyImGui/assets/32405118/1ce3f2e7-7480-4832-a459-a1397493a1a3)

![Captura de pantalla 2024-01-16 194153](https://github.com/DestroyerDarkNess/EasyImGui/assets/32405118/90fe71ea-9ec4-4f10-befe-2f95d83fac51)


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
