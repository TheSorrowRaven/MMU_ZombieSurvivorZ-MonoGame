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
    public class MapManager : SpriteObject
    {
        public static readonly TiledMap Map = Game1.GetContent<TiledMap>("zombie-survivorz");
        public readonly FloorMap FloorLayer;
        public readonly WallsMap WallsLayer;
        public readonly WindowsMap WindowsLayer;
        public readonly DoorsMap DoorsLayer;
        public readonly ZombieSpawnMap ZombieSpawnLayer;
        public readonly CraftingMap CraftingLayer;

        public readonly Vector2 TileSize;
        public readonly Vector2Int MapSize;

        public MapManager()
        {
            TileSize = new(Map.TileWidth, Map.TileHeight);
            MapSize = new(Map.Width, Map.Height);

            FloorLayer = new(Map.GetLayer<TiledMapTileLayer>("Floor"));
            WallsLayer = new(Map.GetLayer<TiledMapTileLayer>("Walls"));
            WindowsLayer = new(Map.GetLayer<TiledMapTileLayer>("Windows"));
            DoorsLayer = new(Map.GetLayer<TiledMapTileLayer>("Doors"));
            ZombieSpawnLayer = new(Map.GetLayer<TiledMapTileLayer>("ZombieSpawn"));
            CraftingLayer = new(Map.GetLayer<TiledMapTileLayer>("Crafting"));
            Active = false;

            Position = new(Map.TileWidth * Map.Width * -0.5f, Map.TileHeight * Map.Height * -0.5f);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public Vector2 LocalToTileTopLeftPosition(Vector2Int local)
        {
            return LocalToTileTopLeftPosition(local.X, local.Y);
        }
        public Vector2 LocalToTileTopLeftPosition(int x, int y)
        {
            Vector2 position = Position;
            return new(x * Map.TileWidth + position.X, y * Map.TileHeight + position.Y);
        }
        public Vector2Int PositionToLocal(Vector2 pos)
        {
            Vector2 position = Position;
            return new Vector2Int(Convert.ToInt32(MathF.Floor((pos.X - position.X) / Map.TileWidth)), Convert.ToInt32(MathF.Floor((pos.Y - position.Y) / Map.TileHeight)));
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

    }
}