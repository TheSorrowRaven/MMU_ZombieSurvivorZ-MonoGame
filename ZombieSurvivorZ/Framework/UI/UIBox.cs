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
    public class UIBox : UIBase
    {

        public UIBox(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Color = Color.LightGray;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

    }
}