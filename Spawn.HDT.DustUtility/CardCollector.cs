using System;
using System.Collections.Generic;
using System.Windows;
using HearthDb.Enums;
using HearthMirror;
using HearthMirror.Objects;

namespace Spawn.HDT.DustUtility
{
    public class CardCollector
    {
        private List<CardWrapper> m_lstUnusedCards;

        public CardCollector()
        {
            m_lstUnusedCards = new List<CardWrapper>();
        }

        public CardWrapper[] GetDustableCards(Parameters parameters)
        {
            List<Card> lstCollection = Reflection.GetCollection();

            CheckForUnusedCards(lstCollection);

            List<CardWrapper> lstRet = new List<CardWrapper>();

            if (lstCollection.Count > 0)
            {
                int nTotalAmount = 0;
                
                ProcessCards(lstRet, parameters, ref nTotalAmount);

                //Post processing
                //Remove low rarity cards if the total amount is over the targeted amount
                if (nTotalAmount > parameters.DustAmount)
                {
                    PostProcessCards(lstRet, parameters, ref nTotalAmount); 
                }
                else { }
            }
            else { }

            return lstRet.ToArray();
        }

        private void PostProcessCards(List<CardWrapper> lstCards, Parameters parameters, ref int nTotalAmount)
        {
            if (lstCards.Count > 0 && parameters.Rarities.Count > 0)
            {
                int nDifference = nTotalAmount - parameters.DustAmount;

                if (nDifference > 0)
                {
                    bool blnDone = false;

                    int nCurrentAmount = 0;

                    for (int i = 0; i < parameters.Rarities.Count && !blnDone; i++)
                    {
                        Rarity rarity = parameters.Rarities[i];

                        List<CardWrapper> lstChunk = lstCards.FindAll(delegate (CardWrapper cw) { return cw.DBCard.Rarity == rarity; });

                        lstChunk.Sort(CompareCount);

                        for (int j = 0; j < lstChunk.Count && !blnDone; j++)
                        {
                            CardWrapper cardWrapper = lstChunk[j];

                            nCurrentAmount += cardWrapper.GetDustValue();

                            int nCurrentDifference = nDifference - nCurrentAmount;

                            blnDone = nCurrentDifference == 0;

                            if (!blnDone)
                            {
                                if (nCurrentDifference < 0)
                                {
                                    blnDone = true;
                                }
                                else
                                {
                                    lstCards.Remove(cardWrapper);
                                }
                            }
                            else
                            {
                                lstCards.Remove(cardWrapper);
                            }
                        }
                    }
                }
                else { }
            }
            else { }
        }

        private int CompareCount(CardWrapper a, CardWrapper b)
        {
            return a.Count.CompareTo(b.Count);
        }

        private void ProcessCards(List<CardWrapper> lstRet, Parameters parameters, ref int nTotalAmount)
        {
            bool blnDone = false;

            for (int i = 0; i < parameters.Rarities.Count && !blnDone; i++)
            {
                List<CardWrapper> lstCards = GetCardsForRarity(parameters.Rarities[i], parameters.IncludeGoldenCards);

                lstCards = FilterForClasses(lstCards, parameters.Classes);

                for (int j = 0; j < lstCards.Count && !blnDone; j++)
                {
                    CardWrapper card = lstCards[j];

                    nTotalAmount += card.GetDustValue();

                    lstRet.Add(card);

                    blnDone = nTotalAmount >= parameters.DustAmount;
                }
            }
        }

        public int GetTotalDustValueForAllCards()
        {
            List<Card> lstCards = Reflection.GetCollection();

            int nRet = 0;

            for (int i = 0; i < lstCards.Count; i++)
            {
                nRet += new CardWrapper(lstCards[i]).GetDustValue();
            }

            return nRet;
        }

        private void CheckForUnusedCards(List<Card> lstCollection)
        {
            if (lstCollection == null)
            {
                throw new ArgumentNullException("lstCollection");
            }
            else { }

            m_lstUnusedCards.Clear();

            List<Deck> lstDecks = Reflection.GetDecks();

            if (lstDecks.Count > 0 && lstDecks[0].Cards.Count > 0)
            {
                for (int i = 0; i < lstCollection.Count; i++)
                {
                    Card card = lstCollection[i];
                    CardWrapper cardWrapper = new CardWrapper(card);

                    for (int j = 0; j < lstDecks.Count; j++)
                    {
                        if (lstDecks[j].Contains(card.Id))
                        {
                            Card c = lstDecks[j].GetCard(card.Id);

                            if (c.Count > cardWrapper.MaxCountInDecks)
                            {
                                cardWrapper.MaxCountInDecks = c.Count;
                            }
                            else { }
                        }
                        else { }
                    }

                    if (cardWrapper.MaxCountInDecks <= 1 && cardWrapper.Card.Count > cardWrapper.MaxCountInDecks && !(cardWrapper.DBCard.Rarity == Rarity.LEGENDARY && cardWrapper.MaxCountInDecks == 1))
                    {
                        m_lstUnusedCards.Add(cardWrapper);
                    }
                    else { }
                }
            }
            else
            {
                MessageBox.Show("Navigate to the \"Play\" page first!", "Dust Utility", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<CardWrapper> FilterForClasses(List<CardWrapper> lstCards, List<CardClass> lstClasses)
        {
            List<CardWrapper> lstRet = new List<CardWrapper>();

            for (int i = 0; i < lstClasses.Count; i++)
            {
                List<CardWrapper> lstChunk = lstCards.FindAll(delegate (CardWrapper cw) { return cw.DBCard.Class == lstClasses[i]; });

                lstRet.AddRange(lstChunk);
            }

            return lstRet;
        }

        private List<CardWrapper> GetCardsForRarity(Rarity rarity, bool blnIncludeGoldenCards = false)
        {
            List<CardWrapper> lstRet = new List<CardWrapper>();

            if (blnIncludeGoldenCards)
            {
                lstRet = m_lstUnusedCards.FindAll(delegate (CardWrapper c) { return HearthDb.Cards.All[c.Card.Id].Rarity == rarity; });
            }
            else
            {
                lstRet = m_lstUnusedCards.FindAll(delegate (CardWrapper c) { return HearthDb.Cards.All[c.Card.Id].Rarity == rarity && c.Card.Premium == false; });
            }

            return lstRet;
        }
    }
}
