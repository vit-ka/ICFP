using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common
{
    /// <summary>
    /// Список значений, позволяющий:
    ///  * Быструю вставку в любое место списка нового элемента или диапозона -- O(1).
    ///  * Быстрый доступ до любого элемента списка -- O(1).
    ///  * Быстрое удаление любого элемента списка или диапозона -- О(1).
    /// 
    /// Список реализован посредством массива ссылок на служебные массивы. Каждый из служебных массивов хранит
    /// конечную информацию. Первичный список используется для быстрой обработки массивов (сдвига, удаление, создания).
    /// </summary>
    /// <typeparam name="ItemType">Тип элемента списка.</typeparam>
    public class RoapList<ItemType> : IEnumerable<ItemType>
    {
        /// <summary>  Размер каждого из дочерних массивов. </summary>
        private const int _blockSize = 5;

        public int Length
        {
            get
            {
                return 0;
            }
        }

        public ItemType this[int index]
        {
            get
            {
                return default(ItemType);
            }

            set
            {
            }
        }

        #region IEnumerable<ItemType> Members

        public IEnumerator<ItemType> GetEnumerator()
        {
            return new List<ItemType>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void Add(int i)
        {
        }

        public void AddRange(ICollection<ItemType> list)
        {
        }

        public void RemoveAt(int i)
        {
        }

        public void RemoveRange(int i, int i1)
        {
        }

        #region Nested type: ListItem

        /// <summary>
        /// Элемент списка, непосредственно хранящий значения.
        /// </summary>
        private class ListItem
        {
            public ListItem(int firstIndex)
            {
                Items = new ItemType[_blockSize];
                FirstIndex = firstIndex;
            }

            public ItemType[] Items
            {
                [DebuggerStepThrough]
                get;
                [DebuggerStepThrough]
                set;
            }

            public int FirstIndex
            {
                [DebuggerStepThrough]
                get;
                [DebuggerStepThrough]
                set;
            }
        }

        #endregion
    }
}