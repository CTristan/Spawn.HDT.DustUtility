using System.Diagnostics;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility
{
    [DebuggerDisplay("{Name} ({Count})")]
    public class GridItem
    {
        public string Count { get; set; }

        public string Name { get; set; }

        public bool Golden { get; set; }

        public int Dust { get; set; }

        public Rarity Rarity { get; set; }

        public CardClass CardClass { get; set; }

        public CardSet CardSet { get; set; }

        public string Set => GetCardSetString();

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
    }
}
