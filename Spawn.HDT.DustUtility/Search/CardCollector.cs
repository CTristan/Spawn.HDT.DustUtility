using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HearthDb.Enums;
using HearthMirror;
using HearthMirror.Objects;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Spawn.HDT.DustUtility.Offline;

namespace Spawn.HDT.DustUtility.Search
{
    public class CardCollector
    {
        #region Static Variables
        private Regex s_numericRegex = new Regex("[0-9]+");
        #endregion

        #region Member Variables
        private MetroWindow m_mainWindow;
        
        private List<CardWrapper> m_lstUnusedCards;
        private bool m_blnOfflineMode;
        #endregion

        #region Properties
        public bool OfflineMode
        {
            get => m_blnOfflineMode;
            set => m_blnOfflineMode = value;
        }
        #endregion

        #region Ctor
        public CardCollector(MetroWindow mainWindow, bool offlineMode = false)
        {
            m_mainWindow = mainWindow;

            m_blnOfflineMode = offlineMode;

            m_lstUnusedCards = new List<CardWrapper>();
        }
        #endregion

        #region GetCardsAsync
        public async Task<CardWrapper[]> GetCardsAsync(Parameters parameters)
        {
            bool blnDustMode = s_numericRegex.IsMatch(parameters.QueryString);

            List<Card> lstCollection = LoadCollection();

            if (parameters.UnusedCardsOnly)
            {
                await CheckForUnusedCardsAsync(lstCollection); 
            }
            else
            {
                m_lstUnusedCards = new List<CardWrapper>();

                for (int i = 0; i < lstCollection.Count; i++)
                {
                    m_lstUnusedCards.Add(new CardWrapper(lstCollection[i]));
                }
            }

            List<CardWrapper> lstRet = new List<CardWrapper>();

            if (lstCollection.Count > 0)
            {
                if (blnDustMode)
                {
                    GetCardsForDustAmount(parameters, lstRet);
                }
                else
                {
                    GetCardByQuery(parameters, lstRet);
                }
            }
            else { }

            return lstRet.ToArray();
        }
        #endregion

        #region GetCardForDustAmount
        private void GetCardsForDustAmount(Parameters parameters, List<CardWrapper> lstCards)
        {
            int nDustAmount = Convert.ToInt32(parameters.QueryString);
            int nTotalAmount = 0;

            bool blnDone = false;

            for (int i = 0; i < parameters.Rarities.Count && !blnDone; i++)
            {
                List<CardWrapper> lstChunk = GetCardsForRarity(parameters.Rarities[i], parameters.IncludeGoldenCards);

                lstChunk = FilterForClasses(lstChunk, parameters.Classes);

                lstChunk = FilterForSets(lstChunk, parameters.Sets);

                lstChunk = lstChunk.OrderBy(c => c.GetDustValue()) .ToList();

                for (int j = 0; j < lstChunk.Count && !blnDone; j++)
                {
                    CardWrapper card = lstChunk[j];

                    nTotalAmount += card.GetDustValue();

                    lstCards.Add(card);

                    blnDone = nTotalAmount >= nDustAmount;
                }
            }

            //Post processing
            //Remove low rarity cards if the total amount is over the targeted amount
            if (nTotalAmount > nDustAmount)
            {
                RemoveRedundantCards(lstCards, parameters, nDustAmount, nTotalAmount);
            }
            else { }
        }
        #endregion

        #region RemoveRedundantCards
        private void RemoveRedundantCards(List<CardWrapper> lstCards, Parameters parameters, int nDustAmount, int nTotalAmount)
        {
            if (lstCards.Count > 0 && parameters.Rarities.Count > 0)
            {
                int nDifference = nTotalAmount - nDustAmount;

                if (nDifference > 0)
                {
                    bool blnDone = false;

                    int nCurrentAmount = 0;

                    for (int i = 0; i < parameters.Rarities.Count && !blnDone; i++)
                    {
                        List<CardWrapper> lstChunk = lstCards.FindAll(c => c.DbCard.Rarity == parameters.Rarities[i]);
                        
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
        #endregion

        #region GetCardsByQuery
        private void GetCardByQuery(Parameters parameters, List<CardWrapper> lstCards)
        {
        }
        #endregion

        #region GetTotalDustValueForAllCards
        public int GetTotalDustValueForAllCards()
        {
            List<Card> lstCards = LoadCollection();

            return lstCards.Sum(c => new CardWrapper(c).GetDustValue());
        }
        #endregion

        #region CheckForUnusedCardsAsync
        private async Task CheckForUnusedCardsAsync(List<Card> lstCollection)
        {
            if (lstCollection == null)
            {
                throw new ArgumentNullException("lstCollection");
            }
            else { }

            m_lstUnusedCards.Clear();

            List<Deck> lstDecks = LoadDecks();

            if (lstDecks.Count > 0 && lstDecks[0].Cards.Count > 0)
            {
                for (int i = 0; i < lstCollection.Count; i++)
                {
                    Card card = lstCollection[i];
                    CardWrapper cardWrapper = new CardWrapper(card);

                    for (int j = 0; j < lstDecks.Count; j++)
                    {
                        if (lstDecks[j].ContainsCard(card.Id))
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

                    if (cardWrapper.MaxCountInDecks < 2 && cardWrapper.Card.Count > cardWrapper.MaxCountInDecks && !(cardWrapper.DbCard.Rarity == Rarity.LEGENDARY && cardWrapper.MaxCountInDecks == 1))
                    {
                        m_lstUnusedCards.Add(cardWrapper);
                    }
                    else { }
                }
            }
            else if (!m_blnOfflineMode)
            {
                await m_mainWindow.ShowMessageAsync(string.Empty, "Navigate to the \"Play\" page first!");
                //MessageBox.Show("Navigate to the \"Play\" page first!", "Dust Utility", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else { }
        }
        #endregion

        #region FilterForClasses
        private List<CardWrapper> FilterForClasses(List<CardWrapper> lstCards, List<CardClass> lstClasses)
        {
            List<CardWrapper> lstRet = new List<CardWrapper>();

            for (int i = 0; i < lstClasses.Count; i++)
            {
                List<CardWrapper> lstChunk = lstCards.FindAll(c => c.DbCard.Class == lstClasses[i]);

                lstRet.AddRange(lstChunk);
            }

            return lstRet;
        }
        #endregion

        #region FilterForSets
        private List<CardWrapper> FilterForSets(List<CardWrapper> lstCards, List<CardSet> lstSets)
        {
            List<CardWrapper> lstRet = new List<CardWrapper>();

            for (int i = 0; i < lstSets.Count; i++)
            {
                List<CardWrapper> lstChunk = lstCards.FindAll(c => c.DbCard.Set == lstSets[i]);

                lstRet.AddRange(lstChunk);
            }

            return lstRet;
        }
        #endregion

        #region GetCardsForRarity
        private List<CardWrapper> GetCardsForRarity(Rarity rarity, bool blnIncludeGoldenCards = false)
        {
            List<CardWrapper> lstRet = new List<CardWrapper>();

            if (blnIncludeGoldenCards)
            {
                lstRet = m_lstUnusedCards.FindAll(c => c.DbCard.Rarity == rarity);
            }
            else
            {
                lstRet = m_lstUnusedCards.FindAll(c => c.DbCard.Rarity == rarity && c.Card.Premium == false);
            }

            return lstRet;
        }
        #endregion

        #region LoadCollection
        private List<Card> LoadCollection()
        {
            List<Card> lstRet = null;

            if (m_blnOfflineMode)
            {
                lstRet = Cache.LoadCollection();
            }
            else
            {
                lstRet = Reflection.GetCollection();
            }

            return lstRet;
        }
        #endregion

        #region LoadDecks
        private List<Deck> LoadDecks()
        {
            List<Deck> lstRet = null;

            if (m_blnOfflineMode)
            {
                lstRet = Cache.LoadDecks();
            }
            else
            {
                lstRet = Reflection.GetDecks();
            }

            return lstRet;
        }
        #endregion
    }
}
