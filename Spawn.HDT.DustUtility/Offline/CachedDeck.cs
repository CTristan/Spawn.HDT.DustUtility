using System.Collections.Generic;

namespace Spawn.HDT.DustUtility.Offline
{
    public class CachedDeck
    {
        #region Properties
        #region Id
        public long Id { get; set; }
        #endregion

        #region Name
        public string Name { get; set; }
        #endregion

        #region Hero
        public string Hero { get; set; }
        #endregion

        #region IsWild
        public bool IsWild { get; set; }
        #endregion

        #region Type
        public int Type { get; set; }
        #endregion

        #region SeasonId
        public int SeasonId { get; set; }
        #endregion

        #region CardBackId
        public int CardBackId { get; set; }
        #endregion

        #region HeroPremium
        public int HeroPremium { get; set; }
        #endregion

        #region Cards
        public List<CachedCard> Cards { get; set; }  
        #endregion
        #endregion
    }
}
