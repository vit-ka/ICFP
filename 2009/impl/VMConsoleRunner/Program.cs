using System;
using System.IO;
using ICFP2009.VirtualMachineLib;
using log4net;

namespace ICFP2009.VMConsoleRunner
{
    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            if (args.Length < 1)
            {
                _log.FatalFormat("Usage: VMConsoleRunner.exe <binary-file>.obf");
                return;
            }

            _log.InfoFormat("Reading binary file...");
            using (var stream = new FileStream(args[0], FileMode.Open, FileAccess.Read))
                VirtualMachine.Instance.LoadBinary(stream);

            _log.InfoFormat("VM Image loaded.");
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.FatalFormat("Fatal exception: {0}", e.ExceptionObject);
        }
    }
}