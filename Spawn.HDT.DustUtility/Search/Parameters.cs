using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility.Search
{
    [Serializable]
    public class Parameters
    {
        #region Static Ctor
        static Parameters()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((s, e) => typeof(Parameters).Assembly);
        }
        #endregion

        #region Properties
        #region DustAmount
        public int DustAmount { get; set; }
        #endregion

        #region IncludeGoldenCards
        public bool IncludeGoldenCards { get; set; }
        #endregion

        #region UnusedCardsOnly
        public bool UnusedCardsOnly { get; set; }
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
            : this(false)
        {

        }

        public Parameters(bool setDefaultValues)
        {
            if (!setDefaultValues)
            {
                Rarities = new List<Rarity>();
                Classes = new List<CardClass>();
                Sets = new List<CardSet>();
            }
            else
            {
                IncludeGoldenCards = false;
                UnusedCardsOnly = true;

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
                    CardSet.ICECROWN,
                    CardSet.NAXX,
                    CardSet.BRM,
                    CardSet.LOE,
                    CardSet.KARA,
                    CardSet.PROMO,
                    CardSet.HOF
                };
            }
        }
        #endregion

        #region DeepClone
        public Parameters DeepClone()
        {
            Parameters retVal = new Parameters();
            
            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, this);

                ms.Position = 0;

                retVal = bf.Deserialize(ms) as Parameters;
            }

            return retVal;
        } 
        #endregion
    }
}
