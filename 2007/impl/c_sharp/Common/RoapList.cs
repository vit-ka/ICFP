using System;
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

        /// <summary>
        /// Количество элементов в списке.
        /// </summary>
        public int Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Индексатор списка.
        /// </summary>
        /// <param name="index">Позиция элемента в списке.</param>
        /// <returns>Элемент списка, находящийся на заданной позиции.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Выбрасывается при обращении по индексу, отсутствующему в списке.</exception>
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

        /// <summary>
        /// Добавляет новый элемент в конец списка.
        /// </summary>
        /// <param name="newElem">Элемент для добавления.</param>
        public void Add(ItemType newElem)
        {
        }

        /// <summary>
        /// Добавляе список элементов в конец списка.
        /// </summary>
        /// <param name="list">Список элементов.</param>
        public void AddRange(ICollection<ItemType> list)
        {
        }

        /// <summary>
        /// Удаляет элемент на определенной позиции.
        /// </summary>
        /// <param name="elemForDeletion">Позиция элемента для удаления.</param>
        public void RemoveAt(int elemForDeletion)
        {
        }

        /// <summary>
        /// Удаляет группу элементов с определенной позиции.
        /// Допустимо передавать в качестве как позиции, так и длины отрицательные значения.
        /// Например RemoveRange(-2,5) удалит три первых элемента в списке,
        /// а RemoveRange(10, -2) в списке из 10 элементов удалит последний элемент.
        /// </summary>
        /// <param name="startIndex">Позиция первого удаляемого элемента.</param>
        /// <param name="count">Количество удаляемых элементов.</param>
        public void RemoveRange(int startIndex, int count)
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

            public int Length
            {
                [DebuggerStepThrough]
                get;
                [DebuggerStepThrough]
                private set;
            }
        }

        #endregion
    }
}