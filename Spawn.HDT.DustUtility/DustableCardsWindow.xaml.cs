using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Spawn.HDT.DustUtility
{
    public partial class DustableCardsWindow
    {
        private CardCollector m_cardCollector;
        private Regex m_numericRegex;
        private Parameters m_parameters;

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

        /// <summary>
        /// Handles the Click event of the GO! button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(inputBox.Text) && m_cardCollector != null && m_parameters != null)
            {
                searchButton.IsEnabled = false;
                searchButton.Content = "...";
                inputBox.IsEnabled = false;
                filterButton.IsEnabled = false;

                try
                {
                    m_parameters.DustAmount = System.Convert.ToInt32(inputBox.Text);
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
                    GridItem[] vItems = Convert(t.Result);

                    Dispatcher.Invoke(() =>
                    {
                        dataGrid.ItemsSource = vItems;

                        searchButton.IsEnabled = true;
                        searchButton.Content = "GO!";
                        inputBox.IsEnabled = true;
                        filterButton.IsEnabled = true;
                    });
                });
            }
            else { }
        }

        /// <summary>
        /// Converts the specified cards.
        /// </summary>
        /// <param name="vCards">The cards.</param>
        /// <returns></returns>
        private GridItem[] Convert(CardWrapper[] vCards)
        {
            GridItem[] vRet = new GridItem[vCards.Length];

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
                    Count = $"{cardWrapper.Card.Count}x",
                    Dust = cardWrapper.GetDustValue(),
                    Golden = cardWrapper.Card.Premium,
                    Name = cardWrapper.GetDBCard().Name,
                    Rarity = cardWrapper.GetDBCard().Rarity,
                    CardClass = cardWrapper.GetDBCard().Class
                };

                switch (item.Rarity)
                {
                    case HearthDb.Enums.Rarity.COMMON:
                        nCommons += cardWrapper.Card.Count;
                        break;
                    case HearthDb.Enums.Rarity.RARE:
                        nRares += cardWrapper.Card.Count;
                        break;
                    case HearthDb.Enums.Rarity.EPIC:
                        nEpics += cardWrapper.Card.Count;
                        break;
                    case HearthDb.Enums.Rarity.LEGENDARY:
                        nLegendaries += cardWrapper.Card.Count;
                        break;
                }

                nTotalDustAmount += item.Dust;
                nTotalCount += cardWrapper.Card.Count;

                vRet[i] = item;
            }

            Dispatcher.Invoke(() =>
            {
                lblCards.Content = $"Total: {nTotalCount}";
                lblDust.Content = $"Dust: {nTotalDustAmount}";
                lblCommons.Content = $"Commons: {nCommons}";
                lblRares.Content = $"Rares: {nRares}";
                lblEpics.Content = $"Epics: {nEpics}";
                lblLegendaries.Content = $"Legendaries: {nLegendaries}";
            });

            return vRet;
        }

        /// <summary>
        /// Validates the input.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
        private void ValidateInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = m_numericRegex.IsMatch(e.Text);
        }

        /// <summary>
        /// Handles the event of the filter button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (m_cardCollector != null && m_parameters != null)
            {
                ParametersWindow rarityWindow = new ParametersWindow(m_parameters);

                rarityWindow.ShowDialog();

                m_parameters = rarityWindow.Parameters;
            }
            else { }
        }

        /// <summary>
        /// Handles the event of the dust button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Your whole collection is worth: {m_cardCollector.GetTotalDustValueForAllCards()} Dust", "Dust Utitlity", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dataGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().ToLower().Equals("cardclass"))
            {
                e.Column.Header = "Class";
            }
            else { }
        }
    }
}
