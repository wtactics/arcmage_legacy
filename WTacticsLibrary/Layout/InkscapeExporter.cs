using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Layout
{
    public static class InkscapeExporter
    {
        private static string InkscapeExe = "\"" + @"C:\Program Files\Inkscape\inkscape.exe" + "\"";
        private static string InkscapePngArgs = "--export-png \"{1}\" --export-area-page --export-dpi {2} --export-width {3} \"{0}\"";

        private static string InkscapePdfArgs = "--export-pdf \"{1}\" --export-area-page --export-dpi {2} \"{0}\"";

        public static void ExportPng(string inputfile, string outputfile, int dpi = 600, int width = 1535)
        {
            try
            {
                var args = string.Format(InkscapePngArgs, inputfile, outputfile, dpi, width);

                var processStartInfo = new ProcessStartInfo(InkscapeExe, args);
                processStartInfo.RedirectStandardInput = false;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;

                var process = new Process();
                ImpersonateUserProcess.Impersonate(process);
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();
         
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

    

        public static void ExportPdf(string inputfile, string outputfile, int dpi = 600)
        {
            try
            {

                var args = string.Format(InkscapePdfArgs, inputfile, outputfile, dpi);
                var processStartInfo = new ProcessStartInfo(InkscapeExe, args);
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;

                var process = new Process();
                ImpersonateUserProcess.Impersonate(process);
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

      

    }
}
