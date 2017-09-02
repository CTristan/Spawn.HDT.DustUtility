using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;

namespace Spawn.HDT.DustUtility.UI
{
    public partial class CardListWindow
    {
        #region DP
        #region DustAmount
        public int DustAmount
        {
            get { return (int)GetValue(DustAmountProperty); }
            set { SetValue(DustAmountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DustAmount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DustAmountProperty =
            DependencyProperty.Register("DustAmount", typeof(int), typeof(CardListWindow), new PropertyMetadata(0));
        #endregion

        #region CommonsCount
        public int CommonsCount
        {
            get { return (int)GetValue(CommonsCountProperty); }
            set { SetValue(CommonsCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommonsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommonsCountProperty =
            DependencyProperty.Register("CommonsCount", typeof(int), typeof(CardListWindow), new PropertyMetadata(0));
        #endregion

        #region RaresCount
        public int RaresCount
        {
            get { return (int)GetValue(RaresCountProperty); }
            set { SetValue(RaresCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RaresCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RaresCountProperty =
            DependencyProperty.Register("RaresCount", typeof(int), typeof(CardListWindow), new PropertyMetadata(0));
        #endregion

        #region EpicsCount
        public int EpicsCount
        {
            get { return (int)GetValue(EpicsCountProperty); }
            set { SetValue(EpicsCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EpicsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EpicsCountProperty =
            DependencyProperty.Register("EpicsCount", typeof(int), typeof(CardListWindow), new PropertyMetadata(0));
        #endregion

        #region LegendariesCount
        public int LegendariesCount
        {
            get { return (int)GetValue(LegendariesCountProperty); }
            set { SetValue(LegendariesCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendariesCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendariesCountProperty =
            DependencyProperty.Register("LegendariesCount", typeof(int), typeof(CardListWindow), new PropertyMetadata(0));
        #endregion

        #region TotalAmount
        public int TotalAmount
        {
            get { return (int)GetValue(TotalAmountProperty); }
            set { SetValue(TotalAmountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalAmount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalAmountProperty =
            DependencyProperty.Register("TotalAmount", typeof(int), typeof(CardListWindow), new PropertyMetadata(0));
        #endregion
        #endregion

        #region Ctor
        public CardListWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        #region OnCardsGridItemDropped
        private async void OnCardsGridItemDropped(object sender, GridItemEventArgs e)
        {
            GridItem item = e.Item;

            if (item.Count > 1)
            {
                string strResult = await this.ShowInputAsync(string.Empty, $"How many copies? ({item.Count} Max.)");

                int nNewCount = -1;

                try
                {
                    nNewCount = Convert.ToInt32(strResult);
                }
                catch
                {
                    //Invalid input
                }

                if (nNewCount > -1)
                {
                    item.Count = Math.Min(nNewCount, item.Count);
                    item.Dust = item.Tag.GetDustValue(item.Count);
                }
                else { }
            }
            else { }

            cardsGrid.GridItems.Add(item);

            TotalAmount += item.Count;
            DustAmount += item.Dust;

            switch (item.Rarity)
            {
                case HearthDb.Enums.Rarity.COMMON:
                    CommonsCount += 1;
                    break;
                case HearthDb.Enums.Rarity.RARE:
                    RaresCount += 1;
                    break;
                case HearthDb.Enums.Rarity.EPIC:
                    EpicsCount += 1;
                    break;
                case HearthDb.Enums.Rarity.LEGENDARY:
                    LegendariesCount += 1;
                    break;
            }
        }
        #endregion

        #region OnCardsGridRowDeleted
        private void OnCardsGridRowDeleted(object sender, GridItemEventArgs e)
        {
            TotalAmount -= e.Item.Count;
            DustAmount -= e.Item.Dust;

            switch (e.Item.Rarity)
            {
                case HearthDb.Enums.Rarity.COMMON:
                    CommonsCount -= 1;
                    break;
                case HearthDb.Enums.Rarity.RARE:
                    RaresCount -= 1;
                    break;
                case HearthDb.Enums.Rarity.EPIC:
                    EpicsCount -= 1;
                    break;
                case HearthDb.Enums.Rarity.LEGENDARY:
                    LegendariesCount -= 1;
                    break;
            }
        }
        #endregion
        #endregion
    }
}