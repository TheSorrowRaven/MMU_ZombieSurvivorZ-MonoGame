using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class ZombieSpawnMap : BaseMap
    {

        private const int minZombies = 0;

        private const float gracePeriod = 20f;
        private const int zombiesIncrease = 10;
        private const float zombiesSpawnTime = 10f;

        public readonly List<Zombie> Zombies = new();
        public readonly List<Vector2Int> SpawnPoints = new();

        private int waveCount;

        public bool allZombiesKilled = false;
        private bool isGrace;
        private float waveTimerCount;
        private int waveZombiesToSpawn;

        private float zombieSpawnCooldown;
        private float zombieSpawnCooldownCount;

        public int zombiesKilled = 0;

        public ZombieSpawnMap(TiledMapTileLayer layer) : base(layer)
        {
            //SpawnZombie(new(32, 32));

        }

        public override void Initialize()
        {
            base.Initialize();

            NextWave();
        }

        public override void Update()
        {
            base.Update();

            waveTimerCount -= Time.deltaTime;

            if (isGrace)
            {
                Game1.Player.IsGracePeriodUpdate();
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
                    //wait for all zombies to be killed
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
            Game1.Player.RoundStartSE.Play();
            if (waveCount % 2 == 0)
            {
                isGrace = true;
                waveTimerCount = gracePeriod;
                waveZombiesToSpawn = 0;
            }
            else
            {
                isGrace = false;
                waveTimerCount = zombiesSpawnTime;
                waveZombiesToSpawn = ((waveCount + 1) / 2) * zombiesIncrease;
            }
            waveCount++;

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
            zombiesKilled++;
        }

        private void SpawnRandomZombie()
        {
            Vector2Int spawnPoint = SpawnPoints[Random.Shared.Next(SpawnPoints.Count)];
            Vector2 position = MapManager.LocalToTileCenterPosition(spawnPoint);
            SpawnZombie(position);
        }

        public Vector2 GetRandomSpawnPosition()
        {
            Vector2Int spawnPoint = SpawnPoints[Random.Shared.Next(SpawnPoints.Count)];
            Vector2 position = MapManager.LocalToTileCenterPosition(spawnPoint);
            return position;
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

        public override void Destroy()
        {
            base.Destroy();
            for (int i = 0; i < Zombies.Count; i++)
            {
                Zombies[i].Destroy();
            }
        }

    }
}