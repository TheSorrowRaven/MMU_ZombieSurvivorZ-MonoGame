using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ZombieSurvivorZ
{
    public class Reticle : SpriteObject
    {

        private float spread = 0;
        private float minSpread = 32;
        private float Spread => spread + minSpread * Scale.X;

        public override World World => World.UI;

        private bool enabled = false;

        public override void Initialize()
        {
            Texture = Game1.GetTexture("cursor_piece");
            Scale = new(0.5f, 0.5f);
        }

        public void Enable()
        {
            enabled = true;
            Game1.Current.SetCursorVisible(false);
        }
        public void Disable()
        {
            enabled = false;
            Game1.Current.SetCursorVisible(true);
        }

        public void SetSpread(float value)
        {
            spread = value;
        }

        public override void Update()
        {
            Position = Input.MousePos;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!enabled)
            {
                return;
            }
            DrawCursorPiece(spriteBatch, new(0, Spread), 180 * MathF.PI / 180);
            DrawCursorPiece(spriteBatch, new(0, -Spread), 0);
            DrawCursorPiece(spriteBatch, new(Spread, 0), 90 * MathF.PI / 180);
            DrawCursorPiece(spriteBatch, new(-Spread, 0), -90 * MathF.PI / 180);
        }

        private void DrawCursorPiece(SpriteBatch spriteBatch, Vector2 offset, float rotation)
        {
            spriteBatch.Draw(Texture, Position + offset, null, Color, Rotation + rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);
        }
    }
}