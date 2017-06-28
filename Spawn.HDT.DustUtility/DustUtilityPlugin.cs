using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using Spawn.HDT.DustUtility.Offline;

namespace Spawn.HDT.DustUtility
{
    public class DustUtilityPlugin : IPlugin
    {
        public string Name => "Dust Utility";

        public string Description => "Enter the amount of dust you want to get and check which cards are currently not used in any deck in order to disenchant them.";

        public string ButtonText => GetButtonText();

        public string Author => "BlackHalo";

        public Version Version => new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString(2));

        public MenuItem MenuItem => m_menuItem;

        private MenuItem m_menuItem;
        private CardCollector m_cardCollector;
        private bool m_blnOfflineMode;
        
        public void OnLoad()
        {
            m_menuItem = new MenuItem()
            {
                Header = Name
            };

            m_menuItem.Click += OnClick;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (Core.Game.IsRunning || m_blnOfflineMode)
            {
                if (m_cardCollector == null)
                {
                    m_cardCollector = new CardCollector(!Core.Game.IsRunning && m_blnOfflineMode);
                }
                else { }

                DustableCardsWindow w = new DustableCardsWindow(m_cardCollector);

                w.Show();
            }
            else if (!m_blnOfflineMode)
            {
                MessageBox.Show("Hearthstone isn't running!", "Dust Utility", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else { }
        }

        public void OnButtonPress()
        {
            m_blnOfflineMode = !m_blnOfflineMode;
        }

        public void OnUnload()
        {
            if (Cache.TimerEnabled)
            {
                Cache.StopTimer(); 
            }
            else { }
        }

        public void OnUpdate()
        {
            if (Core.Game.IsRunning && !Cache.TimerEnabled)
            {
                Cache.StartTimer();
            }
            else { }
        }

        private string GetButtonText()
        {
            string strRet = "Mode: ONLINE";

            if (m_blnOfflineMode)
            {
                strRet = "Mode: OFFLINE";
            }
            else { }

            return strRet;
        }
    }
}
