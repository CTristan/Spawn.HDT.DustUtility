using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Net;
using Spawn.HDT.DustUtility.Search;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Spawn.HDT.DustUtility.UI.Components
{
    public partial class CardImageContainer
    {
        #region CardWrapper DP
        public CardWrapper CardWrapper
        {
            get { return GetValue(CardWrapperProperty) as CardWrapper; }
            set
            {
                SetValue(CardWrapperProperty, value);
                FireCardWrapperChangedEvent();
            }
        }

        // Using a DependencyProperty as the backing store for CardWrapper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardWrapperProperty =
            DependencyProperty.Register("CardWrapper", typeof(CardWrapper), typeof(CardImageContainer), new PropertyMetadata(null));
        #endregion

        #region Member Variables
        private ImageSource m_defaultImageSource;
        private Stream m_currentImageStream;
        #endregion

        #region Custom Events
        public event EventHandler CardWrapperChanged;

        private void FireCardWrapperChangedEvent()
        {
            if (CardWrapperChanged != null)
            {
                CardWrapperChanged(this, EventArgs.Empty);
            }
            else { }
        }
        #endregion

        #region Ctor
        public CardImageContainer()
        {
            InitializeComponent();

            layoutRoot.DataContext = this;

            m_defaultImageSource = image.Source;

            CardWrapperChanged += OnCardWrapperChanged;
        }
        #endregion

        #region Events
        #region OnCardWrapperChanged
        private async void OnCardWrapperChanged(object sender, EventArgs e)
        {
            image.Source = m_defaultImageSource;
            image.Margin = new Thickness();

            if (m_currentImageStream != null)
            {
                Log.WriteLine("Disposing current image...", LogType.Debug);

                m_currentImageStream.Dispose();
                m_currentImageStream = null;
            }
            else { }

            if (CardWrapper != null && Visibility == Visibility.Visible)
            {
                Log.WriteLine($"Loading image for {CardWrapper.Card.Id} (Premium={CardWrapper.Card.Premium})", LogType.Debug);

                m_currentImageStream = (await HearthstoneCardImageManager.GetStreamAsync(CardWrapper.Card.Id, CardWrapper.Card.Premium));

                if (m_currentImageStream != null && CardWrapper != null)
                {
                    if (CardWrapper.Card.Premium)
                    {
                        SetAsGif(m_currentImageStream);
                    }
                    else
                    {
                        image.Source = (Image.FromStream(m_currentImageStream) as Bitmap).ToBitmapImage();
                    }

                    SetMargin();
                }
                else { }
            }
            else { }
        }
        #endregion
        #endregion

        #region GetMargin
        private void SetMargin()
        {
            if (CardWrapper.DbCard.Type == HearthDb.Enums.CardType.HERO)
            {
                image.Margin = new Thickness(-10, -35, 0, 25);
            }
            else
            {
                if (!CardWrapper.Card.Premium &&
                    (CardWrapper.DbCard.Id.Equals("CFM_321")
                    || CardWrapper.DbCard.Id.Equals("CFM_619")
                    || CardWrapper.DbCard.Id.Equals("CFM_621")
                    || CardWrapper.DbCard.Id.Equals("CFM_649")
                    || CardWrapper.DbCard.Id.Equals("CFM_685")
                    || CardWrapper.DbCard.Id.Equals("CFM_902")))
                {
                    image.Margin = new Thickness(0, 0, 0, -25);
                }
                else if (CardWrapper.Card.Premium)
                {
                    if (CardWrapper.DbCard.Type == HearthDb.Enums.CardType.ABILITY && CardWrapper.DbCard.Rarity == HearthDb.Enums.Rarity.LEGENDARY)
                    {
                        image.Margin = new Thickness(-15, -30, 0, 0);
                    }
                    else if (CardWrapper.DbCard.Type == HearthDb.Enums.CardType.ABILITY || CardWrapper.DbCard.Type == HearthDb.Enums.CardType.WEAPON)
                    {
                        image.Margin = new Thickness(0, -30, 0, 0);
                    }
                    else if (CardWrapper.DbCard.Type == HearthDb.Enums.CardType.MINION && CardWrapper.DbCard.Rarity != HearthDb.Enums.Rarity.LEGENDARY)
                    {
                        image.Margin = new Thickness(0, -20, -10, 0);
                    }
                    else
                    {
                        image.Margin = new Thickness();
                    }
                }
                else if (!CardWrapper.Card.Premium)
                {
                    image.Margin = new Thickness(0, -25, 0, 0);
                }
                else
                {
                    image.Margin = new Thickness();
                }
            }
        }
        #endregion

        #region SetAsGif
        private void SetAsGif(Stream stream)
        {
            Log.WriteLine("Setting current image as GIF", LogType.Debug);

            image.SetValue(XamlAnimatedGif.AnimationBehavior.SourceStreamProperty, stream);
        }
        #endregion
    }
}