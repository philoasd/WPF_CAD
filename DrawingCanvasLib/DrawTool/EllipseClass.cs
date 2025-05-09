using DrawingCanvasLib.DrawTool;
using DrawingCanvasLib;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Draw.DrawTool
{
    public class EllipseClass : BaseDrawingClass
    {
        public EllipseClass(SKPoint point)
        {
            ToolType = ToolType.Ellipse;
            StartPoint = point;
            EndPoint = point;
        }

        public override void Draw(SKCanvas canvas)
        {
            if (IsOutLine)
            {
                OutLinePath = new();
                OutLinePath.AddOval(new SKRect(StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y));

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

        //public override ArtWorkProperties GetProperties()
        //{
        //    var artWork = new ArtWorkProperties()
        //    {
        //        Type = ToolType,
        //        StartPoint = StartPoint,
        //        EndPoint = EndPoint,
        //        IsHatch = IsHatch,
        //        IsOutline = IsOutLine,
        //    };
        //    return artWork;
        //}
    }
}
