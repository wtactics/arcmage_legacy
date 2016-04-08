using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Layout
{
    public static class RsvgExporter
    {
        private static string RsvgExe = "\"" + @"C:\Program Files\Inkscape\rsvg-convert.exe" + "\"";
        private static string RsvgPngArgs = "-f png -d {2} -p {2} -w {3} -a -o \"{1}\" \"{0}\"";

      
        public static void ExportPng(string inputfile, string outputfile, int dpi = 600, int width = 1535)
        {
            try
            {
                var args = string.Format(RsvgPngArgs, inputfile, outputfile, dpi, width);

                var processStartInfo = new ProcessStartInfo(RsvgExe, args);
                processStartInfo.RedirectStandardInput = false;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;

                var process = new Process();
                process.StartInfo = processStartInfo;

                var start = DateTime.UtcNow;
                process.Start();
                process.WaitForExit();
                var e = process.ExitCode;
                var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
                var log = $"{args} in {elapsed} ms.";
                Debug.WriteLine(log);
                File.WriteAllText(Path.Combine(Repository.CardsPath, "log.txt"), log);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

     

    }
}
