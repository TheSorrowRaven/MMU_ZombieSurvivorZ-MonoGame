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
        public readonly TiledMap Map;
        public readonly FloorMap FloorLayer;
        public readonly WallsMap WallsLayer;
        public readonly WindowsMap WindowsLayer;
        public readonly DoorsMap DoorsLayer;
        public readonly ZombieSpawnMap ZombieSpawnLayer;
        public readonly CraftingMap CraftingLayer;

        public MapManager()
        {
            Map = Game1.GetContent<TiledMap>("zombie-survivorz");

            FloorLayer = new(Map.GetLayer("Floor"), Map);
            WallsLayer = new(Map.GetLayer("Walls"), Map);
            WindowsLayer = new(Map.GetLayer("Windows"), Map);
            DoorsLayer = new(Map.GetLayer("Doors"), Map);
            ZombieSpawnLayer = new(Map.GetLayer("ZombieSpawn"), Map);
            CraftingLayer = new(Map.GetLayer("Crafting"), Map);
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