using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace HHFO.Models.UI
{
    public class DateFormatConverter : IMultiValueConverter
    {

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = new CultureInfo("ja-JP");
            DateTimeOffset? date = value[0] as DateTimeOffset?;
            
            if (value[1] is DateTimeOffset retweetedDate)
            {
                return retweetedDate.ToString("G", c) + "\r\nRT:" + date?.ToString("G", c);
            }
            return date?.ToString("G", c);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
