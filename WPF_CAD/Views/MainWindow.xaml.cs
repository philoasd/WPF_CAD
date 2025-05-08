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
using WPF_CAD.Utils;
using WPF_CAD.ViewModes;

namespace WPF_CAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewMode? _mainWindowViewMode => App.ServiceProvider?.GetRequiredService<MainWindowViewMode>();

        /// <summary>
        /// 是否正在关闭窗口
        /// </summary>
        public bool IsWindowClosing { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = _mainWindowViewMode;

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            // 关闭窗口时，弹出提示框：提示是否退出
            if (MsgBoxClass.ShowQMsg("Are you sure you want to exit?") == MessageBoxResult.Yes)
            {
                IsWindowClosing = true; // 设置正在关闭窗口的标志
                e.Cancel = false; // 允许关闭窗口
            }
            else
            {
                e.Cancel = true; // 取消关闭窗口
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsWindowClosing = false;

            #region Drawing Canvas Event Binding
            MCanvas.OnDoubleClickEvent += OnDoubleClick;
            #endregion

            UpdateDateTime();
        }

        private void OnDoubleClick(object? sender, EventArgs e)
        {
            MsgBoxClass.ShowMsg("Double Click", MsgBoxClass.MsgBoxType.Information);
        }

        /// <summary>
        /// 更新日期时间
        /// </summary>
        private void UpdateDateTime()
        {
            _ = Task.Run(() =>
            {
                while (!IsWindowClosing)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (_mainWindowViewMode != null) // Additional null check to ensure safety
                        {
                            _mainWindowViewMode.DataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    });
                    Thread.Sleep(1000);
                }
            });
        }
    }
}