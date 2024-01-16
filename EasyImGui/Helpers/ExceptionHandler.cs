using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyImGui.Helpers
{
    public class ExceptionHandler
    {

        public static void HandleException(Exception ex)
        {
            ConsoleColor OldRestoreColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An exception occurred:");

            Console.WriteLine($"Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");

            Console.ForegroundColor = OldRestoreColor;
        }

    }
}
