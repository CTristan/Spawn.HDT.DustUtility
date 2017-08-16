using HearthMirror.Objects;
using Hearthstone_Deck_Tracker.Enums;

namespace Spawn.HDT.DustUtility
{
    public class Account
    {
        public BattleTag BattleTag { get; }
        public Region Region { get; }
        public string AccountString { get; }

        public Account(BattleTag battleTag, Region region)
        {
            BattleTag = battleTag;
            Region = region;

            AccountString = $"{battleTag.Name}_{battleTag.Number}_{region}";
        }
    }
}
