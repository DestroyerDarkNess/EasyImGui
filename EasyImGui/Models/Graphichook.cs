using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyImGui.Models
{
    public class Graphichook
    {
        public bool IsAttached { get; set; } = false;

        public RenderSpy.Graphics.GraphicsType GraphicsType { get; set; } = RenderSpy.Graphics.GraphicsType.unknown;

    }
}
