using System.Windows;
using System.Windows.Media;

namespace WTacticsLibrary.Layout
{
    public static class Styles
    {
        public static FontFamily CaptialFontFamily = new FontFamily("NimbusRomNo9L");
        public static Typeface CapitalFont = new Typeface(CaptialFontFamily, FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
        public static string CapitalFontStyle ="-inkscape-font-specification:NimbusRomNo9L;font-family:NimbusRomNo9L;font-weight:normal;font-style:normal;font-stretch:normal;font-variant:normal;font-size:";

     
        public static FontFamily SymbolFontFamily = new FontFamily("WTactics Symbols");
        public static Typeface SymbolFont = new Typeface(SymbolFontFamily, FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
        public static string SymbolFontStyle = "-inkscape-font-specification:WTactics Symbols;font-family:WTactics Symbols;font-weight:normal;font-style:normal;font-stretch:normal;font-variant:normal;font-size:";

      

        public static FontFamily NormalFontFamily = new FontFamily("Liberation Serif");
        public static Typeface NormalFont = new Typeface(NormalFontFamily, FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
        public static string NormalFontStyle = "-inkscape-font-specification:Liberation Serif;font-family:Liberation Serif;font-weight:normal;font-style:normal;font-stretch:normal;font-variant:normal;font-size:";

        public static Typeface BoldFont = new Typeface(NormalFontFamily, FontStyles.Oblique, FontWeights.Bold, FontStretches.Normal);
        public static string BoldFontStyle = "-inkscape-font-specification:Liberation Serif Boldfont-family:Liberation Serif;font-weight:bold;font-style:normal;font-stretch:normal;font-variant:normal;font-size:";

        public static Typeface ItalicFont = new Typeface(NormalFontFamily, FontStyles.Italic, FontWeights.Regular, FontStretches.Normal);
        public static string ItalicFontStyle = "-inkscape-font-specification:Liberation Serif Italic;font-family:Liberation Serif;font-weight:normal;font-style:italic;font-stretch:normal;font-variant:normal;font-size:";

        public static Typeface BoldItalicFont = new Typeface(NormalFontFamily, FontStyles.Italic, FontWeights.Bold, FontStretches.Normal);
        public static string BoldItalicFontStyle = "-inkscape-font-specification:Liberation Serif Bold Italic;font-family:Liberation Serif;font-weight:bold;font-style:italic;font-stretch:normal;font-variant:normal;font-size:";

        public static double CapitalFontSize = 29.7;

        public static double SymbolLargeFontSize = 35;
        public static double SymbolFontSize = 16;
        public static double FontSize = 11.25;

        public static double LineSpacing = 1.25;
        public static double ParagraphSkip = FontSize / 2.0;

    }
}