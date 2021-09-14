using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WPF_Malovani.ViewModels;
using System.Windows.Input;

namespace WPF_Malovani.Converters
{
    public class ModeToCursor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((AvailableModes)value)
            {
                case AvailableModes.Paint:
                    return Cursors.Pen;
                case AvailableModes.Grab:
                    return Cursors.Hand;
                case AvailableModes.Delete:
                    return Cursors.No;
                default:
                    return Cursors.Pen;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
