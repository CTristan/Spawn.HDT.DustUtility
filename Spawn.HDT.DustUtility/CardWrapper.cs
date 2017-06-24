using HearthMirror.Objects;

namespace Spawn.HDT.DustUtility
{
    public class CardWrapper
    {
        private int m_nMaxCount;
        private Card m_card;
        private HearthDb.Card m_dbCard;

        public Card Card => m_card;
        public int MaxCountInDecks => m_nMaxCount;

        public CardWrapper(Card card)
        {
            m_card = card;

            m_dbCard = HearthDb.Cards.All[m_card.Id];
        }

        public HearthDb.Card GetDBCard()
        {
            return m_dbCard;
        }

        public void SetMaxCountInDecks(int nValue)
        {
            m_nMaxCount = nValue;
        }

        public int GetDustValue()
        {
            int nRet = m_card.GetDustValue();

            if (m_nMaxCount == 0)
            {
                nRet *= m_card.Count;
            }
            else { }

            return nRet;
        }
    }
}
