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
using MonoGame.Extended.Collections;
using Microsoft.Xna.Framework.Audio;

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

            public int BarricadeHealth;

            public const int BarricadeLevelMaxHealth = 60;
            public const int BarricadeMaxLevel = 3;

            public Door(Collision.BoxStaticCollider CL, bool Rotated)
            {
                this.CL = CL;
                this.Rotated = Rotated;
            }

            public int GetBarricadeLevel()
            {
                if (BarricadeHealth <= 0)
                {
                    return 0;
                }
                return (BarricadeHealth - 1) / BarricadeLevelMaxHealth + 1;
            }

            public void Barricade()
            {
                int mod = BarricadeHealth % BarricadeLevelMaxHealth;
                int currentLevel = GetBarricadeLevel();
                if (mod == 0)
                {
                    if (currentLevel < 3)
                    {
                        BarricadeHealth += BarricadeLevelMaxHealth;
                    }
                }
                else
                {
                    BarricadeHealth = currentLevel * BarricadeLevelMaxHealth;
                }
            }

            public bool CanBarricade()
            {
                if (ZombieDestroyCooldown > 0)
                {
                    return false;
                }
                int mod = BarricadeHealth % BarricadeLevelMaxHealth;
                if (mod == 0)
                {
                    return GetBarricadeLevel() < 3;
                }
                return true;
            }

            public bool TryBarricade()
            {
                if (CanBarricade())
                {
                    Barricade();
                    return true;
                }
                return false;
            }

        }

        public readonly Dictionary<Vector2Int, Door> Doors = new();

        private readonly Texture2D tilesetTexture;

        private readonly Rectangle closedDoorRectangleVertical;
        private readonly Rectangle closedDoorRectangleHorizontal;
        private readonly Rectangle openDoorRectangleVertical;
        private readonly Rectangle openDoorRectangleHorizontal;

        private readonly Rectangle Level1BarricadeHorizontal;
        private readonly Rectangle Level1BarricadeVertical;
        private readonly Rectangle Level2BarricadeHorizontal;
        private readonly Rectangle Level2BarricadeVertical;
        private readonly Rectangle Level3BarricadeHorizontal;
        private readonly Rectangle Level3BarricadeVertical;
        
        private readonly List<Vector2Int> ExpandingDoors = new();
        private readonly List<Vector2Int> CooldownDoors = new();

        private const int OpenDoorCost = 0;
        private const int ClosedDoorCost = 2;  //Base cost
        private const float BarricadeCostMultiplier = 0.1f;
        private const int ZombieDestroyCooldown = 1;

        private const float ClosingDoorSpeed = 0.5f;

        public DoorsMap(TiledMapTileLayer layer) : base(layer)
        {
            tilesetTexture = Game1.GetTexture("tileset_temp");

            closedDoorRectangleHorizontal = new(256, 128, 64, 64);
            closedDoorRectangleVertical = new(320, 128, 64, 64);
            openDoorRectangleVertical = new(192, 128, 64, 64);
            openDoorRectangleHorizontal = new(0, 192, 64, 64);


            Level1BarricadeHorizontal = new(0, 256, 64, 64);
            Level1BarricadeVertical = new(64, 256, 64, 64);
            Level2BarricadeHorizontal = new(128, 256, 64, 64);
            Level2BarricadeVertical = new(192, 256, 64, 64);
            Level3BarricadeHorizontal = new(256, 256, 64, 64);
            Level3BarricadeVertical = new(320, 256, 64, 64);
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
                return ClosedDoorCost;  //Returns before Doors is initialized
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
                return ClosedDoorCost + door.BarricadeHealth * BarricadeCostMultiplier;
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

        public bool ToggleDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            if (door.IsOpen)
            {
                CloseDoor(doorCell);
                return false;
            }
            else
            {
                OpenDoor(doorCell);
                return true;
            }
        }

        public void OpenDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            if (door.IsOpen)
            {
                return;
            }

            door.CL.Set(0, 0, 9999, 9999);
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

            MapManager.TileGraph.UpdateNodeCost(doorCell, GetCostFromDoor(doorCell));
        }

        public bool TryBarricadeDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            bool barricaded = door.TryBarricade();

            MapManager.TileGraph.UpdateNodeCost(doorCell, GetCostFromDoor(doorCell));

            return barricaded;
        }

        public void DealDamage(Vector2Int doorCell, int damage)
        {
            Door door = Doors[doorCell];
            if (door.BarricadeHealth == 0)
            {
                OpenDoor(doorCell);
            }
            else
            {
                door.BarricadeHealth = Math.Max(0, door.BarricadeHealth - damage);
                MapManager.TileGraph.UpdateNodeCost(doorCell, GetCostFromDoor(doorCell));
            }
            ZombieAttackCooldownDoor(doorCell);
        }

        private void ZombieAttackCooldownDoor(Vector2Int doorCell)
        {
            Door door = Doors[doorCell];
            door.ZombieDestroyCooldown = ZombieDestroyCooldown;

            if (!CooldownDoors.Contains(doorCell))
            {
                CooldownDoors.Add(doorCell);
            }

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
                rect = door.Rotated ? openDoorRectangleVertical : openDoorRectangleHorizontal;
            }
            else
            {
                rect = door.Rotated ? closedDoorRectangleVertical : closedDoorRectangleHorizontal;
            }
            Color color = Color.White;
            if (door.ZombieDestroyCooldown > 0)
            {
                color = new(0.3f, 0.3f, 0.3f);
            }
            spriteBatch.Draw(tilesetTexture, doorPos, rect, color);



            if (!door.IsOpen && door.GetBarricadeLevel() > 0)
            {
                DrawDoorBarricade(spriteBatch, doorPos, door);
            }
        }

        private void DrawDoorBarricade(SpriteBatch spriteBatch, Vector2 doorPos, Door door)
        {
            Rectangle rect = door.GetBarricadeLevel() switch
            {
                1 => door.Rotated ? Level1BarricadeVertical : Level1BarricadeHorizontal,
                2 => door.Rotated ? Level2BarricadeVertical : Level2BarricadeHorizontal,
                3 => door.Rotated ? Level3BarricadeVertical : Level3BarricadeHorizontal,
                _ => new(),
            };
            spriteBatch.Draw(tilesetTexture, doorPos, rect, Color.White);
        }

    }
}