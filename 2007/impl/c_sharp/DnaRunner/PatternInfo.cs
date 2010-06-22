using System;

namespace DnaRunner
{
    /// <summary>
    /// Information of pattern from DNA.
    /// </summary>
    internal class PatternInfo
    {
        private string _innerStr = string.Empty;

        public void AppendBack(char symbol)
        {
            _innerStr += symbol;
        }

        public void AppendSkip(int number)
        {
            _innerStr += "!" + number;
        }

        public void AppendReplace(string consts)
        {
            _innerStr += "?_" + consts + "_";
        }

        public void IncreaseLevel()
        {
            _innerStr += "(";
        }

        public void DecreaseLevel()
        {
            _innerStr += ")";
        }
    }
}