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

        #region Ctor
        public CardsDataGrid()
        {
            InitializeComponent();
        }
        #endregion

        #region OnDataGridMouseMove
        private void OnDataGridMouseMove(object sender, MouseEventArgs e)
        {
            if (cardImagePopup.IsOpen)
            {
                Point position = e.GetPosition(dataGrid);

                cardImagePopup.HorizontalOffset = position.X + 20;
                cardImagePopup.VerticalOffset = position.Y;
            }
            else { }
        }
        #endregion
    }
}