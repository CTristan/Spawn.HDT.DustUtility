﻿using HearthMirror;
using HearthMirror.Objects;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace Spawn.HDT.DustUtility.Offline
{
    public static class Cache
    {
        #region Constants
        private const string CollectionString = "collection";
        private const string DecksString = "decks";
        #endregion

        #region Static Variables
        private static Timer s_timer;

        private static bool s_blnSaveCollectionInProgress;
        private static bool s_blnSaveDecksInProgress;
        #endregion

        #region Static Properties
        public static bool TimerEnabled => s_timer != null;

        //public static bool SaveProcessSuccessful => s_blnSavedCollection && s_blnSavedDecks;
        #endregion

        #region SaveCollection
        public static bool SaveCollection(Account account, List<Card> lstCollection = null, string strType = CollectionString)
        {
            bool blnRet = false;

            if (lstCollection == null)
            {
                lstCollection = Reflection.GetCollection();
            }
            else { }

            if (lstCollection != null && lstCollection.Count > 0 && !s_blnSaveCollectionInProgress)
            {
                s_blnSaveCollectionInProgress = true;

                string strPath = DustUtilityPlugin.GetFullFileName(account, strType);

                List<CachedCard> lstCachedCards = lstCollection.ToCachedCards();

                if (File.Exists(strPath))
                {
                    File.Delete(strPath);
                }
                else { }

                using (StreamWriter writer = new StreamWriter(strPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CachedCard>));

                    serializer.Serialize(writer, lstCachedCards);
                }

                blnRet = true;

                s_blnSaveCollectionInProgress = false;
            }
            else { }

            return blnRet;
        }
        #endregion

        #region SaveDecks
        public static bool SaveDecks(Account account)
        {
            bool blnRet = false;

            List<Deck> lstDecks = Reflection.GetDecks();

            if (lstDecks != null && (lstDecks.Count > 0 && lstDecks[0].Cards.Count > 0) && !s_blnSaveDecksInProgress)
            {
                s_blnSaveDecksInProgress = true;

                List<CachedDeck> lstCachedDecks = new List<CachedDeck>();

                for (int i = 0; i < lstDecks.Count; i++)
                {
                    Deck deck = lstDecks[i];

                    CachedDeck cachedDeck = new CachedDeck()
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Hero = deck.Hero,
                        IsWild = deck.IsWild,
                        Type = deck.Type,
                        SeasonId = deck.SeasonId,
                        CardBackId = deck.CardBackId,
                        HeroPremium = deck.HeroPremium,
                        Cards = deck.Cards.ToCachedCards()
                    };

                    lstCachedDecks.Add(cachedDeck);
                }

                string strPath = DustUtilityPlugin.GetFullFileName(account, DecksString);

                if (File.Exists(strPath))
                {
                    File.Delete(strPath);
                }
                else { }

                using (StreamWriter writer = new StreamWriter(strPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CachedDeck>));

                    serializer.Serialize(writer, lstCachedDecks);
                }

                blnRet = true;

                s_blnSaveDecksInProgress = false;
            }
            else { }

            return blnRet;
        }
        #endregion

        #region LoadCollection
        public static List<Card> LoadCollection(Account account, string strType = CollectionString)
        {
            List<Card> lstRet = new List<Card>();

            string strPath = DustUtilityPlugin.GetFullFileName(account, strType);

            if (File.Exists(strPath))
            {
                List<CachedCard> lstCachedCards = new List<CachedCard>();

                using (StreamReader reader = new StreamReader(strPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CachedCard>));

                    lstCachedCards = (List<CachedCard>)serializer.Deserialize(reader);
                }

                for (int i = 0; i < lstCachedCards.Count; i++)
                {
                    CachedCard cachedCard = lstCachedCards[i];

                    lstRet.Add(new Card(cachedCard.Id, cachedCard.Count, cachedCard.IsGolden));
                }
            }
            else { }

            return lstRet;
        }
        #endregion

        #region LoadDecks
        public static List<Deck> LoadDecks(Account account)
        {
            List<Deck> lstRet = new List<Deck>();

            string strPath = DustUtilityPlugin.GetFullFileName(account, DecksString);

            if (File.Exists(strPath))
            {
                List<CachedDeck> lstCachedDecks = new List<CachedDeck>();

                using (StreamReader reader = new StreamReader(strPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CachedDeck>));

                    lstCachedDecks = (List<CachedDeck>)serializer.Deserialize(reader);
                }

                for (int i = 0; i < lstCachedDecks.Count; i++)
                {
                    CachedDeck cachedDeck = lstCachedDecks[i];

                    Deck deck = new Deck()
                    {
                        Id = cachedDeck.Id,
                        Name = cachedDeck.Name,
                        Hero = cachedDeck.Hero,
                        IsWild = cachedDeck.IsWild,
                        Type = cachedDeck.Type,
                        SeasonId = cachedDeck.SeasonId,
                        CardBackId = cachedDeck.CardBackId,
                        HeroPremium = cachedDeck.HeroPremium,
                        Cards = cachedDeck.Cards.ToCards()
                    };

                    lstRet.Add(deck);
                }
            }
            else { }

            return lstRet;
        }
        #endregion

        #region StartTimer
        public static void StartTimer()
        {
            if (s_timer != null)
            {
                StopTimer();
            }
            else { }

            s_timer = new Timer(OnTick, null, 0, 1000 * 10); //every 10s, if successful then every 5 min

            Log.WriteLine("Started cache timer", LogType.Debug);
        }
        #endregion

        #region StopTimer
        public static void StopTimer()
        {
            s_timer.Dispose();
            s_timer = null;

            Log.WriteLine("Stopped cache timer", LogType.Debug);
        }
        #endregion

        #region OnTick
        private static void OnTick(object state)
        {
            Log.WriteLine("Cache OnTick", LogType.Debug);

            bool blnSuccess = true;

            Account account = new Account(Reflection.GetBattleTag(), Helper.GetCurrentRegion().Result);

            //DisenchantedCardsHistory.CheckCollection(account, Reflection.GetCollection());

            Log.WriteLine("Saving collection", LogType.Debug);

            blnSuccess &= SaveCollection(account);

            if (blnSuccess)
            {
                Log.WriteLine("Saved collection successfuly", LogType.Info);
            }
            else { }

            Log.WriteLine("Saving decks", LogType.Debug);

            blnSuccess &= SaveDecks(account);

            if (blnSuccess)
            {
                Log.WriteLine("Saved decks successfuly", LogType.Info);
            }
            else { }

            if (blnSuccess)
            {
                int nTime = 1000 * 60;

                s_timer.Change(nTime, nTime);

                Log.WriteLine("Changed interval to 1 min.", LogType.Debug);
            }
            else { }
        }
        #endregion
    }
}