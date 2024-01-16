using EasyImGui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiWindow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool ShowImguiDemo = false;
        MultiImGuiForm MultiForm = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            
            MultiForm = new MultiImGuiForm();
            MultiForm.Render += delegate
            {
                if (!MultiForm.Draw)
                    return false;

                DearImguiSharp.ImGui.Begin("Another Window in C#", ref MultiForm.Draw, (int)DearImguiSharp.ImGuiWindowFlags.None);
                DearImguiSharp.ImGui.Text("Hello from another window!");

                if (DearImguiSharp.ImGui.Button("ImguiDemo", new DearImguiSharp.ImVec2() { X = 200, Y = 20 }))
                    ShowImguiDemo = !ShowImguiDemo;

                if (ShowImguiDemo)
                    DearImguiSharp.ImGui.ShowDemoWindow(ref ShowImguiDemo);

                DearImguiSharp.ImGui.End();
                return true;
            };

            MultiForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MultiForm != null)
                MultiForm.Draw = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MultiForm != null)
                MultiForm.Draw = false;
        }
    }
}
