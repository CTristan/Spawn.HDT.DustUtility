using Spawn.HDT.DustUtility.Update;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class DownloadProgressDialog
    {
        #region Ctor
        public DownloadProgressDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        #region OnWindowLoaded
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            GitHubUpdateManager.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(async (s, args) => await UpdateProgressBarAsync(args.ProgressPercentage));

            GitHubUpdateManager.DownloadCompleted += OnDownloadFinished;

            GitHubUpdateManager.Download(GitHubUpdateManager.NewVersion);
        }
        #endregion

        #region OnClick
        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (downloadPanel.Visibility == Visibility.Visible)
            {
                GitHubUpdateManager.CancelDownload();
            }
            else { }

            Close();
        }
        #endregion
        #endregion

        #region UpdateProgressBarAsync
        public async Task UpdateProgressBarAsync(int nValue)
        {
            progressBar.Value = nValue;

            if (progressBar.Value == 100)
            {
                await Task.Delay(750);

                UpdateUI();
            }
            else { }
        }
        #endregion

        #region OnDownloadFinished
        private void OnDownloadFinished(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            string strPath = Path.Combine(DustUtilityPlugin.DataDirectory, "update.zip");

            using (FileStream fs = File.Open(strPath, FileMode.Create))
            {
                fs.Write(e.Result, 0, e.Result.Length);
            }

            string strPluginDir = Path.Combine(Hearthstone_Deck_Tracker.Config.AppDataPath, "Plugins");

            string[] vOldFiles = Directory.GetFiles(strPluginDir, "*DustUtility*");

            for (int i = 0; i < vOldFiles.Length; i++)
            {
                File.Delete(vOldFiles[i]);
            }

            ZipFile.ExtractToDirectory(strPath, strPluginDir);

            File.Delete(strPath);
        }
        #endregion

        #region UpdateUI
        private void UpdateUI()
        {
            downloadPanel.Visibility = Visibility.Hidden;
            finishedPanel.Visibility = Visibility.Visible;

            button.Content = "_Ok";
        }
        #endregion
    }
}