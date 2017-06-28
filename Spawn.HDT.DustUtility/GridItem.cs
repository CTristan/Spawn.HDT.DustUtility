using System.Diagnostics;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility
{
    [DebuggerDisplay("{Name} ({Count})")]
    public class GridItem
    {
        #region Properties
        #region Count
        public string Count { get; set; }
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

        #region CardClass
        public CardClass CardClass { get; set; }
        #endregion

        #region CardSet
        public CardSet CardSet { get; set; }
        #endregion

        #region Set
        public string Set => GetCardSetString(); 
        #endregion
        #endregion

        #region GetCardSetString
        private string GetCardSetString()
        {
            string strRet = string.Empty;

            switch (CardSet)
            {
                case CardSet.EXPERT1:
                    strRet = "CLASSIC";
                    break;
                case CardSet.GVG:
                    strRet = "GVG";
                    break;
                case CardSet.TGT:
                    strRet = "TGT";
                    break;
                case CardSet.OG:
                    strRet = "OLD GODS";
                    break;
                case CardSet.GANGS:
                    strRet = "MSG";
                    break;
                case CardSet.UNGORO:
                    strRet = "UNGORO";
                    break;
                case CardSet.NAXX:
                    strRet = "NAXX";
                    break;
                case CardSet.BRM:
                    strRet = "BRM";
                    break;
                case CardSet.LOE:
                    strRet = "LOE";
                    break;
                case CardSet.KARA:
                    strRet = "KARAZHAN";
                    break;
                case CardSet.HOF:
                case CardSet.PROMO:
                    strRet = "HALL OF FAME";
                    break;
            }

            return strRet;
        } 
        #endregion
    }
}
