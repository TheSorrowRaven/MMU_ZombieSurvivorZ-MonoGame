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
    public class UIText : UIBase
    {

        private readonly bool fontUndefined;

        private SpriteFont spriteFont;
        public SpriteFont SpriteFont
        {
            get => spriteFont;
            set
            {
                spriteFont = value;
                StringSize = spriteFont.MeasureString(text);
            }
        }

        private Vector2 StringSize;

        public Vector2 Align = new(0.5f, 0.5f);

        private string text = "";
        public string Text
        {
            get => text;
            set
            {
                text = value;
                StringSize = spriteFont.MeasureString(text);
            }
        }

        public UIText(UIBase parent, Vector2 pos, Vector2 size, SpriteFont font = null) : base(parent, pos, size)
        {
            if (font == null)
            {
                SpriteFont = Font.Biggest;
                fontUndefined = true;
            }
            else
            {
                SpriteFont = font;
                fontUndefined = false;
            }

            Color = Color.Black;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        private Vector2 GetActualPosition()
        {
            Vector2 pos = GetOffset();
            float relScale = MathF.Max(StringSize.X / Scale.X, StringSize.Y / Scale.Y);
            if (!fontUndefined)
            {
                if (relScale > 1)
                {
                    Console.WriteLine($"Text cannot fit! Required:{StringSize}, Size:{Scale}");
                    return pos;
                }
            }
            else
            {
                if (relScale > 0.9f)
                {
                    SpriteFont = Font.Smaller(SpriteFont);
                }
            }
            Vector2 diff = Scale - StringSize;
            return pos + (diff * Align);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, Text, GetActualPosition(), Color, Rotation, Origin, Vector2.One, SpriteEffects.None, RenderOrder);
        }

    }
}