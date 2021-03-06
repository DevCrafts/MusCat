﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using MusCat.ViewModels.Entities;

namespace MusCat.Converters
{
    /// <summary>
    /// For a particular performer YearsConverter yields string of active years, e.g. "1970 - 2010"
    /// </summary>
    class YearsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var performer = value as PerformerViewModel;

            if (performer == null || performer.Albums.Count == 0)
            {
                return "";
            }
            
            var yearStart = performer.Albums.Min(t => t.ReleaseYear);
            var yearEnd = performer.Albums.Max(t => t.ReleaseYear);

            return yearEnd != yearStart ? $"{yearStart} - {yearEnd}" : yearStart.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
