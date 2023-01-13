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

        private const int maxBlood = 100;

        private Texture2D bloodTexture;

        private readonly (Vector2, float)[] bloodArr = new (Vector2, float)[maxBlood];
        private int bloodIndex = 0;
        private int bloodCount = 0;

        public BloodManager()
        {
            bloodTexture = Game1.GetTexture("bloodsplat");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void AddBlood(Vector2 pos, float rotation)
        {
            bloodArr[bloodIndex] = (pos, rotation);

            bloodIndex++;
            bloodCount++;
            if (bloodIndex >= maxBlood)
            {
                bloodIndex = 0;
                bloodCount = maxBlood;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            for (int i = 0; i < bloodCount; i++)
            {
                var blood = bloodArr[i];
                spriteBatch.Draw(bloodTexture, blood.Item1, new(0, 0, 128, 128), Color.White, blood.Item2, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            }
        }

    }
}