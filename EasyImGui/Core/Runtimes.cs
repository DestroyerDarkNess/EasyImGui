using EasyImGui.Properties;
using Microsoft.Win32;
using System;
namespace EasyImGui.Core
{
    public class Runtimes
    {
        public byte[] CimguiLib { get; set; }
        public byte[] ImGuiImplLib { get; set; }
        public static Runtimes Get()
        {
            try
            {
                if (IsWindowsArm())
                {
                    return new Runtimes
                    {
                        CimguiLib = Resources.win_arm64_cimgui,
                        ImGuiImplLib = Resources.win_arm64_ImGuiImpl
                    };
                }
                return Environment.Is64BitProcess
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
        private static bool IsWindowsArm()
        {
            var value = Registry.GetValue("HKEY_LOCAL_MACHINE\\\\SYSTEM\\\\CurrentControlSet\\\\Control\\\\Session Manager\\\\Environment", "PROCESSOR_ARCHITECTURE", null);
            return value != null && string.Equals(value.ToString(), "ARM", StringComparison.OrdinalIgnoreCase);
        }
    }
}