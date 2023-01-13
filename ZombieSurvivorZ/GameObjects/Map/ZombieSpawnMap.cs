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

        public ZombieSpawnMap(TiledMapTileLayer layer) : base(layer)
        {
            Zombies.Add(new()
            {
                Position = new(32, 32),
                Scale = new(0.25f, 0.25f)
            });
        }

        public bool ZombieIsBlockingDoorClosing(Vector2Int doorCell)
        {
            for (int i = 0; i < Zombies.Count; i++)
            {
                Zombie zombie = Zombies[i];
                Vector2Int zombieCell = Game1.MapManager.PositionToLocal(zombie.Position);
                if (zombieCell == doorCell)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsTileWalkable(ushort x, ushort y, out float cost)
        {
            cost = 0;
            return true;
        }

    }
}