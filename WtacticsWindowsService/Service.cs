using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WtacticsWindowsService
{
    public partial class Service : ServiceBase
    {
        

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WTacticsGameService.ServiceController.Start();
        }

        protected override void OnStop()
        {
            WTacticsGameService.ServiceController.Stop();
        }
    }
}
