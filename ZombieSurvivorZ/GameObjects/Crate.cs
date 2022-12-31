using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;

namespace ZombieSurvivorZ
{
    public class Crate : SpriteObject
    {

        private Collision.BoxStaticCollider CrateCL;

        public Crate()
        {
            Texture = Game1.GetTexture("Objects/crate");
        }

        public override void Initialize()
        {
            CrateCL = new Collision.BoxStaticCollider(this, 32, 32);
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Game1.CollisionDebugging)
            {
                spriteBatch.DrawRectangle((RectangleF)CrateCL.Bounds, Color.Blue);
            }
        }

    }
}