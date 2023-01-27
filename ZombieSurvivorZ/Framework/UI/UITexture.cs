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
    public class UITexture : UIBase
    {

        private Vector2 actualSize;

        public override Vector2 Size
        {
            get
            {
                return actualSize;
            }
            set
            {
                actualSize = value;
                Vector2 actualScale = actualSize / new Vector2(Texture.Width, Texture.Height);
                Scale = actualScale;
            }
        }

        public new Texture2D Texture
        {
            get => base.Texture;
            set
            {
                base.Texture = value;
                Vector2 actualScale = actualSize / new Vector2(Texture.Width, Texture.Height);
                Scale = actualScale;
            }
        }

        public UITexture(UIBase parent, Vector2 pos, Vector2 size, Texture2D texture) : base(parent, pos, size / new Vector2(texture.Width, texture.Height))
        {
            actualSize = size;
            Texture = texture;
            Color = Color.White;
        }

    }
}