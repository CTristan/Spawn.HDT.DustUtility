using System.Windows;

namespace Spawn.HDT.DustUtility.UI.Components
{
    public partial class CardTooltip
    {
        #region CardImageSource DP
        public string CardImageSource
        {
            get { return (string)GetValue(CardImageSourceProperty); }
            set { SetValue(CardImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardImageSourceProperty =
            DependencyProperty.Register("CardImageSource", typeof(string), typeof(CardTooltip), new PropertyMetadata("/Spawn.HDT.DustUtility;component/Resources/legend_cardback.png"));
        #endregion

        #region Ctor
        public CardTooltip()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }
        #endregion
    }
}