using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyImGui.Helpers
{
    public class Utils
    {

         public  static bool IsTouchScreenAvailable()
        {
             int maxTouchPoints = GetSystemMetrics(0x95); // SM_MAXIMUMTOUCHES = 0x95

            return maxTouchPoints > 0;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

    }
}
