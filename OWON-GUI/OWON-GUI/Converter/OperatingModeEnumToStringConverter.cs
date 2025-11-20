using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Converter
{

    public enum OperatingModeEnum
    {
        StandBy = 0,
        CostantVoltage = 1,
        CostantCurrent = 2,
        Failure = 3,
    }


    internal class OperatingModeEnumToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!(value is OperatingModeEnum))
                return null;

            OperatingModeEnum vcast = (OperatingModeEnum)value;
            if (vcast == OperatingModeEnum.StandBy)
                return "Stand By";
            else if (vcast == OperatingModeEnum.CostantVoltage)
                return "Costant Voltage";
            else if (vcast == OperatingModeEnum.CostantCurrent)
                return "Costant Current";
            else if (vcast == OperatingModeEnum.Failure)
                return "Failure";

            return null;

        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
