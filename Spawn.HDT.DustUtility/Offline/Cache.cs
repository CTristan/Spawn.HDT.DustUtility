using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using HearthMirror;
using HearthMirror.Objects;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace Spawn.HDT.DustUtility.Offline
{
    public class Cache
    {
        #region Constants
        private const string CollectionString = "collection";
        private const string DecksString = "decks";
        #endregion

        #region Member Variables
        private Timer m_timer;

        private Account m_account;

        private bool m_blnSaveCollectionInProgress;
        private bool m_blnSavedCollection;
        private bool m_blnSaveDecksInProgress;
        private bool m_blnSavedDecks;
        #endregion

        #region Properties
        public bool TimerEnabled => m_timer != null;

        public bool SaveProcessSuccessful => m_blnSavedCollection && m_blnSavedDecks;
        #endregion

        #region Ctor
        public Cache(Account account)
        {
            m_account = account;
        } 
        #endregion

        #region SaveCollection
        public bool SaveCollection()
        {
            bool blnRet = false;

            List<Card> lstCollection = Reflection.GetCollection();

            if (lstCollection != null && lstCollection.Count > 0 && !m_blnSaveCollectionInProgress)
            {
                m_blnSaveCollectionInProgress = true;

                List<CachedCard> lstCachedCards = lstCollection.ToCachedCards();

                string strPath = GetFullFileName(CollectionString);

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

                m_blnSaveCollectionInProgress = false;

                blnRet = true;
            }
            else { }

            return blnRet;
        }
        #endregion

        #region SaveDecks
        public bool SaveDecks()
        {
            bool blnRet = false;

            List<Deck> lstDecks = Reflection.GetDecks();

            if (lstDecks != null && (lstDecks.Count > 0 && lstDecks[0].Cards.Count > 0) && !m_blnSaveDecksInProgress)
            {
                m_blnSaveDecksInProgress = true;

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

                string strPath = GetFullFileName(DecksString);

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

                m_blnSaveDecksInProgress = false;

                blnRet = true;
            }
            else { }

            return blnRet;
        }
        #endregion

        #region LoadCollection
        public List<Card> LoadCollection()
        {
            List<Card> lstRet = new List<Card>();

            string strPath = GetFullFileName(CollectionString);

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
        public List<Deck> LoadDecks()
        {
            List<Deck> lstRet = new List<Deck>();

            string strPath = GetFullFileName(DecksString);

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
        public void StartTimer()
        {
            m_timer = new Timer(OnTick, null, 0, 1000 * 10); //every 10s, if successful then every 5 min

            Log.WriteLine("Started cache timer", LogType.Debug);
        }
        #endregion

        #region StopTimer
        public void StopTimer()
        {
            m_timer.Dispose();
            m_timer = null;

            Log.WriteLine("Stopped cache timer", LogType.Debug);
        }
        #endregion

        #region OnTick
        private void OnTick(object state)
        {
            Log.WriteLine("Cache OnTick", LogType.Debug);

            bool blnSuccess = true;

            if (!m_blnSavedCollection)
            {
                Log.WriteLine("Saving collection", LogType.Debug);

                blnSuccess &= SaveCollection();

                m_blnSavedCollection = blnSuccess;

                if (m_blnSavedCollection)
                {
                    Log.WriteLine("Saved collection successfuly", LogType.Info);
                }
                else { }
            }
            else { }

            if (!m_blnSavedDecks)
            {
                Log.WriteLine("Saving decks", LogType.Debug);

                blnSuccess &= SaveDecks();

                m_blnSavedDecks = blnSuccess;

                if (m_blnSavedCollection)
                {
                    Log.WriteLine("Saved decks successfuly", LogType.Info);
                }
                else { }
            }
            else { }

            if (blnSuccess)
            {
                int nTime = 1000 * 60 * 5;

                m_timer.Change(nTime, nTime);

                Log.WriteLine("Changed interval to 5 min.", LogType.Debug);
            }
            else { }
        }
        #endregion

        #region GetFullFileName
        private string GetFullFileName(string strType)
        {
            string strBaseDir = Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.DataDir, DustUtilityPlugin.DataFolder);

            if (!Directory.Exists(strBaseDir))
            {
                Directory.CreateDirectory(strBaseDir);
            }
            else { }

            return Path.Combine(strBaseDir, $"{m_account.AccountString}_{strType}.xml");
        } 
        #endregion
    }
}
