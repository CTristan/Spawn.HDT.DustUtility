using HearthMirror.Objects;

namespace Spawn.HDT.DustUtility.Search
{
    public class CardWrapper
    {
        #region Member Variables
        private Card m_card;
        private HearthDb.Card m_dbCard;
        #endregion

        #region Properties
        #region Card
        public Card Card => m_card;
        #endregion

        #region DbCard
        public HearthDb.Card DbCard => m_dbCard;
        #endregion

        #region MaxCountInDecks
        public int MaxCountInDecks { get; set; }
        #endregion

        #region Count
        public int Count => m_card.Count - MaxCountInDecks; 
        #endregion
        #endregion

        #region Ctor
        public CardWrapper(Card card)
        {
            m_card = card;

            m_dbCard = HearthDb.Cards.All[m_card.Id];
        }
        #endregion

        #region GetDustValue
        public int GetDustValue()
        {
            int nRet = m_card.GetDustValue();

            if (MaxCountInDecks == 0)
            {
                nRet *= m_card.Count;
            }
            else { }

            return nRet;
        } 
        #endregion
    }
}
