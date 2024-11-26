using SharpDX.Direct3D9;
using SharpDX.DXGI;
using System;
using System.IO;

namespace EasyImGui.Core
{
    public class Diagnostic
    {
        private static readonly string[] RequiredDlls =
        {
            "VULKAN-1.DLL",
            "OPENGL32.DLL",
            "D3DCOMPILER_47.DLL",
            "D3DX9_43.DLL",
            "VCRUNTIME140.DLL",
            "API-MS-WIN-CRT-STRING-L1-1-0.DLL",
            "API-MS-WIN-CRT-STDIO-L1-1-0.DLL",
            "API-MS-WIN-CRT-HEAP-L1-1-0.DLL",
            "API-MS-WIN-CRT-UTILITY-L1-1-0.DLL",
            "API-MS-WIN-CRT-MATH-L1-1-0.DLL",
            "API-MS-WIN-CRT-RUNTIME-L1-1-0.DLL"
        };

        public static bool RunDiagnostic()
        {
            Console.WriteLine("Starting diagnosis...");
            bool allLibrariesPresent = true;

            foreach (var dll in RequiredDlls)
            {
                if (IsDllAvailable(dll))
                {
                    PrintColored("[PASS] ", ConsoleColor.Green);
                    Console.WriteLine(dll);
                }
                else
                {
                    allLibrariesPresent = false;
                    PrintColored("[FAIL] ", ConsoleColor.Red);
                    Console.WriteLine(dll);
                    Console.WriteLine(GetSolutionForDll(dll));
                }
            }

            // Check Direct3D compatibility
            try
            {
                using (Direct3DEx context = new Direct3DEx())
                {
                    if (context.GetAdapterMonitor(0) == IntPtr.Zero)
                    {
                        PrintColored("[FAIL] ", ConsoleColor.Red);
                        Console.WriteLine("No compatible graphics adapter detected.");
                    }
                    else
                    {
                        var Devicecaps = context.GetDeviceCaps(0, DeviceType.Hardware);

                        // Check 3D Acceleration
                        if ((Devicecaps.DeviceCaps & DeviceCaps.HWTransformAndLight) == DeviceCaps.HWTransformAndLight)
                        {
                            PrintColored("[PASS] ", ConsoleColor.Green);
                            Console.WriteLine("3D Acceleration is enabled.");
                        }
                        else
                        {
                            PrintColored("[FAIL] ", ConsoleColor.Red);
                            Console.WriteLine("3D Acceleration is disabled.");
                        }

                        // Check Video Memory
                        allLibrariesPresent = CheckVideoMemory();

                    }
                }
            }
            catch
            {
                PrintColored("[FAIL] ", ConsoleColor.Red);
                Console.WriteLine("Direct3D is not available, please enable 3D Acceleration and check video memory available (256 MB or more)");
                allLibrariesPresent = false;
            }


            if (allLibrariesPresent)
            {
                PrintColored("All tests passed successfully.\n", ConsoleColor.Green);
            }
            else
            {
                PrintColored("Diagnostics completed. Some checks failed.\n", ConsoleColor.Red);
            }

            return allLibrariesPresent;
        }

        private static bool IsDllAvailable(string dllName)
        {
            string systemFolder = Environment.Is64BitProcess
                ? Environment.GetFolderPath(Environment.SpecialFolder.System)
                : Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);

            string[] searchPaths = {
            systemFolder,
            Path.Combine(systemFolder, "downlevel")
        };

            foreach (var path in searchPaths)
            {
                string dllPath = Path.Combine(path, dllName);
                if (File.Exists(dllPath))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetSolutionForDll(string dllName)
        {
            switch (dllName.ToUpper())
            {
                case "VULKAN-1.DLL":
                    return $"Install the latest drivers for your graphics card that includes support for Vulkan,{Environment.NewLine} or alternatively download: https://vulkan.lunarg.com/sdk/home#windowsand and install according to the instructions: {Environment.NewLine}" +
                           $"x64 : You can download 'Runtime - Runtime Installer' {Environment.NewLine}" +
                           $"x86 : You will need to download 'Runtime zip - Zip file of the runtime components' and extract the files to the x86 folder in {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "System32")}";
                case "OPENGL32.DLL":
                    return $"Make sure your system has OpenGL installed. This is usually included with your graphics card drivers, {Environment.NewLine} or alternatively download and install: https://github.com/pal1000/mesa-dist-win/releases/download/24.2.7/mesa3d-24.2.7-release-msvc.7z";
                case "D3DCOMPILER_47.DLL":
                    return $"Install the latest version of DirectX Runtime from the official Microsoft website: https://www.microsoft.com/en-us/download/details.aspx?id=8109";
                case "D3DX9_43.DLL":
                    return $"Install the latest version of DirectX Runtime from the official Microsoft website: https://www.microsoft.com/en-us/download/details.aspx?id=8109";
                case "VCRUNTIME140.DLL":
                    return $"Install the 'Microsoft Visual C++ Redistributable' for both architectures (x86 and x64) regardless of your system type. {Environment.NewLine}" +
                           $"ARM: https://aka.ms/vs/17/release/vc_redist.arm64.exe {Environment.NewLine}" +
                           $"x64: https://aka.ms/vs/17/release/vc_redist.x64.exe {Environment.NewLine}" +
                           $"x86: https://aka.ms/vs/17/release/vc_redist.x86.exe {Environment.NewLine}" +
                           $"{Console.ForegroundColor = ConsoleColor.Cyan}[Note]{Console.ForegroundColor = ConsoleColor.White} If your operating system is x64 you will also need to install x64 and also x84.";
                case string name when name.StartsWith("API-MS-WIN-CRT"):
                    return "Install the 'Microsoft Visual C++ Redistributable'";
                default:
                    return "No specific solution was found for this library. Please check if it is available on your system.";
            }
        }

        private static void PrintColored(string text, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = previousColor;
        }

        public static bool CheckVideoMemory()
        {
            bool anyPass = false;

            try
            {
                var factory = new Factory4();
                var adapter = new Adapter4(factory.GetAdapter1(0).NativePointer);

                var dedicatedMemory = adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.Local).AvailableForReservation / (1024 * 1024);

                if (dedicatedMemory >= 128)
                {
                    PrintColored("[PASS] ", ConsoleColor.Green);
                    Console.WriteLine($"GPU Video Dedicated Memory: {dedicatedMemory} MB");
                    anyPass = true;
                }
                else
                {
                    PrintColored("[FAIL] ", ConsoleColor.Red);
                    Console.WriteLine($"No adapters with sufficient video memory found. (256 MB required) Detected: {dedicatedMemory} MB");
                }


                return anyPass;
            }
            catch (Exception innerEx)
            {
                PrintColored("[FAIL] ", ConsoleColor.Red);
                Console.WriteLine($"Error checking video memory: {innerEx.Message}");
                return false;
            }

        }

    }

}
