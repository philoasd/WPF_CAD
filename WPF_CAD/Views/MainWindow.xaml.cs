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
using WPF_CAD.ExternalClass;
using WPF_CAD.Utils;
using WPF_CAD.ViewModes;

namespace WPF_CAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewMode _mainWindowViewMode => App.ServiceProvider.GetRequiredService<MainWindowViewMode>();

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
                e.Cancel = false; // 允许关闭窗口
            }
            else
            {
                e.Cancel = true; // 取消关闭窗口
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDateTime();

            for (int i = 0; i < 10; i++)
            {
                _mainWindowViewMode.DrawingList.Add($"Drawing {i + 1}");
            }

            _mainWindowViewMode.SelectedDrawingInfomation.Add(new DrawingClass
            {
                IsSelected = false,
            });
        }

        /// <summary>
        /// 更新日期时间
        /// </summary>
        private void UpdateDateTime()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _mainWindowViewMode.DataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    });
                    Thread.Sleep(1000);
                }
            });
        }
    }
}