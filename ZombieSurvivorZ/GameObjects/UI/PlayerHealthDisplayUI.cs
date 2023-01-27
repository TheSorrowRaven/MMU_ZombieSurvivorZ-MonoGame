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
    public class PlayerHealthDisplayUI : UIBox
    {

        public readonly UIBorder border;
        public readonly UIText text;

        public float originalSize;

        public PlayerHealthDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Color = Color.DarkRed;
            originalSize = size.X;

            border = new(this, new(0, 0), size);
            border.Color = Color.Black;

            text = new(this, new(0, 0), size);
            text.Align = new(0.5f, 0.5f);
            text.Color = Color.AntiqueWhite;
        }

        public void HealthUpdated(int health, int maxHealth)
        {
            float size = (float)health / maxHealth * originalSize;
            Size = new(size, Size.Y);
            text.Text = $"{health} / {maxHealth}";
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            border.SetActive(active);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Destroy()
        {
            base.Destroy();
            border.Destroy();
            text.Destroy();
        }
    }
}