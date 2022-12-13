using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ZombieSurvivorZ
{
    public class Cursor : SpriteObject
    {

        private float spread = 0;
        private float minSpread = 32;
        private float Spread => spread + minSpread * Scale.X;

        public override World World => World.UI;

        public override void Initialize()
        {
            Texture = Game1.GetTexture("cursor_piece");
            Scale = new(0.5f, 0.5f);
        }

        public override void Update()
        {
            Position = Input.MousePos;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawCursorPiece(spriteBatch, new(0, Spread), 180 * MathF.PI / 180);
            DrawCursorPiece(spriteBatch, new(0, -Spread), 0);
            DrawCursorPiece(spriteBatch, new(Spread, 0), 90 * MathF.PI / 180);
            DrawCursorPiece(spriteBatch, new(-Spread, 0), -90 * MathF.PI / 180);
        }

        private void DrawCursorPiece(SpriteBatch spriteBatch, Vector2 offset, float rotation)
        {
            spriteBatch.Draw(Texture, Position + offset, null, Color, Rotation + rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);
        }

        public void SetSpread(float value)
        {
            spread = value;
        }
    }
}