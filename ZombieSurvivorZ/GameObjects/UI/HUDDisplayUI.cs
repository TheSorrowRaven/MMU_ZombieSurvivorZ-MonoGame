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
    public class HUDDisplayUI : UIBox
    {

        private readonly UIBox BottomRightContainer;
        private readonly UIBox BottomRightLineSeparator;
        public readonly MaterialsDisplayUI MaterialsDisplayUI;
        public readonly AmmoDisplayUI AmmoDisplayUI;

        public readonly DoorHealthDisplayUI DoorHealthDisplayUI;
        public readonly PlayerHealthDisplayUI PlayerHealthDisplayUI;

        public readonly TimerDisplayUI TimerDisplayUI;

        public HUDDisplayUI() : base(null, new(0, 0), new(Game1.ScreenSize.X, Game1.ScreenSize.Y))
        {
            Vector2 size = new(200, 100);
            BottomRightContainer = new(this, new(Game1.ScreenSize.X - size.X, Game1.ScreenSize.Y - size.Y), size);
            BottomRightContainer.SetActive(false);

            Vector2 lSize = new(70, 2);
            BottomRightLineSeparator = new(BottomRightContainer, (size / 2) - (lSize / 2), lSize);
            BottomRightLineSeparator.Color = Color.Black;

            AmmoDisplayUI = new(BottomRightContainer, new(0, 0), new(200, 50));
            MaterialsDisplayUI = new(BottomRightContainer, new(0, 50), new(200, 50));

            DoorHealthDisplayUI = new(this, new(0, 0), new(64, 20));
            DoorHealthDisplayUI.SetActive(false);

            PlayerHealthDisplayUI = new(this, new(20, Game1.ScreenSize.Y - 60), new(250, 40));
            TimerDisplayUI = new(this, new(Game1.ScreenSize.X / 2 - 200, 8), new(400, 64));
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            BottomRightContainer.SetActive(active);
            BottomRightLineSeparator.SetActive(active);
            MaterialsDisplayUI.SetActive(active);
            AmmoDisplayUI.SetActive(active);
            DoorHealthDisplayUI.SetActive(active);
            PlayerHealthDisplayUI.SetActive(active);
            TimerDisplayUI.SetActive(active);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

        public override void Destroy()
        {
            base.Destroy();
            BottomRightContainer.Destroy();
            BottomRightLineSeparator.Destroy();
            MaterialsDisplayUI.Destroy();
            AmmoDisplayUI.Destroy();
            DoorHealthDisplayUI.Destroy();
            PlayerHealthDisplayUI.Destroy();
            TimerDisplayUI.Destroy();
        }

    }
}