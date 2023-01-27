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

namespace ZombieSurvivorZ
{
    public class UIBorderedBox : UIBorder
    {

        public UIBox Box;

        public UIBorderedBox(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Color = Color.Black;
            Box = new(this, new(0, 0), size);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            Box.SetActive(active);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new(GetOffset(), Scale), Color, Thickness);
        }

    }
}