using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsLibrary.Layout;
using ImageMagick;

namespace SvgToCMYKPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputfile = @"C:\Projects\WTactics\arcmage\boxes\gaianloveforlife_uneasyalliance\arcmage_box.svg";
            var pngfile = @"C:\Projects\WTactics\arcmage\boxes\gaianloveforlife_uneasyalliance\arcmage_box.png";
            var outputfile = @"C:\Projects\WTactics\arcmage\boxes\gaianloveforlife_uneasyalliance\arcmage_box.pdf";
            var dpi = 600;
            var width = 7016;
            InkscapeExporter.ExportPng(inputfile, pngfile,dpi,width);
            using (MagickImage image = new MagickImage(pngfile))
            {

                image.AddProfile(ColorProfile.SRGB);
                image.AddProfile(ColorProfile.USWebCoatedSWOP);

                // Save the result
                image.Write(outputfile);
            }

        }
    }
}
