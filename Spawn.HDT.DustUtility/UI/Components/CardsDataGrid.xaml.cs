using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Spawn.HDT.DustUtility.UI.Components
{
    public partial class CardsDataGrid
    {
        #region DP
        #region GridItems DP
        public ObservableCollection<GridItem> GridItems
        {
            get { return GetValue(GridItemsProperty) as ObservableCollection<GridItem>; }
            set { SetValue(GridItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridItemsProperty =
            DependencyProperty.Register("GridItems", typeof(ObservableCollection<GridItem>), typeof(CardsDataGrid), new PropertyMetadata(new ObservableCollection<GridItem>()));
        #endregion

        #region AllowDrag DP
        public bool AllowDrag
        {
            get { return (bool)GetValue(AllowDragProperty); }
            set { SetValue(AllowDragProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowDrag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowDragProperty =
            DependencyProperty.Register("AllowDrag", typeof(bool), typeof(CardsDataGrid), new PropertyMetadata(false));
        #endregion

        #region ContextMenuEnabled DP
        public bool ContextMenuEnabled
        {
            get { return (bool)GetValue(ContextMenuEnabledProperty); }
            set { SetValue(ContextMenuEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextMenuEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMenuEnabledProperty =
            DependencyProperty.Register("ContextMenuEnabled", typeof(bool), typeof(CardsDataGrid), new PropertyMetadata(true));
        #endregion
        #endregion

        #region Member Variables
        private bool m_blnDblClick;
        #endregion

        #region Custom Events
        public event EventHandler<GridItemEventArgs> RowDeleted;
        #endregion

        #region Ctor
        public CardsDataGrid()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        #region OnDataGridMouseDoubleClick
        private void OnDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            m_blnDblClick = true;

            OpenPopup();
        }
        #endregion

        #region OnDataGridMouseDown
        private void OnDataGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = dataGrid.InputHitTest(e.GetPosition(dataGrid));

            if (element is System.Windows.Controls.ScrollViewer)
            {
                dataGrid.SelectedIndex = -1;
            }
            else { }

            if (!m_blnDblClick)
            {
                ClosePopup();
            }
            else { }

            m_blnDblClick = false;
        }
        #endregion

        #region OnDataGridSelectionChanged
        private void OnDataGridSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ClosePopup();
        }
        #endregion

        #region OnDeleteRowClick
        private void OnDeleteRowClick(object sender, RoutedEventArgs e)
        {
            int nIndex = dataGrid.SelectedIndex;

            System.Diagnostics.Debug.WriteLine($"Deleting row at index \"{nIndex}\"");

            if (dataGrid.SelectedItem is GridItem)
            {
                //MetroWindow window = Window.GetWindow(this) as MetroWindow;

                //MessageDialogResult result = await window.ShowMessageAsync(string.Empty, "Are you sure you want to remove the selected card?", MessageDialogStyle.AffirmativeAndNegative);

                //if (result == MessageDialogResult.Affirmative)
                //{
                GridItem item = dataGrid.SelectedItem as GridItem;

                GridItems.RemoveAt(nIndex);

                if (RowDeleted != null)
                {
                    RowDeleted(this, new GridItemEventArgs(item));
                }
                else { }
                //}
                //else { }
            }
            else { }
        }
        #endregion

        #region OnContextMenuOpening
        private void OnContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            e.Handled = !ContextMenuEnabled;
        }
        #endregion

        #region OnPopupMouseDown
        private void OnPopupMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePopup();
        }
        #endregion
        #endregion

        #region OpenPopup
        private void OpenPopup()
        {
            if (dataGrid.SelectedItem is GridItem)
            {
                cardImagePopup.IsOpen = true;

                cardImageContainer.CardWrapper = (dataGrid.SelectedItem as GridItem).Tag;
            }
            else { }
        }
        #endregion

        #region ClosePopup
        private void ClosePopup()
        {
            if (cardImagePopup.IsOpen)
            {
                cardImagePopup.IsOpen = false;

                cardImageContainer.CardWrapper = null;
            }
            else { }
        }
        #endregion
    }
}