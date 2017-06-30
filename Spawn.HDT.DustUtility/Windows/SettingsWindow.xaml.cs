namespace Spawn.HDT.DustUtility.Windows
{
    public partial class SettingsWindow
    {
        #region Ctor
        public SettingsWindow()
        {
            InitializeComponent();

            cbOfflineMode.IsChecked = Settings.OfflineMode;
        } 
        #endregion

        #region Events
        #region OnSaveClick
        private void OnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.OfflineMode = cbOfflineMode.IsChecked.Value;

            Close();
        }
        #endregion

        #region OnCloseClick
        private void OnCloseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
        #endregion 
        #endregion
    }
}
