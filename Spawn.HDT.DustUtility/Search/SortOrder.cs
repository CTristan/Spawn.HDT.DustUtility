using System;
using System.Collections.Generic;

namespace Spawn.HDT.DustUtility.Search
{
    public class SortOrder
    {
        #region Member Variables
        private List<Item> m_lstItems; 
        #endregion

        #region Properties
        public List<Item> Items => m_lstItems; 
        #endregion

        #region Ctor
        private SortOrder()
        {
            m_lstItems = new List<Item>();
        }
        #endregion

        #region [STATIC]
        #region Parse
        public static SortOrder Parse(string strValue)
        {
            SortOrder retVal = null;

            if (!string.IsNullOrEmpty(strValue))
            {
                retVal = new SortOrder();

                string[] vItems = strValue.Split(';');

                for (int i = 0; i < vItems.Length; i++)
                {
                    Item item = (Item)Enum.Parse(typeof(Item), vItems[i]);

                    retVal.m_lstItems.Add(item);
                }
            }
            else { }

            return retVal;
        }
        #endregion
        #endregion

        public enum Item
        {
            Count,
            Name,
            Golden,
            Dust,
            Rarity,
            Class,
            CardSet,
            Cost
        }
    }
}
