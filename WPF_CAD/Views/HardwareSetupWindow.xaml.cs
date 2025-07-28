using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_CAD.ViewModes;
using Mapster;

namespace WPF_CAD.Views
{
    /// <summary>
    /// Interaction logic for HardwareSetupWindow.xaml
    /// </summary>
    public partial class HardwareSetupWindow : Window
    {
        private HardwareWindowViewModel? HardwareWindowViewMode => this.DataContext as HardwareWindowViewModel;
        private MainWindowViewModel? MainWindowViewMode => this.Owner?.DataContext as MainWindowViewModel;

        public HardwareSetupWindow(HardwareWindowViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;

            this.Loaded += HardwareSetupWindow_Loaded;
            this.Closed += HardwareSetupWindow_Closed;
        }

        private void HardwareSetupWindow_Closed(object? sender, EventArgs e)
        {
            
        }

        private void HardwareSetupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(HardwareWindowViewMode==null|| MainWindowViewMode == null)
            {
                return;
            }

            // todo: 加载hardware配置逻辑
            HardwareWindowViewMode.DefaultLaserProperties = MainWindowViewMode.MachineConfig?.DefaultLaserPropertie?.Adapt<DrawingCanvasLib.LaserProperties>() ?? new DrawingCanvasLib.LaserProperties();
        }
    }
}
