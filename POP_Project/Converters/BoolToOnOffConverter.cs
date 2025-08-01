﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace POP_Project.Converters
{
    public class BoolToOnOffConverter : IValueConverter
    {        
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? "ON" : "OFF";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (string)value == "ON";
            }
    }
}
