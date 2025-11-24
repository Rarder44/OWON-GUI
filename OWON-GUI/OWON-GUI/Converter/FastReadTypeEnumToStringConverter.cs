using Avalonia.Data.Converters;
using OWON_GUI.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Converter
{



    internal class FastReadTypeEnumToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!(value is FastReadType))
                return null;

            FastReadType type = (FastReadType)value;
            if (type == FastReadType.Current)
                return "A";
            else if (type == FastReadType.Voltage)
                return "V";
            else if (type == FastReadType.Power)
                return "W";
            else if (type == FastReadType.Current_Voltage)
                return "A & V";
            else if (type == FastReadType.Current_Voltage_Power)
                return "A, V & W";

            return null;

        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
