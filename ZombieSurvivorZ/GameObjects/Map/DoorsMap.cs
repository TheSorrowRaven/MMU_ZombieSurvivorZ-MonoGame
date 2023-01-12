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
            public enum BarricadeState
            {
                Base,
                Level1,
                Level2,
                Level3
            }

            public readonly Collision.BoxStaticCollider CL;
            public bool IsOpen = false;
            public bool Rotated = false;
            public bool barricaded = false;
            public float barricadingCount = 0f;
            public float barricadeTime = 3f;
            public BarricadeState barricadeLevel = BarricadeState.Base;
            public float ClosingScale = 0;

            public Door(Collision.BoxStaticCollider CL, bool Rotated)
            {
                this.CL = CL;
                this.Rotated = Rotated;
            }

            public void Barricade()
            {
                TryBarricade();
            }

            private void TryBarricade()
            {
                if (barricadeLevel == BarricadeState.Level3)
                {
                    return;
                }

                barricadingCount += (float)Time.gameTime.ElapsedGameTime.TotalSeconds;
                Console.WriteLine(barricadingCount);

                if (barricadingCount > barricadeTime)
                {
                    ResetBarricading();
                    barricadeLevel += 1;
                    Console.WriteLine(barricadeLevel);
                }
            }

            public void ResetBarricading()
            {
                barricadingCount = 0f;
            }
        }

        public readonly Dictionary<Vector2Int, Door> Doors = new();
        public readonly List<Vector2Int> OpenDoors = new();
        public readonly List<Vector2Int> ClosedDoors = new();

        private readonly Texture2D tilesetTexture;
        private readonly Rectangle closedDoorRectangleHorizontal;
        private readonly Rectangle closedDoorRectangleVertical;
        private readonly Rectangle openDoorRectangleHorizontal;
        private readonly Rectangle openDoorRectangleVertical;
        private readonly Rectangle Level1BarricadeHorizontal;
        private readonly Rectangle Level1BarricadeVertical;
        private readonly Rectangle Level2BarricadeHorizontal;
        private readonly Rectangle Level2BarricadeVertical;
        

        private readonly List<Vector2Int> ExpandingDoors = new();

        private const int OpenDoorCost = 0;
        private const int ClosedDoorCost = 1000;  //Base cost

        private const float ClosingDoorSpeed = 0.1f;

        public DoorsMap(TiledMapTileLayer layer) : base(layer)
        {
            tilesetTexture = Game1.GetTexture("tileset_temp");
            closedDoorRectangleHorizontal = new(256, 128, 64, 64);
            closedDoorRectangleVertical = new(320, 128, 64, 64);
            openDoorRectangleHorizontal = new(192, 128, 64, 64);
            openDoorRectangleVertical = new(0, 192, 64, 64);
            Level1BarricadeHorizontal = new(0, 256, 64, 64);
            Level1BarricadeVertical = new(64, 256, 64, 64);
            Level2BarricadeHorizontal = new(128, 256, 64, 64);
            Level2BarricadeVertical = new(192, 256, 64, 64);
        }

        protected override void InitializeTile(int x, int y, TiledMapTile tile)
        {
            base.InitializeTile(x, y, tile);
            Vector2 topLeftTile = LocalToTileTopLeftPosition(x, y);
            Collision.BoxStaticCollider box = new(this, TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);
            Vector2Int doorCell = new(x, y);
            Doors.Add(doorCell, new(box, tile.IsFlippedHorizontally));
            ClosedDoors.Add(doorCell);
        }

        public override void Update()
        {
            base.Update();

            for (int i = 0; i < ExpandingDoors.Count; i++)
            {
                Vector2Int doorCell = ExpandingDoors[i];
                Door door = Doors[doorCell];

                door.ClosingScale += Time.deltaTime / ClosingDoorSpeed;
                if (door.ClosingScale > 1)
                {
                    door.ClosingScale = 1;
                    ExpandingDoors.RemoveAt(i);
                    i--;
                }

                Vector2 topLeftTile = LocalToTileTopLeftPosition(doorCell.X, doorCell.Y);
                Vector2 size = TileSize * door.ClosingScale;
                door.CL.Set(size.X, size.Y, topLeftTile.X, topLeftTile.Y);
            }

        }

        public float GetCostFromDoor(Vector2Int doorCell)
        {
            if (!Doors.TryGetValue(doorCell, out Door door))
            {
                return ClosedDoorCost; //Base cost since nothing is barricaded yet
            }
            return GetCostFromDoor(door);
        }
        public float GetCostFromDoor(Door door)
        {
            if (door.IsOpen)
            {
                return OpenDoorCost;
            }
            else
            {
                return ClosedDoorCost; //TODO add base cost with barricade weight
            }
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
                cost = GetCostFromDoor(new Vector2Int(x, y));
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
            ExpandingDoors.Remove(doorCell);

            Door door = Doors[doorCell];
            door.CL.Set(0, 0, 0, 0);

            OpenDoors.Add(doorCell);
            ClosedDoors.Remove(doorCell);

            MapManager.TileGraph.UpdateNodeCost(doorCell, OpenDoorCost);
        }

        public void CloseDoor(Vector2Int doorCell)
        {
            ExpandingDoors.Add(doorCell);

            Door door = Doors[doorCell];
            door.ClosingScale = 0;
            //Vector2 topLeftTile = LocalToTileTopLeftPosition(doorCell.X, doorCell.Y);
            //door.CL.Set(TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);

            ClosedDoors.Add(doorCell);
            OpenDoors.Remove(doorCell);

            MapManager.TileGraph.UpdateNodeCost(doorCell, GetCostFromDoor(doorCell)); //TODO add base cost with barricade weight
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < OpenDoors.Count; i++)
            {
                Vector2 doorPos = LocalToTileTopLeftPosition(OpenDoors[i]);

                if (Doors[OpenDoors[i]].Rotated)
                {
                    switch (Doors[OpenDoors[i]].barricadeLevel)
                    {
                        case Door.BarricadeState.Base:
                            break;
                        case Door.BarricadeState.Level1:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level1BarricadeHorizontal, Color.White);
                            break;
                        case Door.BarricadeState.Level2:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level2BarricadeHorizontal, Color.White);
                            break;
                        case Door.BarricadeState.Level3:

                            break;
                    }
                    spriteBatch.Draw(tilesetTexture, doorPos, openDoorRectangleHorizontal, Color.White);
                }
                else
                {
                    switch (Doors[OpenDoors[i]].barricadeLevel)
                    {
                        case Door.BarricadeState.Base:
                            break;
                        case Door.BarricadeState.Level1:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level1BarricadeVertical, Color.White);
                            break;
                        case Door.BarricadeState.Level2:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level2BarricadeVertical, Color.White);
                            break;
                        case Door.BarricadeState.Level3:

                            break;
                    }
                    spriteBatch.Draw(tilesetTexture, doorPos, openDoorRectangleVertical, Color.White);
                }

            }
            for (int i = 0; i < ClosedDoors.Count; i++)
            {
                Vector2 doorPos = LocalToTileTopLeftPosition(ClosedDoors[i]);

                if (Doors[ClosedDoors[i]].Rotated)
                {
                    switch (Doors[ClosedDoors[i]].barricadeLevel)
                    {
                        case Door.BarricadeState.Base:
                            break;
                        case Door.BarricadeState.Level1:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level1BarricadeVertical, Color.White);
                            break;
                        case Door.BarricadeState.Level2:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level2BarricadeVertical, Color.White);
                            break;
                        case Door.BarricadeState.Level3:

                            break;
                    }
                    spriteBatch.Draw(tilesetTexture, doorPos, closedDoorRectangleVertical, Color.White);
                }
                else
                {
                    switch (Doors[ClosedDoors[i]].barricadeLevel)
                    {
                        case Door.BarricadeState.Base:
                            break;
                        case Door.BarricadeState.Level1:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level1BarricadeHorizontal, Color.White);
                            break;
                        case Door.BarricadeState.Level2:
                            spriteBatch.Draw(tilesetTexture, doorPos, Level2BarricadeHorizontal, Color.White);
                            break;
                        case Door.BarricadeState.Level3:

                            break;
                    }
                    spriteBatch.Draw(tilesetTexture, doorPos, closedDoorRectangleHorizontal, Color.White);
                }
            }

        }

        public bool Barricaded(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];

            return door.barricaded;
        }
    }
}