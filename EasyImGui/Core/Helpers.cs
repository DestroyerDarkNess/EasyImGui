using System;

namespace EasyImGui.Core
{
    internal class Helpers
    {
        public static void WriteException(Exception ex)
        {
            ConsoleColor OldRestoreColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An exception occurred:");

            Console.WriteLine($"Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");

            Console.ForegroundColor = OldRestoreColor;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        public static bool IsTouchScreenAvailable()
        {
            int maxTouchPoints = GetSystemMetrics(0x95); // SM_MAXIMUMTOUCHES = 0x95
            return maxTouchPoints > 0;
        }

    }
}
