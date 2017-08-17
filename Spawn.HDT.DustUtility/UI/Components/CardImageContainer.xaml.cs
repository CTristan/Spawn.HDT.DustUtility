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
        private Stream m_currentImage;
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

        #region OnCardWrapperChanged
        private async void OnCardWrapperChanged(object sender, EventArgs e)
        {
            if (CardWrapper != null && Visibility == Visibility.Visible)
            {
                m_currentImage = (await HearthstoneCardImageManager.GetStreamAsync(CardWrapper.Card.Id, CardWrapper.Card.Premium));

                image.Source = (Image.FromStream(m_currentImage) as Bitmap).ToBitmapImage();
            }
            else
            {
                if (m_currentImage != null)
                {
                    m_currentImage.Dispose();
                    //m_currentImage = null;
                }
                else { }

                image.Source = m_defaultImageSource;
            }
        }
        #endregion
    }
}