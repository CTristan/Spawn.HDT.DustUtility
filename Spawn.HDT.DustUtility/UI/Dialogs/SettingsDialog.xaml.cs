using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Net;
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
            clearCacheButton.IsEnabled = cacheDirectoryExists;
        }
        #endregion

        #region Events
        #region OnWindowLoaded
        private void OnWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            cbOfflineMode.IsChecked = Settings.OfflineMode;
            cbCheckForUpdates.IsChecked = Settings.CheckForUpdate;
            cbCardImageTooltip.IsChecked = Settings.CardImageTooltip;
            cbLocalImageCache.IsChecked = Settings.LocalImageCache;
        }
        #endregion

        #region OnOkClick
        private void OnOkClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.OfflineMode = cbOfflineMode.IsChecked.Value;
            Settings.CheckForUpdate = cbCheckForUpdates.IsChecked.Value;
            Settings.CardImageTooltip = cbCardImageTooltip.IsChecked.Value;
            Settings.LocalImageCache = cbLocalImageCache.IsChecked.Value;

            Log.WriteLine("Saved settings", LogType.Info);

            Close();
        }
        #endregion

        #region OnCancelClick
        private void OnCancelClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region OnLocalImageCacheIsEnabledChanged
        private void OnLocalImageCacheIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (!((bool)e.NewValue))
            {
                cbLocalImageCache.IsChecked = false;
            }
            else { }
        }
        #endregion

        #region OnClearLocalImageCacheClick
        private void OnClearLocalImageCacheClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the local image cache?", "Dust Utility", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    HearthstoneCardImageManager.ClearLocalCache();

                    clearCacheButton.IsEnabled = false;
                }
                else { }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Couldn't clear cache directory! Check log for more information.", "Dust Utility", MessageBoxButton.OK, MessageBoxImage.Error);

                Log.WriteLine($"Couldn't clear cache directory: {ex}", LogType.Error);
            }
        }
        #endregion
        #endregion
    }
}