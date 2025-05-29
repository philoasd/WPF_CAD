using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPF_CAD.Converters
{
    /// <summary>
    /// 将绘图相关的转换器类，用于将绘图转换为布尔值：如果绘图存在，则返回true，否则返回false。
    /// </summary>
    public class DrawingConvertToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // do nothing, as this converter is one-way
            return Binding.DoNothing;
        }
    }
}
