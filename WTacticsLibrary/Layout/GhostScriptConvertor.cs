using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Layout
{
    public static class GhostScriptConvertor
    {
        private static string GhostScriptExe = "\"" + @"C:\Program Files\gs\gs9.19\bin\gswin64c.exe" + "\"";
    
        private static string GhostScriptArgs = "-o \"{1}\" -sDEVICE=pdfwrite -sProcessColorModel=DeviceCMYK -sColorConversionStrategy=CMYK -sColorConversionStrategyForImages=CMYK -dEncodeColorImages=false \"{0}\"";
        

        public static void ConvertToCMYKPdf(string inputfile, string outputfile)
        {
            try
            {

                var args = string.Format(GhostScriptArgs, inputfile, outputfile);
                var processStartInfo = new ProcessStartInfo(GhostScriptExe, args);
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
