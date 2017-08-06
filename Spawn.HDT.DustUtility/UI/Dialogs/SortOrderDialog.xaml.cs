using System;
using System.Collections.Generic;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class SortOrderDialog
    {
        #region Member Variables
        private int m_nMaxCount; 
        #endregion

        #region Ctor
        public SortOrderDialog()
        {
            InitializeComponent();

            SortOrder sortOrder = SortOrder.Parse(Settings.SortOrder);

            for (int i = 0; i < sortOrder.Items.Count; i++)
            {
                lbItems.Items.Add(sortOrder.Items[i]);
            }

            m_nMaxCount = Enum.GetValues(typeof(SortOrder.Item)).Length;

            addButtom.IsEnabled = lbItems.Items.Count < m_nMaxCount;

            removeButton.IsEnabled = false;
            moveUpButton.IsEnabled = false;
            moveDownButton.IsEnabled = false;
        }
        #endregion

        #region Events
        #region OnItemSelectionChanged
        private void OnItemSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            removeButton.IsEnabled = lbItems.SelectedIndex > -1;
            moveUpButton.IsEnabled = lbItems.SelectedIndex > 0;
            moveDownButton.IsEnabled = lbItems.SelectedIndex > -1 && lbItems.SelectedIndex < lbItems.Items.Count - 1;
        }
        #endregion

        #region OnAddItemClick
        private void OnAddItemClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SortOrder.Item[] vItems = (SortOrder.Item[])Enum.GetValues(typeof(SortOrder.Item));

            List<SortOrder.Item> lstUnusedItems = new List<SortOrder.Item>();

            for (int i = 0; i < vItems.Length; i++)
            {
                if (!lbItems.Items.Contains(vItems[i]))
                {
                    lstUnusedItems.Add(vItems[i]);
                }
                else { }
            }

            AddSortOrderItemDialog dialog = new AddSortOrderItemDialog(lstUnusedItems)
            {
                Owner = this
            };

            if (dialog.ShowDialog().Value)
            {
                lbItems.Items.Add(dialog.SelectedItem);

                addButtom.IsEnabled = lbItems.Items.Count < m_nMaxCount;
            }
            else { }
        }
        #endregion

        #region OnRemoveItemClick
        private void OnRemoveItemClick(object sender, System.Windows.RoutedEventArgs e)
        {
            int nIndex = lbItems.SelectedIndex;

            lbItems.Items.RemoveAt(nIndex);

            if (lbItems.Items.Count > 0)
            {
                lbItems.SelectedIndex = nIndex - 1; 
            }
            else { }

            addButtom.IsEnabled = lbItems.Items.Count < m_nMaxCount;
        }
        #endregion

        #region OnMoveUpClick
        private void OnMoveUpClick(object sender, System.Windows.RoutedEventArgs e)
        {
            int nIndex = lbItems.SelectedIndex;

            var item = lbItems.Items[nIndex - 1];

            lbItems.Items[nIndex - 1] = lbItems.Items[nIndex];

            lbItems.Items[nIndex] = item;

            lbItems.SelectedIndex = nIndex - 1;
        }
        #endregion

        #region OnMoveDownClick
        private void OnMoveDownClick(object sender, System.Windows.RoutedEventArgs e)
        {
            int nIndex = lbItems.SelectedIndex;

            var item = lbItems.Items[nIndex + 1];

            lbItems.Items[nIndex + 1] = lbItems.Items[nIndex];

            lbItems.Items[nIndex] = item;

            lbItems.SelectedIndex = nIndex + 1;
        } 
        #endregion

        #region OnSaveClick
        private void OnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            string strOrder = string.Empty;

            for (int i = 0; i < lbItems.Items.Count; i++)
            {
                strOrder = $"{strOrder};{lbItems.Items[i]}";
            }

            strOrder = strOrder.Substring(1, strOrder.Length - 1);

            Settings.SortOrder = strOrder;

            DialogResult = true;

            Close();
        }
        #endregion 
        #endregion
    }
}
