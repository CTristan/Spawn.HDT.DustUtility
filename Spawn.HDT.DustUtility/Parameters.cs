using System.Collections.Generic;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility
{
    public class Parameters
    {
        public int DustAmount { get; set; }

        public bool IncludeGoldenCards { get; set; }

        public List<Rarity> Rarities { get; set; }

        public List<CardClass> Classes { get; set; }

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
        }
    }
}
