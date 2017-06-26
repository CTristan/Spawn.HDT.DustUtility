using System;
using System.Windows;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility
{
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

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Parameters != null)
            {
                cbGolden.IsChecked = Parameters.IncludeGoldenCards;

                LoadRarities();

                LoadClasses();

                LoadClassSets();
            }
            else { }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (Parameters != null)
            {
                Parameters.IncludeGoldenCards = cbGolden.IsChecked.Value;

                SetRarities();

                SetClasses();

                SetCardSets();
            }
            else { }

            Close();
        }

        private void SetRarities()
        {
            Parameters.Rarities.Clear();

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
        }

        private void SetClasses()
        {
            Parameters.Classes.Clear();

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
        }

        private void SetCardSets()
        {
            Parameters.Sets.Clear();

            if (cbExpert.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.EXPERT1);
            }
            else { }

            if (cbGoblins.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.GVG);
            }
            else { }

            if (cbTournament.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.TGT);
            }
            else { }

            if (cbOldGods.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.OG);
            }
            else { }

            if (cbGadgetzan.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.GANGS);
            }
            else { }

            if (cbUngoro.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.UNGORO);
            }
            else { }

            if (cbNaxx.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.NAXX);
            }
            else { }

            if (cbMountain.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.BRM);
            }
            else { }

            if (cbLeague.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.LOE);
            }
            else { }

            if (cbKarazhan.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.KARA);
            }
            else { }

            if (cbHall.IsChecked.Value)
            {
                Parameters.Sets.Add(CardSet.PROMO);
                Parameters.Sets.Add(CardSet.HOF);
            }
            else { }
        }

        private void LoadRarities()
        {
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
        }

        private void LoadClasses()
        {
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

        private void LoadClassSets()
        {
            for (int i = 0; i < Parameters.Sets.Count; i++)
            {
                switch (Parameters.Sets[i])
                {
                    case CardSet.EXPERT1:
                        cbExpert.IsChecked = true;
                        break;
                    case CardSet.GVG:
                        cbGoblins.IsChecked = true;
                        break;
                    case CardSet.TGT:
                        cbTournament.IsChecked = true;
                        break;
                    case CardSet.OG:
                        cbOldGods.IsChecked = true;
                        break;
                    case CardSet.GANGS:
                        cbGadgetzan.IsChecked = true;
                        break;
                    case CardSet.UNGORO:
                        cbUngoro.IsChecked = true;
                        break;
                    case CardSet.NAXX:
                        cbNaxx.IsChecked = true;
                        break;
                    case CardSet.BRM:
                        cbMountain.IsChecked = true;
                        break;
                    case CardSet.LOE:
                        cbLeague.IsChecked = true;
                        break;
                    case CardSet.KARA:
                        cbKarazhan.IsChecked = true;
                        break;
                    case CardSet.HOF:
                    case CardSet.PROMO:
                        cbHall.IsChecked = true;
                        break;
                }
            }
        }
    }
}
