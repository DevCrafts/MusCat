﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace MusCat.Converters
{
    /// <summary>
    /// Converter:   Number of albums (4)  =>  Brief signature indicating the number of performer's albums ("4 albums")
    /// </summary>
    class AlbumCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var albumCount = (int)value;

            switch (albumCount)
            {
                case 0:
                    return "No albums";
                case 1:
                    return "1 album";
                default:
                    return albumCount + " albums";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
