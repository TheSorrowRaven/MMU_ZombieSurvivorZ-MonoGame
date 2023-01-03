using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace ZombieSurvivorZ
{
    public class WallsMap : BaseMap
    {

        public WallsMap(TiledMapTileLayer layer) : base(layer)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            for (ushort y = 0; y < Layer.Height; y++)
            {
                for (ushort x = 0; x < Layer.Width; x++)
                {
                    TiledMapTile tile = Layer.GetTile(x, y);
                    if (tile.GlobalIdentifier == 0)
                    {
                        continue;
                    }
                    Collision.BoxStaticCollider box = new(this, Layer.TileWidth, Layer.TileHeight, x * Layer.TileWidth + ActualPosition.X, y * Layer.TileHeight + ActualPosition.Y);
                    Console.WriteLine(box.Bounds);
                }
            }
        }

    }
}