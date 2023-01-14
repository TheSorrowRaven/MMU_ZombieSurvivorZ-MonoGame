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
    public class ZombieSpawnMap : BaseMap
    {

        public readonly List<Zombie> Zombies = new();
        public readonly List<Vector2Int> SpawnPoints = new();

        public ZombieSpawnMap(TiledMapTileLayer layer) : base(layer)
        {
            SpawnZombie(new(32, 32));
        }

        public override void Update()
        {
            base.Update();

            if (Time.frameCount % 30 == 0)
            {
                Vector2Int spawnPoint = SpawnPoints[Random.Shared.Next(SpawnPoints.Count)];
                Vector2 position = MapManager.LocalToTileCenterPosition(spawnPoint);
                SpawnZombie(position);
                Console.WriteLine(Zombies.Count);
            }

        }

        public void ZombieDied(Zombie zombie)
        {
            Zombies.Remove(zombie);
        }

        public void SpawnZombie(Vector2 position)
        {
            Zombies.Add(new()
            {
                Position = position,
                Scale = new(0.25f, 0.25f)
            });
        }

        public bool ZombieIsBlockingDoorClosing(Vector2Int doorCell)
        {
            for (int i = 0; i < Zombies.Count; i++)
            {
                Zombie zombie = Zombies[i];
                if (zombie.CellPosition == doorCell)
                {
                    return true;
                }
            }
            return false;
        }


        protected override void InitializeTile(int x, int y, TiledMapTile tile)
        {
            base.InitializeTile(x, y, tile);

            SpawnPoints.Add(new(x, y));
        }

        public override bool IsTileWalkable(ushort x, ushort y, out float cost)
        {
            cost = 0;
            return true;
        }

    }
}