using HearthMirror.Objects;

namespace Spawn.HDT.DustUtility
{
    public class CardWrapper
    {
        private Card m_card;
        private HearthDb.Card m_dbCard;

        public Card Card => m_card;
        public int MaxCountInDecks { get; set; }

        public CardWrapper(Card card)
        {
            m_card = card;

            m_dbCard = HearthDb.Cards.All[m_card.Id];
        }

        public HearthDb.Card GetDBCard()
        {
            return m_dbCard;
        }

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
    }
}
