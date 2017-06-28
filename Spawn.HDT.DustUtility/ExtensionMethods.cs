using System.Collections.Generic;
using HearthDb;
using HearthDb.Enums;
using Spawn.HDT.DustUtility.Offline;

namespace Spawn.HDT.DustUtility
{
    public static class ExtensionMethods
    {
        #region Contains
        public static bool Contains(this HearthMirror.Objects.Deck deck, string strId)
        {
            return GetCard(deck, strId) != null;
        }
        #endregion

        #region GetCard
        public static HearthMirror.Objects.Card GetCard(this HearthMirror.Objects.Deck deck, string strId)
        {
            return deck.Cards.Find(delegate (HearthMirror.Objects.Card c) { return strId.Equals(c.Id); });
        }
        #endregion

        #region GetDustValue
        public static int GetDustValue(this HearthMirror.Objects.Card card)
        {
            int nRet = 0;

            if (card != null)
            {
                Card c = Cards.All[card.Id];

                if (c.Set != CardSet.CORE)
                {
                    switch (c.Rarity)
                    {
                        case Rarity.COMMON:
                            nRet = (card.Premium ? 50 : 5);
                            break;
                        case Rarity.RARE:
                            nRet = (card.Premium ? 100 : 20);
                            break;
                        case Rarity.EPIC:
                            nRet = (card.Premium ? 400 : 100);
                            break;
                        case Rarity.LEGENDARY:
                            nRet = (card.Premium ? 1600 : 400);
                            break;
                    }
                }
                else { }
            }
            else { }

            return nRet;
        }
        #endregion

        #region ToCachedCards
        public static List<CachedCard> ToCachedCards(this List<HearthMirror.Objects.Card> lstCards)
        {
            List<CachedCard> lstRet = new List<CachedCard>();

            for (int i = 0; i < lstCards.Count; i++)
            {
                HearthMirror.Objects.Card card = lstCards[i];

                CachedCard cachedCard = new CachedCard()
                {
                    Id = card.Id,
                    Count = card.Count,
                    IsGolden = card.Premium
                };

                lstRet.Add(cachedCard);
            }

            return lstRet;
        }
        #endregion

        #region ToCards
        public static List<HearthMirror.Objects.Card> ToCards(this List<CachedCard> lstCachedCards)
        {
            List<HearthMirror.Objects.Card> lstRet = new List<HearthMirror.Objects.Card>();

            for (int i = 0; i < lstCachedCards.Count; i++)
            {
                CachedCard cachedCard = lstCachedCards[i];

                lstRet.Add(new HearthMirror.Objects.Card(cachedCard.Id, cachedCard.Count, cachedCard.IsGolden));
            }

            return lstRet;
        } 
        #endregion
    }
}
