using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsGameConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            WTacticsGameService.ServiceController.Start();
            Console.WriteLine("Arcmage Game Server");
            Console.WriteLine("Press any key to stop");
            Console.ReadKey();
            WTacticsGameService.ServiceController.Stop();
        }
    }
}
