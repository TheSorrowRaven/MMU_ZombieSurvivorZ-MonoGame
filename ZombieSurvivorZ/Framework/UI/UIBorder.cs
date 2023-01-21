using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace ZombieSurvivorZ
{
    public class UIBorder : UIBase
    {

        public float Thickness { get; set; } = 1f;

        //Not recommended to use UIBorder as a parent
        public UIBorder(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Color = Color.Black;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new(GetOffset(), Scale), Color, Thickness);
        }

    }
}