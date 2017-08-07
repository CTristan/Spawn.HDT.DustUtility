using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Spawn.HDT.DustUtility.Converters;
using Spawn.HDT.DustUtility.Search;
using Spawn.HDT.DustUtility.UI.Dialogs;
using Spawn.HDT.DustUtility.Update;

namespace Spawn.HDT.DustUtility.UI
{
    public partial class DustableCardsWindow
    {
        #region Constants
        private const string SearchResultKey = "searchResult";
        #endregion

        #region Member Variables
        private Regex m_numericRegex;

        private CardCollector m_cardCollector;
        private Parameters m_parameters;
        #endregion

        #region Ctor
        public DustableCardsWindow()
        {
            InitializeComponent();
        }

        public DustableCardsWindow(bool offlineMode)
            : this()
        {
            m_numericRegex = new Regex("[^0-9]+");

            m_cardCollector = new CardCollector(this, offlineMode);
            
            m_parameters = new Parameters();

            if (offlineMode)
            {
                Title = $"{Title} [OFFLINE MODE]";
            }
            else { }
        }
        #endregion

        #region Events
        #region OnWindowLoaded
        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Settings.CheckForUpdate && await GitHubUpdateManager.CheckForUpdateAsync())
            {
                MessageDialogResult result = await this.ShowMessageAsync("New update available", $"Update {GitHubUpdateManager.NewVersion.ToString(2)} has been released.\r\n\r\nDo you want to download it?", MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    System.Diagnostics.Process.Start(GitHubUpdateManager.LatestReleaseUrl);
                }
                else { }
            }
            else { }
        }
        #endregion

        #region OnGoClick
        private async void OnGoClick(object sender, RoutedEventArgs e)
        {
            await SearchAsync();
        }
        #endregion

        #region OnFiltersClick
        private void OnFiltersClick(object sender, RoutedEventArgs e)
        {
            if (m_cardCollector != null && m_parameters != null)
            {
                ParametersDialog dialog = new ParametersDialog(m_parameters)
                {
                    Owner = this
                };

                dialog.ShowDialog();

                m_parameters = dialog.Parameters;
            }
            else { }
        }
        #endregion

        #region OnTotalDustClick
        private async void OnTotalDustClick(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync(string.Empty, $"Your collection is worth: {m_cardCollector.GetTotalDustValueForAllCards()} Dust");
        }
        #endregion

        #region OnAutoGeneratingColumn
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().ToLowerInvariant().Equals("cardclass"))
            {
                e.Column.Header = "Class";
            }
            else if (e.Column.Header.ToString().ToLowerInvariant().Equals("count"))
            {
                ((e.Column as DataGridTextColumn).Binding as Binding).Converter = new CountLabelConverter();
            }
            else { }

            e.Cancel = e.Column.Header.ToString().ToLowerInvariant().Equals("cardset") || e.Column.Header.ToString().ToLowerInvariant().Equals("tag");
        }
        #endregion

        #region OnSortOrderClick
        private void OnSortOrderClick(object sender, RoutedEventArgs e)
        {
            SortOrderDialog dialog = new SortOrderDialog()
            {
                Owner = this
            };

            if (dialog.ShowDialog().Value)
            {
                IEnumerable<GridItem> items = dataGrid.ItemsSource as IEnumerable<GridItem>;

                dataGrid.ItemsSource = OrderItems(items);
            }
            else { }
        }
        #endregion

        #region OnPreviewTextInput
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = m_numericRegex.IsMatch(e.Text);
        }
        #endregion

        #region OnInputBoxPreviewKeyDown
        private async void OnInputBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
        private void OnInputBoxGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
        #endregion
        #endregion

        #region SearchAsync
        private async Task SearchAsync()
        {
            if (!string.IsNullOrEmpty(inputBox.Text) && m_cardCollector != null && m_parameters != null)
            {
                searchButton.IsEnabled = false;
                searchButton.Content = "...";
                inputBox.IsEnabled = false;
                filterButton.IsEnabled = false;
                sortOrderButton.IsEnabled = false;

                try
                {
                    m_parameters.DustAmount = Convert.ToInt32(inputBox.Text);
                }
                catch
                {
                    m_parameters.DustAmount = Int32.MaxValue;
                    inputBox.Text = m_parameters.DustAmount.ToString();
                }

                await Task.Delay(1); //Return to ui thread

                CardWrapper[] vCards = await m_cardCollector.GetDustableCardsAsync(m_parameters);

                GetResult(vCards).CopyTo(GetSearchResultComponent());

                searchButton.IsEnabled = true;
                searchButton.Content = "GO!";
                inputBox.IsEnabled = true;
                filterButton.IsEnabled = true;
                sortOrderButton.IsEnabled = true;

                //dataGrid.ItemsSource = Convert(vCards);

                //await Task.Run(() => m_cardCollector.GetDustableCards(m_parameters)).ContinueWith(t =>
                //{
                //    GetResult(t.Result).CopyTo(GetSearchResultComponent());

                //    Dispatcher.InvokeAsync(() =>
                //    {
                //        //TODO
                //        //dataGrid.ItemsSource = lstItems;

                //        searchButton.IsEnabled = true;
                //        searchButton.Content = "GO!";
                //        inputBox.IsEnabled = true;
                //        filterButton.IsEnabled = true;
                //        sortOrderButton.IsEnabled = true;
                //    });
                //});
            }
            else { }
        }
        #endregion

        #region GetResult
        private SearchResultContainer GetResult(CardWrapper[] vCards)
        {
            SearchResultContainer retVal = new SearchResultContainer();

            List<GridItem> lstItems = new List<GridItem>(vCards.Length);

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
                    Rarity = wrapper.DbCard.Rarity.GetString(),
                    CardClass = wrapper.DbCard.Class.GetString(),
                    Set = wrapper.DbCard.Set.GetString(),
                    Tag = wrapper
                };

                retVal.TotalCount += item.Count;
                retVal.Dust += item.Dust;

                lstItems.Add(item);
            }

            //Sort
            lstItems = OrderItems(lstItems).ToList();

            for (int i = 0; i < lstItems.Count; i++)
            {
                retVal.GridItems.Add(lstItems[i]);
            }

            //Dispatcher.Invoke(() =>
            //{
            //    lblCards.Content = $"Total: {nTotalCount}";
            //    lblDust.Content = $"Dust: {nTotalDustAmount}";
            //    lblCommons.Content = $"Commons: {nCommons}";
            //    lblRares.Content = $"Rares: {nRares}";
            //    lblEpics.Content = $"Epics: {nEpics}";
            //    lblLegendaries.Content = $"Legendaries: {nLegendaries}";
            //});
            
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
                IOrderedEnumerable<GridItem> orderedList = null;

                switch (sortOrder.Items[0])
                {
                    case SortOrder.Item.Count:
                        orderedList = items.OrderBy(c => c.Count);
                        break;
                    case SortOrder.Item.Name:
                        orderedList = items.OrderBy(c => c.Name);
                        break;
                    case SortOrder.Item.Golden:
                        orderedList = items.OrderBy(c => c.Golden);
                        break;
                    case SortOrder.Item.Dust:
                        orderedList = items.OrderBy(c => c.Dust);
                        break;
                    case SortOrder.Item.Rarity:
                        orderedList = items.OrderBy(c => c.Rarity);
                        break;
                    case SortOrder.Item.Class:
                        orderedList = items.OrderBy(c => c.CardClass);
                        break;
                    case SortOrder.Item.CardSet:
                        orderedList = items.OrderBy(c => c.Set);
                        break;
                    case SortOrder.Item.Cost:
                        orderedList = items.OrderBy(c => c.Tag.DbCard.Cost);
                        break;
                }

                for (int i = 1; i < sortOrder.Items.Count; i++)
                {
                    switch (sortOrder.Items[i])
                    {
                        case SortOrder.Item.Count:
                            orderedList = orderedList.ThenBy(c => c.Count);
                            break;
                        case SortOrder.Item.Name:
                            orderedList = orderedList.ThenBy(c => c.Name);
                            break;
                        case SortOrder.Item.Golden:
                            orderedList = orderedList.ThenBy(c => c.Golden);
                            break;
                        case SortOrder.Item.Dust:
                            orderedList = orderedList.ThenBy(c => c.Dust);
                            break;
                        case SortOrder.Item.Rarity:
                            orderedList = orderedList.ThenBy(c => c.Rarity);
                            break;
                        case SortOrder.Item.Class:
                            orderedList = orderedList.ThenBy(c => c.CardClass);
                            break;
                        case SortOrder.Item.CardSet:
                            orderedList = orderedList.ThenBy(c => c.Set);
                            break;
                        case SortOrder.Item.Cost:
                            orderedList = orderedList.ThenBy(c => c.Tag.DbCard.Cost);
                            break;
                    }
                }

                retVal = orderedList;
            }
            else
            {
                //lstRet = list.OrderBy(item => item.Rarity).ThenBy(item => item.Golden).ThenBy(item => item.Dust).ThenBy(item => item.CardClass).ThenBy(item => item.CardSet).ThenBy(item => item.Name).ToList();
                retVal = items;
            }

            return retVal;
        }
        #endregion

        #region GetSearchResultComponent
        public SearchResultContainer GetSearchResultComponent()
        {
            return FindResource(SearchResultKey) as SearchResultContainer;
        }
        #endregion
    }
}
