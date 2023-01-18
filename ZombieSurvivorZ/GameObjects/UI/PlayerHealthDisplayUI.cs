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
    public class PlayerHealthDisplayUI : UIBox
    {

        public readonly UIBorder border;
        public readonly UIText text;

        public float originalSize;

        public PlayerHealthDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Color = Color.DarkRed;
            originalSize = size.X;

            border = new(this, new(0, 0), size);
            border.Color = Color.Black;

            text = new(this, new(0, 0), size);
            text.Align = new(0.5f, 0.5f);
            text.Color = Color.AntiqueWhite;
        }

        public void HealthUpdated(int health, int maxHealth)
        {
            float size = (float)health / maxHealth * originalSize;
            Size = new(size, Size.Y);
            text.Text = $"{health} / {maxHealth}";
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            border.SetActive(active);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Destroy()
        {
            base.Destroy();
            border.Destroy();
            text.Destroy();
        }
    }
}