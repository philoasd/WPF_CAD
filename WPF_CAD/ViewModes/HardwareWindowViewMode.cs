using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawingCanvasLib;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace WPF_CAD.ViewModes
{
    public class HardwareWindowViewMode : ObservableObject
    {
        private MainWindowViewMode? MainWindowViewMode => App.ServiceProvider?.GetService<MainWindowViewMode>();

        #region Main window

        public RelayCommand<Window> SaveHardwareConfigCommand => new((obj) =>
        {
            if (obj == null) { return; }

            // todo:保存hardware配置逻辑
            if (MainWindowViewMode != null)
            {
                MainWindowViewMode.MachineConfig.DefaultLaserPropertie = this.DefaultLaserProperties.Adapt<LaserProperties>();
            }

            // 关闭窗口
            obj.Close();
        });

        public RelayCommand<Window> CloseHardwareWindowCommand => new((obj) =>
        {
            if (obj == null) { return; }

            // 关闭窗口
            obj.Close();
        });

        #endregion

        private LaserProperties _defaultLaserProperties = new();
        public LaserProperties DefaultLaserProperties
        {
            get => _defaultLaserProperties;
            set => SetProperty(ref _defaultLaserProperties, value);
        }
    }
}
