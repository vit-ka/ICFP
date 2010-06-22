using System;

namespace DnaRunner
{
    /// <summary>
    /// Information about template in DNA.
    /// </summary>
    internal class TemplateInfo
    {
        private string _innerStr = string.Empty;

        public void AppendBack(string symbol)
        {
            _innerStr += symbol;
        }

        public void AddReference(int reference, int level)
        {
            _innerStr += "[" + reference + "_" + level + "]";
        }

        public void AddLengthOfReference(int reference)
        {
            _innerStr += "|" + reference + "|";
        }
    }
}