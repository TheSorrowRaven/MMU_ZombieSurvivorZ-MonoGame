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

        public Color Color { get; set; } = Color.White;

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

        public int RenderOrder { get; set; } = 0;

        // Internally, this is the origin used when drawing.
        private Vector2 originPixels;
        public Vector2 OriginPixels => originPixels;

        private Rectangle bounds;

        public SpriteObject()
        {

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

        public override Rectangle Bounds()
        {
            Vector2 pos = Position - originPixels;
            bounds.X = (int)pos.X;
            bounds.Y = (int)pos.Y;
            return bounds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);
        }
    }
}