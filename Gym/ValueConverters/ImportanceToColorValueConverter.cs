using System;
using System.Globalization;
using Microsoft.Maui.Graphics;
using Gym.Model;

namespace Gym.ValueConverters
{
    public class ImportanceToColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var workHour = new WorkHour();
            if (value is int intValue && intValue == workHour.Capacity)
            {
                return Colors.Green;
            }
            else if (value is int intValue1 && intValue1 == workHour.Capacity/2)
            {
                return Colors.Yellow;
            }

            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}