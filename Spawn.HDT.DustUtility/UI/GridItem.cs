using HearthDb.Enums;
using Spawn.HDT.DustUtility.Search;
using System.Diagnostics;

namespace Spawn.HDT.DustUtility.UI
{
    [DebuggerDisplay("{Name} ({Count})")]
    public class GridItem
    {
        #region Properties
        #region Count
        public int Count { get; set; }
        #endregion

        #region Name
        public string Name { get; set; }
        #endregion

        #region Golden
        public bool Golden { get; set; }
        #endregion

        #region Dust
        public int Dust { get; set; }
        #endregion

        #region Rarity
        public Rarity Rarity { get; set; }
        #endregion

        #region RarityString
        public string RarityString { get; set; }
        #endregion

        #region CardClass
        public string CardClass { get; set; }
        #endregion

        #region CardSet
        public string CardSet { get; set; }
        #endregion

        #region ManaCost
        public int ManaCost { get; set; }
        #endregion

        #region Tag
        public CardWrapper Tag { get; set; }
        #endregion
        #endregion

        #region CreateCopy
        public GridItem CreateCopy()
        {
            return new GridItem()
            {
                Count = Count,
                Name = Name,
                Golden = Golden,
                Dust = Dust,
                Rarity = Rarity,
                RarityString = RarityString,
                CardClass = CardClass,
                CardSet = CardSet,
                ManaCost = ManaCost,
                Tag = Tag
            };
        }
        #endregion

        #region [STATIC] FromCardWrapper
        public static GridItem FromCardWrapper(CardWrapper wrapper)
        {
            return new GridItem()
            {
                Count = wrapper.Count,
                Dust = wrapper.GetDustValue(),
                Golden = wrapper.Card.Premium,
                Name = wrapper.DbCard.Name,
                Rarity = wrapper.DbCard.Rarity,
                RarityString = wrapper.DbCard.Rarity.GetString(),
                CardClass = wrapper.DbCard.Class.GetString(),
                CardSet = wrapper.DbCard.Set.GetString(),
                ManaCost = wrapper.DbCard.Cost,
                Tag = wrapper
            }; ;
        }
        #endregion
    }
}