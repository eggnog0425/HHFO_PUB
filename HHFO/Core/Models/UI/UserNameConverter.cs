using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace HHFO.Models.UI
{
    public class UserNameConverter : IMultiValueConverter
    {

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string user = value[0] as string;
            string retweetedUser = value[1] as string;
            if(user == "" && retweetedUser == "")
            {
                return "";
            }
            else if (retweetedUser == "")
            {
                return user;
            }
            else
            {
                return retweetedUser + " RT: " + user;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
