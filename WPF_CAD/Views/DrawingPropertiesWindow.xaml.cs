using DrawingCanvasLib.DrawTool;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WPF_CAD.Utils;
using WPF_CAD.ViewModes;

namespace WPF_CAD.Views
{
    /// <summary>
    /// Interaction logic for DrawingPropertiesWindow.xaml
    /// </summary>
    public partial class DrawingPropertiesWindow : Window
    {
        private DrawingPropertiesWindowViewMode? DrawingPropertiesWindowViewMode => this.DataContext as DrawingPropertiesWindowViewMode;
        private MainWindowViewMode? MainWindowViewMode => this.Owner?.DataContext as MainWindowViewMode;

        public DrawingPropertiesWindow(DrawingPropertiesWindowViewMode vm)
        {
            InitializeComponent();

            this.DataContext = vm;

            this.Loaded += DrawingPropertiesWindow_Loaded;
            this.Closing += DrawingPropertiesWindow_Closing;
        }

        private void DrawingPropertiesWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (DrawingPropertiesWindowViewMode != null && MainWindowViewMode != null && MainWindowViewMode.SelectedDrawing != null)
            {
                DrawingPropertiesWindowViewMode.UpdateDrawingPropertiesCommand.Execute(MainWindowViewMode?.SelectedDrawing);
            }
        }

        private void DrawingPropertiesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DrawingPropertiesWindowViewMode != null && MainWindowViewMode != null && MainWindowViewMode.SelectedDrawing != null)
            {
                DrawingPropertiesWindowViewMode.UpdateDrawingPropertiesWindowCommand.Execute(MainWindowViewMode?.SelectedDrawing);
            }
        }
    }
}
