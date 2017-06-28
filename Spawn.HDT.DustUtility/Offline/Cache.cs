using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using HearthMirror;
using HearthMirror.Objects;

namespace Spawn.HDT.DustUtility.Offline
{
    public static class Cache
    {
        #region Static Variables
        private static Timer s_timer;
        private static bool s_blnSaveCollectionInProgress;
        private static bool s_blnSaveDecksInProgress; 
        #endregion

        #region Constants
        private const string COLLECTION = "collection";
        private const string DECKS = "decks";
        #endregion

        #region Static Properties
        public static bool TimerEnabled => s_timer != null;
        #endregion

        #region SaveCollection
        public static bool SaveCollection()
        {
            bool blnRet = false;

            List<Card> lstCollection = Reflection.GetCollection();

            if (lstCollection != null && lstCollection.Count > 0 && !s_blnSaveCollectionInProgress)
            {
                s_blnSaveCollectionInProgress = true;

                List<CachedCard> lstCachedCards = lstCollection.ToCachedCards();

                string strPath = GetFullFileName(COLLECTION);

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

                s_blnSaveCollectionInProgress = false;

                blnRet = true;
            }
            else { }

            return blnRet;
        }
        #endregion

        #region SaveDecks
        public static bool SaveDecks()
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

                string strPath = GetFullFileName(DECKS);

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

                s_blnSaveDecksInProgress = false;

                blnRet = true;
            }
            else { }

            return blnRet;
        }
        #endregion

        #region LoadCollection
        public static List<Card> LoadCollection()
        {
            List<Card> lstRet = new List<Card>();

            string strPath = GetFullFileName(COLLECTION);

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
        public static List<Deck> LoadDecks()
        {
            List<Deck> lstRet = new List<Deck>();

            string strPath = GetFullFileName(DECKS);

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
            s_timer = new Timer(OnTick, null, 0, 1000 * 10); //every 10s, if successful then every 5 min
        }
        #endregion

        #region StopTimer
        public static void StopTimer()
        {
            s_timer.Dispose();
            s_timer = null;
        }
        #endregion

        #region OnTick
        private static void OnTick(object state)
        {
            Debug.WriteLine("Cache OnTick");

            bool blnSuccess = true;

            blnSuccess &= SaveCollection();

            blnSuccess &= SaveDecks();

            if (blnSuccess)
            {
                int nTime = 1000 * 60 * 5;
                s_timer.Change(nTime, nTime);
            }
            else { }
        }
        #endregion

        #region GetFullFileName
        private static string GetFullFileName(string strFileName)
        {
            string strBaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HearthstoneDeckTracker", "DustUtility");

            if (!Directory.Exists(strBaseDir))
            {
                Directory.CreateDirectory(strBaseDir);
            }
            else { }

            return Path.Combine(strBaseDir, $"{strFileName}.xml");
        } 
        #endregion
    }
}
