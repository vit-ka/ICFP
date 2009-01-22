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
        /// Список ссылок на блоки, хранящие данные. Используется для быстрой вставки/удаления
        /// в середину списка.
        /// </summary>
        private readonly IList<ListBlock> _linkList;

        public RoapList()
        {
            _linkList = new List<ListBlock>();
        }

        /// <summary>
        /// Количество элементов в списке.
        /// </summary>
        public int Count
        {
            get
            {
                ListBlock lastBlock = GetLastBlock();

                return lastBlock.FirstIndex + lastBlock.Length;
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
                int blockIndex = SearchBlockByElemIndex(index, 0, _linkList.Count);

                if (blockIndex == -1)
                {
                    throw new IndexOutOfRangeException(
                        string.Format(
                            "Index must be in range [0;{0}). But index is {1}.",
                            Count,
                            index));
                }

                int indexInBlock = index - _linkList[blockIndex].FirstIndex;

                return _linkList[blockIndex][indexInBlock];
            }

            set
            {
                int blockIndex = SearchBlockByElemIndex(index, 0, _linkList.Count);

                if (blockIndex == -1)
                {
                    throw new IndexOutOfRangeException(
                        string.Format(
                            "Index must be in range [0;{0}). But index is {1}.",
                            Count,
                            index));
                }

                int indexInBlock = index - _linkList[blockIndex].FirstIndex;

                _linkList[blockIndex][indexInBlock] = value;
            }
        }

        #region IEnumerable<ItemType> Members

        public IEnumerator<ItemType> GetEnumerator()
        {
            return new List<ItemType>(_linkList[0].Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Ищет блок, содержащий элемент с требуемым индексом.
        /// </summary>
        /// <param name="index">Индекс элемента, который требуется найти.</param>
        /// <param name="from">Начало диапазона поиска, включительно.</param>
        /// <param name="to">Конец диапазоно поиска, не включительно.</param>
        /// <returns>Индекс блока, содержащего элемент. -1, если блок не найден.</returns>
        private int SearchBlockByElemIndex(int index, int from, int to)
        {
            int indexForChecking = (to - from) / 2 + from;

            // Если такого индекса нет в списке, то возвращаемся.
            if (indexForChecking < 0 || indexForChecking >= _linkList.Count)
                return -1;

            // Проверяем что в блоке, по данному индексу.
            ListBlock block = _linkList[indexForChecking];

            // Если индексы элементов в проверяемом блоке меньше чем требуемый,
            // то продолжаем поиск в правой части.
            if (block.FirstIndex + block.Length < index)
                return SearchBlockByElemIndex(index, indexForChecking + 1, to);

            // Если индексы элементов в проверяемом блоке больше чем требуемый,
            // то продолжаем поиск в левой части.
            if (block.FirstIndex > index)
                return SearchBlockByElemIndex(index, from, indexForChecking);

            // Если индексы элементов в проверяемом блоке содержат требуемый,
            // то мы нашли что хотели.
            if (block.FirstIndex >= index && block.FirstIndex + block.Length <= index)
                return indexForChecking;

            // Если ничего из перечисленного не выполняется, то блока в списке нет.
            return -1;
        }

        /// <summary>
        /// Добавляет новый элемент в конец списка.
        /// </summary>
        /// <param name="newElem">Элемент для добавления.</param>
        public void Add(ItemType newElem)
        {
            Guard.ArgumentNotNull(newElem, "newElem");

            // Берем последний блок, если список еще пустой, то создаем новый блок.
            ListBlock lastBlock = GetLastBlock();

            // Если последний блок полный, то добавляем новый блок.
            if (lastBlock.Length + 1 > _blockSize)
            {
                // Создаем новый блок, который будет последним в списке.
                var newBlock = new ListBlock(Count);
                _linkList.Insert(_linkList.Count, newBlock);

                lastBlock = newBlock;
            }

            // Добавляем в последний блок новый элемент.
            lastBlock[lastBlock.Length] = newElem;
        }

        /// <summary>
        /// Возвращает последний блок в списке. Если список еще пустой, то создает новый блок.
        /// </summary>
        /// <returns>Последний блок в списке.</returns>
        private ListBlock GetLastBlock()
        {
            ListBlock lastBlock = null;

            if (_linkList.Count == 0)
            {
                lastBlock = new ListBlock(0);
                _linkList.Add(lastBlock);
            }
            else
                lastBlock = _linkList[_linkList.Count - 1];
            return lastBlock;
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
        /// Например RemoveRange(-2, 5) удалит три первых элемента в списке,
        /// а RemoveRange(10, -2) в списке из 10 элементов удалит последний элемент.
        /// </summary>
        /// <param name="startIndex">Позиция первого удаляемого элемента.</param>
        /// <param name="count">Количество удаляемых элементов.</param>
        public void RemoveRange(int startIndex, int count)
        {
        }

        #region Nested type: ListBlock

        /// <summary>
        /// Элемент списка, непосредственно хранящий значения.
        /// </summary>
        private class ListBlock
        {
            public ListBlock(int firstIndex)
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

            public ItemType this[int index]
            {
                get
                {
                    if (index < 0 || index >= Length)
                    {
                        throw new ArgumentOutOfRangeException(
                            string.Format("Index must be in range [0;{0}). But index is {1}.", Length, index));
                    }

                    return Items[index];
                }

                set
                {
                    if (Items[index].Equals(default(ItemType)))
                        ++Length;

                    Items[index] = value;
                }
            }
        }

        #endregion
    }
}