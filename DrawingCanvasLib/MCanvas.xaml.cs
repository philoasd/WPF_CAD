using DrawingCanvasLib.DrawTool;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
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

namespace DrawingCanvasLib
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MCanvas : UserControl
    {
        #region DependencyProperty

        public static readonly DependencyProperty CurPosProperty =
            DependencyProperty.Register("CurPos", typeof(SKPoint), typeof(MCanvas), new PropertyMetadata(new SKPoint(0, 0)));
        /// <summary>
        /// 当前鼠标位置
        /// </summary>
        public SKPoint CurPos
        {
            get => (SKPoint)GetValue(CurPosProperty);
            set => SetValue(CurPosProperty, value);
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(SKBitmap), typeof(MCanvas), new PropertyMetadata(null));
        /// <summary>
        /// 要绘制的图片
        /// </summary>
        public SKBitmap Image
        {
            get => (SKBitmap)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty SelectedDrawingProperty =
            DependencyProperty.Register("SelectedDrawing", typeof(BaseToolClass), typeof(MCanvas), new PropertyMetadata(null));
        /// <summary>
        /// 当前选中的图形
        /// </summary>
        public BaseToolClass SelectedDrawing
        {
            get => (BaseToolClass)GetValue(SelectedDrawingProperty);
            set => SetValue(SelectedDrawingProperty, value);
        }

        public static readonly DependencyProperty DrawingToolProperty =
            DependencyProperty.Register("DrawingTool", typeof(ToolType), typeof(MCanvas), new PropertyMetadata(ToolType.Select));
        /// <summary>
        /// 当前的画笔工具
        /// </summary>
        public ToolType DrawingTool
        {
            get => (ToolType)GetValue(DrawingToolProperty);
            set => SetValue(DrawingToolProperty, value);
        }

        public static readonly DependencyProperty ArtWorkListProperty =
            DependencyProperty.Register("ArtWorkList", typeof(List<BaseToolClass>), typeof(MCanvas), new PropertyMetadata(new List<BaseToolClass>()));
        /// <summary>
        /// 当前绘制的图形列表
        /// </summary>
        public List<BaseToolClass> ArtWorkList
        {
            get => (List<BaseToolClass>)GetValue(ArtWorkListProperty);
            set => SetValue(ArtWorkListProperty, value);
        }

        #endregion

        #region Value
        /// <summary>
        /// 平移偏移量
        /// </summary>
        private SKPoint _translate { get; set; } = new SKPoint(0, 0);

        /// <summary>
        /// 缩放比例
        /// </summary>
        private float _scale { get; set; } = 1.0f;

        /// <summary>
        /// 最大缩放比例
        /// </summary>
        private float _maxZoom => 1000.0f;

        /// <summary>
        /// 最小缩放比例
        /// </summary>
        private float _minZoom => 1.0f;

        /// <summary>
        /// 当前拖拽的角
        /// </summary>
        private int _activeHandleIndex { get; set; } = -1;
        #endregion

        #region Drawing Tool Value

        /// <summary>
        /// 直线
        /// </summary>
        private LineClass _line { get; set; } = null;

        #endregion

        public MCanvas()
        {
            InitializeComponent();

            this.Loaded += MCanvas_Loaded;
        }

        private void MCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            this.Canvas.PaintSurface += Canvas_PaintSurface;
            this.Canvas.MouseUp += Canvas_MouseUp;
            this.Canvas.MouseDown += Canvas_MouseDown;
            this.Canvas.MouseMove += Canvas_MouseMove;
            this.Canvas.MouseWheel += Canvas_MouseWheel;
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取当前鼠标位置（在窗口中的位置）
            var mousePosition = e.GetPosition(this.Canvas);

            // 根据滚轮增量调整缩放比例
            float zoomFactor = e.Delta > 0 ? 1.25f : 0.8f;
            _scale *= zoomFactor;

            if (_scale < _minZoom)
            {
                _scale = _minZoom;
                _translate = new SKPoint(0, 0);
                this.Canvas.InvalidateVisual();
                return;
            }
            else if (_scale > _maxZoom)
            {
                _scale = _maxZoom;
                //return;
            }

            // 调整平移，使得鼠标对应的画布位置保持不变
            var offsetX = mousePosition.X - CurPos.X * _scale;
            var offsetY = mousePosition.Y - CurPos.Y * _scale;
            _translate = new SKPoint((float)offsetX, (float)offsetY);

            // 重新绘制
            this.Canvas.InvalidateVisual();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sender as IInputElement);
            CurPos = new SKPoint(
                (float)((pos.X - _translate.X) / _scale),
                (float)((pos.Y - _translate.Y) / _scale)
            );
            BaseToolClass.CurrentPoint = CurPos;

            switch (DrawingTool)
            {
                case ToolType.Select:
                    {
                        break;
                    }
                case ToolType.Line:
                    {
                        if (_line != null)
                        {
                            _line.EndPoint = BaseToolClass.CurrentPoint;
                            this.Canvas.InvalidateVisual();
                        }
                        break;
                    }
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (DrawingTool)
            {
                case ToolType.Select:
                    {
                        CheckClickPoint(BaseToolClass.CurrentPoint);
                        this.Canvas.InvalidateVisual();
                        break;
                    }
                case ToolType.Line:
                    {
                        _line = new(BaseToolClass.CurrentPoint);
                        ArtWorkList.Add(_line);
                        break;
                    }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = null;

            switch (DrawingTool)
            {
                case ToolType.Select:
                    {
                        break;
                    }
                case ToolType.Line:
                    {
                        _line = null;
                        break;
                    }
            }
        }

        private void Canvas_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.White);

            // 应用缩放和平移
            e.Surface.Canvas.Translate(_translate.X, _translate.Y);
            e.Surface.Canvas.Scale(_scale);

            // 绘制图片，自适应图片大小
            if (Image != null)
            {
                var info = e.Info; // 当前画布的大小（控件大小）

                // 原始图像尺寸
                var imageWidth = Image.Width;
                var imageHeight = Image.Height;

                // 控件尺寸（目标尺寸）
                var targetWidth = info.Width;
                var targetHeight = info.Height;

                // 计算等比缩放
                float scale = Math.Min((float)targetWidth / imageWidth, (float)targetHeight / imageHeight);

                float scaledWidth = imageWidth * scale;
                float scaledHeight = imageHeight * scale;

                // 让图像居中
                float x = (targetWidth - scaledWidth) / 2f;
                float y = (targetHeight - scaledHeight) / 2f;

                var destRect = new SKRect(x, y, x + scaledWidth, y + scaledHeight);
                e.Surface.Canvas.DrawBitmap(Image, destRect);
            }

            // 绘制图形
            foreach (var tool in ArtWorkList)
            {
                tool.PaintStyle.StrokeWidth = 1 / _scale;
                tool.Draw(e.Surface.Canvas);
                tool.ResizePaintStyle.StrokeWidth = 1 / _scale;
            }
        }

        /// <summary>
        /// 检查点击点是否在图形内
        /// </summary>
        /// <param name="clickPoint"></param>
        /// <returns></returns>
        private void CheckClickPoint(SKPoint clickPoint)
        {
            foreach (var tool in ArtWorkList)
            {
                tool.IsSelected = false;
                //if (IsPointInRect(tool.StartPoint, tool.EndPoint, clickPoint, tool.CircleSize * tool.PaintStyle.StrokeWidth))
                if (tool.IsPointInPath(clickPoint))
                {
                    tool.IsSelected = true;

                    // 将其他图形的选中状态设置为 false
                    foreach (var otherTool in ArtWorkList)
                    {
                        if (otherTool != tool)
                        {
                            otherTool.IsSelected = false;
                        }
                    }

                    break;
                }
            }

            this.Canvas.InvalidateVisual();
        }

        private void UpdateDrawingPosition(SKPoint offset)
        {
            foreach (var tool in ArtWorkList)
            {
                if (!tool.IsSelected) { continue; }

                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y + offset.Y);
                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y + offset.Y);
                this.Canvas.InvalidateVisual();
            }
        }

        private void UpdateDrawingSize(SKPoint offset, int flag)
        {
            if (flag < 1) { return; }
            foreach (var tool in ArtWorkList)
            {
                if (!tool.IsSelected) { continue; }

                if (tool.StartPoint.X < tool.EndPoint.X && tool.StartPoint.Y < tool.EndPoint.Y) // 起始点在左上角
                {
                    switch (flag)
                    {
                        case 1: // 左上 ↖
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y + offset.Y);
                                break;
                            }
                        case 2: // 右上 ↗
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X, tool.StartPoint.Y + offset.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y);
                                break;
                            }
                        case 3: // 左下 ↙
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                        case 4: // 右下 ↘
                            {
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                    }
                }
                else if (tool.StartPoint.X > tool.EndPoint.X && tool.StartPoint.Y < tool.EndPoint.Y) // 起始点在右上角
                {
                    switch (flag)
                    {
                        case 1: // 左上 ↖
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X, tool.StartPoint.Y + offset.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y);
                                break;
                            }
                        case 2: // 右上 ↗
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y + offset.Y);
                                break;
                            }
                        case 3: // 左下 ↙
                            {
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                        case 4: // 右下 ↘
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                    }
                }
                else if (tool.StartPoint.X < tool.EndPoint.X && tool.StartPoint.Y > tool.EndPoint.Y) // 起始点在左下角
                {
                    switch (flag)
                    {
                        case 1: // 左上 ↖
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                        case 2: // 右上 ↗
                            {
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                        case 3: // 左下 ↙
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y + offset.Y);
                                break;
                            }
                        case 4: // 右下 ↘
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X, tool.StartPoint.Y + offset.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y);
                                break;
                            }
                    }
                }
                else if (tool.StartPoint.X > tool.EndPoint.X && tool.StartPoint.Y > tool.EndPoint.Y) // 起始点在右下角
                {
                    switch (flag)
                    {
                        case 1: // 左上 ↖
                            {
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                        case 2: // 右上 ↗
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X, tool.EndPoint.Y + offset.Y);
                                break;
                            }
                        case 3: // 左下 ↙
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X, tool.StartPoint.Y + offset.Y);
                                tool.EndPoint = new SKPoint(tool.EndPoint.X + offset.X, tool.EndPoint.Y);
                                break;
                            }
                        case 4: // 右下 ↘
                            {
                                tool.StartPoint = new SKPoint(tool.StartPoint.X + offset.X, tool.StartPoint.Y + offset.Y);
                                break;
                            }
                    }
                }

                this.Canvas.InvalidateVisual();
            }
        }

        /// <summary>
        /// 当鼠标移动到选中图形的最大外矩形的4个角时，改变鼠标形状
        /// </summary>
        /// <param name="point"></param>
        private void ChangeMouseShape(SKPoint point)
        {
            foreach (var tool in ArtWorkList)
            {
                if (!tool.IsSelected) { continue; }

                // 当前选中路径的外接矩形大小
                var rect = tool.OutLinePath.TightBounds;

                var topLeftX = rect.Left;
                var topLeftY = rect.Top;
                var width = rect.Width;
                var height = rect.Height;

                // 如果当前鼠标位置在圆形上，则改变鼠标形状
                var circleSize = tool.CircleSize * tool.PaintStyle.StrokeWidth;
                if (point.X >= topLeftX - circleSize && point.X <= topLeftX + circleSize &&
                    point.Y >= topLeftY - circleSize && point.Y <= topLeftY + circleSize)
                {
                    Mouse.OverrideCursor = Cursors.ScrollNW; // 左上 ↖
                    _activeHandleIndex = 1;
                }
                else if (point.X >= topLeftX + width - circleSize && point.X <= topLeftX + width + circleSize &&
                         point.Y >= topLeftY - circleSize && point.Y <= topLeftY + circleSize)
                {
                    Mouse.OverrideCursor = Cursors.ScrollNE; // 右上 ↗
                    _activeHandleIndex = 2;
                }
                else if (point.X >= topLeftX - circleSize && point.X <= topLeftX + circleSize &&
                         point.Y >= topLeftY + height - circleSize && point.Y <= topLeftY + height + circleSize)
                {
                    Mouse.OverrideCursor = Cursors.ScrollSW; // 左下 ↙
                    _activeHandleIndex = 3;
                }
                else if (point.X >= topLeftX + width - circleSize && point.X <= topLeftX + width + circleSize &&
                         point.Y >= topLeftY + height - circleSize && point.Y <= topLeftY + height + circleSize)
                {
                    Mouse.OverrideCursor = Cursors.ScrollSE; // 右下 ↘
                    _activeHandleIndex = 4;
                }
                else if (tool.IsPointInPath(point))
                //else if (IsPointInRect(new SKPoint(topLeftX, topLeftY), new SKPoint(topLeftX + width, topLeftY + height), point, circleSize))
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                    _activeHandleIndex = 0;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    _activeHandleIndex = -1;
                }
            }
        }
    }
}
