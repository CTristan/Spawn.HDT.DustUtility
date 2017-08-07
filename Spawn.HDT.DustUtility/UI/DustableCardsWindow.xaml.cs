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
using Spawn.HDT.DustUtility.Converter;
using Spawn.HDT.DustUtility.UI.Dialogs;
using Spawn.HDT.DustUtility.Update;

namespace Spawn.HDT.DustUtility.UI
{
    public partial class DustableCardsWindow
    {
        #region Member Variables
        private CardCollector m_cardCollector;
        private Regex m_numericRegex;
        private Parameters m_parameters;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DustableCardsWindow"/> class.
        /// </summary>
        public DustableCardsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DustableCardsWindow"/> class.
        /// </summary>
        /// <param name="dustManager">The dust manager.</param>
        public DustableCardsWindow(CardCollector cardCollector)
            : this()
        {
            m_cardCollector = cardCollector;
            m_numericRegex = new Regex("[^0-9]+");
            m_parameters = new Parameters();
        }
        #endregion

        #region Events
        #region Window_Loaded
        private async void Window_Loaded(object sender, RoutedEventArgs e)
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
        /// <summary>
        /// Handles the Click event of the GO! button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnGoClick(object sender, RoutedEventArgs e)
        {
            StartSearch();
        }
        #endregion

        #region OnFiltersClick
        /// <summary>
        /// Handles the event of the filters button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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
        /// <summary>
        /// Handles the event of the dust button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void OnTotalDustClick(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync(string.Empty, $"Your collection is worth: {m_cardCollector.GetTotalDustValueForAllCards()} Dust");
        }
        #endregion

        #region OnAutoGeneratingColumn
        /// <summary>
        /// Called when a column is automatically generated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataGridAutoGeneratingColumnEventArgs"/> instance containing the event data.</param>
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

        #region ValidateInput
        /// <summary>
        /// Validates the input.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
        private void ValidateInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = m_numericRegex.IsMatch(e.Text);
        }
        #endregion

        #region OnInputBoxPreviewKeyDown
        private void OnInputBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSearch();
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

        #region StartSearch
        private void StartSearch()
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

                //CardWrapper[] vCards = m_cardCollector.GetDustableCards(m_parameters);

                //dataGrid.ItemsSource = Convert(vCards);

                Task.Run(() => m_cardCollector.GetDustableCards(m_parameters)).ContinueWith(t =>
                {
                    List<GridItem> lstItems = CreateGridItems(t.Result);

                    Dispatcher.Invoke(() =>
                    {
                        dataGrid.ItemsSource = lstItems;

                        searchButton.IsEnabled = true;
                        searchButton.Content = "GO!";
                        inputBox.IsEnabled = true;
                        filterButton.IsEnabled = true;
                        sortOrderButton.IsEnabled = true;
                    });
                });
            }
            else { }
        } 
        #endregion

        #region CreateGridItems
        /// <summary>
        /// Converts the specified cards into grid items and sorts them.
        /// </summary>
        /// <param name="vCards">The cards.</param>
        /// <returns></returns>
        private List<GridItem> CreateGridItems(CardWrapper[] vCards)
        {
            List<GridItem> lstRet = new List<GridItem>(vCards.Length);

            //Convert

            int nTotalDustAmount = 0;
            int nTotalCount = 0;

            int nCommons = 0;
            int nRares = 0;
            int nEpics = 0;
            int nLegendaries = 0;

            for (int i = 0; i < vCards.Length; i++)
            {
                CardWrapper cardWrapper = vCards[i];

                GridItem item = new GridItem()
                {
                    Count = cardWrapper.Count,
                    Dust = cardWrapper.GetDustValue(),
                    Golden = cardWrapper.Card.Premium,
                    Name = cardWrapper.DbCard.Name,
                    Rarity = cardWrapper.DbCard.Rarity.GetString(),
                    CardClass = cardWrapper.DbCard.Class.GetString(),
                    Set = cardWrapper.DbCard.Set.GetString(),
                    Tag = cardWrapper
                };

                switch (cardWrapper.DbCard.Rarity)
                {
                    case HearthDb.Enums.Rarity.COMMON:
                        nCommons += cardWrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.RARE:
                        nRares += cardWrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.EPIC:
                        nEpics += cardWrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.LEGENDARY:
                        nLegendaries += cardWrapper.Count;
                        break;
                }

                nTotalDustAmount += item.Dust;
                nTotalCount += cardWrapper.Count;

                lstRet.Add(item);
            }

            //Sort
            lstRet = OrderItems(lstRet).ToList();

            Dispatcher.Invoke(() =>
            {
                lblCards.Content = $"Total: {nTotalCount}";
                lblDust.Content = $"Dust: {nTotalDustAmount}";
                lblCommons.Content = $"Commons: {nCommons}";
                lblRares.Content = $"Rares: {nRares}";
                lblEpics.Content = $"Epics: {nEpics}";
                lblLegendaries.Content = $"Legendaries: {nLegendaries}";
            });

            return lstRet;
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
    }
}
