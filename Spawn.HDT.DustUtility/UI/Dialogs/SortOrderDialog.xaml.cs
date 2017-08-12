using System;
using System.Collections.Generic;
using System.Linq;
using Spawn.HDT.DustUtility.Search;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class SortOrderDialog
    {
        #region Static Variables
        private static ItemContainerComparer s_itemContainerComparer = new ItemContainerComparer();
        #endregion

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
            AddSortOrderItemDialog dialog = new AddSortOrderItemDialog(GetUnusedItems())
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
            SaveSortOrder();

            DialogResult = true;

            Close();
        }
        #endregion
        #endregion

        #region SaveSortOrder
        private void SaveSortOrder()
        {
            string strOrder = string.Empty;

            for (int i = 0; i < lbItems.Items.Count; i++)
            {
                strOrder = $"{strOrder};{(lbItems.Items[i] as SortOrder.ItemContainer).Value}";
            }

            strOrder = strOrder.Substring(1, strOrder.Length - 1);

            Settings.SortOrder = strOrder;
        }
        #endregion

        #region GetUnusedItems
        private List<SortOrder.ItemContainer> GetUnusedItems()
        {
            List<SortOrder.ItemContainer> lstRet = new List<SortOrder.ItemContainer>();

            SortOrder.Item[] vItems = (SortOrder.Item[])Enum.GetValues(typeof(SortOrder.Item));

            SortOrder.ItemContainer[] vItemContainers = new SortOrder.ItemContainer[lbItems.Items.Count];

            lbItems.Items.CopyTo(vItemContainers, 0);

            for (int i = 0; i < vItems.Length; i++)
            {
                SortOrder.ItemContainer itemContainer = new SortOrder.ItemContainer(vItems[i]);

                if (!vItemContainers.Contains(itemContainer, s_itemContainerComparer))
                {
                    lstRet.Add(itemContainer);
                }
                else { }
            }

            return lstRet;
        } 
        #endregion

        private class ItemContainerComparer : IEqualityComparer<SortOrder.ItemContainer>
        {
            public bool Equals(SortOrder.ItemContainer x, SortOrder.ItemContainer y)
            {
                bool blnRet = false;

                if (x != null && y != null)
                {
                    blnRet = GetHashCode(x) == GetHashCode(y);
                }
                else { }

                return blnRet;
            }

            public int GetHashCode(SortOrder.ItemContainer obj)
            {
                return obj.Name.GetHashCode() + obj.Value.GetHashCode();
            }
        }
    }
}
