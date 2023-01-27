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

namespace ZombieSurvivorZ
{
    public class SpriteObject : GameObject
    {
        private Texture2D _texture;

        // Using C# property allows us to do something after setting values!
        public Texture2D Texture
        {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;
                Recalculate();
            }
        }

        private Color DrawColor { get; set; } = Color.White;


        private byte alpha = 255;
        public float Alpha
        {
            get => alpha / 255f;
            set
            {
                float val = value;
                if (val < 0) val = 0;
                else if (val > 1) val = 1;
                alpha = (byte)(val * 255);
                UpdateColor();
            }
        }

        private Color _color;
        public Color Color
        {
            get
            {
                return DrawColor;
            }
            set
            {
                Color c = value;
                c.A = 255;
                _color = c;
                UpdateColor();
                ColorUpdated(c);
            }
        }

        private void UpdateColor()
        {
            DrawColor = _color * Alpha;
        }

        protected virtual void ColorUpdated(Color color)
        {

        }


        // It is far easier for developers to set origin in normalized values
        // Center is (0.5, 0.5) instead of having to query texture bounds
        private Vector2 _origin = new(0.5f, 0.5f);

        // Using C# property allows us to do something after setting values!
        public Vector2 Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
                Recalculate();
            }
        }

        public int RenderOrder { get; set; } = 1;

        // Internally, this is the origin used when drawing.
        private Vector2 originPixels;
        public Vector2 OriginPixels => originPixels;

        private Rectangle bounds;

        public SpriteObject()
        {
            _color = Color;
        }

        private void Recalculate()
        {
            if (_texture == null)
            {
                originPixels.X = originPixels.Y = 0;
                bounds.Width = bounds.Height = 0;
                return;
            }

            Rectangle textureBounds = Texture.Bounds;
            originPixels = new Vector2(
                textureBounds.X + textureBounds.Width * Origin.X,
                textureBounds.Y + textureBounds.Height * Origin.Y
                );

            bounds.Width = Texture.Width;
            bounds.Height = Texture.Height;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);
        }
    }
}