using Clipper2Lib;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DrawingCanvasLib.DrawTool
{
    public abstract class BaseToolClass
    {
        public static SKPoint CurrentPoint { get; set; } = new();
        public SKPoint EndPoint { get; set; } = new();
        public SKPoint StartPoint { get; set; } = new();
        public string UID => Guid.NewGuid().ToString();
        public ToolType ToolType { get; set; } = 0;
        public List<SKPoint> Points { get; set; } = new();
        public bool IsHatch { get; set; } = false;
        public bool IsOutLine { get; set; } = true;
        protected static int UpSize => 1000;
        public int CircleSize => 10;

        public SKPath OutLinePath { get; set; } = new();
        public SKPath HatchPath { get; set; } = new();
        public float HatchSpacing { get; set; } = 1.0f; // 填充间距

        private bool _isSeleted = false;
        public bool IsSelected
        {
            get => _isSeleted;
            set
            {
                _isSeleted = value;
                _paintStyle.Color = value ? SKColors.DarkRed : SKColors.Black; // 修改已有实例的颜色
            }
        }

        private readonly SKPaint _paintStyle = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeWidth = 1,
            IsAntialias = true,
            IsStroke = true,
            StrokeCap = SKStrokeCap.Round,
        };

        private readonly SKPaint _resizePaintStyle = new SKPaint
        {
            Color = SKColors.Blue,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = true,
            IsStroke = true,
            StrokeCap = SKStrokeCap.Round,
        };

        public SKPaint PaintStyle => _paintStyle; // 只读属性返回相同的实例
        public SKPaint ResizePaintStyle => _resizePaintStyle; // 只读属性返回相同的实例

        public abstract void Draw(SKCanvas canvas);

        public void DrawResizeRect(SKCanvas canvas)
        {
            if (IsSelected)
            {
                // 当前选中路径的外接矩形大小
                var rect = OutLinePath.TightBounds;

                var topLeftX = rect.Left;
                var topLeftY = rect.Top;
                var width = rect.Width;
                var height = rect.Height;

                var path = new SKPath();
                // 添加矩形
                path.AddRect(rect);

                // 添加四个角的调整大小圆形
                var circleSize = CircleSize * PaintStyle.StrokeWidth;
                path.AddCircle(topLeftX, topLeftY, circleSize);
                path.AddCircle(topLeftX + width, topLeftY, circleSize);
                path.AddCircle(topLeftX, topLeftY + height, circleSize);
                path.AddCircle(topLeftX + width, topLeftY + height, circleSize);
                // 在 Canvas 上绘制路径
                canvas.DrawPath(path, ResizePaintStyle);
            }
        }

        protected SKPath GetHatchPath(SKPath outLinePath)
        {
            // 当前选中路径的外接矩形大小
            var rect = outLinePath.TightBounds;
            var topLeftX = rect.Left;
            var topLeftY = rect.Top;
            var width = rect.Width;
            var height = rect.Height;

            // 生成填充路径集合
            //HatchSpacing = HatchSpacing * (1000 * PaintStyle.StrokeWidth);
            PathsD openSub = new PathsD();
            for (double y = (topLeftY + HatchSpacing) * UpSize; y < (topLeftY + height) * UpSize; y += HatchSpacing * UpSize)
            {
                openSub.Add(Clipper.MakePath(new double[]
                {
                    topLeftX * UpSize, y, (topLeftX + width) * UpSize, y
                }));
            }

            #region 当前选中路径拆解
            PathsD closePath = new PathsD();
            var svgPath = outLinePath.ToSvgPathData();
            var geometry = Geometry.Parse(svgPath);
            PathGeometry pathGeometry = geometry.GetFlattenedPathGeometry(0.01, ToleranceType.Absolute);
            PathFigure[] pathFigures = pathGeometry.Figures.ToArray();
            if (pathFigures.Length == 0)
            {
                return new SKPath();
            }

            List<Point> pointList = new List<Point>();

            foreach (var pathFigure in pathFigures)
            {
                List<Point> tempPoints = new List<Point>();
                foreach (var segment in pathFigure.Segments)
                {
                    if (segment is PolyLineSegment lineSegment)
                    {
                        foreach (var point in lineSegment.Points)
                        {
                            tempPoints.Add(new Point(point.X * UpSize, point.Y * UpSize));
                        }
                    }
                    else
                    {
                        if (geometry is not LineGeometry)
                        {
                            var point = ((LineSegment)segment).Point;

                            tempPoints.Add(new Point(point.X * UpSize, point.Y * UpSize));
                        }
                        else
                        {
                            tempPoints.Add(((LineGeometry)geometry).StartPoint);
                            tempPoints.Add(((LineGeometry)geometry).EndPoint);
                        }
                    }
                }
                tempPoints.Add(tempPoints[0]);
                tempPoints.Add(new Point(double.NaN, double.NaN));
                pointList.AddRange(tempPoints);
            }

            double[] points = new double[(pointList.Count) * 2];
            for (int i = 0; i < pointList.Count * 2; i += 2)
            {
                points[i] = pointList[i / 2].X;
                points[i + 1] = pointList[i / 2].Y;
            }
            closePath.Add(Clipper.MakePath(points));
            #endregion

            // 获取裁剪后填充路径
            ClipperD clipper = new ClipperD();
            clipper.AddSubject(closePath);
            clipper.AddOpenSubject(openSub);
            clipper.AddClip(closePath);
            PathsD solution = new PathsD();
            PathsD res = new PathsD();
            clipper.Execute(ClipType.Intersection, Clipper2Lib.FillRule.NonZero, solution, res);

            // 将裁剪后的填充路径添加到选中的路径中
            SKPath hatchPath = new SKPath();
            foreach (var item in res)
            {
                SKPoint[] points1 =
                [
                    new SKPoint((float)item[0].x/ UpSize, (float)item[0].y/ UpSize),
                    new SKPoint((float)item[1].x/ UpSize, (float)item[1].y/ UpSize)
                ];

                hatchPath.AddPoly(points1, false);
            }

            return hatchPath;
        }

        public bool IsPointInPath(SKPoint point)
        {
            if (OutLinePath == null)
            {
                return false;
            }

            // 当前选中路径的外接矩形大小
            var rect = OutLinePath.TightBounds;

            var topLeftX = rect.Left;
            var topLeftY = rect.Top;
            var width = rect.Width;
            var height = rect.Height;

            // 判断点是否在路径内
            //return OutLinePath.Contains(point.X, point.Y);
            return (point.X >= topLeftX && point.X <= topLeftX + width && point.Y >= topLeftY && point.Y <= topLeftY + height);
        }

        public string ConverPathToSvgData()
        {
            var sb = new StringBuilder();
            using (var iterator = OutLinePath.CreateRawIterator())
            {
                SKPoint[] points = new SKPoint[4];
                SKPathVerb verb;

                while ((verb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (verb)
                    {
                        case SKPathVerb.Move:
                            sb.AppendFormat("M {0} {1} ", points[0].X, points[0].Y);
                            break;
                        case SKPathVerb.Line:
                            sb.AppendFormat("L {0} {1} ", points[1].X, points[1].Y);
                            break;
                        case SKPathVerb.Quad:
                            sb.AppendFormat("Q {0} {1}, {2} {3} ", points[1].X, points[1].Y, points[2].X, points[2].Y);
                            break;
                        case SKPathVerb.Cubic:
                            sb.AppendFormat("C {0} {1}, {2} {3}, {4} {5} ", points[1].X, points[1].Y, points[2].X, points[2].Y, points[3].X, points[3].Y);
                            break;
                        case SKPathVerb.Close:
                            sb.Append("Z ");
                            break;
                    }
                }
            }
            //return sb.ToString().Trim();

            var sb1 = new StringBuilder();
            sb1.AppendLine($"<svg xmlns='http://www.w3.org/2000/svg' width='{OutLinePath.TightBounds.Width}' height='{OutLinePath.TightBounds.Height}' viewBox='0 0 {OutLinePath.TightBounds.Width} {OutLinePath.TightBounds.Height}'>");

            // 将 SKPath 转换为 SVG 的 path data
            string pathData = sb.ToString().Trim();
            sb1.AppendLine($"  <path d='{pathData}' fill='none' stroke='black' stroke-width='1' />");

            sb1.AppendLine("</svg>");
            return sb1.ToString();
        }

        public abstract ArtWorkProperties GetProperties();
    }
}
