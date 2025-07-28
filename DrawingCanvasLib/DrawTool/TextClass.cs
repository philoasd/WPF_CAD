using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DrawingCanvasLib.DrawTool
{
    public class TextClass : BaseDrawingClass
    {
        public TextClass(SKPoint point)
        {
            ToolType = ToolType.Text;
            StartPoint = point;
            EndPoint = point;
            DrawingName = "Text";
        }

        public override void Draw(SKCanvas canvas)
        {
            if (IsOutLine)
            {
                OutLinePath = new();
                PaintStyle.TextSize = this.TextSize;
                PaintStyle.Typeface = this.TextType;
                OutLinePath = PaintStyle.GetTextPath(DrawText, StartPoint.X, StartPoint.Y);

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
    }
}
