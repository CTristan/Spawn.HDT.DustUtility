using Hearthstone_Deck_Tracker.Utility.Logging;
using Spawn.HDT.DustUtility.Search;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spawn.HDT.DustUtility.UI
{
    public partial class MainWindow
    {
        #region SearchAsync
        private async Task SearchAsync()
        {
            if (m_cardCollector != null && m_parameters != null)
            {
                await Task.Delay(1); //Return to ui thread

                UpdateUIState(false);

                m_parameters.QueryString = inputBox.Text;

                CardWrapper[] vCards = await m_cardCollector.GetCardsAsync(m_parameters);

                CreateSearchResult(vCards).CopyTo(GetSearchResultContainerComponent());

                UpdateUIState(true);
            }
            else { }
        }
        #endregion

        #region CreateSearchResult
        private SearchResultContainer CreateSearchResult(CardWrapper[] vCards)
        {
            SearchResultContainer retVal = new SearchResultContainer();

            Log.WriteLine("Creating search result...", LogType.Debug);

            GridItem[] vItems = new GridItem[vCards.Length];

            for (int i = 0; i < vCards.Length; i++)
            {
                CardWrapper wrapper = vCards[i];

                switch (wrapper.DbCard.Rarity)
                {
                    case HearthDb.Enums.Rarity.COMMON:
                        retVal.CommonsCount += wrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.RARE:
                        retVal.RaresCount += wrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.EPIC:
                        retVal.EpicsCount += wrapper.Count;
                        break;
                    case HearthDb.Enums.Rarity.LEGENDARY:
                        retVal.LegendariesCount += wrapper.Count;
                        break;
                }

                GridItem item = GridItem.FromCardWrapper(wrapper);

                retVal.TotalCount += item.Count;
                retVal.Dust += item.Dust;

                vItems[i] = item;
            }

            //Sort
            vItems = OrderItems(vItems).ToArray();

            for (int i = 0; i < vItems.Length; i++)
            {
                retVal.GridItems.Add(vItems[i]);
            }

            return retVal;
        }
        #endregion

        #region OrderItems
        private IEnumerable<GridItem> OrderItems(IEnumerable<GridItem> items)
        {
            IEnumerable<GridItem> retVal;

            SortOrder sortOrder = SortOrder.Parse(Settings.SortOrder);

            if (sortOrder != null && sortOrder.Items.Count > 0)
            {
                IQueryable<GridItem> query = items.AsQueryable();

                for (int i = 0; i < sortOrder.Items.Count; i++)
                {
                    query = query.OrderBy(sortOrder.Items[i].Value.ToString(), i);
                }

                retVal = query.ToList();
            }
            else
            {
                //lstRet = list.OrderBy(item => item.Rarity).ThenBy(item => item.Golden).ThenBy(item => item.Dust).ThenBy(item => item.CardClass).ThenBy(item => item.CardSet).ThenBy(item => item.Name).ToList();
                retVal = items;
            }

            return retVal;
        }
        #endregion

        #region GetSearchResultContainerComponent
        public SearchResultContainer GetSearchResultContainerComponent()
        {
            return FindResource(SearchResultKey) as SearchResultContainer;
        }
        #endregion
    }
}