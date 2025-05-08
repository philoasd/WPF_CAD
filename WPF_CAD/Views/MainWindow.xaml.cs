using DrawingCanvasLib;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_CAD.Modes;
using WPF_CAD.Utils;
using WPF_CAD.ViewModes;
using WPF_CAD.Views;

namespace WPF_CAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer CurTimer { get; set; }
        private MainWindowViewMode? MainWindowViewMode => this.DataContext as MainWindowViewMode;

        public MainWindow(MainWindowViewMode vm)
        {
            InitializeComponent();

            this.DataContext = vm;

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            // 关闭窗口时，弹出提示框：提示是否退出
            if (MsgBoxClass.ShowQMsg("Are you sure you want to exit?") == MessageBoxResult.Yes)
            {
                CurTimer.Stop(); // 停止计时器
                e.Cancel = false; // 允许关闭窗口
            }
            else
            {
                e.Cancel = true; // 取消关闭窗口
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #region Drawing Canvas Event Binding
            MCanvas.OnDoubleClickEvent += OnDoubleClick;
            #endregion

            UpdateDateTime();
        }

        private void OnDoubleClick(object? sender, EventArgs e)
        {
            // 打开drawing properties window
            var drawingPropertiesWindow = App.ServiceProvider?.GetRequiredService<DrawingPropertiesWindow>();
            if (drawingPropertiesWindow == null)
            {
                MsgBoxClass.ShowMsg("Failed to open drawing properties window.", MsgBoxClass.MsgBoxType.Error);
                return;
            }
            drawingPropertiesWindow.Owner = this;
            drawingPropertiesWindow.ShowDialog();
        }

        /// <summary>
        /// 更新日期时间
        /// </summary>
        private void UpdateDateTime()
        {
            CurTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            CurTimer.Tick += (s, e) =>
            {
                var now = DateTime.Now;
                var dateTimeString = now.ToString("yyyy-MM-dd HH:mm:ss");
                if (MainWindowViewMode != null)
                {
                    MainWindowViewMode.DataTime = dateTimeString;
                }
            };
            CurTimer.Start();
        }
    }
}