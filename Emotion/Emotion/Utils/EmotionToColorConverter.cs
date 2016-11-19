using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Emotion.Utils
{
    public class EmotionToColorConverter: IValueConverter
    {
        public Color Happy { get; set; }
        public Color Sad { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = (float) value;
            return num < 0.5f ? Sad : Happy;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
