using HearthDb;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility
{
    public static class ExtensionMethods
    {
        public static bool Contains(this HearthMirror.Objects.Deck deck, string strId)
        {
            return GetCard(deck, strId) != null;
        }

        public static HearthMirror.Objects.Card GetCard(this HearthMirror.Objects.Deck deck, string strId)
        {
            return deck.Cards.Find(delegate (HearthMirror.Objects.Card c) { return strId.Equals(c.Id); });
        }

        public static int GetDustValue(this HearthMirror.Objects.Card card)
        {
            int nRet = 0;

            if (card != null)
            {
                Card c = Cards.All[card.Id];

                if (c.Set != CardSet.CORE)
                {
                    switch (c.Rarity)
                    {
                        case Rarity.COMMON:
                            nRet = (card.Premium ? 50 : 5);
                            break;
                        case Rarity.RARE:
                            nRet = (card.Premium ? 100 : 20);
                            break;
                        case Rarity.EPIC:
                            nRet = (card.Premium ? 400 : 100);
                            break;
                        case Rarity.LEGENDARY:
                            nRet = (card.Premium ? 1600 : 400);
                            break;
                    }
                }
                else { }
            }
            else { }

            return nRet;
        }
    }
}
