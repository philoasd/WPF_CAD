using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawingCanvasLib.DrawTool;
using SkiaSharp;

namespace WPF_CAD.ViewModes
{
    public class DrawingPropertiesWindowViewMode : ObservableObject
    {
        #region Flag

        private bool IsNeedUpdateDrawingProperties { get; set; } = false;

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                SetProperty(ref _selectedIndex, value);
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// 确定按钮命令
        /// </summary>
        public RelayCommand<Window> OKCommand => new((obj) =>
        {
            if (obj == null) { return; }

            IsNeedUpdateDrawingProperties = true;

            // 关闭窗口
            obj.Close();
        });

        /// <summary>
        /// 取消按钮命令
        /// </summary>
        public RelayCommand<Window> CancelCommand => new((obj) =>
        {
            if (obj == null) { return; }

            IsNeedUpdateDrawingProperties = false;
            // 关闭窗口
            obj.Close();
        });

        /// <summary>
        /// 更新绘图属性的窗口信息命令
        /// </summary>
        public RelayCommand<BaseDrawingClass> UpdateDrawingPropertiesWindowCommand => new((obj) =>
        {
            if (obj == null) { return; }

            switch (obj.ToolType)
            {
                case DrawingCanvasLib.ToolType.Line:
                    {
                        IsShowLinePage = Visibility.Visible;
                        this.LineFromX = obj.StartPoint.X;
                        this.LineFromY = obj.StartPoint.Y;
                        this.LineToX = obj.EndPoint.X;
                        this.LineToY = obj.EndPoint.Y;
                        break;
                    }
                case DrawingCanvasLib.ToolType.Rectangle:
                    {
                        IsShowFormatPage = Visibility.Visible;
                        SelectedIndex = 5;

                        // 当前选中路径的外接矩形大小
                        var rect = obj.OutLinePath.TightBounds;
                        this.FormatWidth = rect.Width;
                        this.FormatHeight = rect.Height;
                        this.FormatCenterX = rect.MidX;
                        this.FormatCenterY = rect.MidY;

                        break;
                    }
            }
        });

        /// <summary>
        /// 更新绘图属性命令
        /// </summary>
        public RelayCommand<BaseDrawingClass> UpdateDrawingPropertiesCommand => new((obj) =>
        {
            if (obj == null || !IsNeedUpdateDrawingProperties) { return; }

            switch (obj.ToolType)
            {
                case DrawingCanvasLib.ToolType.Line:
                    {
                        obj.StartPoint = new SKPoint((float)this.LineFromX, (float)this.LineFromY);
                        obj.EndPoint = new SKPoint((float)this.LineToX, (float)this.LineToY);
                        break;
                    }
                case DrawingCanvasLib.ToolType.Rectangle:
                    {
                        var newStartPoint = new SKPoint((float)(this.FormatCenterX + this.FormatRelativeMoveX) - (float)this.FormatWidth / 2, (float)(this.FormatCenterY + this.FormatRelativeMoveY) - (float)this.FormatHeight / 2);
                        var newEndPoint = new SKPoint((float)(this.FormatCenterX + this.FormatRelativeMoveX) + (float)this.FormatWidth / 2, (float)(this.FormatCenterY + this.FormatRelativeMoveY) + (float)this.FormatHeight / 2);
                        obj.StartPoint = newStartPoint;
                        obj.EndPoint = newEndPoint;
                        break;
                    }
            }
        });

        #endregion

        #region Line page

        private Visibility _isShowLinePage = Visibility.Collapsed;
        public Visibility IsShowLinePage
        {
            get => _isShowLinePage;
            set
            {
                SetProperty(ref _isShowLinePage, value);
                if (value == Visibility.Visible)
                {
                    IsShowDataPage = Visibility.Collapsed;
                    IsShowTextPage = Visibility.Collapsed;
                    IsShowBarcodePage = Visibility.Collapsed;
                    IsShowSerializePage = Visibility.Collapsed;
                    IsShowFormatPage = Visibility.Collapsed;
                    IsShowHatchPage = Visibility.Collapsed;
                }
            }
        }

        private double _lineFromX = 0;
        public double LineFromX
        {
            get => _lineFromX;
            set => SetProperty(ref _lineFromX, value);
        }

        private double _lineFromY = 0;
        public double LineFromY
        {
            get => _lineFromY;
            set => SetProperty(ref _lineFromY, value);
        }

        private double _lineToX = 0;
        public double LineToX
        {
            get => _lineToX;
            set => SetProperty(ref _lineToX, value);
        }

        private double _lineToY = 0;
        public double LineToY
        {
            get => _lineToY;
            set => SetProperty(ref _lineToY, value);
        }

        #endregion

        #region Data page

        private Visibility _isShowDataPage = Visibility.Collapsed;
        public Visibility IsShowDataPage
        {
            get => _isShowDataPage;
            set
            {
                SetProperty(ref _isShowDataPage, value);
                if (value == Visibility.Visible)
                {
                    //IsShowLinePage = Visibility.Collapsed;
                    //IsShowTextPage = Visibility.Collapsed;
                    //IsShowBarcodePage = Visibility.Collapsed;
                    //IsShowSerializePage = Visibility.Collapsed;
                    //IsShowFormatPage = Visibility.Collapsed;
                    //IsShowHatchPage = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Text page

        private Visibility _isShowTextPage = Visibility.Collapsed;
        public Visibility IsShowTextPage
        {
            get => _isShowTextPage;
            set
            {
                SetProperty(ref _isShowTextPage, value);
                if (value == Visibility.Visible)
                {
                    IsShowDataPage = Visibility.Visible;
                    IsShowLinePage = Visibility.Collapsed;
                    IsShowBarcodePage = Visibility.Collapsed;
                    IsShowSerializePage = Visibility.Visible;
                    IsShowFormatPage = Visibility.Visible;
                    IsShowHatchPage = Visibility.Visible;
                }
            }
        }

        #endregion

        #region Barcode page

        private Visibility _isShowBarcodePage = Visibility.Collapsed;
        public Visibility IsShowBarcodePage
        {
            get => _isShowBarcodePage;
            set
            {
                SetProperty(ref _isShowBarcodePage, value);
                if (value == Visibility.Visible)
                {
                    IsShowDataPage = Visibility.Visible;
                    IsShowTextPage = Visibility.Collapsed;
                    IsShowLinePage = Visibility.Collapsed;
                    IsShowSerializePage = Visibility.Visible;
                    IsShowFormatPage = Visibility.Visible;
                    IsShowHatchPage = Visibility.Visible;
                }
            }
        }
        #endregion

        #region Serialization

        private Visibility _isShowSerializePage = Visibility.Collapsed;
        public Visibility IsShowSerializePage
        {
            get => _isShowSerializePage;
            set
            {
                SetProperty(ref _isShowSerializePage, value);
                if (value == Visibility.Visible)
                {
                    //IsShowDataPage = Visibility.Collapsed;
                    //IsShowTextPage = Visibility.Collapsed;
                    //IsShowBarcodePage = Visibility.Collapsed;
                    //IsShowLinePage = Visibility.Collapsed;
                    //IsShowFormatPage = Visibility.Collapsed;
                    //IsShowHatchPage = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Format

        private Visibility _isShowFormatPage = Visibility.Collapsed;
        public Visibility IsShowFormatPage
        {
            get => _isShowFormatPage;
            set
            {
                SetProperty(ref _isShowFormatPage, value);
                if (value == Visibility.Visible)
                {
                    IsShowHatchPage = Visibility.Visible;
                }
            }
        }

        private double _formatWidth = 0;
        public double FormatWidth
        {
            get => _formatWidth;
            set => SetProperty(ref _formatWidth, value);
        }

        private double _formatHeight = 0;
        public double FormatHeight
        {
            get => _formatHeight;
            set => SetProperty(ref _formatHeight, value);
        }

        private int _formatRotate = 0;
        public int FormatRotate
        {
            get => _formatRotate;
            set => SetProperty(ref _formatRotate, value);
        }

        private int _formatItalics = 0;
        public int FormatItalics
        {
            get => _formatItalics;
            set => SetProperty(ref _formatItalics, value);
        }

        private double _formatCenterX = 0;
        public double FormatCenterX
        {
            get => _formatCenterX;
            set => SetProperty(ref _formatCenterX, value);
        }

        private double _formatCenterY = 0;
        public double FormatCenterY
        {
            get => _formatCenterY;
            set => SetProperty(ref _formatCenterY, value);
        }

        private double _formatRelativeMoveX = 0;
        public double FormatRelativeMoveX
        {
            get => _formatRelativeMoveX;
            set => SetProperty(ref _formatRelativeMoveX, value);
        }

        private double _formatRelativeMoveY = 0;
        public double FormatRelativeMoveY
        {
            get => _formatRelativeMoveY;
            set => SetProperty(ref _formatRelativeMoveY, value);
        }

        #endregion

        #region Hatch

        private Visibility _isShowHatchPage = Visibility.Collapsed;
        public Visibility IsShowHatchPage
        {
            get => _isShowHatchPage;
            set
            {
                SetProperty(ref _isShowHatchPage, value);
                if (value == Visibility.Visible)
                {
                    //IsShowDataPage = Visibility.Collapsed;
                    //IsShowTextPage = Visibility.Collapsed;
                    //IsShowBarcodePage = Visibility.Collapsed;
                    //IsShowSerializePage = Visibility.Collapsed;
                    //IsShowFormatPage = Visibility.Collapsed;
                    //IsShowLinePage = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Marking

        #endregion
    }
}
