using System;
using System.Collections;
using System.Collections.Generic;

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
        public void Add(int i)
        {
        }

        public int Length
        {
            get
            {
                return 0;
            }
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

        public IEnumerator<ItemType> GetEnumerator()
        {
            return new List<ItemType>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}