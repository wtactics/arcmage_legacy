using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace WTacticsLibrary.Layout
{
    public static class MeausureFont
    {

        public static Size MeasureString(string text, double fontSize, Typeface typeFace)
        {
            var ft = new FormattedText
            (
               text,
               CultureInfo.CurrentCulture,
               FlowDirection.LeftToRight,
               typeFace,
               fontSize,
               Brushes.Black
            );
            return new Size(ft.Width, ft.Height);
        }
    }
}
