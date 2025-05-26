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
using WPF_CAD.Views;
using WPF_Draw.DrawTool;

namespace WPF_CAD.ViewModes
{
    public class MainWindowViewMode : ObservableObject
    {
        private ProcessMode? ProcessMode => App.ServiceProvider?.GetRequiredService<ProcessMode>();

        #region Event
        public event EventHandler? OnRefreshEvent;
        #endregion

        public MainWindowViewMode()
        {
            Title = $"{_mianTitle} - {OpenFileName}";
        }


        #region Values
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

        private MachineConfigClass _machineConfig = new();
        public MachineConfigClass MachineConfig
        {
            get => _machineConfig;
            set => SetProperty(ref _machineConfig, value);
        }

        #endregion

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

            this.DrawingList.Clear();
            LoadArtWork(OpenFilePath);
            RefreshCommand.Execute(null);
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
                if (saveFileDialog.ShowDialog() != true) { return; }
                OpenFilePath = saveFileDialog.FileName;
                OpenFileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                Title = $"{_mianTitle} - {OpenFileName}";
            }

            if (this.DrawingList.Count == 0)
            {
                MessageBox.Show("No drawing to save.");
                return;
            }

            SaveArtWork(OpenFilePath);

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

            SaveArtWork(OpenFilePath);
        });

        public RelayCommand SaveFileAsPltCommand => new(() =>
        {

        });

        public RelayCommand RefreshCommand => new(() =>
        {
            OnRefreshEvent?.Invoke(this, EventArgs.Empty);
        });

        public RelayCommand<Window> HardWareSetupCommand => new((obj) =>
        {
            // 打开hardware setup window
            var hardwareWindow = App.ServiceProvider?.GetRequiredService<HardwareSetupWindow>();
            if (hardwareWindow == null) { return; }
            if (obj != null)
            {
                hardwareWindow.Owner = obj; // 设置父窗口
            }

            hardwareWindow?.ShowDialog();
        });
        #endregion

        #region Function
        /// <summary>
        /// 保存当前绘图
        /// </summary>
        public void SaveArtWork(string path)
        {
            List<ArtWorkProperties> artWorkList = new List<ArtWorkProperties>();
            foreach (var tool in this.DrawingList)
            {
                artWorkList.Add(tool.GetProperties());
            }

            // Serialize artWorkList to a file or database
            // For example, using JSON serialization:
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(artWorkList, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// 加载绘图
        /// </summary>
        public void LoadArtWork(string path)
        {
            // Deserialize from a file or database
            string json = File.ReadAllText(path);
            List<ArtWorkProperties>? artWorkList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ArtWorkProperties>>(json);
            if (artWorkList == null || artWorkList.Count == 0)
            {
                MessageBox.Show("No artwork to load.");
                return;
            }

            this.DrawingList.Clear();
            foreach (var artWork in artWorkList)
            {
                switch (artWork.Type)
                {
                    case ToolType.Line: // line
                        {
                            var line = new LineClass(artWork.StartPoint);
                            line.EndPoint = artWork.EndPoint;
                            //line.OutlineProperties = artWork.OutLineProperties;
                            //line.HatchProperties = artWork.HatchProperties;

                            this.DrawingList.Add(line);
                            break;
                        }
                    case ToolType.Rectangle: // rectangle
                        {
                            var rect = new RectangleClass(artWork.StartPoint);
                            rect.EndPoint = artWork.EndPoint;
                            rect.IsHatch = artWork.IsHatch;
                            rect.IsOutLine = artWork.IsOutline;
                            //rect.OutlineProperties = artWork.OutLineProperties;
                            //rect.HatchProperties = artWork.HatchProperties;

                            this.DrawingList.Add(rect);
                            break;
                        }
                    case ToolType.Ellipse: // Ellipse
                        {
                            var ellipse = new EllipseClass(artWork.StartPoint);
                            ellipse.EndPoint = artWork.EndPoint;
                            ellipse.IsHatch = artWork.IsHatch;
                            ellipse.IsOutLine = artWork.IsOutline;
                            //ellipse.OutlineProperties = artWork.OutLineProperties;
                            //ellipse.HatchProperties = artWork.HatchProperties;

                            this.DrawingList.Add(ellipse);
                            break;
                        }
                    case ToolType.Polygon: // Polygon
                        {
                            break;
                        }
                    case ToolType.Text: // Text
                        {
                            //var text = new TextClass(artWork.StartPoint);
                            //text.EndPoint = artWork.EndPoint;
                            //text.IsHatch = artWork.IsHatch;
                            //text.IsOutLine = artWork.IsOutline;
                            //text.DrawText = artWork.DrawText;
                            //text.TextSize = artWork.TextSize;
                            //text.Font = artWork.Font;
                            //this.DrawingList.Add(text);
                            break;
                        }
                    case ToolType.Barcode: // BarCode
                        {
                            break;
                        }
                }
            }

            #region 设置绘图属性

            foreach (var drawing in this.DrawingList)
            {
                var target = artWorkList.Find(x => x.Type == drawing.ToolType);
                if(target == null) { continue; }

                // hatch 属性
                drawing.IsHatch = target.IsHatch;
                drawing.HatchSpacing = target.HatchSpacing;

                // marking 属性
                drawing.OutlineProperties = target.OutLineProperties;
                drawing.HatchProperties = target.HatchProperties;
            }

            #endregion
        }
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
