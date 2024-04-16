
<h1 align="center">RenderSpy</h1>
<p align="center">
  <a href="https://github.com/DestroyerDarkNess/RenderSpy/blob/master/LICENSE">
    <img src="https://img.shields.io/github/license/Rebzzel/kiero.svg?style=flat-square"/>
  </a>
   <img src="https://img.shields.io/badge/platform-Windows-0078d7.svg"/>
  <br>
  Universal graphical and input hook for a D3D9-D3D12, OpenGL and Vulkan based games
  <br>
  💠 Please leave a Star to the repository ✅ if you liked it. ✨
</p>

[![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/colored.png)](#table-of-contents)

# Samples

| Sample | Description       |
|----------|---------------|
| [Universal.FPS.Counter](https://github.com/DestroyerDarkNess/Universal.FPS.Counter) | A Universal FPS Counter for games |
| [RenderSpy.Imgui](https://github.com/DestroyerDarkNess/RenderSpy.Imgui) | Easily use imgui in your favorite game |
| [d3d11 SpriteText Render](https://github.com/DestroyerDarkNess/Text-Render-DX11) | Create a window with sharDX and render text in directx 11 using SpriteTextRenderer |

# Features
 *Note: The objectives of the list that have: [❌] - It means I have no idea how to solve it.*

It includes all the libraries you need to draw, RenderSpy after compilation joins all the dependencies using ILMerge which means it appears to offer hooks, it also offers all the libraries you need to draw your overlay.

- [x] OpenGL.Net 
- [x] SharpDX and its Wrappers [SharpDX.Direct3D9 - SharpDX.Direct3D10 - SharpDX.Direct3D11 ... and more!!]
- [x] MinHook.NET [For your custom hooks.]

RenderSpy when incorporating these libraries has a weight of approximately 5mb, but if you use Costura.Fody in your assembly the weight is considerably reduced to approximately 1mb.

[![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/colored.png)](#table-of-contents)

### Graphics

| API | Support       |
|----------|---------------|
| DirectX9 | ✅ |
| DirectX10 | ✅ |
| DirectX11 | ✅ |
| DirectX12 | ❌ Work in Progress |
| OpenGL | ✅ |
| Vulkan | ❌ I don't know how to hook it |


### Inputs

- [x] DirectInputHook ✨
- [x] GetRawInputData 
- [x] DefWindowProc
- [x] GetWindowLongPtr
- [x] SetWindowLongPtr 
- [x] SetCursorPos
      
[![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/colored.png)](#table-of-contents)

### Example

```C
    RenderSpy.Graphics.GraphicsType GraphicsT = RenderSpy.Graphics.Detector.GetCurrentGraphicsType();

            RenderSpy.Interfaces.IHook CurrentHook = null;

            switch (GraphicsT)
            {
                case RenderSpy.Graphics.GraphicsType.d3d9:

                    Graphics.d3d9.Present PresentHook_9 = new Graphics.d3d9.Present();
                    PresentHook_9.Install();
                    CurrentHook = PresentHook_9;

                    PresentHook_9.PresentEvent += (IntPtr device, IntPtr sourceRect, IntPtr destRect, IntPtr hDestWindowOverride, IntPtr dirtyRegion) =>
                    {
                        // You Custom Code.
                        return PresentHook_9.Present_orig(device, sourceRect, destRect, hDestWindowOverride, dirtyRegion);
                    };

                    break;
                case RenderSpy.Graphics.GraphicsType.d3d10:

                    Graphics.d3d10.Present PresentHook_10 = new Graphics.d3d10.Present();
                    PresentHook_10.Install();
                    CurrentHook = PresentHook_10;

                    PresentHook_10.PresentEvent += (swapChainPtr, syncInterval, flags) =>
                    {
                        // You Custom Code.
                        return PresentHook_10.Present_orig(swapChainPtr, syncInterval, flags);
                    };

                    break;
                case RenderSpy.Graphics.GraphicsType.d3d11:

                    Graphics.d3d11.Present PresentHook_11 = new Graphics.d3d11.Present();
                    PresentHook_11.Install();
                    CurrentHook = PresentHook_11;

                    PresentHook_11.PresentEvent += (swapChainPtr, syncInterval, flags) =>
                    {
                        // You Custom Code.
                        return PresentHook_11.Present_orig(swapChainPtr, syncInterval, flags);
                    };

                    break;
                case RenderSpy.Graphics.GraphicsType.d3d12:

                    break;
                case RenderSpy.Graphics.GraphicsType.opengl:

                    Graphics.opengl.wglSwapBuffers glSwapBuffersHook = new Graphics.opengl.wglSwapBuffers();
                    glSwapBuffersHook.Install();
                    CurrentHook = glSwapBuffersHook;

                    glSwapBuffersHook.wglSwapBuffersEvent += (IntPtr hdc) =>
                    {
                        // You Custom Code.
                        return glSwapBuffersHook.wglSwapBuffers_orig(hdc); ;
                    };


                    break;
                case RenderSpy.Graphics.GraphicsType.vulkan:

                    break;
                default:

                    break;
            }

      // ... you more code

      // Terminate.... runtines.
      // Destroy Hook
      // CurrentHook.Uninstall();


```
[![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/colored.png)](#table-of-contents)

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






