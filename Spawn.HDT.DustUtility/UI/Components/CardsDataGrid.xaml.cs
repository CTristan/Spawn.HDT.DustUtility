using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Spawn.HDT.DustUtility.UI.Components
{
    public partial class CardsDataGrid
    {
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

        #region Custom Events
        public event EventHandler<GridItemEventArgs> RowDoubleClick;
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
            if (dataGrid.SelectedItem != null)
            {
                GridItemEventArgs args = new GridItemEventArgs(dataGrid.SelectedItem as GridItem);

                if (RowDoubleClick != null)
                {
                    RowDoubleClick(sender, args);
                }
                else { }
            }
            else { }
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
        }
        #endregion

        #endregion
    }
}