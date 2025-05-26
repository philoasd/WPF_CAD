using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DrawingCanvasLib;

namespace WPF_CAD.Utils
{
    public class MachineConfigClass
    {
        public LaserProperties LaserPropertie { get; set; } = new();
    }
}
