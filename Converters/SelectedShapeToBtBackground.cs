using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WPF_Malovani.ViewModels;

namespace WPF_Malovani.Converters
{
    public class SelectedShapeToBtBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((AvailableShapes)value == (AvailableShapes)parameter)
            {
                return Application.Current.FindResource("SelectedForeground");
            }

            return Application.Current.FindResource("FontColor");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
