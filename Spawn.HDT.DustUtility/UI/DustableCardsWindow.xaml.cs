using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

            if (Settings.SearchParameters == null)
            {
                m_parameters = new Parameters(true); 
            }
            else
            {
                m_parameters = Settings.SearchParameters.DeepClone();
            }

            if (offlineMode)
            {
                Title = $"{Title} [OFFLINE MODE]";
            }
            else { }
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

        #region OnAutoGeneratingColumn
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string strHeader = e.Column.Header.ToString().ToLowerInvariant();

            if (strHeader.Equals("cardclass"))
            {
                e.Column.Header = "Class";
            }
            else if (strHeader.Equals("cardset"))
            {
                e.Column.Header = "Set";
            }
            else if (strHeader.Equals("count"))
            {
                ((e.Column as DataGridTextColumn).Binding as Binding).Converter = new CountLabelConverter();
            }
            else { }

            e.Cancel = strHeader.Equals("tag") || strHeader.Equals("manacost");
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
        private void OnInputBoxGotFocus(object sender, System.Windows.RoutedEventArgs e)
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
                    CardSet = wrapper.DbCard.Set.GetString(),
                    ManaCost = wrapper.DbCard.Cost,
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
                IQueryable<GridItem> query = items.AsQueryable();
                
                for (int i = 0; i < sortOrder.Items.Count; i++)
                {
                    query = query.OrderBy(sortOrder.Items[i].Value.ToString());
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

        #region GetSearchResultComponent
        public SearchResultContainer GetSearchResultComponent()
        {
            return FindResource(SearchResultKey) as SearchResultContainer;
        }
        #endregion
    }
}
