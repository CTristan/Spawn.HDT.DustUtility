using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Search;
using Spawn.HDT.DustUtility.UI.Components;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Spawn.HDT.DustUtility.UI
{
    public static class AttachedProperties
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
            DependencyProperty.RegisterAttached("RowPopup", typeof(Popup), typeof(AttachedProperties), new PropertyMetadata(null));
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
            DependencyProperty.RegisterAttached("RowPopupCardWrapper", typeof(CardWrapper), typeof(AttachedProperties), new PropertyMetadata(null, new PropertyChangedCallback(SetRowPopupCardIdCallback)));

        private static void SetRowPopupCardIdCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridRow && (e.NewValue != null && e.NewValue is CardWrapper))
            {
                CardWrapper cardWrapper = e.NewValue as CardWrapper;

                Log.WriteLine($"Setting new card id for popup: Id={cardWrapper.Card.Id} Premium={cardWrapper.Card.Premium}", LogType.Debug);

                Popup popup = GetRowPopup(d);

                CardImageContainer container = popup.Child as CardImageContainer;

                container.CardWrapper = cardWrapper;
            }
            else { }

            //if (s_container != null)
            //{
            //    s_container.CardWrapper = e.NewValue as CardWrapper;

            //    if (e.NewValue != null)
            //    {
            //        Log.WriteLine($"Setting new card id for popup: Id={s_container.CardWrapper.Card.Id}", LogType.Debug);
            //    }
            //    else { }
            //}
            //else { }
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
            DependencyProperty.RegisterAttached("ShowPopup", typeof(bool), typeof(AttachedProperties), new PropertyMetadata(false, new PropertyChangedCallback(ShowPopupCallback)));

        private static void ShowPopupCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.CardImageTooltip && d is DataGridRow)
            {
                Popup p = GetRowPopup(d);
                p.IsOpen = Convert.ToBoolean(e.NewValue);
            }
            else { }
        }
        #endregion
        #endregion
    }
}