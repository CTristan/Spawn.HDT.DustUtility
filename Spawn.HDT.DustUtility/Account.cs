using System;
using HearthMirror.Objects;
using Hearthstone_Deck_Tracker.Enums;

namespace Spawn.HDT.DustUtility
{
    public class Account
    {
        #region Properties
        public BattleTag BattleTag { get; }
        public Region Region { get; }
        public string AccountString { get; }
        #endregion

        #region Ctor
        public Account(BattleTag battleTag, Region region)
        {
            BattleTag = battleTag;
            Region = region;

            AccountString = $"{battleTag.Name}_{battleTag.Number}_{region}";
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            bool blnRet = false;

            if (obj is Account)
            {
                Account acc = obj as Account;

                if (acc.BattleTag != null)
                {
                    blnRet = acc.BattleTag.Name.Equals(BattleTag.Name);

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
