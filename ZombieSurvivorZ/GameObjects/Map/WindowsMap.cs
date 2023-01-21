using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace ZombieSurvivorZ
{
    public class WindowsMap : BaseMap
    {

        public WindowsMap(TiledMapTileLayer layer) : base(layer)
        {

        }

        protected override void InitializeTile(int x, int y, TiledMapTile tile)
        {
            base.InitializeTile(x, y, tile);
            Vector2 topLeftTile = LocalToTileTopLeftPosition(x, y);
            new Collision.RaycastIgnoredBoxStaticCollider(this, TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);
        }

        public override bool IsTileWalkable(ushort x, ushort y, out float cost)
        {
            TiledMapTile tile = Layer.GetTile(x, y);
            cost = 0;
            return tile.GlobalIdentifier == 0;
        }

    }
}