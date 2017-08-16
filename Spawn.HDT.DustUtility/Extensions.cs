using HearthDb;
using HearthDb.Enums;
using Spawn.HDT.DustUtility.Offline;
using Spawn.HearthstonePackHistory.Hearthstone;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Spawn.HDT.DustUtility
{
    public static class Extensions
    {
        #region ContainsCard
        public static bool ContainsCard(this HearthMirror.Objects.Deck deck, string strId)
        {
            return GetCard(deck, strId) != null;
        }
        #endregion

        #region GetCard
        public static HearthMirror.Objects.Card GetCard(this HearthMirror.Objects.Deck deck, string strId)
        {
            return deck.Cards.Find(delegate (HearthMirror.Objects.Card c) { return string.CompareOrdinal(c.Id, strId) == 0; });
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

        #region GetString
        public static string GetString(this CardSet cardSet)
        {
            return CardSets.All[cardSet];
        }

        public static string GetString(this Rarity rarity)
        {
            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

            return textInfo.ToTitleCase(rarity.ToString().ToLowerInvariant());
        }

        public static string GetString(this CardClass cardClass)
        {
            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

            return textInfo.ToTitleCase(cardClass.ToString().ToLowerInvariant());
        }
        #endregion

        #region OrderBy
        // All credits to Aaron Powell https://stackoverflow.com/a/307600
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string strProperty, int nIteration)
        {
            var type = typeof(T);

            var property = type.GetProperty(strProperty);

            var parameter = Expression.Parameter(type, "p");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            var expr = Expression.Lambda(propertyAccess, parameter);

            string strMethod = "OrderBy";

            if (nIteration > 0)
            {
                strMethod = "ThenBy";
            }
            else { }

            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), strMethod, new System.Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(expr));

            return source.Provider.CreateQuery<T>(resultExp);
        }
        #endregion
    }
}