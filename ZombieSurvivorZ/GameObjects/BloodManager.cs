using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class BloodManager : SpriteObject
    {

        public override World World => World.floor;

        private const int maxBlood = 1000;

        private Texture2D humanBloodTexture;
        private Texture2D zombieBloodTexture;

        private Rectangle rect = new(0, 0, 128, 128);
        private Vector2 origin = new(64, 64);
        private Vector2 scale = new(0.4f, 0.4f);

        private readonly (Vector2, float)[] humanBloodArr = new (Vector2, float)[maxBlood];
        private int humanBloodIndex = 0;
        private int humanBloodCount = 0;

        private readonly (Vector2, float)[] zombieBloodArr = new (Vector2, float)[maxBlood];
        private int zombieBloodIndex = 0;
        private int zombieBloodCount = 0;


        public BloodManager()
        {
            humanBloodTexture = Game1.GetTexture("bloodsplat");
            zombieBloodTexture = Game1.GetTexture("bloodsplat_zombie");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void AddHumanBlood(Vector2 pos, float rotation)
        {
            humanBloodArr[humanBloodIndex] = (pos, rotation + (45 * MathF.PI / 180));

            humanBloodIndex++;
            humanBloodCount = Math.Min(humanBloodCount + 1, maxBlood);
            if (humanBloodIndex >= maxBlood)
            {
                humanBloodIndex = 0;
                humanBloodCount = maxBlood;
            }
        }
        public void AddZombieBlood(Vector2 pos, float rotation)
        {
            zombieBloodArr[zombieBloodIndex] = (pos, rotation + (45 * MathF.PI / 180));

            zombieBloodIndex++;
            zombieBloodCount = Math.Min(zombieBloodCount + 1, maxBlood);
            if (zombieBloodIndex >= maxBlood)
            {
                zombieBloodIndex = 0;
                zombieBloodCount = maxBlood;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < zombieBloodCount; i++)
            {
                var blood = zombieBloodArr[i];
                spriteBatch.Draw(zombieBloodTexture, blood.Item1, rect, Color.White, blood.Item2, origin, scale, SpriteEffects.None, 0);
            }
            for (int i = 0; i < humanBloodCount; i++)
            {
                var blood = humanBloodArr[i];
                spriteBatch.Draw(humanBloodTexture, blood.Item1, rect, Color.White, blood.Item2, origin, scale, SpriteEffects.None, 0);
            }
        }

    }
}