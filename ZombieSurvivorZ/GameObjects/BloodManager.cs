using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ZombieSurvivorZ
{
    public class BloodManager : SpriteObject
    {

        public override World World => World.floor;

        private const int humanMaxBlood = 1000;
        private const int zombieMaxBlood = 1000;
        private const float rotate = 45 * MathF.PI / 180;

        private readonly Texture2D humanBloodTexture;
        private readonly Texture2D zombieBloodTexture;

        private Rectangle rect = new(0, 0, 128, 128);
        private Vector2 origin = new(64, 64);
        private Vector2 scale = new(0.4f, 0.4f);

        private readonly (Vector2, float)[] humanBloodArr = new (Vector2, float)[humanMaxBlood];
        private int humanBloodIndex = 0;
        private int humanBloodCount = 0;

        private readonly (Vector2, float)[] zombieBloodArr = new (Vector2, float)[zombieMaxBlood];
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
            humanBloodArr[humanBloodIndex] = (pos, rotation + rotate);

            humanBloodIndex++;
            humanBloodCount = Math.Min(humanBloodCount + 1, humanMaxBlood);
            if (humanBloodIndex >= humanMaxBlood)
            {
                humanBloodIndex = 0;
                humanBloodCount = humanMaxBlood;
            }
        }
        public void AddZombieBlood(Vector2 pos, float rotation)
        {
            zombieBloodArr[zombieBloodIndex] = (pos, rotation + rotate);

            zombieBloodIndex++;
            zombieBloodCount = Math.Min(zombieBloodCount + 1, zombieMaxBlood);
            if (zombieBloodIndex >= zombieMaxBlood)
            {
                zombieBloodIndex = 0;
                zombieBloodCount = zombieMaxBlood;
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