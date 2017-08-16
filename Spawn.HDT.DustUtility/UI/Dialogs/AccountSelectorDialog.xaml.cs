using System.Collections.Generic;
using System.Windows;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class AccountSelectorDialog
    {
        #region Properties
        public string SelectedAccount { get; private set; }
        #endregion

        #region Ctor
        public AccountSelectorDialog()
        {
            InitializeComponent();
        }

        public AccountSelectorDialog(List<string> accounts)
            : this()
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                string[] vTemp = accounts[i].Split('_');

                string strDisplayString = $"{vTemp[0]}#{vTemp[1]} ({vTemp[2]})";

                cbAccounts.Items.Add(new AccountContainer() { DisplayString = strDisplayString, ValueString = accounts[i] });
            }

            if (accounts.Count > 0)
            {
                cbAccounts.SelectedIndex = 0;
            }
            else { }
        }
        #endregion

        #region Events
        #region OnOkClick
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            SelectedAccount = cbAccounts.SelectedValue.ToString();

            DialogResult = true;

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

        private class AccountContainer
        {
            public string DisplayString { get; set; }
            public string ValueString { get; set; }
        }
    }
}