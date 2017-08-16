using Spawn.HDT.DustUtility.Search;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class AddSortOrderItemDialog
    {
        #region Member Variables
        private SortOrder.ItemContainer m_selectedItem;
        #endregion

        #region Properties
        public SortOrder.ItemContainer SelectedItem => m_selectedItem;
        #endregion

        #region Ctor
        public AddSortOrderItemDialog()
            : this(null)
        {
        }

        public AddSortOrderItemDialog(List<SortOrder.ItemContainer> items)
        {
            InitializeComponent();

            if (items == null || items?.Count == 0)
            {
                items = new List<SortOrder.ItemContainer>();

                SortOrder.Item[] vItems = (SortOrder.Item[])Enum.GetValues(typeof(SortOrder.Item));

                for (int i = 0; i < vItems.Length; i++)
                {
                    items.Add(new SortOrder.ItemContainer(vItems[i]));
                }
            }
            else { }

            cbItems.ItemsSource = items;

            cbItems.SelectedIndex = 0;
        }
        #endregion

        #region Events
        #region OnOkClick
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            m_selectedItem = (SortOrder.ItemContainer)cbItems.SelectedItem;

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
    }
}