using System.Diagnostics;
using HearthDb.Enums;
using Spawn.HDT.DustUtility.Search;

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
    }
}
