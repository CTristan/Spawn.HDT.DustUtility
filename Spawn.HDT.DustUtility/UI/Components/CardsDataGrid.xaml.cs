using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Search;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Spawn.HDT.DustUtility.UI.Components
{
    public partial class CardsDataGrid
    {
        #region Attached DP
        #region RowPopup
        public static Popup GetRowPopup(DependencyObject obj)
        {
            return (Popup)obj.GetValue(RowPopupProperty);
        }

        public static void SetRowPopup(DependencyObject obj, Popup value)
        {
            obj.SetValue(RowPopupProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowPopup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowPopupProperty =
            DependencyProperty.RegisterAttached("RowPopup", typeof(Popup), typeof(CardsDataGrid), new PropertyMetadata(null));
        #endregion

        #region RowPopupCardWrapper
        public static string GetRowPopupCardWrapper(DependencyObject obj)
        {
            return (string)obj.GetValue(RowPopupCardWrapperProperty);
        }

        public static void SetRowPopupCardWrapper(DependencyObject obj, string value)
        {
            obj.SetValue(RowPopupCardWrapperProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowPopupCardWrapper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowPopupCardWrapperProperty =
            DependencyProperty.RegisterAttached("RowPopupCardWrapper", typeof(CardWrapper), typeof(CardsDataGrid), new PropertyMetadata(null, new PropertyChangedCallback(SetRowPopupCardIdCallback)));

        private static void SetRowPopupCardIdCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (s_container != null)
            {
                s_container.CardWrapper = e.NewValue as CardWrapper;

                if (e.NewValue != null)
                {
                    Log.WriteLine($"Setting new card id for popup: Id={s_container.CardWrapper.Card.Id}", LogType.Debug);
                }
                else { }
            }
            else { }
        }
        #endregion

        #region ShowPopup
        public static bool GetShowPopup(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowPopupProperty);
        }

        public static void SetShowPopup(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowPopupProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowPopup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPopupProperty =
            DependencyProperty.RegisterAttached("ShowPopup", typeof(bool), typeof(CardsDataGrid), new PropertyMetadata(false, new PropertyChangedCallback(ShowPopupCallback)));

        private static void ShowPopupCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.CardImageTooltip && d is DataGridRow)
            {
                if (((DataGridRow)d).IsFocused == true)
                {
                    Popup p = GetRowPopup(d);
                    p.IsOpen = Convert.ToBoolean(e.NewValue);
                }
                else
                {
                    Popup p = GetRowPopup(d);
                    p.IsOpen = Convert.ToBoolean(e.NewValue);
                }
            }
            else { }
        }
        #endregion
        #endregion

        #region SearchResultContainer DP
        public ObservableCollection<GridItem> GridItems
        {
            get { return GetValue(GridItemsProperty) as ObservableCollection<GridItem>; }
            set { SetValue(GridItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridItemsProperty =
            DependencyProperty.Register("GridItems", typeof(ObservableCollection<GridItem>), typeof(CardsDataGrid), new PropertyMetadata(new ObservableCollection<GridItem>()));
        #endregion

        #region Static Variables
        private static CardImageContainer s_container;
        #endregion

        #region Ctor
        public CardsDataGrid()
        {
            InitializeComponent();

            s_container = cardImageContainer;
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