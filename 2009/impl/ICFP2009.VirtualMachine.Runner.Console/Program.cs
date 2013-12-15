using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ICFP2009.VirtualMachine.Runner.Console
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Log.ErrorFormat("Usage: {0}.exe <virtual-machine-blob-file>", typeof(Program).Assembly.GetName().Name);
        }
    }
}
