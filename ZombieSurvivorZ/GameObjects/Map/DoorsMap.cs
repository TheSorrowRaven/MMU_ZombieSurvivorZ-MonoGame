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
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class DoorsMap : BaseMap
    {

        public readonly Dictionary<Vector2Int, Collision.BoxStaticCollider> DoorColliders = new();

        public DoorsMap(TiledMapTileLayer layer) : base(layer)
        {

        }
        protected override void InitializeTile(int x, int y, TiledMapTile tile)
        {
            base.InitializeTile(x, y, tile);
            Vector2 topLeftTile = LocalToTileTopLeftPosition(x, y);
            Collision.BoxStaticCollider box = new(this, TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);
            DoorColliders.Add(new(x, y), box);
        }

    }
}