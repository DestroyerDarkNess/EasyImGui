using System.Runtime.InteropServices;

namespace EasyImGui.Core.PInvoke
{
    // MARGIN struct used with DwmApi
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMargin
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyBottomHeight;
        public int cyTopHeight;
    }
}
