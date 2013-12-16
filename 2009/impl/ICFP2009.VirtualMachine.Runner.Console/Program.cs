using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICFP2009.VirtualMachine.Core;
using log4net;

namespace ICFP2009.VirtualMachine.Runner.Console
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Log.ErrorFormat("Usage: {0}.exe <virtual-machine-blob-file>", typeof(Program).Assembly.GetName().Name);
                return;
            }

            var fileName = args[0];

            Log.InfoFormat("Going to load file '{0}'...", fileName);
            var loader = new Loader(fileName);
            loader.LoadFromFile();
        }
    }
}
