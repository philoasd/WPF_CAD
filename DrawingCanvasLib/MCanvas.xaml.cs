using DrawingCanvasLib.DrawTool;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using WPF_Draw.DrawTool;

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
            DependencyProperty.Register("SelectedDrawing", typeof(BaseDrawingClass), typeof(MCanvas), new PropertyMetadata(null, OnSelectedDrawingChanged));
        /// <summary>
        /// 当前选中的图形
        /// </summary>
        public BaseDrawingClass SelectedDrawing
        {
            get => (BaseDrawingClass)GetValue(SelectedDrawingProperty);
            set => SetValue(SelectedDrawingProperty, value);
        }
        private static void OnSelectedDrawingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MCanvas canvas)
            {
                if (e.NewValue is BaseDrawingClass tool)
                {
                    foreach (var item in canvas.ArtWorkList)
                    {
                        item.IsSelected = false;
                        if (item == tool)
                        {
                            item.IsSelected = true;

                            canvas.Canvas.InvalidateVisual();
                        }
                    }
                    canvas.Canvas.InvalidateVisual();
                }
            }
        }

        public static readonly DependencyProperty DrawingToolProperty =
            DependencyProperty.Register("DrawingTool", typeof(ToolType), typeof(MCanvas), new PropertyMetadata(ToolType.Select, OnDrawingToolChanged));
        /// <summary>
        /// 当前的画笔工具
        /// </summary>
        public ToolType DrawingTool
        {
            get => (ToolType)GetValue(DrawingToolProperty);
            set
            {
                SetValue(DrawingToolProperty, value);
            }
        }
        private static void OnDrawingToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MCanvas canvas)
            {
                if (e.NewValue is ToolType toolType)
                {
                    //if (toolType == ToolType.Clear)
                    //{
                    //    canvas.ArtWorkList.Clear();
                    //    canvas.Canvas.InvalidateVisual();
                    //}
                    switch (toolType)
                    {
                        case ToolType.Clear:
                            {
                                canvas.ArtWorkList.Clear();
                                canvas.Canvas.InvalidateVisual();
                                break;
                            }
                    }
                }
            }
        }

        public static readonly DependencyProperty ArtWorkListProperty =
            DependencyProperty.Register("ArtWorkList", typeof(ObservableCollection<BaseDrawingClass>), typeof(MCanvas), new PropertyMetadata(new ObservableCollection<BaseDrawingClass>()));
        /// <summary>
        /// 当前绘制的图形列表
        /// </summary>
        public ObservableCollection<BaseDrawingClass> ArtWorkList
        {
            get => (ObservableCollection<BaseDrawingClass>)GetValue(ArtWorkListProperty);
            set
            {
                SetValue(ArtWorkListProperty, value);
            }
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

        /// <summary>
        /// 当前点击点
        /// </summary>
        private SKPoint _clickPoint { get; set; } = new SKPoint(0, 0);

        /// <summary>
        /// 上次点击时间，用于判断双击
        /// </summary>
        private DateTime _lastClickTime { get; set; } = DateTime.Now;
        #endregion

        #region Drawing Tool Value

        /// <summary>
        /// 绘图元素
        /// </summary>
        private BaseDrawingClass? _drawing { get; set; } = null;

        #endregion

        #region event
        public static EventHandler? OnDoubleClickEvent;
        #endregion

        public MCanvas()
        {
            InitializeComponent();

            this.Loaded += MCanvas_Loaded;
        }

        private void MCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            this.Canvas.PaintSurface += Canvas_PaintSurface;
            this.Canvas.MouseLeftButtonUp += Canvas_LeftButtonUp;
            //this.Canvas.MouseLeftButtonDown += Canvas_LeftButtonDown;
            this.Canvas.MouseMove += Canvas_MouseMove;
            this.Canvas.MouseWheel += Canvas_MouseWheel;
            this.Canvas.MouseDown += Canvas_MouseDown;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _clickPoint = BaseDrawingClass.CurrentPoint;

            // 根据鼠标按下的按钮类型区分：左键、中键、右键
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    {
                        // 左键按下
                        Canvas_LeftButtonDown(sender, e);
                        break;
                    }
                case MouseButton.Middle:
                    {
                        // 中键按下
                        break;
                    }
                case MouseButton.Right:
                    {
                        // 右键按下
                        break;
                    }
            }
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
            BaseDrawingClass.CurrentPoint = CurPos;
            UpdateSelectedDrawing();

            if (_drawing != null)
            {
                _drawing.EndPoint = BaseDrawingClass.CurrentPoint;
                this.Canvas.InvalidateVisual();
            }

            #region dragging
            if (e.LeftButton.HasFlag(MouseButtonState.Pressed)) // drawing
            {
                var offsetX = BaseDrawingClass.CurrentPoint.X - _clickPoint.X;
                var offsetY = BaseDrawingClass.CurrentPoint.Y - _clickPoint.Y;
                var offset = new SKPoint(offsetX, offsetY);

                switch (_activeHandleIndex)
                {
                    case 0:
                        {
                            UpdateDrawingPosition(offset);
                            break;
                        }
                    default:
                        {
                            UpdateDrawingSize(offset, _activeHandleIndex);
                            break;
                        }
                }

                _clickPoint = BaseDrawingClass.CurrentPoint;
            }
            else if (e.MiddleButton.HasFlag(MouseButtonState.Pressed)) // canvas
            {
#if DEBUG
                Debug.WriteLine($"Middle Button: {BaseDrawingClass.CurrentPoint}");
#endif
                // 如果当前缩放比例大于最小值，则允许平移
                if (_scale <= _minZoom) { return; }

                var offsetX = BaseDrawingClass.CurrentPoint.X - _clickPoint.X;
                var offsetY = BaseDrawingClass.CurrentPoint.Y - _clickPoint.Y;
                // 根据缩放比例调整平移量
                _translate = new SKPoint(_translate.X + offsetX * _scale, _translate.Y + offsetY * _scale);
                _clickPoint = BaseDrawingClass.CurrentPoint;
                this.Canvas.InvalidateVisual();
            }
            else
            {
                ChangeMouseShape(BaseDrawingClass.CurrentPoint);
            }
            #endregion
        }

        private void Canvas_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if DEBUG
            Debug.WriteLine($"LeftButtonDown: {BaseDrawingClass.CurrentPoint}");
#endif
            //_clickPoint = BaseDrawingClass.CurrentPoint;

            switch (DrawingTool)
            {
                case ToolType.Select:
                    {
                        CheckClickPoint(BaseDrawingClass.CurrentPoint);
                        this.Canvas.InvalidateVisual();
                        break;
                    }
                case ToolType.Line:
                    {
                        _drawing = new LineClass(BaseDrawingClass.CurrentPoint);
                        break;
                    }
                case ToolType.Rectangle:
                    {
                        _drawing = new RectangleClass(BaseDrawingClass.CurrentPoint);
                        break;
                    }
                case ToolType.Ellipse:
                    {
                        _drawing = new EllipseClass(BaseDrawingClass.CurrentPoint);
                        break;
                    }
            }

            if (_drawing != null)
            {
                ArtWorkList.Add(_drawing);
                _drawing.OnDrawingCenter = (msg) =>
                {
                    CenterDrawing(msg);
                };
            }

            #region 双击事件
            DateTime now = DateTime.Now; // 获取当前点击时间
            TimeSpan timeSpan = now - _lastClickTime; // 计算时间间隔
#if DEBUG
            Debug.WriteLine($"click time: {timeSpan.TotalMilliseconds} ms");
#endif
            if (timeSpan.TotalMilliseconds < 180) // 如果时间间隔小于100毫秒，则认为是双击
            {
                // 双击事件处理逻辑
                if (SelectedDrawing != null)
                {
                    OnDoubleClickEvent?.Invoke(null, EventArgs.Empty);
                }
            }
            else
            {
                _lastClickTime = now; // 更新上次点击时间
            }
            #endregion
        }

        private void Canvas_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = null;
            _drawing = null;

            DrawingTool = ToolType.Select; // 结束绘制
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
            SelectedDrawing = null;

            foreach (var tool in ArtWorkList)
            {
                tool.IsSelected = false;
                //if (IsPointInRect(tool.StartPoint, tool.EndPoint, clickPoint, tool.CircleSize * tool.PaintStyle.StrokeWidth))
                if (tool.IsPointInPath(clickPoint))
                {
                    tool.IsSelected = true;
                    SelectedDrawing = tool;
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

        /// <summary>
        /// 更新选中图形
        /// </summary>
        private void UpdateSelectedDrawing()
        {
            foreach (var tool in ArtWorkList)
            {
                if (tool.IsSelected)
                {
                    SelectedDrawing = tool;
                    break;
                }
            }
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

        /// <summary>
        /// 刷新画布
        /// </summary>
        public void Fresh()
        {
            this.Canvas.InvalidateVisual();
        }

        /// <summary>
        /// 将绘图元素居中到画布中心
        /// </summary>
        /// <param name="drawing"></param>
        private void CenterDrawing(BaseDrawingClass drawing)
        {
            // 获取当前路径的外接矩形
            var rect = drawing.OutLinePath.TightBounds;
            var drawingWidth = rect.Width;
            var drawingHeight = rect.Height;

            // 控件尺寸（目标尺寸）
            var info = this.Canvas;
            var targetWidth = info.ActualWidth;
            var targetHeight = info.ActualHeight;

            drawing.StartPoint = new SKPoint((float)(targetWidth / 2 - drawingWidth / 2), (float)(targetHeight / 2 - drawingHeight / 2));
            drawing.EndPoint = new SKPoint((float)(targetWidth / 2 + drawingWidth / 2), (float)(targetHeight / 2 + drawingHeight / 2));
        }
    }
}
