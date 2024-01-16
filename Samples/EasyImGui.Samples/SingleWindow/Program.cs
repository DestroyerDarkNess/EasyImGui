using EasyImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SingleWindow
{
    class Program
    {
        static void Main(string[] args)
        {

            ImGuiForm SingleImguiWindow = new ImGuiForm(true, true);
            //SingleImguiWindow.ImGuiWindowFlagsEx = DearImguiSharp.ImGuiWindowFlags.NoTitleBar;

            SingleImguiWindow.Render += delegate
            {

                if (DearImguiSharp.ImGui.Button("Message", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                {
                    Task.Run(() =>
                    {
                        MessageBox.Show("Hello World!");
                    });
                }

                if (DearImguiSharp.ImGui.Button("Close Me", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                    SingleImguiWindow.Visible = false;

                return true;
            };

            Application.Run(SingleImguiWindow);

        }
    }
}
