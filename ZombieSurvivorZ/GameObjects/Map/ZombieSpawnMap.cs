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

        public class Wave
        {
            public bool isGrace;
            public int zombies;
            public float period;

        }

        private const int minZombies = 1;

        public readonly List<Zombie> Zombies = new();
        public readonly List<Vector2Int> SpawnPoints = new();
        public readonly Wave[] waves = new Wave[]
        {
            new Wave()
            {
                //prep
                isGrace = true,
                period = 20f,
            },
            new Wave()
            {
                zombies = 6,
                period = 20f
            },
            new Wave()
            {
                isGrace = true,
                period = 10f,
            },
            new Wave()
            {
                zombies = 100,
                period = 30f
            },
        };

        private int waveIndex;
        private Wave currentWave;

        public bool allZombiesKilled = false;
        private bool isGrace;
        private float waveTimerCount;
        private int waveZombiesToSpawn;

        private float zombieSpawnCooldown;
        private float zombieSpawnCooldownCount;

        public ZombieSpawnMap(TiledMapTileLayer layer) : base(layer)
        {
            SpawnZombie(new(32, 32));

        }

        public override void Initialize()
        {
            base.Initialize();

            waveIndex = -1;
            NextWave();
        }

        public override void Update()
        {
            base.Update();

            waveTimerCount -= Time.deltaTime;

            if (isGrace)
            {

            }
            else
            {
                if (waveZombiesToSpawn > 0)
                {
                    zombieSpawnCooldownCount -= Time.deltaTime;
                    if (zombieSpawnCooldownCount < 0)
                    {
                        zombieSpawnCooldownCount = zombieSpawnCooldown;
                        SpawnRandomZombie();
                        waveZombiesToSpawn--;
                    }
                }
            }

            if (waveTimerCount < 0)
            {
                if (isGrace)
                {
                    NextWave();
                }
                else
                {
                    waveTimerCount = 0;
                    if (!MinimumZombiesReachedToProgress())
                    {
                        Game1.HUDDisplayUI.TimerDisplayUI.SetTimerColor(Color.Red);
                        Game1.HUDDisplayUI.TimerDisplayUI.SetTimerText($"Zombies Left: {Zombies.Count}");
                    }
                    else
                    {
                        NextWave();
                    }
                }
            }

            if (waveTimerCount > 0)
            {
                if (isGrace)
                {
                    if (waveTimerCount < 10f)
                    {
                        Game1.HUDDisplayUI.TimerDisplayUI.SetTimerColor(Color.Red);
                    }
                    else
                    {
                        Game1.HUDDisplayUI.TimerDisplayUI.SetTimerColor(Color.White);
                    }
                    TimeSpan timeSpan = new(0, 0, 0, 0, (int)(waveTimerCount * 1000));
                    Game1.HUDDisplayUI.TimerDisplayUI.SetTimer(timeSpan);
                }
            }


        }

        private void NextWave()
        {
            waveIndex++;
            if (waveIndex >= waves.Length)
            {
                //TODO repeat?
                waveIndex = 0;
            }
            currentWave = waves[waveIndex];

            waveZombiesToSpawn = currentWave.zombies;
            waveTimerCount = currentWave.period;
            isGrace = currentWave.isGrace;
            allZombiesKilled = waveZombiesToSpawn > 0;

            zombieSpawnCooldown = waveTimerCount / waveZombiesToSpawn;
            zombieSpawnCooldownCount = zombieSpawnCooldown;

            if (!isGrace)
            {
                Game1.HUDDisplayUI.TimerDisplayUI.SetTimerColor(Color.Red);
                Game1.HUDDisplayUI.TimerDisplayUI.SetTimerText("Zombies Incoming!");
            }
        }

        public bool MinimumZombiesReachedToProgress()
        {
            return Zombies.Count <= minZombies;
        }

        public void ZombieDied(Zombie zombie)
        {
            Zombies.Remove(zombie);
        }

        private void SpawnRandomZombie()
        {
            Vector2Int spawnPoint = SpawnPoints[Random.Shared.Next(SpawnPoints.Count)];
            Vector2 position = MapManager.LocalToTileCenterPosition(spawnPoint);
            SpawnZombie(position);
        }

        public void SpawnZombie(Vector2 position)
        {
            if (Random.Shared.NextSingle() > 0.1f)
            {
                Zombies.Add(new Zombie()
                {
                    Position = position
                });
            }
            else
            {
                Zombies.Add(new BabyZombie()
                {
                    Position = position
                });
            }
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