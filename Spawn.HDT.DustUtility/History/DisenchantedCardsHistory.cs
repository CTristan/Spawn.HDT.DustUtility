using HearthMirror.Objects;
using Spawn.HDT.DustUtility.Offline;
using System.Collections.Generic;
using System.Linq;

namespace Spawn.HDT.DustUtility.History
{
    public static class DisenchantedCardsHistory
    {
        #region Constants
        private const string DisenchantedString = "disenchanted";
        #endregion

        #region Static Variables
        private static CardComparer s_cardComparer = new CardComparer();
        private static bool s_blnCheckInProgress;
        #endregion

        #region CheckCollection
        public static void CheckCollection(Account account, List<Card> lstCurrentCollection)
        {
            if (!s_blnCheckInProgress && account != null && (lstCurrentCollection != null && lstCurrentCollection.Count > 0))
            {
                s_blnCheckInProgress = true;

                List<Card> lstOldCollection = Cache.LoadCollection(account);

                List<Card> lstDisenchantedCardsHistory = Cache.LoadCollection(account, DisenchantedString);

                if (lstOldCollection != null && lstOldCollection.Count > 0)
                {
                    List<Card> a = lstCurrentCollection.Except(lstOldCollection, s_cardComparer).ToList();
                    List<Card> b = lstOldCollection.Except(lstCurrentCollection, s_cardComparer).ToList();

                    for (int i = 0; i < b.Count; i++)
                    {
                        Card cardB = b[i];

                        Card cardA = a.Find(c => c.Id.Equals(cardB.Id) && c.Premium == cardB.Premium);

                        int nCount = cardB.Count;

                        if (cardA != null)
                        {
                            nCount = cardB.Count - cardA.Count;
                        }
                        else { }

                        lstDisenchantedCardsHistory.Add(new Card(cardB.Id, nCount, cardB.Premium));
                    }

                    Cache.SaveCollection(account, lstDisenchantedCardsHistory, DisenchantedString);
                }
                else { }

                s_blnCheckInProgress = false;
            }
            else { }
        }
        #endregion

        private class CardComparer : IEqualityComparer<Card>
        {
            public bool Equals(Card x, Card y)
            {
                return x.Id.Equals(y.Id) && x.Premium == y.Premium && x.Count == y.Count;
            }

            public int GetHashCode(Card obj)
            {
                return obj.Id.GetHashCode() + obj.Premium.GetHashCode() + obj.Count.GetHashCode();
            }
        }
    }
}