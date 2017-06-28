using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Spawn.HDT.DustUtility
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
        #region OnGoClick
        /// <summary>
        /// Handles the Click event of the GO! button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnGoClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(inputBox.Text) && m_cardCollector != null && m_parameters != null)
            {
                searchButton.IsEnabled = false;
                searchButton.Content = "...";
                inputBox.IsEnabled = false;
                filterButton.IsEnabled = false;

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
                    List<GridItem> lstItems = ConvertAndSort(t.Result);

                    Dispatcher.Invoke(() =>
                    {
                        dataGrid.ItemsSource = lstItems;

                        searchButton.IsEnabled = true;
                        searchButton.Content = "GO!";
                        inputBox.IsEnabled = true;
                        filterButton.IsEnabled = true;
                    });
                });
            }
            else { }
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
                ParametersWindow rarityWindow = new ParametersWindow(m_parameters);

                rarityWindow.ShowDialog();

                m_parameters = rarityWindow.Parameters;
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
        private void OnTotalDustClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Your collection is worth: {m_cardCollector.GetTotalDustValueForAllCards()} Dust", "Dust Utitlity", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region OnAutoGeneratingColumn
        /// <summary>
        /// Called when a column is automatically generated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs"/> instance containing the event data.</param>
        private void OnAutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().ToLower().Equals("cardclass"))
            {
                e.Column.Header = "Class";
            }
            else if (e.Column.Header.ToString().ToLower().Equals("cardset"))
            {
                e.Cancel = true;
            }
            else { }
        }
        #endregion
        #endregion

        #region ConvertAndSort
        /// <summary>
        /// Converts the specified cards and sorts them.
        /// </summary>
        /// <param name="vCards">The cards.</param>
        /// <returns></returns>
        private List<GridItem> ConvertAndSort(CardWrapper[] vCards)
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
                    Count = $"{cardWrapper.Count}x",
                    Dust = cardWrapper.GetDustValue(),
                    Golden = cardWrapper.Card.Premium,
                    Name = cardWrapper.DbCard.Name,
                    Rarity = cardWrapper.DbCard.Rarity,
                    CardClass = cardWrapper.DbCard.Class,
                    CardSet = cardWrapper.DbCard.Set
                };

                switch (item.Rarity)
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
            lstRet = lstRet.OrderBy(item => item.Rarity).ThenBy(item => item.Golden).ThenBy(item => item.Dust).ThenBy(item => item.CardClass).ThenBy(item => item.CardSet).ThenBy(item => item.Name).ToList();

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
    }
}
