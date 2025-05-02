using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingCanvasLib
{
    /// <summary>
    /// 绘图属性类
    /// </summary>
    public class ArtWorkProperties
    {
        #region 基础属性
        public ToolType Type { get; set; } = ToolType.Select;
        public SKPoint StartPoint { get; set; } = new SKPoint(0, 0);
        public SKPoint EndPoint { get; set; } = new SKPoint(0, 0);
        public bool IsHatch { get; set; } = false;
        public bool IsOutline { get; set; } = true;
        public string DrawText { get; set; } = "TEXT";
        public float TextSize { get; set; } = 20f;
        public SKFont Font { get; set; } = new SKFont(SKTypeface.FromFamilyName("Arial"), 20f);
        #endregion
    }

    /// <summary>
    /// 绘图工具类型
    /// </summary>
    public enum ToolType
    {
        Clear = -1,
        Select,
        Line,
        Rectangle,
        Ellipse,
        Polygon,
        Text,
        Barcode,
    }
}
