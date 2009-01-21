namespace Dna
{
    public class Processor
    {
        private string _dna;
        private string _rna;

        public void ImportDna(string dna)
        {
            _dna = dna;
        }

        public void ProcessDna()
        {
                
        }

        public string ExportDna()
        {
            return _rna;
        }
    }
}