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
        public float HatchSpacing { get; set; } = 1.0f;
        public bool IsOutline { get; set; } = true;
        public string DrawText { get; set; } = "TEXT";
        public float TextSize { get; set; } = 20f;
        public SKFont Font { get; set; } = new SKFont(SKTypeface.FromFamilyName("Arial"), 20f);
        #endregion

        #region Laser属性
        public LaserProperties OutLineProperties { get; set; } = new LaserProperties();
        public LaserProperties HatchProperties { get; set; } = new LaserProperties();
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
        Text,
        Barcode,
    }

    /// <summary>
    /// 激光属性类
    /// </summary>
    public class LaserProperties
    {
        public int MarkSize { get; set; } = 100;
        public double FoucsHeight { get; set; } = 0.0;
        public int MarkingPasses { get; set; } = 1;
        public int MarkingSpeed { get; set; } = 1;
        public double LaserPower { get; set; } = 1;
        public int LaserFrequnency { get; set; } = 1;
        public double PulseWidth { get; set; } = 1;
        public double LaserOnDelay { get; set; } = 0;
        public double LaserOffDelay { get; set; } = 0.002;
        public double PolygonDelay { get; set; } = 0;
        public double MarkDelay { get; set; } = 0;
        public double JumpDelay { get; set; } = 0;
        public bool IsFixParameters { get; set; } = false;
    }
}
