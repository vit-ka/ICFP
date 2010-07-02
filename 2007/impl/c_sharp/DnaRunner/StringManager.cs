using System;
using System.Linq;
using RopeStrings;

namespace DnaRunner
{
    /// <summary>
    /// Class for helps changes realization of string.
    /// </summary>
    internal class StringManager
    {
        private string _internalString;
        private RopeString _ropeString;

        public StringManager(string str)
        {
            _internalString = str;
            _ropeString = new RopeString(str);
        }

        public int Length
        {
            get
            {
                if (_ropeString.Length != _internalString.Length)
                    throw new InvalidOperationException("Length of strings are different!");

                return _internalString.Length;
            }
        }

        public void AppendToBack(StringManager anotherStr, int fromIndex)
        {
            _internalString += anotherStr._internalString.Substring(fromIndex);
            _ropeString.AppendToBack(anotherStr._ropeString, fromIndex);

            if (_ropeString.ToString() != _internalString)
                throw new InvalidOperationException("Strings are different!");
        }

        public bool HasPatternAtPosition(string[] patterns, int fromIndex)
        {
            var resultStr = patterns.Any(pattern => _internalString.Substring(fromIndex, pattern.Length) == pattern);
            var resultRope = patterns.Any(pattern => _ropeString.HasPatternAtPosition(pattern, fromIndex));

            if (resultStr != resultRope)
                throw new InvalidOperationException("Has pattern are different!");

            return resultRope;
        }

        public int IndexOf(string searchPattern, int fromIndex)
        {
            var indexStr = _internalString.IndexOf(searchPattern, fromIndex);
            var indexRope = _ropeString.IndexOf(searchPattern, fromIndex);

                       
            if (indexRope != indexStr)
                throw new InvalidOperationException("IndexOf are different!");

            return indexRope;
        }

        public StringManager Substring(int fromIndex, int length)
        {
            var subRopeRope = _ropeString.Substring(fromIndex, length);
            var subRope = new StringManager(string.Empty)
                {
                    _internalString = subRopeRope.ToString(),
                    _ropeString = subRopeRope
                    
                };

            var subStr = new StringManager(_internalString.Substring(fromIndex, length));

            if (subStr.ToString() != subRope.ToString())
                throw new InvalidOperationException("Substring are different!");

            return subRope;
        }

        public override string ToString()
        {
            if (_internalString != _ropeString.ToString())
                throw new InvalidOperationException("ToString() are different!");

            return _ropeString.ToString();
        }
    }
}