
using DearImguiSharp;
using EasyImGui.Helpers;
using RenderSpy.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace EasyImGui.Core
{
    public class ImGuiHook : IDisposable
    {
      

        public  bool Imgui_Ini { get; set; } = false;

        public DearImguiSharp.ImGuiContext Context = null;
        public DearImguiSharp.ImGuiIO IO = null;
        public IntPtr WindowHandle = IntPtr.Zero;

        RenderSpy.Graphics.GraphicsType GraphicsT { get; set; } = RenderSpy.Graphics.Detector.GetCurrentGraphicsType();
        public List<RenderSpy.Interfaces.IHook> CurrentHooks { get; set; } = new List<RenderSpy.Interfaces.IHook>();


        #region " Render Event "

        public delegate bool RenderUI_EventHandler();

        public event RenderUI_EventHandler Render = null;

        public event RenderUI_EventHandler OnLostDevice = null;
        public event RenderUI_EventHandler OnResetDevice = null;

        private bool OnRenderEvent()
        {

            try {
                if (Render != null)
                {
                    return Render.Invoke();
                }
                else { return false; }
            } catch (Exception ex) { ExceptionHandler.HandleException(ex); return false;  }

        }

        public delegate bool ImguiIsReady();

        public event RenderUI_EventHandler ConfigContex = null;

        private bool OnConfigContex()
        {

            try
            {
                if (ConfigContex != null)
                {
                    return ConfigContex.Invoke();
                }
                else { return false; }
            }
            catch (Exception ex) { ExceptionHandler.HandleException(ex); return false; }

        }

        #endregion

        #region " Hooks "

        public bool LoadCImgui(bool InMemory = false)
        {
            try {

                string DropPath = Path.Combine(Path.GetTempPath(), Process.GetCurrentProcess().ProcessName + "_" + Process.GetCurrentProcess().Id );
                string CimguiPath = Path.Combine(DropPath, "cimgui.dll");

                try
                {
                    if (Directory.Exists(DropPath) == false) { Directory.CreateDirectory(DropPath); } 
                }
                catch { }

                byte[] Cimgui  = RenderSpy.ImGui.CImgui.Get();

                if (InMemory == true)
                {
                    RenderSpy.Tools.DLLFromMemory DllMemory = new RenderSpy. Tools.DLLFromMemory(Cimgui);
                    DllMemory.MemoryCallEntryPoint();
                } else
                {
                    try
                    {
                        System.IO.File.WriteAllBytes(CimguiPath, Cimgui);
                    }
                    catch { }
                    RenderSpy.Globals.WinApi.LoadLibrary(CimguiPath);
                }

                return true;

            } catch ( Exception Ex )  { Helpers.ExceptionHandler.HandleException(Ex); return false; }

        }

        public Models.GraphicHook Attach(IntPtr GameHandle)
        {

            Models.GraphicHook Results = new Models.GraphicHook();

            try
            {

                if ( RenderSpy.Graphics.Detector.GetCurrentGraphicsType() == RenderSpy.Graphics.GraphicsType.d3d9)
                {

                    RenderSpy.Graphics.d3d9.Present PresentHook_9 = new RenderSpy.Graphics.d3d9.Present();
                    PresentHook_9.Install();

                    CurrentHooks.Add(PresentHook_9);

                    PresentHook_9.PresentEvent += (IntPtr device, IntPtr sourceRect, IntPtr destRect, IntPtr hDestWindowOverride, IntPtr dirtyRegion) =>
                    {
                        if (Imgui_Ini == false)
                        {
                            Imgui_Ini = true;

                            try
                            {

                                SharpDX.Direct3D9.Device dev = (SharpDX.Direct3D9.Device)device;
                                var swapChain = dev.GetSwapChain(0);
                                var DevwindowHandle = dev.CreationParameters.HFocusWindow;
                                var swapChainHandle = swapChain.PresentParameters.DeviceWindowHandle;
                                DevwindowHandle = DevwindowHandle == IntPtr.Zero ? swapChainHandle : DevwindowHandle;

                                if (DevwindowHandle != IntPtr.Zero)
                                {
                                    WindowHandle = DevwindowHandle;

                                }
                                else { WindowHandle = GameHandle; }


                                if (Context == null) { Context = DearImguiSharp.ImGui.CreateContext(null); }

                                IO = Context.IO;

                                DearImguiSharp.ImGui.StyleColorsDark(null);

                                OnConfigContex();

                                DearImguiSharp.ImGui.ImGuiImplWin32Init(WindowHandle);
                                DearImguiSharp.ImGui.__Internal.ImGuiImplDX9Init(device);

                            }
                            catch (Exception ex) { Imgui_Ini = false; Helpers.ExceptionHandler.HandleException(ex); }

                        }
                        else
                        {
                            try
                            {
                                RenderBegin();
                                OnRenderEvent();
                                RenderEnd();
                            }
                            catch (Exception Ex) { Helpers.ExceptionHandler.HandleException(Ex); }

                        }

                        return PresentHook_9.Present_orig(device, sourceRect, destRect, hDestWindowOverride, dirtyRegion);
                    };

                    RenderSpy.Graphics.d3d9.Reset ResetHook_9 = new RenderSpy.Graphics.d3d9.Reset();
                    ResetHook_9.Install();
                    CurrentHooks.Add(ResetHook_9);

                    ResetHook_9.Reset_Event += (IntPtr device, ref SharpDX.Direct3D9.PresentParameters presentParameters) =>
                    {
                        OnLostDevice?.Invoke();
                        if (Imgui_Ini == true) { DearImguiSharp.ImGui.ImGuiImplDX9InvalidateDeviceObjects(); }

                        int Reset = ResetHook_9.Reset_orig(device, ref presentParameters);
                      
                        if (Imgui_Ini == true) { DearImguiSharp.ImGui.ImGuiImplDX9CreateDeviceObjects(); }
                        OnResetDevice?.Invoke();

                        return Reset;
                    };

                    Results.IsAttached = (CurrentHooks.Count != 0);

                } else { throw new Exception("Undefine Render"); }

              

            }
            catch (Exception ex)
            {
                Results.IsAttached = false;
                Helpers.ExceptionHandler.HandleException(ex);
            }

            return Results;
        }

        #endregion

        #region " ImGui Methods "

        private bool RenderBegin()
        {
            bool Result = true;

            try
            {
                switch (GraphicsT)
                {
                    case RenderSpy.Graphics.GraphicsType.d3d9:

                            DearImguiSharp.ImGui.ImGuiImplWin32NewFrame();
                            DearImguiSharp.ImGui.ImGuiImplDX9NewFrame();
                            DearImguiSharp.ImGui.NewFrame();
                      
                        break;
                    case RenderSpy.Graphics.GraphicsType.d3d10:


                        break;
                    case RenderSpy.Graphics.GraphicsType.d3d11:

                      

                        break;
                    case RenderSpy.Graphics.GraphicsType.d3d12:

                      

                        break;
                    case RenderSpy.Graphics.GraphicsType.opengl:

                        
                        break;
                    case RenderSpy.Graphics.GraphicsType.vulkan:

                        break;
                    default:

                        break;
                }

            }
            catch 
            {
                Result = false;
            }

            return Result;
        }

        private bool  RenderEnd()
        {
            bool Result = true;

            try
            {
               
                switch (GraphicsT)
                {
                    case RenderSpy.Graphics.GraphicsType.d3d9:

                        DearImguiSharp.ImGui.EndFrame();
                        DearImguiSharp.ImGui.Render();

                        if ((IO.ConfigFlags & (int)ImGuiConfigFlags.ViewportsEnable) > 0)
                        {
                            try {
                                DearImguiSharp.ImGui.UpdatePlatformWindows();
                                DearImguiSharp.ImGui.RenderPlatformWindowsDefault(IntPtr.Zero, IntPtr.Zero);
                            } catch { }
                        }

                        try
                        {
                            var drawData = DearImguiSharp.ImGui.GetDrawData();
                                 DearImguiSharp.ImGui.ImGuiImplDX9RenderDrawData(drawData);
                        }
                        catch { }

                        break;
                    case RenderSpy.Graphics.GraphicsType.d3d10:


                        break;
                    case RenderSpy.Graphics.GraphicsType.d3d11:



                        break;
                    case RenderSpy.Graphics.GraphicsType.d3d12:



                        break;
                    case RenderSpy.Graphics.GraphicsType.opengl:


                        break;
                    case RenderSpy.Graphics.GraphicsType.vulkan:

                        break;
                    default:

                        break;
                }
            }
            catch 
            {
                Result = false;
            }

            return Result;
        }

        #endregion

        #region IDisposable implementation with finalizer

        private bool isDisposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    foreach (IHook HOOK in CurrentHooks)
                    {
                        HOOK.Uninstall();
                    }
                    CurrentHooks.Clear();
                }
            }
            isDisposed = true;
        }

        #endregion

    }
}
