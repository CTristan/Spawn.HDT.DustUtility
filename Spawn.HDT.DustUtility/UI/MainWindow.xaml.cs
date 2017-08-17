using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using Spawn.HDT.DustUtility.Search;
using Spawn.HDT.DustUtility.UI.Components;
using Spawn.HDT.DustUtility.UI.Dialogs;
using Spawn.HDT.DustUtility.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Spawn.HDT.DustUtility.UI
{
    public partial class MainWindow
    {
        #region Constants
        private const string SearchResultKey = "searchResult";
        #endregion

        #region Static
        private static string s_strSearchHelpText;

        static MainWindow()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Search terms:").Append(Environment.NewLine)
                .Append("- Dust amount (e.g. 500)").Append(Environment.NewLine)
                .Append("- Card name (e.g. Aya Blackpaw, Aya, Black)").Append(Environment.NewLine)
                .Append("- Card tribe (e.g. Dragon, Elemental, etc.)").Append(Environment.NewLine)
                .Append("- Card mechanics (e.g. Battlecry, Taunt, etc.)").Append(Environment.NewLine)
                .Append("- Card set (e.g. Un'goro, Gadgetzan, Goblins, etc.)").Append(Environment.NewLine)
                .Append("- Card type (e.g. Minion, Weapon, etc.)").Append(Environment.NewLine);

            s_strSearchHelpText = sb.ToString();
        }
        #endregion

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
            DependencyProperty.RegisterAttached("RowPopup", typeof(Popup), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        #region RowPopupCardWrapper
        private static CardImageContainer s_container;

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
            DependencyProperty.RegisterAttached("RowPopupCardWrapper", typeof(CardWrapper), typeof(MainWindow), new PropertyMetadata(null, new PropertyChangedCallback(SetRowPopupCardIdCallback)));

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
            DependencyProperty.RegisterAttached("ShowPopup", typeof(bool), typeof(MainWindow), new PropertyMetadata(false, new PropertyChangedCallback(ShowPopupCallback)));

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

        #region Member Variables
        private DustUtilityPlugin m_plugin;

        private CardCollector m_cardCollector;
        private Parameters m_parameters;
        #endregion

        #region Ctor
        public MainWindow()
        {
            InitializeComponent();

            s_container = cardImageContainer;
        }

        public MainWindow(DustUtilityPlugin plugin, Account account, bool offlineMode)
            : this()
        {
            m_plugin = plugin;

            m_cardCollector = new CardCollector(this, account, offlineMode);

            if (Settings.SearchParameters == null)
            {
                m_parameters = new Parameters(true);
            }
            else
            {
                m_parameters = Settings.SearchParameters.DeepClone();
            }

            if (!account.IsEmpty)
            {
                Title = $"{Title} [{account.BattleTag.Name} ({account.Region})]";
            }
            else { }

            if (offlineMode)
            {
                Title = $"{Title} [OFFLINE MODE]";
            }
            else { }

            Log.WriteLine($"Account={account.AccountString}", LogType.Debug);
            Log.WriteLine($"OfflineMode={offlineMode}", LogType.Debug);
        }
        #endregion

        #region Events
        #region OnWindowLoaded
        private async void OnWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Settings.CheckForUpdate && await GitHubUpdateManager.CheckForUpdateAsync())
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($"Update {GitHubUpdateManager.NewVersion.ToString(3)} has been released.").Append(Environment.NewLine + Environment.NewLine)
                    .Append("Release Notes:").Append(Environment.NewLine)
                    .Append(GitHubUpdateManager.ReleaseNotes).Append(Environment.NewLine + Environment.NewLine)
                    .Append("Would you like to download it?");

                MessageDialogResult result = await this.ShowMessageAsync("New update available", sb.ToString(), MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    System.Diagnostics.Process.Start(GitHubUpdateManager.LatestReleaseUrl);

                    Log.WriteLine("Opening github release page...", LogType.Debug);
                }
                else { }
            }
            else { }
        }
        #endregion

        #region OnGoClick
        private async void OnGoClick(object sender, System.Windows.RoutedEventArgs e)
        {
            await SearchAsync();
        }
        #endregion

        #region OnFiltersClick
        private void OnFiltersClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (m_cardCollector != null && m_parameters != null)
            {
                ParametersDialog dialog = new ParametersDialog(m_parameters.DeepClone())
                {
                    Owner = this
                };

                if (dialog.ShowDialog().Value)
                {
                    m_parameters = dialog.Parameters.DeepClone();

                    Settings.SearchParameters = dialog.Parameters.DeepClone();
                }
                else { }
            }
            else { }
        }
        #endregion

        #region OnTotalDustClick
        private async void OnTotalDustClick(object sender, System.Windows.RoutedEventArgs e)
        {
            await this.ShowMessageAsync(string.Empty, $"Your collection is worth: {m_cardCollector.GetTotalDustValueForAllCards()} Dust");
        }
        #endregion

        #region OnSearchHelpClick
        private async void OnSearchHelpClick(object sender, System.Windows.RoutedEventArgs e)
        {
            await this.ShowMessageAsync(string.Empty, s_strSearchHelpText);
        }
        #endregion

        #region OnSortOrderClick
        private void OnSortOrderClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SortOrderDialog dialog = new SortOrderDialog()
            {
                Owner = this
            };

            if (dialog.ShowDialog().Value)
            {
                dataGrid.ItemsSource = OrderItems(dataGrid.ItemsSource as IEnumerable<GridItem>);
            }
            else { }
        }
        #endregion

        #region OnInputBoxPreviewKeyDown
        private async void OnInputBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && searchButton.IsEnabled)
            {
                await SearchAsync();
            }
            else { }
        }
        #endregion

        #region OnInputBoxTextChanged
        private void OnInputBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            searchButton.IsEnabled = inputBox.Text.Length > 0;
        }
        #endregion

        #region OnInputBoxGotFocus
        private void OnInputBoxGotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
        #endregion

        #region OnSwitchAccountsClick
        private void OnSwitchAccountsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            m_plugin.SwitchAccounts();
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
        #endregion

        #region SearchAsync
        private async Task SearchAsync()
        {
            if (m_cardCollector != null && m_parameters != null)
            {
                await Task.Delay(1); //Return to ui thread

                UpdateUIState(false);

                m_parameters.QueryString = inputBox.Text;

                CardWrapper[] vCards = await m_cardCollector.GetCardsAsync(m_parameters);

                CreateSearchResult(vCards).CopyTo(GetSearchResultContainerComponent());

                UpdateUIState(true);
            }
            else { }
        }
        #endregion

        #region UpdateUIState
        private void UpdateUIState(bool blnIsEnabled)
        {
            Log.WriteLine($"Updating UI state: Enabled={blnIsEnabled}", LogType.Debug);

            if (blnIsEnabled)
            {
                searchButton.Content = "GO!";
            }
            else
            {
                searchButton.Content = "...";
            }

            searchButton.IsEnabled = blnIsEnabled;
            inputBox.IsEnabled = blnIsEnabled;
            filterButton.IsEnabled = blnIsEnabled;
            sortOrderButton.IsEnabled = blnIsEnabled;
        }
        #endregion

        #region CreateSearchResult
        private SearchResultContainer CreateSearchResult(CardWrapper[] vCards)
        {
            SearchResultContainer retVal = new SearchResultContainer();

            Log.WriteLine("Creating search result...", LogType.Debug);

            GridItem[] vItems = new GridItem[vCards.Length];

            for (int i = 0; i < vCards.Length; i++)
            {
                CardWrapper wrapper = vCards[i];

                switch (wrapper.DbCard.Rarity)
                {
                    case HearthDb.Enums.Rarity.COMMON:
                        retVal.CommonsCount += wrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.RARE:
                        retVal.RaresCount += wrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.EPIC:
                        retVal.EpicsCount += wrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.LEGENDARY:
                        retVal.LegendariesCount += wrapper.Count;
                        break;
                }

                GridItem item = new GridItem()
                {
                    Count = wrapper.Count,
                    Dust = wrapper.GetDustValue(),
                    Golden = wrapper.Card.Premium,
                    Name = wrapper.DbCard.Name,
                    Rarity = wrapper.DbCard.Rarity,
                    RarityString = wrapper.DbCard.Rarity.GetString(),
                    CardClass = wrapper.DbCard.Class.GetString(),
                    CardSet = wrapper.DbCard.Set.GetString(),
                    ManaCost = wrapper.DbCard.Cost,
                    Tag = wrapper
                };

                retVal.TotalCount += item.Count;
                retVal.Dust += item.Dust;

                vItems[i] = item;
            }

            //Sort
            vItems = OrderItems(vItems).ToArray();

            for (int i = 0; i < vItems.Length; i++)
            {
                retVal.GridItems.Add(vItems[i]);
            }

            return retVal;
        }
        #endregion

        #region OrderItems
        private IEnumerable<GridItem> OrderItems(IEnumerable<GridItem> items)
        {
            IEnumerable<GridItem> retVal;

            SortOrder sortOrder = SortOrder.Parse(Settings.SortOrder);

            if (sortOrder != null && sortOrder.Items.Count > 0)
            {
                IQueryable<GridItem> query = items.AsQueryable();

                for (int i = 0; i < sortOrder.Items.Count; i++)
                {
                    query = query.OrderBy(sortOrder.Items[i].Value.ToString(), i);
                }

                retVal = query.ToList();
            }
            else
            {
                //lstRet = list.OrderBy(item => item.Rarity).ThenBy(item => item.Golden).ThenBy(item => item.Dust).ThenBy(item => item.CardClass).ThenBy(item => item.CardSet).ThenBy(item => item.Name).ToList();
                retVal = items;
            }

            return retVal;
        }
        #endregion

        #region GetSearchResultContainerComponent
        public SearchResultContainer GetSearchResultContainerComponent()
        {
            return FindResource(SearchResultKey) as SearchResultContainer;
        }
        #endregion
    }
}