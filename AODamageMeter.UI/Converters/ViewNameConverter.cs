﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace AODamageMeter.UI.Converters
{
    public class ViewNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedViewingMode = (ViewingMode)values[0];
            var selectedCharacter = (Character)values[1];

            return selectedViewingMode == ViewingMode.DamageDone ? "Damage Done"
                : selectedViewingMode == ViewingMode.DamageDoneInfo ? $"{selectedCharacter?.UncoloredName}'s Damage Done"
                : throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
