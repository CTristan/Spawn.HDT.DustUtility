using System;

namespace Spawn.HDT.DustUtility.UI
{
    public class GridItemEventArgs : EventArgs
    {
        #region Properties
        public GridItem Item { get; }
        #endregion

        #region Ctor
        public GridItemEventArgs(GridItem item)
        {
            Item = item;
        }
        #endregion
    }
}