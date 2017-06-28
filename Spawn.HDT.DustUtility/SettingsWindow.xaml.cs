namespace Spawn.HDT.DustUtility
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            cbOfflineMode.IsChecked = Settings.OfflineMode;
        }

        private void OnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.OfflineMode = cbOfflineMode.IsChecked.Value;

            Close();
        }

        private void OnCloseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
