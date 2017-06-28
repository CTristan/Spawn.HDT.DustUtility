using System.Collections.Generic;

namespace Spawn.HDT.DustUtility.Offline
{
    public class CachedDeck
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Hero { get; set; }
        public bool IsWild { get; set; }
        public int Type { get; set; }
        public int SeasonId { get; set; }
        public int CardBackId { get; set; }
        public int HeroPremium { get; set; }
        public List<CachedCard> Cards { get; set; }
    }
}
