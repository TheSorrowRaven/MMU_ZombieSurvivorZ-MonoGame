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
            public bool Rotated = false;
            public float ClosingScale = 0;
            public float ZombieDestroyCooldown = 0;

            public Door(Collision.BoxStaticCollider CL, bool Rotated)
            {
                this.CL = CL;
                this.Rotated = Rotated;
            }

        }

        private const int ZombieDestroyCooldown = 1;

        public readonly Dictionary<Vector2Int, Door> Doors = new();

        private readonly Texture2D tilesetTexture;
        private readonly Rectangle closedDoorRectangleHorizontal;
        private readonly Rectangle closedDoorRectangleVertical;
        private readonly Rectangle openDoorRectangleHorizontal;
        private readonly Rectangle openDoorRectangleVertical;

        private readonly List<Vector2Int> ExpandingDoors = new();
        private readonly List<Vector2Int> CooldownDoors = new();

        private const int OpenDoorCost = 0;
        private const int ClosedDoorCost = 1000;  //Base cost

        private const float ClosingDoorSpeed = 0.5f;

        public DoorsMap(TiledMapTileLayer layer) : base(layer)
        {
            tilesetTexture = Game1.GetTexture("tileset_temp");
            closedDoorRectangleHorizontal = new(256, 128, 64, 64);
            closedDoorRectangleVertical = new(320, 128, 64, 64);
            openDoorRectangleHorizontal = new(192, 128, 64, 64);
            openDoorRectangleVertical = new(0, 192, 64, 64);
        }

        protected override void InitializeTile(int x, int y, TiledMapTile tile)
        {
            base.InitializeTile(x, y, tile);
            Vector2 topLeftTile = LocalToTileTopLeftPosition(x, y);
            Collision.BoxStaticCollider box = new(this, TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);
            Vector2Int doorCell = new(x, y);
            Doors.Add(doorCell, new(box, tile.IsFlippedHorizontally));
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

                Vector2 tileCenter = MapManager.LocalToTileCenterPosition(doorCell);
                Vector2 size = TileSize;
                Vector2 tileTopLeft = tileCenter;
                if (door.Rotated)
                {
                    size.X *= door.ClosingScale;
                    tileTopLeft.X -= size.X / 2;
                }
                else
                {
                    size.Y *= door.ClosingScale;
                    tileTopLeft.Y -= size.Y / 2;
                }
                door.CL.Set(size.X, size.Y, tileTopLeft.X, tileTopLeft.Y);
            }


            for (int i = 0; i < CooldownDoors.Count; i++)
            {
                Vector2Int doorCell = CooldownDoors[i];
                Door door = Doors[doorCell];

                door.ZombieDestroyCooldown -= Time.deltaTime;
                if (door.ZombieDestroyCooldown < 0)
                {
                    door.ZombieDestroyCooldown = 0;
                    CooldownDoors.RemoveAt(i);
                    i--;
                }
            }

        }

        public bool TryGetDoor(Vector2Int doorCell, out Door door)
        {
            return Doors.TryGetValue(doorCell, out door);
        }

        public bool TryGetClosedDoor(Vector2Int doorCell, out Door door)
        {
            if (!Doors.TryGetValue(doorCell, out door))
            {
                return false;
            }
            if (door.IsOpen)
            {
                return false;
            }
            return true;
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
        }

        public void OpenDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            if (door.IsOpen)
            {
                return;
            }

            door.CL.Set(0, 0, 0, 0);
            door.IsOpen = true;

            ExpandingDoors.Remove(doorCell);

            MapManager.TileGraph.UpdateNodeCost(doorCell, OpenDoorCost);
        }

        public void CloseDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            if (!door.IsOpen)
            {
                return;
            }

            if (door.ZombieDestroyCooldown > 0)
            {
                return;
            }

            if (MapManager.ZombieSpawnLayer.ZombieIsBlockingDoorClosing(doorCell))
            {
                //TODO cannot close door! zombie is blocking
                return;
            }


            door.ClosingScale = 0;
            door.IsOpen = false;
            //Vector2 topLeftTile = LocalToTileTopLeftPosition(doorCell.X, doorCell.Y);
            //door.CL.Set(TileSize.X, TileSize.Y, topLeftTile.X, topLeftTile.Y);
            ExpandingDoors.Add(doorCell);

            MapManager.TileGraph.UpdateNodeCost(doorCell, GetCostFromDoor(doorCell)); //TODO add base cost with barricade weight
        }


        public void DealDamage(Vector2Int doorCell, int damage)
        {


            //if (door.health <= 0)
            {
                OpenDoor(doorCell);
                ZombieAttackCooldownDoor(doorCell);
            }
        }

        private void ZombieAttackCooldownDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            door.ZombieDestroyCooldown = ZombieDestroyCooldown;

            CooldownDoors.Add(doorCell);

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in Doors)
            {
                DrawDoor(spriteBatch, item.Key);
            }

        }

        private void DrawDoor(SpriteBatch spriteBatch, Vector2Int doorCell)
        {
            Vector2 doorPos = LocalToTileTopLeftPosition(doorCell);
            Door door = Doors[doorCell];
            Rectangle rect;
            if (door.IsOpen)
            {
                if (door.Rotated)
                {
                    rect = openDoorRectangleHorizontal;
                }
                else
                {
                    rect = openDoorRectangleVertical;
                }
            }
            else
            {
                if (door.Rotated)
                {
                    rect = closedDoorRectangleVertical;
                }
                else
                {
                    rect = closedDoorRectangleHorizontal;
                }
            }
            Color color = Color.White;
            if (door.ZombieDestroyCooldown > 0)
            {
                color = new(0.3f, 0.3f, 0.3f);
            }
            spriteBatch.Draw(tilesetTexture, doorPos, rect, color);
        }

    }
}