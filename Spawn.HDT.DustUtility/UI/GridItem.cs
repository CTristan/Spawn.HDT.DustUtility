using System.Diagnostics;
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
        public string Rarity { get; set; }
        #endregion

        #region CardClass
        public string CardClass { get; set; }
        #endregion

        #region Set
        public string Set { get; set; }
        #endregion

        #region Tag
        public CardWrapper Tag { get; set; }
        #endregion
        #endregion
    }
}
