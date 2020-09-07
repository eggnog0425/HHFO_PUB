using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace HHFO.Models.UI
{
    [ValueConversion(typeof(string), typeof(string))]
    public class ThumbnailConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + ":thumb";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + ":thumb";
        }
    #endregion
    }
}
