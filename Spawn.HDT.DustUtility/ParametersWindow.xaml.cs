using HearthDb.Enums;
using System;
using System.Windows;

namespace Spawn.HDT.DustUtility
{
    /// <summary>
    /// Interaktionslogik für ParametersWindow.xaml
    /// </summary>
    public partial class ParametersWindow
    {
        public Parameters Parameters { get; set; }

        public ParametersWindow()
        {
            InitializeComponent();
        }

        public ParametersWindow(Parameters parameters)
            : this()
        {
            Parameters = parameters;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Parameters != null)
            {
                Parameters.Rarities.Clear();
                Parameters.Classes.Clear();

                if (cbCommon.IsChecked.Value)
                {
                    Parameters.Rarities.Add(Rarity.COMMON);
                }
                else { }

                if (cbRare.IsChecked.Value)
                {
                    Parameters.Rarities.Add(Rarity.RARE);
                }
                else { }

                if (cbEpic.IsChecked.Value)
                {
                    Parameters.Rarities.Add(Rarity.EPIC);
                }
                else { }

                if (cbLegendary.IsChecked.Value)
                {
                    Parameters.Rarities.Add(Rarity.LEGENDARY);
                }
                else { }

                if (cbDruid.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.DRUID);
                }
                else { }

                if (cbHunter.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.HUNTER);
                }
                else { }

                if (cbMage.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.MAGE);
                }
                else { }

                if (cbPaladin.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.PALADIN);
                }
                else { }

                if (cbPriest.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.PRIEST);
                }
                else { }

                if (cbRogue.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.ROGUE);
                }
                else { }

                if (cbShaman.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.SHAMAN);
                }
                else { }

                if (cbWarlock.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.WARLOCK);
                }
                else { }

                if (cbWarrior.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.WARRIOR);
                }
                else { }

                if (cbNeutral.IsChecked.Value)
                {
                    Parameters.Classes.Add(CardClass.NEUTRAL);
                }
                else { }

                Parameters.IncludeGoldenCards = cbGolden.IsChecked.Value; 
            }
            else { }

            Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Parameters != null)
            {
                cbGolden.IsChecked = Parameters.IncludeGoldenCards;

                for (int i = 0; i < Parameters.Rarities.Count; i++)
                {
                    switch (Parameters.Rarities[i])
                    {
                        case Rarity.COMMON:
                            cbCommon.IsChecked = true;
                            break;
                        case Rarity.RARE:
                            cbRare.IsChecked = true;
                            break;
                        case Rarity.EPIC:
                            cbEpic.IsChecked = true;
                            break;
                        case Rarity.LEGENDARY:
                            cbLegendary.IsChecked = true;
                            break;
                    }
                }

                for (int i = 0; i < Parameters.Classes.Count; i++)
                {
                    switch (Parameters.Classes[i])
                    {
                        case CardClass.DRUID:
                            cbDruid.IsChecked = true;
                            break;
                        case CardClass.HUNTER:
                            cbHunter.IsChecked = true;
                            break;
                        case CardClass.MAGE:
                            cbMage.IsChecked = true;
                            break;
                        case CardClass.PALADIN:
                            cbPaladin.IsChecked = true;
                            break;
                        case CardClass.PRIEST:
                            cbPriest.IsChecked = true;
                            break;
                        case CardClass.ROGUE:
                            cbRogue.IsChecked = true;
                            break;
                        case CardClass.SHAMAN:
                            cbShaman.IsChecked = true;
                            break;
                        case CardClass.WARLOCK:
                            cbWarlock.IsChecked = true;
                            break;
                        case CardClass.WARRIOR:
                            cbWarrior.IsChecked = true;
                            break;
                        case CardClass.NEUTRAL:
                            cbNeutral.IsChecked = true;
                            break;
                    }
                }
            }
            else { }
        }
    }
}
