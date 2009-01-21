using System;
using System.IO;
using Common;
using Dna;
using log4net;

namespace DnaToRnaRunner
{
    public class Program
    {
        /// <summary> Менеджер логов </summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        /// <summary> Исходный файл ДНК. </summary>
        private static string _dnaFile;

        /// <summary> Результирующий файл РНК. </summary>
        private static string _rnaFile;

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: DnaToRnaRunner.exe <dna-file> <output-rna-file>");
                return;
            }

            _dnaFile = args[0];
            _rnaFile = args[1];

            Guard.FileExists(_dnaFile);

            string readedDna = File.ReadAllText(_dnaFile);

            var processor = new Processor();
            processor.ImportDna(readedDna);
            processor.ProcessDna();
            string exportedRna = processor.ExportDna();

            File.WriteAllText(_rnaFile, exportedRna);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Fatal("Unhandled exception.", (Exception) e.ExceptionObject);
        }
    }
}