using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DrawingCanvasLib;
using Microsoft.Extensions.DependencyInjection;
using WPF_CAD.Modes;

namespace WPF_CAD.Utils
{
    public class MachineConfigClass
    {
        private ProcessMode? ProcessMode => App.ServiceProvider?.GetService<ProcessMode>();

        private LaserProperties _defaultLaserPropertie = new();
        public LaserProperties DefaultLaserPropertie
        {
            get => _defaultLaserPropertie;
            set
            {
                if (value != null)
                {
                    _defaultLaserPropertie = value;
                    ProcessMode?.SaveMachineConfig();
                }
            }
        }
    }
}
