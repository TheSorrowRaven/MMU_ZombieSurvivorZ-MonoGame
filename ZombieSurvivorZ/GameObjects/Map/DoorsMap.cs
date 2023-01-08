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
        public class Door
        {
            public readonly Collision.BoxStaticCollider CL;
            public bool IsOpen = false;

            public Door(Collision.BoxStaticCollider CL)
            {
                this.CL = CL;
            }

        }

        public readonly Dictionary<Vector2Int, Door> Doors = new();
        public readonly List<Vector2> OpenDoors = new();
        public readonly List<Vector2> ClosedDoors = new();

        private readonly Texture2D tilesetTexture;
        private readonly Rectangle closedDoorRectangle;
        private readonly Rectangle openDoorRectangle;

        private const int OpenDoorCost = 0;
        private const int ClosedDoorCost = 1000;  //Base cost

        public DoorsMap(TiledMapTileLayer layer) : base(layer)
        {
            tilesetTexture = Game1.GetTexture("tileset_temp");
            closedDoorRectangle = new(0, 0, 64, 64);
            openDoorRectangle = new(128, 0, 64, 64);
        }

        protected override void InitializeTile(int x, int y, TiledMapTile tile)
        {
            base.InitializeTile(x, y, tile);
            Vector2 topLeftTile = LocalToTileTopLeftPosition(x, y);
            Collision.BoxStaticCollider box = new(this, TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);
            Doors.Add(new(x, y), new(box));
            ClosedDoors.Add(topLeftTile);
        }

        public override bool IsTileWalkable(ushort x, ushort y, out float cost)
        {
            TiledMapTile tile = Layer.GetTile(x, y);
            if (tile.GlobalIdentifier == 0)
            {
                cost = OpenDoorCost;
            }
            else
            {
                cost = ClosedDoorCost; //Base cost since nothing is barricaded yet
            }
            return true;
        }

        public bool SurroundingAreDoors(Vector2Int pos, out Vector2Int cell)
        {
            cell = pos;
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2Int c = new(cell.X + x, cell.Y + y);
                    if (Doors.ContainsKey(c))
                    {
                        cell = c;
                        return true;
                    }
                }
            }
            return false;
        }

        public void ToggleDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            if (door.IsOpen)
            {
                CloseDoor(doorCell);
            }
            else
            {
                OpenDoor(doorCell);
            }
            door.IsOpen = !door.IsOpen;

        }

        public void OpenDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            door.CL.Set(0, 0, 0, 0);

            Vector2 doorPos = LocalToTileTopLeftPosition(doorCell.X, doorCell.Y);
            OpenDoors.Add(doorPos);
            ClosedDoors.Remove(doorPos);

            MapManager.TileGraph.UpdateNodeCost(doorCell, OpenDoorCost);
        }

        public void CloseDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            Vector2 topLeftTile = LocalToTileTopLeftPosition(doorCell.X, doorCell.Y);
            door.CL.Set(TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);

            Vector2 doorPos = LocalToTileTopLeftPosition(doorCell.X, doorCell.Y);
            ClosedDoors.Add(doorPos);
            OpenDoors.Remove(doorPos);

            MapManager.TileGraph.UpdateNodeCost(doorCell, ClosedDoorCost); //TODO add base cost with barricade weight
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < OpenDoors.Count; i++)
            {
                //spriteBatch.Draw(tilesetTexture, OpenDoors[i], openDoorRectangle, Color.White);
            }
            for (int i = 0; i < ClosedDoors.Count; i++)
            {
                spriteBatch.Draw(tilesetTexture, ClosedDoors[i], closedDoorRectangle, Color.White);
            }

        }

    }
}