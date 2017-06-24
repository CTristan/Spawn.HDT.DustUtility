using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using System;
using System.Reflection;
using System.Windows.Controls;

namespace Spawn.HDT.StrangerCards
{
    public class StrangerCardsPlugin : IPlugin
    {
        #region Properties

        public string Name => "Stranger Cards";

        public string Description => "Visualizes the player hand in order to keep track of which cards started in your deck and which didn't.";

        public string ButtonText => null;

        public string Author => "BlackHalo";

        public Version Version => new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString(2));

        public MenuItem MenuItem => null;

        #endregion Properties

        private StrangerCardsOverlay m_overlay;

        public void OnButtonPress()
        {
        }

        public void OnLoad()
        {
            GameEvents.OnInMenu.Add(OnInMenu);
            GameEvents.OnGameStart.Add(OnGameStart);

            m_overlay = new StrangerCardsOverlay();

            if (!Core.Game.IsInMenu)
            {
                m_overlay.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                m_overlay.Visibility = System.Windows.Visibility.Collapsed;
            }

            Core.OverlayCanvas.Children.Add(m_overlay);
        }

        public void OnUnload()
        {
            Core.OverlayCanvas.Children.Remove(m_overlay);
        }

        public void OnUpdate()
        {
            m_overlay.UpdateMarker();
            m_overlay.UpdatePosition();
        }

        private void OnGameStart()
        {
            m_overlay.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnInMenu()
        {
            m_overlay.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}