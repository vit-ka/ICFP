﻿using System;
using System.Collections.Generic;
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

            _log.InfoFormat("Starting interpretation...");


            VirtualMachine.Instance.Ports.Input[0x3e80] = 1001;
            VirtualMachine.Instance.RunOneStep();

            foreach (KeyValuePair<short, double> portValue in VirtualMachine.Instance.Ports.Output)
            {
                Console.WriteLine("Port: 0x{0:x}. Value: {1}", portValue.Key, portValue.Value);
            }


            VirtualMachine.Instance.Ports.Input[0x02] = 1000;
            VirtualMachine.Instance.Ports.Input[0x03] = -1000;

            for (int i = 0; i < 1000; ++i )
                VirtualMachine.Instance.RunOneStep();

            foreach (KeyValuePair<short, double> portValue in VirtualMachine.Instance.Ports.Output)
            {
                Console.WriteLine("Port: 0x{0:x}. Value: {1}", portValue.Key, portValue.Value);
            }

            _log.InfoFormat("Interpretation finished...");
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.FatalFormat("Fatal exception: {0}", e.ExceptionObject);
        }
    }
}