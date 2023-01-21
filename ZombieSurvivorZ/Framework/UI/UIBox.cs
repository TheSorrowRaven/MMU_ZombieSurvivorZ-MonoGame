using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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