using Hearthstone_Deck_Tracker.Hearthstone;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Regex m_numericRegex;

        public MainWindow()
        {
            InitializeComponent();

            m_numericRegex = new Regex("[^0-9]+");
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dataGrid.ItemsSource = null;

            HearthDb.Card[] vCards = new HearthDb.Card[HearthDb.Cards.All.Count];

            HearthDb.Cards.All.Values.CopyTo(vCards, 0);

            List<GridItem> lstItems = new List<GridItem>();

            for (int i = 0; i < 25; i++)
            {
                HearthDb.Card dbCard = vCards[i];

                Card card = new Card(dbCard);

                GridItem item = new GridItem()
                {
                    Count = "1x",
                    DisenchantmentValue = 400,
                    Golden = false,
                    Name = dbCard.Name
                };

                lstItems.Add(item);
            }

            dataGrid.ItemsSource = lstItems;
        }

        private void ValidateInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = m_numericRegex.IsMatch(e.Text);
        }
    }
}
