using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace HHFO.Models.UI
{
    [ValueConversion(typeof(IReactiveProperty<long>), typeof(bool))]
    public class ReactiveLongConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IReactiveProperty<long>)value)?.Value ?? 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
