using Clipper2Lib;
using DrawingCanvasLib.DrawTool;
using DrawingCanvasLib;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPF_Draw.DrawTool
{
    public class RectangleClass : BaseToolClass
    {
        public float Radius { get; set; } = 0;

        public RectangleClass(SKPoint point)
        {
            ToolType = ToolType.Rectangle;
            StartPoint = point;
            EndPoint = point;
        }

        public override void Draw(SKCanvas canvas)
        {
            if (IsOutLine)
            {
                OutLinePath = new();
                OutLinePath.AddRoundRect(new SKRect(StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y), Radius, Radius);

                canvas.DrawPath(OutLinePath, PaintStyle);
            }

            if (IsHatch)
            {
                HatchPath = GetHatchPath(OutLinePath);
                canvas.DrawPath(HatchPath, PaintStyle);
            }

            if (IsSelected)
            {
                DrawResizeRect(canvas);
            }
        }

        public override ArtWorkProperties GetProperties()
        {
            var artWork = new ArtWorkProperties()
            {
                Type = ToolType,
                StartPoint = StartPoint,
                EndPoint = EndPoint,
                IsHatch = IsHatch,
                IsOutline = IsOutLine,
            };
            return artWork;
        }
    }
}
