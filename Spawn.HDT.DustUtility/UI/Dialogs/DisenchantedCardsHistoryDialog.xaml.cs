using HearthMirror.Objects;
using Spawn.HDT.DustUtility.History;
using System.Collections.Generic;

namespace Spawn.HDT.DustUtility.UI.Dialogs
{
    public partial class DisenchantedCardsHistoryDialog
    {
        #region Ctor
        public DisenchantedCardsHistoryDialog()
        {
            InitializeComponent();
        }

        public DisenchantedCardsHistoryDialog(Account account)
            : this()
        {
            grid.GridItems.Clear();

            List<Card> lstHistory = DisenchantedCardsHistory.GetHistory(account);

            for (int i = 0; i < lstHistory.Count; i++)
            {
                grid.GridItems.Add(GridItem.FromCardWrapper(new Search.CardWrapper(lstHistory[i])));
            }
        }
        #endregion
    }
}