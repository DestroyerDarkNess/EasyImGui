using EasyImGui.Properties;
using Microsoft.Win32;
using System;
namespace EasyImGui.Core
{
    public class Runtimes
    {
        public enum Architecture
        {
            Auto = 0,
            X86 = 1,
            X64 = 2,
            ARM = 3
        }

        public byte[] CimguiLib { get; set; }
        public byte[] ImGuiImplLib { get; set; }
        public static Runtimes Get(Architecture architecture = Architecture.Auto)
        {
            try
            {
                if (architecture == Architecture.Auto) { architecture = GetArchitecture(); }

                if (architecture == Architecture.ARM)
                {
                    return new Runtimes
                    {
                        CimguiLib = Resources.win_arm64_cimgui,
                        ImGuiImplLib = Resources.win_arm64_ImGuiImpl
                    };
                }
                return architecture == Architecture.X64
                    ? new Runtimes
                    {
                        CimguiLib = Resources.win_x64_cimgui,
                        ImGuiImplLib = Resources.win_x64_ImGuiImpl
                    }
                    : new Runtimes
                    {
                        CimguiLib = Resources.win_x86_cimgui,
                        ImGuiImplLib = Resources.win_x86_ImGuiImpl
                    };
            }
            catch
            {
                return new Runtimes
                {
                    CimguiLib = Resources.win_x86_cimgui,
                    ImGuiImplLib = Resources.win_x86_ImGuiImpl
                };
            }
        }

        private static Architecture GetArchitecture()
        {
            if (IsWindowsArm()) { return Architecture.ARM; }
            if (Environment.Is64BitProcess) { return Architecture.X64; }
            return Architecture.X86;
        }

        private static bool IsWindowsArm()
        {
            var value = Registry.GetValue("HKEY_LOCAL_MACHINE\\\\SYSTEM\\\\CurrentControlSet\\\\Control\\\\Session Manager\\\\Environment", "PROCESSOR_ARCHITECTURE", null);
            return value != null && string.Equals(value.ToString(), "ARM", StringComparison.OrdinalIgnoreCase);
        }
    }
}