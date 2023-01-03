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

        public MapManager()
        {
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


        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

    }
}