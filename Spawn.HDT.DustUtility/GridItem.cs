using System.Diagnostics;
using HearthDb.Enums;

namespace Spawn.HDT.DustUtility
{
    [DebuggerDisplay("{Name} ({Count})")]
    public class GridItem
    {
        public string Count { get; set; }

        public string Name { get; set; }

        public bool Golden { get; set; }

        public int Dust { get; set; }

        public Rarity Rarity { get; set; }

        public CardClass CardClass { get; set; }
    }
}
