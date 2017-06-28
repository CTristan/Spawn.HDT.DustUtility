namespace Spawn.HDT.DustUtility
{
    public static class Settings
    {
        public static bool OfflineMode
        {
            get
            {
                return Properties.Settings.Default.OfflineMode;
            }
            set
            {
                Properties.Settings.Default.OfflineMode = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
