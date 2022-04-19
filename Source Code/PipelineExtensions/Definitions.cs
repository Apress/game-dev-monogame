using System.Collections.Generic;

namespace Engine2D.Objects.Atlas
{
    public class AtlasDefinition
    {
        public int Width;
        public int Height;

        public List<TileBlockDefinition> TileBlocks;
    }
    
    public class TileBlockDefinition
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int TilesWidth;
        public int TilesHeight;
    }
}
