using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;

namespace Spawn.HDT.DustUtility
{
    public class DustUtilityPlugin : IPlugin
    {
        public string Name => "Dust Utility";

        public string Description => "Enter the amount of dust you want to get and check which cards are currently not used in any deck in order to disenchant them.";

        public string ButtonText => null;

        public string Author => "CLJunge";

        public Version Version => new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString(2));

        public MenuItem MenuItem => m_menuItem;

        private MenuItem m_menuItem;
        private CardCollector m_cardCollector;
        
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
            if (Core.Game.IsRunning)
            {
                if (m_cardCollector == null)
                {
                    m_cardCollector = new CardCollector();
                }
                else { }

                DustableCardsWindow w = new DustableCardsWindow(m_cardCollector);

                w.Show();
            }
            else
            {
                MessageBox.Show("Hearthstone isn't running!", "Dust Utility", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnButtonPress()
        {
        }

        public void OnUnload()
        {
        }

        public void OnUpdate()
        {
        }
    }
}
