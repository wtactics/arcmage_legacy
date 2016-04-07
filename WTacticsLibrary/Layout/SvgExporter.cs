using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Svg;

namespace WTacticsLibrary.Layout
{
    public class SvgExporter
    {
        public static void ExportPng(string inputfile, string outputfile, int dpi = 600)
        {
            var sampleDoc = SvgDocument.Open(inputfile);
            sampleDoc.Ppi = dpi;
            var size = new SizeF();
            sampleDoc.RasterizeDimensions(ref size,1535,0);
            sampleDoc.Draw().Save(outputfile, ImageFormat.Png);
        }
    }
}
