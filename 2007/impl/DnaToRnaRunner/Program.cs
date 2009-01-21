using System;

namespace DnaToRnaRunner
{
    public class Program
    {
        private static string _dnaFile;
        private static string _rnaFile;

        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: DnaToRnaRunner.exe <dna-file> <output-rna-file>");
                return;
            }

            _dnaFile = args[0];
            _rnaFile = args[1];
        }
    }
}
