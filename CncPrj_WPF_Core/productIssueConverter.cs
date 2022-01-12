using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CncPrj_WPF_Core
{
    internal class productIssueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            if(value.ToString() != "Normal")
            {
                return Brushes.OrangeRed; //row foreground -> OrangeRed
            }
            else
            {
                return Brushes.White;   //row foreground -> white
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
