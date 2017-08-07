using System.Collections.Generic;
using System.ComponentModel;
using Spawn.HDT.DustUtility.UI;

namespace Spawn.HDT.DustUtility.Search
{
    public class SearchResult : INotifyPropertyChanged
    {
        #region Member Variables
        private int m_nDust;
        private int m_nCommonsCount;
        private int m_nRaresCount;
        private int m_nEpicsCount;
        private int m_nLegendariesCount;
        private int m_nTotalCount;
        private IEnumerable<GridItem> m_gridItems;
        #endregion

        #region Properties
        #region Dust
        public int Dust
        {
            get => m_nDust;
            set
            {
                m_nDust = value;
                OnPropertyChanged("Dust");
            }
        } 
        #endregion

        #region CommonsCount
        public int CommonsCount
        {
            get => m_nCommonsCount;
            set
            {
                m_nCommonsCount = value;
                OnPropertyChanged("CommonsCount");
            }
        } 
        #endregion

        #region RaresCount
        public int RaresCount
        {
            get => m_nRaresCount;
            set
            {
                m_nRaresCount = value;
                OnPropertyChanged("RaresCount");
            }
        } 
        #endregion

        #region EpicsCount
        public int EpicsCount
        {
            get => m_nEpicsCount;
            set
            {
                m_nEpicsCount = value;
                OnPropertyChanged("EpicsCount");
            }
        } 
        #endregion

        #region LegendariesCount
        public int LegendariesCount
        {
            get => m_nLegendariesCount;
            set
            {
                m_nLegendariesCount = value;
                OnPropertyChanged("LegendariesCount");
            }
        } 
        #endregion

        #region GridItems
        public IEnumerable<GridItem> GridItems
        {
            get => m_gridItems;
            set
            {
                m_gridItems = value;
                OnPropertyChanged("GridItems");
            }
        } 
        #endregion
        #endregion

        #region Custom Events
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string strPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        } 
        #endregion
        #endregion
    }
}
