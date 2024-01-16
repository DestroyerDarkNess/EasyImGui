
using RenderSpy.Globals;
using RenderSpy.Inputs;
using RenderSpy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyImGui.Core
{
    public class InputHook : IDisposable
    {
        private int WheelDelta = SystemInformation.MouseWheelScrollDelta;

        public DearImguiSharp.ImGuiIO IO = null;
        public List<RenderSpy.Interfaces.IHook> CurrentHooks { get; set; } = new List<RenderSpy.Interfaces.IHook>();

        #region " Event "

        public delegate bool Input_EventHandler(IntPtr hWnd, WM msg, Keys wParam, IntPtr lParam);

        public event Input_EventHandler InputEnvet = null;

        public bool OnInputEvent(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (InputEnvet != null)
                {
                    int KeyID = wParam.ToInt32();
                    return InputEnvet.Invoke(hWnd, (WM)msg, (Keys)KeyID, lParam);
                }
                else { return false; }
            }
            catch (Exception ex) { Helpers.ExceptionHandler.HandleException(ex); return false; }
        }

        #endregion

        #region " Hooks "

        public bool Attach(IntPtr GameHandle)
        {
            bool Results = false;

            try
            {
                IntPtr DirectInput = WinApi.GetModuleHandle("dinput8.dll");

                if (DirectInput != IntPtr.Zero) {

                    SetWindowLongPtr SetWindowLongPtr_Hook = new SetWindowLongPtr();
                    SetWindowLongPtr_Hook.WindowHandle = GameHandle;
                    SetWindowLongPtr_Hook.Install();
                    CurrentHooks.Add(SetWindowLongPtr_Hook);
                    SetWindowLongPtr_Hook.WindowProc += (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) =>
                    {
                        try
                        {
                            OnInputEvent(hWnd, msg, wParam, lParam);
                        }
                        catch { }

                        return IntPtr.Zero;
                    };

                    DirectInputHook DirectInputHook_Hook = new DirectInputHook();
                    DirectInputHook_Hook.WindowHandle = GameHandle;
                    DirectInputHook_Hook.Install();
                    CurrentHooks.Add(DirectInputHook_Hook);
                    DirectInputHook_Hook.GetDeviceState += (IntPtr hDevice, int cbData, IntPtr lpvData) =>
                    {

                        if ( IO != null)
                        {
                            try
                            {

                                int Result = DirectInputHook_Hook.Hook_orig(hDevice, cbData, lpvData);

                                if (Result == 0)
                                {
                                  
                                    if (cbData == 16 || cbData == 20)
                                    {
                                        DirectInputHook.LPDIMOUSESTATE MouseData = DirectInputHook_Hook.GetMouseData(lpvData);
                                        IO.ManualMouseDown(0, (MouseData.rgbButtons[0] != 0));
                                        IO.ManualMouseDown(1, (MouseData.rgbButtons[1] != 0));

                                        IO.MouseWheel += (float)(MouseData.lZ / (float)WheelDelta);

                                    }

                                }
                                return Result;
                            }
                            catch { }

                        }

                        return DirectInputHook_Hook.Hook_orig(hDevice, cbData, lpvData);
                    };


                } else {


                    DefWindowProc DefWindowProcW_Hook = new DefWindowProc();
                    DefWindowProcW_Hook.WindowHandle = GameHandle;
                    DefWindowProcW_Hook.Install();
                    CurrentHooks.Add(DefWindowProcW_Hook);
                    DefWindowProcW_Hook.WindowProc += (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) =>
                    {

                        try
                        {
                            OnInputEvent(hWnd, msg, wParam, lParam);
                        }
                        catch { }

                        return IntPtr.Zero;
                    };

                  
                }


                Results = (CurrentHooks.Count != 0);

            }
            catch (Exception ex)
            {
                Results = false;
                Helpers.ExceptionHandler.HandleException(ex);
            }

            return Results;
        }

        [Obsolete]
        public static  bool Universal(DearImguiSharp.ImGuiIO IO)
        {
           try {

                if ( IO != null)
                {
             
                    int LButton = WinApi.GetAsyncKeyState(Keys.LButton); IO.ManualMouseDown(0, (LButton != 0));

                    int RButton = WinApi.GetAsyncKeyState(Keys.RButton); IO.ManualMouseDown(1, (RButton != 0));

                    int MButton = WinApi.GetAsyncKeyState(Keys.MButton); IO.ManualMouseDown(2, (MButton != 0)); 

                    int XButton1 = WinApi.GetAsyncKeyState(Keys.XButton1); IO.ManualMouseDown(3, (XButton1 != 0));

                    int XButton2 = WinApi.GetAsyncKeyState(Keys.XButton2); IO.ManualMouseDown(4, (XButton2 != 0));

                } 

                return true;

            } catch (Exception Ex) { Helpers.ExceptionHandler.HandleException(Ex); return false; } 

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
