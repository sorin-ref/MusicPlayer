using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicPlayer
{
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileName = (string)value;
            return fileName.Replace(Properties.Settings.Default.MusicFolder.Replace("%User%", Environment.UserName) + Path.DirectorySeparatorChar, string.Empty).Replace(Path.DirectorySeparatorChar.ToString(), " / ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
