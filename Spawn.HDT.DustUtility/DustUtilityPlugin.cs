using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HearthMirror;
using HearthMirror.Objects;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Offline;
using Spawn.HDT.DustUtility.UI;
using Spawn.HDT.DustUtility.UI.Dialogs;

namespace Spawn.HDT.DustUtility
{
    public class DustUtilityPlugin : IPlugin
    {
        #region Constants
        public const string DataFolder = "DustUtility";
        #endregion

        #region Member Variables
        private MenuItem m_menuItem;

        private Account m_account;
        private Cache m_cache;
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
            if (m_cache != null && m_cache.TimerEnabled)
            {
                m_cache.StopTimer();
            }
            else { }
        }
        #endregion

        #region OnUpdate
        public void OnUpdate()
        {
            if (!Core.Game.IsRunning && (m_cache != null && m_cache.TimerEnabled && m_cache.SaveProcessSuccessful))
            {
                m_cache.StopTimer();
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
                    ObtainAccount();
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
            if (m_cache != null)
            {
                m_cache.StopTimer();
            }
            else { }

            m_cache = new Cache(m_account);

            if (Core.Game.IsRunning)
            {
                m_cache.StartTimer();
            }
            else { }

            Log.WriteLine("Opening main window", LogType.Debug);

            new MainWindow(m_account, m_cache, !Core.Game.IsRunning && Settings.OfflineMode).Show();
        }
        #endregion

        #region ObtainAccount
        private void ObtainAccount()
        {
            if (Core.Game.IsRunning)
            {
                m_account = new Account(Reflection.GetBattleTag(), Hearthstone_Deck_Tracker.Helper.GetCurrentRegion().Result);
            }
            else
            {
                AccountSelectorDialog accSelectorDialog = new AccountSelectorDialog(GetAccountList());

                if (accSelectorDialog.ShowDialog().Value)
                {
                    string[] vTemp = accSelectorDialog.SelectedAccount.Split('_');

                    BattleTag battleTag = new BattleTag()
                    {
                        Name = vTemp[0],
                        Number = Convert.ToInt32(vTemp[1])
                    };

                    m_account = new Account(battleTag, (Region)Enum.Parse(typeof(Region), vTemp[2]));
                }
                else { }
            }
        }
        #endregion

        #region GetAccountList
        private List<string> GetAccountList()
        {
            List<string> lstRet = new List<string>();

            string strPath = Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.DataDir, DataFolder);

            if (Directory.Exists(strPath))
            {
                string[] vFiles = Directory.GetFiles(strPath, "*collection.xml");

                for (int i = 0; i < vFiles.Length; i++)
                {
                    lstRet.Add(Path.GetFileNameWithoutExtension(vFiles[i]));
                }
            }
            else { }

            return lstRet;
        } 
        #endregion
    }
}
