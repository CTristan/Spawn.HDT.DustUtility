using System.Collections.Generic;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility
{
    public class Parameters
    {
        #region Properties
        #region DustAmount
        public int DustAmount { get; set; }
        #endregion

        #region IncludeGoldenCards
        public bool IncludeGoldenCards { get; set; }
        #endregion

        #region Rarites
        public List<Rarity> Rarities { get; set; }
        #endregion

        #region Classes
        public List<CardClass> Classes { get; set; }
        #endregion

        #region Sets
        public List<CardSet> Sets { get; set; } 
        #endregion
        #endregion

        #region Ctor
        public Parameters()
        {
            IncludeGoldenCards = false;

            Rarities = new List<Rarity>
            {
                Rarity.COMMON,
                Rarity.RARE,
                Rarity.EPIC,
                Rarity.LEGENDARY
            };

            Classes = new List<CardClass>
            {
                CardClass.DRUID,
                CardClass.HUNTER,
                CardClass.MAGE,
                CardClass.PALADIN,
                CardClass.PRIEST,
                CardClass.ROGUE,
                CardClass.SHAMAN,
                CardClass.WARLOCK,
                CardClass.WARRIOR,
                CardClass.NEUTRAL
            };

            Sets = new List<CardSet>
            {
                CardSet.EXPERT1,
                CardSet.GVG,
                CardSet.TGT,
                CardSet.OG,
                CardSet.GANGS,
                CardSet.UNGORO,
                CardSet.NAXX,
                CardSet.BRM,
                CardSet.LOE,
                CardSet.KARA,
                CardSet.PROMO,
                CardSet.HOF
            };
        } 
        #endregion
    }
}
