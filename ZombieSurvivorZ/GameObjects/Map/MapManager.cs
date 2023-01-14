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
    public class MapManager : SpriteObject
    {
        public static readonly TiledMap Map = Game1.GetContent<TiledMap>("zombie-survivorz");

        public readonly TileGraph TileGraph;

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

            TileGraph = new();

            Active = false;

            Position = new(Map.TileWidth * Map.Width * -0.5f, Map.TileHeight * Map.Height * -0.5f);
        }

        public override void Initialize()
        {
            base.Initialize();
            TileGraph.BFSConstructGraph(GetTileData, 25, 24);
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
        public Vector2 LocalToTileCenterPosition(Vector2Int local)
        {
            return LocalToTileCenterPosition(local.X, local.Y);
        }
        public Vector2 LocalToTileCenterPosition(int x, int y)
        {
            Vector2 position = LocalToTileTopLeftPosition(x, y);
            position += TileSize / 2;
            return position;
        }
        public Vector2Int PositionToLocal(Vector2 pos)
        {
            if (float.IsNaN(pos.X) || float.IsNaN(pos.Y))
            {
                return new(0, 0);
            }
            Vector2 position = Position;
            return new Vector2Int(Convert.ToInt32(MathF.Floor((pos.X - position.X) / Map.TileWidth)), Convert.ToInt32(MathF.Floor((pos.Y - position.Y) / Map.TileHeight)));
        }

        public Stack<Vector2Int> Pathfind(Vector2Int start, Vector2Int end)
        {
            Stack<Vector2Int> path = AStar.Compute(TileGraph, start, end);
            return path;
        }
        public void Pathfind(Vector2Int start, Vector2Int end, Stack<Vector2Int> path)
        {
            path.Clear();
            AStar.Compute(TileGraph, start, end, path);
        }

        public TileGraph.TileData GetTileData(ushort x, ushort y)
        {
            TileGraph.TileData tileData = new();
            if (!WallsLayer.IsTileWalkable(x, y, out _))
            {
                tileData.walkable = false;
                return tileData;
            }
            if (!WindowsLayer.IsTileWalkable(x, y, out _))
            {
                tileData.walkable = false;
                return tileData;
            }
            if (!CraftingLayer.IsTileWalkable(x, y, out _))
            {
                tileData.walkable = false;
                return tileData;
            }
            // DoorsLayer will always return true
            if (DoorsLayer.IsTileWalkable(x, y, out float cost))
            {
                tileData.walkable = true;
                tileData.cost = cost;
                return tileData;
            }
            tileData.walkable = true;
            return tileData;
        }

        public Vector2Int GetNearestSideTile(Vector2Int tile, Vector2 position, bool horizontal)
        {
            Vector2 center = LocalToTileCenterPosition(tile);
            if (horizontal)
            {
                return new(tile.X + (position.X < center.X ? -1 : 1), tile.Y);
            }
            else
            {
                return new(tile.X, tile.Y + (position.Y < center.Y ? -1 : 1));
            }
        }

        public Vector2Int Get0CostWalkableSurroundingTile(Vector2Int center)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    Vector2Int cell = new(center.X + x, center.Y + y);
                    if (CellIs0CostWalkable(cell))
                    {
                        return cell;
                    }
                }
            }
            return center;
        }
        //public Vector2Int Get0CostWalkableSideTile(Vector2Int center)
        //{
        //    for (int y = -1; y <= 1; y++)
        //    {
        //        for (int x = -1; x <= 1; x++)
        //        {
        //            if (x == 0 && y == 0)
        //            {
        //                continue;
        //            }
        //            if (Math.Abs(x * y) == 1)
        //            {
        //                continue;
        //            }
        //            Vector2Int cell = new(center.X + x, center.Y + y);
        //            if (CellIs0CostWalkable(cell))
        //            {
        //                return cell;
        //            }
        //        }
        //    }
        //    return center;
        //}
        public bool CellIs0CostWalkable(Vector2Int cell)
        {
            TileGraph.TileData tileData = GetTileData((ushort)cell.X, (ushort)cell.Y);
            return tileData.walkable && tileData.cost == 0;
        }
        public bool CellIsWalkable(Vector2Int cell)
        {
            TileGraph.TileData tileData = GetTileData((ushort)cell.X, (ushort)cell.Y);
            return tileData.walkable;
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