using HearthMirror.Objects;
using Hearthstone_Deck_Tracker.Enums;
using System;

namespace Spawn.HDT.DustUtility
{
    public class Account
    {
        #region Properties
        public BattleTag BattleTag { get; }
        public Region Region { get; }
        public string AccountString { get; }

        public static Account Empty => new Account(null, Region.UNKNOWN);

        public bool IsEmpty => BattleTag == null && Region == Region.UNKNOWN;
        #endregion

        #region Ctor
        public Account(BattleTag battleTag, Region region)
        {
            BattleTag = battleTag;
            Region = region;

            if (BattleTag != null)
            {
                AccountString = $"{battleTag.Name}_{battleTag.Number}_{region}";
            }
            else
            {
                AccountString = string.Empty;
            }
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            bool blnRet = false;

            if (obj is Account)
            {
                Account acc = obj as Account;

                blnRet = true;

                if (acc.BattleTag != null)
                {
                    blnRet &= acc.BattleTag.Name.Equals(BattleTag.Name);

                    blnRet &= acc.BattleTag.Number == BattleTag.Number;
                }
                else { }

                blnRet &= acc.Region == Region;
            }
            else
            {
                blnRet = base.Equals(obj);
            }

            return blnRet;
        }
        #endregion

        #region GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region [STATIC] Parse
        public static Account Parse(string strAccountString)
        {
            string[] vTemp = strAccountString.Split('_');

            BattleTag battleTag = new BattleTag()
            {
                Name = vTemp[0],
                Number = Convert.ToInt32(vTemp[1])
            };

            return new Account(battleTag, (Region)Enum.Parse(typeof(Region), vTemp[2]));
        }
        #endregion
    }
}