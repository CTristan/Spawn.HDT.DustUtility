using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Net;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class SettingsDialog
    {
        #region Ctor
        public SettingsDialog()
        {
            InitializeComponent();
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
        private void OnClearLocalImageCacheClick(object sender, System.Windows.RoutedEventArgs e)
        {
            HearthstoneCardImageManager.ClearLocalCache();
        }
        #endregion
        #endregion
    }
}