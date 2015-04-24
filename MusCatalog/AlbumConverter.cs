﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusCatalog
{
    class AlbumConverter:  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Performers perf = value as Performers;

            if (perf != null && perf.Albums != null )
            {
                switch (perf.Albums.Count)
                {
                    case 0: return "No albums";
                    case 1: return "1 album";
                    default: return perf.Albums.Count + " albums";
                }
            }
            
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
