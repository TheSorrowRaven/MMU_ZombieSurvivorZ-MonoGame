using MonoGame.Extended.Tiled;

namespace ZombieSurvivorZ
{
    public class FloorMap : BaseMap
    {

        public override World World => World.floor;

        public FloorMap(TiledMapTileLayer layer) : base(layer)
        {

        }

        public override bool IsTileWalkable(ushort x, ushort y, out float cost)
        {
            cost = 0;
            return true;
        }
    }
}