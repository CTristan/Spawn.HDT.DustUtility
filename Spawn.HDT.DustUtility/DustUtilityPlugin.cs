using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Offline;
using Spawn.HDT.DustUtility.UI;
using Spawn.HDT.DustUtility.UI.Dialogs;

namespace Spawn.HDT.DustUtility
{
    public class DustUtilityPlugin : IPlugin
    {
        #region Member Variables
        private MenuItem m_menuItem;
        #endregion

        #region Properties
        #region Name
        public string Name => "Dust Utility";
        #endregion

        #region Description
        public string Description => "Enter the amount of dust you want to get and check which cards are currently not used in any deck in order to disenchant them.";
        #endregion

        #region ButtonText
        public string ButtonText => "Settings";
        #endregion

        #region Author
        public string Author => "CLJunge";
        #endregion

        #region Version
        public Version Version => new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
        #endregion

        #region MenuItem
        public MenuItem MenuItem => m_menuItem; 
        #endregion
        #endregion

        #region OnLoad
        public void OnLoad()
        {
            m_menuItem = new MenuItem()
            {
                Header = Name
            };

            m_menuItem.Click += OnClick;
        }
        #endregion
        
        #region OnButtonPress
        public void OnButtonPress()
        {
            Log.WriteLine("Opening settings dialog", LogType.Debug);

            SettingsDialog dialog = new SettingsDialog();

            dialog.ShowDialog();
        }
        #endregion

        #region OnUnload
        public void OnUnload()
        {
            if (Cache.TimerEnabled)
            {
                Cache.StopTimer();
            }
            else { }
        }
        #endregion

        #region OnUpdate
        public void OnUpdate()
        {
            if (!Core.Game.IsRunning && Cache.TimerEnabled && Cache.SaveProcessSuccessful)
            {
                Cache.StopTimer();
            }
            else { }
        }
        #endregion

        #region OnClick
        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (Core.Game.IsRunning || Settings.OfflineMode)
            {
                if (Core.Game.IsRunning && !Cache.TimerEnabled)
                {
                    Cache.StartTimer();
                }
                else { }

                Log.WriteLine("Opening main window", LogType.Debug);

                new MainWindow(!Core.Game.IsRunning && Settings.OfflineMode).Show();
            }
            else if (!Settings.OfflineMode)
            {
                MessageBox.Show("Hearthstone isn't running!", "Dust Utility", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else { }
        }
        #endregion
    }
}
