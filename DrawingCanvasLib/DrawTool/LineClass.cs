using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingCanvasLib.DrawTool
{
    public class LineClass : BaseToolClass
    {
        public LineClass(SKPoint point)
        {
            ToolType = ToolType.Line;
            StartPoint = point;
            EndPoint = point;
        }

        public override void Draw(SKCanvas canvas)
        {
            OutLinePath = new();
            OutLinePath.MoveTo(StartPoint);
            OutLinePath.LineTo(EndPoint);
            //path.Close();
            canvas.DrawPath(OutLinePath, PaintStyle);

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
