using System;
using System.Collections.Generic;
using System.Windows;
using Spawn.HDT.DustUtility.Search;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class AddSortOrderItemDialog
    {
        #region Properties
        public SortOrder.Item SelectedItem => (SortOrder.Item)cbItems.SelectedItem; 
        #endregion

        #region Ctor
        public AddSortOrderItemDialog()
            : this(null)
        {

        }

        public AddSortOrderItemDialog(List<SortOrder.Item> items)
        {
            InitializeComponent();

            if (items != null && items.Count > 0)
            {
                cbItems.ItemsSource = items;
            }
            else
            {
                cbItems.ItemsSource = Enum.GetValues(typeof(SortOrder.Item));
            }

            cbItems.SelectedIndex = 0;
        }
        #endregion

        #region Events
        #region OnOkClick
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
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
