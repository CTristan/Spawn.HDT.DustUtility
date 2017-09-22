using Hearthstone_Deck_Tracker.Utility.Logging;
using System.Windows;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class SettingsDialog
    {
        #region Ctor
        public SettingsDialog()
        {
            InitializeComponent();
        }

        public SettingsDialog(bool cacheDirectoryExists)
            : this()
        {
            //clearCacheButton.IsEnabled = cacheDirectoryExists;
        }
        #endregion

        #region Events
        #region OnWindowLoaded
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            cbOfflineMode.IsChecked = Settings.OfflineMode;
            cbCheckForUpdates.IsChecked = Settings.CheckForUpdate;
            //cbCardImageTooltip.IsChecked = Settings.CardImageTooltip;
            //cbLocalImageCache.IsChecked = Settings.LocalImageCache;
        }
        #endregion

        #region OnOkClick
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Settings.OfflineMode = cbOfflineMode.IsChecked.Value;
            Settings.CheckForUpdate = cbCheckForUpdates.IsChecked.Value;
            //Settings.CardImageTooltip = cbCardImageTooltip.IsChecked.Value;
            //Settings.LocalImageCache = cbLocalImageCache.IsChecked.Value;

            Log.WriteLine("Saved settings", LogType.Info);

            Close();
        }
        #endregion

        #region OnCancelClick
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        #endregion
    }
}