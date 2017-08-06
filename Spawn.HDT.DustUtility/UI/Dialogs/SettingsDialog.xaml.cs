namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class SettingsDialog
    {
        #region Ctor
        public SettingsDialog()
        {
            InitializeComponent();

            cbOfflineMode.IsChecked = Settings.OfflineMode;
            cbCheckForUpdates.IsChecked = Settings.CheckForUpdate;
        }
        #endregion

        #region Events
        #region OnOkClick
        private void OnOkClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.OfflineMode = cbOfflineMode.IsChecked.Value;
            Settings.CheckForUpdate = cbCheckForUpdates.IsChecked.Value;

            Close();
        }
        #endregion

        #region OnCancelClick
        private void OnCancelClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
        #endregion 
        #endregion
    }
}
