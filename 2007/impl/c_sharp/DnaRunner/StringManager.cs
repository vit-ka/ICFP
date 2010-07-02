using System.Linq;

namespace DnaRunner
{
    /// <summary>
    /// Class for helps changes realization of string.
    /// </summary>
    internal class StringManager
    {
        private string _internalString;

        public StringManager(string str)
        {
            _internalString = str;
        }

        public int Length
        {
            get { return _internalString.Length; }
        }

        public void AppendToBack(StringManager anotherStr, int fromIndex)
        {
            _internalString += anotherStr._internalString.Substring(fromIndex);
        }

        public bool HasPatternAtPosition(string[] patterns, int fromIndex)
        {
            return patterns.Any(pattern => _internalString.Substring(fromIndex, pattern.Length) == pattern);
        }

        public int IndexOf(string searchPattern, int fromIndex)
        {
            return _internalString.IndexOf(searchPattern, fromIndex);
        }

        public StringManager Substring(int fromIndex, int length)
        {
            return new StringManager(_internalString.Substring(fromIndex, length));
        }

        public override string ToString()
        {
            return _internalString;
        }
    }
}