using System.Collections;
using System.Collections.Generic;

namespace DnaRunner
{
    /// <summary>
    /// Information about template in DNA.
    /// </summary>
    internal class TemplateInfo : IEnumerable<TemplateItemInfo>
    {
        private readonly IList<TemplateItemInfo> _items = new List<TemplateItemInfo>();

        #region IEnumerable<TemplateItemInfo> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TemplateItemInfo> GetEnumerator()
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
            _items.Add(new TemplateItemInfo(symbol));
        }

        public void AddReference(int reference, int level)
        {
            _items.Add(new TemplateItemInfo(reference, level));
        }

        public void AddLengthOfReference(int reference)
        {
            _items.Add(new TemplateItemInfo(reference));
        }
    }

    internal class TemplateItemInfo
    {
        private readonly bool _isAsNat;
        private readonly bool _isBase;
        private readonly bool _isProtect;

        private readonly int _level;
        private readonly int _reference;

        private readonly char _symbol;

        public TemplateItemInfo(char symbol)
        {
            _isBase = true;
            _symbol = symbol;
        }

        public TemplateItemInfo(int reference, int level)
        {
            _isProtect = true;
            _reference = reference;
            _level = level;
        }

        public TemplateItemInfo(int reference)
        {
            _isAsNat = true;
            _reference = reference;
        }

        public bool IsBase
        {
            get
            {
                return _isBase;
            }
        }

        public bool IsProtect
        {
            get
            {
                return _isProtect;
            }
        }

        public bool IsAsNat
        {
            get
            {
                return _isAsNat;
            }
        }

        public int Reference
        {
            get
            {
                return _reference;
            }
        }

        public char Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public int Level
        {
            get
            {
                return _level;
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

            if (_isProtect)
                return Reference + "_" + Level;

            if (_isAsNat)
                return "|" + Reference + "|";

            return string.Empty;
        }
    }
}