using System;

namespace EasyImGui.Core.PInvoke
{
    public static class DwmApi
    {
        public delegate void DwmExtendFrameIntoClientAreaDelegate(IntPtr hWnd, ref NativeMargin pMargins);
        public static readonly DwmExtendFrameIntoClientAreaDelegate DwmExtendFrameIntoClientArea;

        static DwmApi()
        {
            IntPtr library = DynamicImport.ImportLibrary("dwmapi.dll");

            DwmExtendFrameIntoClientArea = DynamicImport.Import<DwmExtendFrameIntoClientAreaDelegate>(library, "DwmExtendFrameIntoClientArea");
        }
    }
}
