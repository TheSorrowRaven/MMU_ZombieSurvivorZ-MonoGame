/********************************************
Course : TGD3351 Game Algorithms
Session: Trimester 1, 2022/23
ID and Name #1 : 1191101213 RavenLimZheXuan
Contacts #1 : 601155873318 1191101213@student.mmu.edu.my
ID and Name #2 : 1181103109 EuwernYongChernJun
Contacts #2 : 60163371078 1181103109@student.mmu.edu.my
********************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

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

        public Vector2 StringSize { get; private set; }

        public Vector2 Align = new(0.5f, 0.5f);

        private string text = "";
        public string Text
        {
            get => text;
            set
            {
                text = value;
                StringSize = spriteFont.MeasureString(text);
                if (fontUndefined)
                {
                    SpriteFont = Font.Biggest;
                }
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
            return pos + GetLocalAlignedPosition();
        }
        private Vector2 GetLocalAlignedPosition()
        {
            float relScale = MathF.Max(StringSize.X / Scale.X, StringSize.Y / Scale.Y);
            if (!fontUndefined)
            {
                if (relScale > 1)
                {
                    Console.WriteLine($"Text cannot fit! Required:{StringSize}, Size:{Scale}");
                    return Vector2.Zero;
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
            return diff * Align;
        }

        public RectangleF GetTextBounds()
        {
            Vector2 pos = GetActualPosition();
            Vector2 size = StringSize;
            return new(pos, size);
        }

        public RectangleF GetTextLocalBounds()
        {
            Vector2 pos = GetLocalAlignedPosition();
            Vector2 size = StringSize;
            return new(pos, size);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, Text, GetActualPosition(), Color, Rotation, Origin, Vector2.One, SpriteEffects.None, RenderOrder);
        }

    }
}