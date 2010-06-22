using System.Collections;
using System.Collections.Generic;

namespace DnaRunner
{
    /// <summary>
    /// Information of pattern from DNA.
    /// </summary>
    internal class PatternInfo : IEnumerable<PatternItemInfo>
    {
        private readonly IList<PatternItemInfo> _items = new List<PatternItemInfo>();

        #region IEnumerable<PatternItemInfo> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<PatternItemInfo> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        public void AppendBack(char symbol)
        {
            _items.Add(new PatternItemInfo(symbol));
        }

        public void AppendSkip(int number)
        {
            _items.Add(new PatternItemInfo(number));
        }

        public void AppendSearch(string consts)
        {
            _items.Add(new PatternItemInfo(consts));
        }

        public void IncreaseLevel()
        {
            _items.Add(new PatternItemInfo(true));
        }

        public void DecreaseLevel()
        {
            _items.Add(new PatternItemInfo(false));
        }
    }

    internal class PatternItemInfo
    {
        private readonly bool _isBase;
        private readonly bool _isLevelDown;
        private readonly bool _isLevelUp;
        private readonly bool _isSearch;
        private readonly bool _isSkip;

        private readonly string _searchPattern;
        private readonly int _skipCount;
        private readonly char _symbol;

        public PatternItemInfo(char symbol)
        {
            _isBase = true;
            _symbol = symbol;
        }

        public PatternItemInfo(int skipCount)
        {
            _isSkip = true;
            _skipCount = skipCount;
        }

        public PatternItemInfo(string searchPattern)
        {
            _isSearch = true;
            _searchPattern = searchPattern;
        }

        public PatternItemInfo(bool isLevelUp)
        {
            if (isLevelUp)
                _isLevelUp = true;
            else
                _isLevelDown = true;
        }

        public bool IsBase
        {
            get
            {
                return _isBase;
            }
        }

        public bool IsLevelUp
        {
            get
            {
                return _isLevelUp;
            }
        }

        public bool IsLevelDown
        {
            get
            {
                return _isLevelDown;
            }
        }

        public bool IsSkip
        {
            get
            {
                return _isSkip;
            }
        }

        public bool IsSearch
        {
            get
            {
                return _isSearch;
            }
        }

        public string SearchPattern
        {
            get
            {
                return _searchPattern;
            }
        }

        public int SkipCount
        {
            get
            {
                return _skipCount;
            }
        }

        public char Symbol
        {
            get
            {
                return _symbol;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (_isBase)
                return Symbol.ToString();

            if (_isSkip)
                return "!" + SkipCount;

            if (_isSearch)
                return "?" + SearchPattern;

            if (_isLevelUp)
                return "(";

            if (_isLevelDown)
                return ")";

            return string.Empty;
        }
    }
}