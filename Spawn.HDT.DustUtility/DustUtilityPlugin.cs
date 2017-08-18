using HearthMirror;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Offline;
using Spawn.HDT.DustUtility.UI;
using Spawn.HDT.DustUtility.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Spawn.HDT.DustUtility
{
    public class DustUtilityPlugin : IPlugin
    {
        #region Static Variables
        public static string DataDirectory = Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.DataDir, "DustUtility");
        #endregion

        #region Member Variables
        private MenuItem m_menuItem;
        private Window m_window;

        private Account m_account;
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
            if (!Core.Game.IsRunning && Cache.TimerEnabled)
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
                if (m_account == null)
                {
                    ObtainAccount(false);
                }
                else { }

                if (m_account != null)
                {
                    OpenMainWindow();
                }
                else { }
            }
            else if (!Settings.OfflineMode)
            {
                MessageBox.Show("Hearthstone isn't running!", Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else { }
        }
        #endregion

        #region OpenMainWindow
        private void OpenMainWindow()
        {
            if (m_window == null)
            {
                if (Settings.OfflineMode && Core.Game.IsRunning && !Cache.TimerEnabled)
                {
                    Cache.StartTimer();
                }
                else { }

                Log.WriteLine($"Opening main window for {m_account.AccountString}", LogType.Info);

                m_window = new MainWindow(this, m_account, !Core.Game.IsRunning && Settings.OfflineMode);

                m_window.Closed += new EventHandler((s, e) => m_window = null);

                m_window.Show();
            }
            else { }
        }
        #endregion

        #region ObtainAccount
        private void ObtainAccount(bool blnIsSwitching)
        {
            if (Core.Game.IsRunning && !blnIsSwitching)
            {
                m_account = new Account(Reflection.GetBattleTag(), Hearthstone_Deck_Tracker.Helper.GetCurrentRegion().Result);
            }
            else
            {
                List<string> lstAccounts = GetAccountList();

                if (lstAccounts.Count > 0)
                {
                    AccountSelectorDialog accSelectorDialog = new AccountSelectorDialog(lstAccounts);

                    if (accSelectorDialog.ShowDialog().Value)
                    {
                        m_account = Account.Parse(accSelectorDialog.SelectedAccount);
                    }
                    else { }
                }
                else
                {
                    m_account = Account.Empty;
                }
            }

            Log.WriteLine($"Account: {m_account?.AccountString}", LogType.Debug);
        }
        #endregion

        #region GetAccountList
        private List<string> GetAccountList()
        {
            List<string> lstRet = new List<string>();

            if (Directory.Exists(DataDirectory))
            {
                string[] vFiles = Directory.GetFiles(DataDirectory, "*_collection.xml");

                for (int i = 0; i < vFiles.Length; i++)
                {
                    lstRet.Add(Path.GetFileNameWithoutExtension(vFiles[i]));
                }
            }
            else { }

            return lstRet;
        }
        #endregion

        #region SwitchAccounts
        public void SwitchAccounts()
        {
            if (m_window != null)
            {
                Log.WriteLine("Switching accounts...", LogType.Debug);

                Account oldAcc = m_account;

                m_account = null;

                ObtainAccount(true);

                if (m_account == null)
                {
                    m_account = oldAcc;
                }
                else { }

                if (!m_account.Equals(oldAcc))
                {
                    m_window.Close();
                }
                else { }

                OpenMainWindow();

                Log.WriteLine($"Switched accounts: Old={oldAcc.AccountString} New={m_account.AccountString}", LogType.Info);
            }
            else { }
        }
        #endregion

        #region GetFullFileName
        public static string GetFullFileName(Account account, string strType)
        {
            string strRet = string.Empty;

            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }
            else { }

            if (!account.IsEmpty)
            {
                strRet = Path.Combine(DataDirectory, $"{account.AccountString}_{strType}.xml");
            }
            else
            {
                strRet = Path.Combine(DataDirectory, $"{strType}.xml");
            }

            return strRet;
        }
        #endregion
    }
}