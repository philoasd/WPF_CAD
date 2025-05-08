using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawingCanvasLib;
using DrawingCanvasLib.DrawTool;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using SkiaSharp;
using WPF_CAD.Modes;
using WPF_CAD.Utils;

namespace WPF_CAD.ViewModes
{
    public class MainWindowViewMode : ObservableObject
    {
        private ProcessMode? ProcessMode => App.ServiceProvider?.GetRequiredService<ProcessMode>();

        public MainWindowViewMode()
        {
            Title = $"{_mianTitle} - {OpenFileName}";
        }

        public string _mianTitle => "WPF_CAD Software";

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _openFileName = string.Empty;
        public string OpenFileName
        {
            get => _openFileName;
            set => SetProperty(ref _openFileName, value);
        }

        private string _openFilePath = string.Empty;
        public string OpenFilePath
        {
            get => _openFilePath;
            set => SetProperty(ref _openFilePath, value);
        }

        private string _status = string.Empty;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _dataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string DataTime
        {
            get => _dataTime;
            set => SetProperty(ref _dataTime, value);
        }

        private bool _isAutoMode = false;
        public bool IsAutoMode
        {
            get => _isAutoMode;
            set
            {
                SetProperty(ref _isAutoMode, value);
                if (value)
                {
                    ProcessMode?.EnteringAutoMode();
                }
                else
                {
                    ProcessMode?.ExitingAutoMode();
                }
            }
        }

        private ObservableCollection<BaseDrawingClass> _drawingList = new();
        public ObservableCollection<BaseDrawingClass> DrawingList
        {
            get => _drawingList;
            set => SetProperty(ref _drawingList, value);
        }

        private BaseDrawingClass? _selectedDrawing = null;
        public BaseDrawingClass? SelectedDrawing
        {
            get => _selectedDrawing;
            set
            {
                SetProperty(ref _selectedDrawing, value);

                if (value != null)
                {
                    // 当前选中路径的外接矩形大小
                    var rect = value.OutLinePath.TightBounds;

                    DrawingWidth = rect.Width;
                    DrawingHeight = rect.Height;

                    DrawingCenterX = rect.Left + rect.Width / 2;
                    DrawingCenterY = rect.Top + rect.Height / 2;
                }
            }
        }

        #region Drawing Properties

        private float _drawingWidth = 0.0f;
        public float DrawingWidth
        {
            get => _drawingWidth;
            set
            {
                // 保留三位小数
                value = (float)Math.Round(value, 3);
                SetProperty(ref _drawingWidth, value);
            }
        }

        private float _drawingHeight = 0.0f;
        public float DrawingHeight
        {
            get => _drawingHeight;
            set
            {
                // 保留三位小数
                value = (float)Math.Round(value, 3);
                SetProperty(ref _drawingHeight, value);
            }
        }

        private float _drawingCenterX = 0.0f;
        public float DrawingCenterX
        {
            get => _drawingCenterX;
            set
            {
                // 保留三位小数
                value = (float)Math.Round(value, 3);
                SetProperty(ref _drawingCenterX, value);
            }
        }

        private float _drawingCenterY = 0.0f;
        public float DrawingCenterY
        {
            get => _drawingCenterY;
            set
            {
                // 保留三位小数
                value = (float)Math.Round(value, 3);
                SetProperty(ref _drawingCenterY, value);
            }
        }

        #endregion

        private SKPoint _curPos = new SKPoint(0, 0);
        public SKPoint CurPos
        {
            get => _curPos;
            set
            {
                SetProperty(ref _curPos, value);

                Status = $"X: {CurPos.X:F3}, Y: {CurPos.Y:F3}";
            }
        }

        private ToolType _drawingTool = ToolType.Select;
        public ToolType DrawingTool
        {
            get => _drawingTool;
            set
            {
                SetProperty(ref _drawingTool, value);
                // 这里可以添加切换工具的逻辑
            }
        }

        #region menu commands
        public RelayCommand NewFileCommand => new(() =>
        {
            // 关闭当前文件，清空当前显示内容
            OpenFileName = string.Empty;
            Title = $"{_mianTitle} - {OpenFileName}";

            DrawingTool = ToolType.Clear;
        });

        public RelayCommand OpenFileCommand => new(() =>
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Font Files (*.json)|*.json",
                DefaultExt = ".json",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFilePath = openFileDialog.FileName;
                OpenFileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                Title = $"{_mianTitle} - {OpenFileName}";

                string json = File.ReadAllText(OpenFilePath);
            }
        });

        public RelayCommand SaveFileCommand => new(() =>
        {
            if (string.IsNullOrEmpty(OpenFileName))
            {
                // 打开一个文件选择框
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Font Files (*.json)|*.json",
                    DefaultExt = ".json",
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    OpenFilePath = saveFileDialog.FileName;
                    OpenFileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                    Title = $"{_mianTitle} - {OpenFileName}";
                }
                else
                {
                    return;
                }
            }

            string json = "TEST";
            File.WriteAllText(OpenFilePath, json);
            MsgBoxClass.ShowMsg("Save File Successfully", MsgBoxClass.MsgBoxType.Information);
        });

        public RelayCommand SaveAsFileCommand => new(() =>
        {
            // 打开一个文件选择框
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Font Files (*.json)|*.json",
                DefaultExt = ".json",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                OpenFilePath = saveFileDialog.FileName;
                OpenFileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                Title = $"{_mianTitle} - {OpenFileName}";

                string json = "TEST";
                File.WriteAllText("artwork.json", json);
                MsgBoxClass.ShowMsg("Save File Successfully", MsgBoxClass.MsgBoxType.Information);
            }
        });

        public RelayCommand SaveFileAsPltCommand => new(() =>
        {

        });

        public RelayCommand HardWareSetupCommand => new(() =>
        {
            // 打开hardware setup window
        });
        #endregion

        #region toolbar commands

        /// <summary>
        /// 切换绘图工具
        /// </summary>
        public RelayCommand<string> SwitchDrawingToolCommand => new((obj) =>
        {
            if (obj == null) { return; }
            var toolType = int.Parse(obj.ToString());

            DrawingTool = (ToolType)toolType;
        });

        #endregion
    }
}
