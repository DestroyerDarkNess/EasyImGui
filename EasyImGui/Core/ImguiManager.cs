using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.D3D9;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace EasyImGui.Core
{
    public class ImguiManager : IDisposable
    {
        #region " Declare " 

        private bool _isInitialized = false;
        private bool Imgui_Ini = false;

        private RenderSpy.Graphics.d3d9.Present PresentHook_9 = new RenderSpy.Graphics.d3d9.Present();
        private RenderSpy.Graphics.d3d9.Reset ResetHook_9 = new RenderSpy.Graphics.d3d9.Reset();

        public Hexa.NET.ImGui.ImGuiContextPtr Context;
        public Hexa.NET.ImGui.ImGuiIOPtr IO;
        public IntPtr WindowTargetHandle = IntPtr.Zero;
        public bool Imgui_Alive = false;

        #endregion

        #region " Evens " 

        public delegate bool RenderUI_EventHandler();
        public event RenderUI_EventHandler Render = null;
        public event RenderUI_EventHandler ConfigContex = null;
        public event RenderUI_EventHandler OnLostDevice = null;
        public event RenderUI_EventHandler OnResetDevice = null;

        #endregion

        string DropPath = string.Empty;
        string CimguiPath = string.Empty;
        string ImGuiImplPath = string.Empty;

        public ImguiManager()
        {
            DropPath = Path.Combine(Path.GetTempPath(), Process.GetCurrentProcess().ProcessName + "_" + Process.GetCurrentProcess().Id);
            CimguiPath = Path.Combine(DropPath, "cimgui.dll");
            ImGuiImplPath = Path.Combine(DropPath, "ImGuiImpl.dll");
        }

        public bool Initialize(IntPtr WindowTargetHandle)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                try
                {
                    if (!LoadBindings()) return false;

                    PresentHook_9.Install();
                    PresentHook_9.PresentEvent += (IntPtr device, IntPtr sourceRect, IntPtr destRect, IntPtr hDestWindowOverride, IntPtr dirtyRegion) =>
                    {
                        try
                        {
                            if (!Imgui_Ini)
                            {
                                Imgui_Ini = true;

                                SharpDX.Direct3D9.Device dev = (SharpDX.Direct3D9.Device)device;
                                var swapChain = dev.GetSwapChain(0);
                                var DevwindowHandle = dev.CreationParameters.HFocusWindow;
                                var swapChainHandle = swapChain.PresentParameters.DeviceWindowHandle;
                                DevwindowHandle = DevwindowHandle == IntPtr.Zero ? swapChainHandle : DevwindowHandle;

                                if (DevwindowHandle != IntPtr.Zero)
                                {
                                    WindowTargetHandle = DevwindowHandle;
                                }

                                Context = Hexa.NET.ImGui.ImGui.CreateContext();

                                Hexa.NET.ImGui.ImGui.SetCurrentContext(Context);

                                IO = Hexa.NET.ImGui.ImGui.GetIO();

                                try
                                {
                                    ConfigContex?.Invoke();
                                }
                                catch (Exception ex) { Helpers.WriteException(ex); }

                                Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32.SetCurrentContext(Context);
                                Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32.Init(WindowTargetHandle);

                                Hexa.NET.ImGui.Backends.D3D9.ImGuiImplD3D9.SetCurrentContext(Context);

                                unsafe
                                {
                                    IDirect3DDevice9* devicePtr = (IDirect3DDevice9*)(void*)device;
                                    IDirect3DDevice9Ptr devicePtrWrapper = new IDirect3DDevice9Ptr(devicePtr);
                                    Imgui_Alive = Hexa.NET.ImGui.Backends.D3D9.ImGuiImplD3D9.Init(devicePtrWrapper);
                                }

                            }
                            else
                            {
                                if (Imgui_Alive)
                                {

                                    try
                                    {
                                        Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32.NewFrame();
                                        Hexa.NET.ImGui.Backends.D3D9.ImGuiImplD3D9.NewFrame();

                                        Hexa.NET.ImGui.ImGui.NewFrame();
                                    }
                                    catch { }

                                    try
                                    {
                                        Render?.Invoke();
                                    }
                                    catch (Exception ex) { Helpers.WriteException(ex); }

                                    try
                                    {
                                        Hexa.NET.ImGui.ImGui.EndFrame();
                                        Hexa.NET.ImGui.ImGui.Render();
                                    }
                                    catch { }

                                    try
                                    {
                                        if ((IO.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) > 0)
                                        {
                                            Hexa.NET.ImGui.ImGui.UpdatePlatformWindows();
                                            Hexa.NET.ImGui.ImGui.RenderPlatformWindowsDefault();
                                        }
                                        Hexa.NET.ImGui.Backends.D3D9.ImGuiImplD3D9.RenderDrawData(Hexa.NET.ImGui.ImGui.GetDrawData());
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch (Exception ex) { Imgui_Ini = false; Helpers.WriteException(ex); }

                        return PresentHook_9.Present_orig(device, sourceRect, destRect, hDestWindowOverride, dirtyRegion);
                    };

                    ResetHook_9.Install();

                    ResetHook_9.Reset_Event += (IntPtr device, ref SharpDX.Direct3D9.PresentParameters presentParameters) =>
                    {
                        OnLostDevice?.Invoke();
                        if (Imgui_Ini == true) { Hexa.NET.ImGui.Backends.D3D9.ImGuiImplD3D9.InvalidateDeviceObjects(); }

                        int Reset = ResetHook_9.Reset_orig(device, ref presentParameters);

                        if (Imgui_Ini == true) { Hexa.NET.ImGui.Backends.D3D9.ImGuiImplD3D9.CreateDeviceObjects(); }
                        OnResetDevice?.Invoke();

                        return Reset;
                    };

                    return true;
                }
                catch (Exception ex)
                {
                    Helpers.WriteException(ex);
                    return false;
                }

            }
            return false;
        }

        public Runtimes.Architecture Architecture { get; set; } = Runtimes.Architecture.Auto;

        public bool LoadBindings()
        {
            try
            {
                Runtimes ImguiRuntimes = Runtimes.Get(Architecture);

                if (ImguiRuntimes == null) return false;

                try
                {
                    if (Directory.Exists(DropPath) == false) { Directory.CreateDirectory(DropPath); }
                    File.WriteAllBytes(CimguiPath, ImguiRuntimes.CimguiLib);
                    File.WriteAllBytes(ImGuiImplPath, ImguiRuntimes.ImGuiImplLib);
                }
                catch (Exception ex) { Helpers.WriteException(ex); }

                IntPtr cimgui_handle = RenderSpy.Globals.WinApi.LoadLibrary(CimguiPath);

                if (cimgui_handle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    string errorMessage = new Win32Exception(errorCode).Message;
                    throw new Exception($"Failed to load cimgui.dll: {errorMessage}");
                }

                IntPtr ImGuiImpl_handle = RenderSpy.Globals.WinApi.LoadLibrary(ImGuiImplPath);

                if (ImGuiImpl_handle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    string errorMessage = new Win32Exception(errorCode).Message;
                    throw new Exception($"Failed to load ImGuiImpl.dll: {errorMessage}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Helpers.WriteException(ex);
                return false;
            }
        }

        #region IDisposable implementation with finalizer

        private bool isDisposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Imgui_Alive = false;
                    PresentHook_9.Uninstall();
                    ResetHook_9.Uninstall();
                    Hexa.NET.ImGui.Backends.D3D9.ImGuiImplD3D9.Shutdown();
                    Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32.Shutdown();
                    Hexa.NET.ImGui.ImGui.DestroyContext(Context);

                    try
                    {
                        IntPtr cimguiHandle = RenderSpy.Globals.WinApi.GetModuleHandle(Path.GetFileName(CimguiPath));
                        if (cimguiHandle == IntPtr.Zero && !RenderSpy.Globals.WinApi.FreeLibrary(cimguiHandle))
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            Console.WriteLine($"Failed to unload cimgui.dll: {new Win32Exception(errorCode).Message}");
                        }

                        IntPtr imguiImplHandle = RenderSpy.Globals.WinApi.GetModuleHandle(Path.GetFileName(ImGuiImplPath));
                        if (imguiImplHandle == IntPtr.Zero && !RenderSpy.Globals.WinApi.FreeLibrary(imguiImplHandle))
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            Console.WriteLine($"Failed to unload ImGuiImpl.dll: {new Win32Exception(errorCode).Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error unloading DLLs: {ex.Message}");
                    }

                    if (File.Exists(CimguiPath)) try { File.Delete(CimguiPath); } catch { }

                    if (File.Exists(ImGuiImplPath)) try { File.Delete(ImGuiImplPath); } catch { }
                }
            }
            isDisposed = true;
        }

        #endregion
    }
}
