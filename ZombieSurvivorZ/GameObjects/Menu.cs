/********************************************
Course : TGD3351 Game Algorithms
Session: Trimester 1, 2022/23
ID and Name #1 : 1191101213 RavenLimZheXuan
Contacts #1 : 601155873318 1191101213@student.mmu.edu.my
ID and Name #2 : 1181103109 EuwernYongChernJun
Contacts #2 : 60163371078 1181103109@student.mmu.edu.my
********************************************/
using Microsoft.Xna.Framework;

namespace ZombieSurvivorZ
{
    public class Menu : SpriteObject
    {

        public UIButton StartGame;
        public UIButton Quit;

        public Menu()
        {
            Game1.Current.SetCursorVisible(true);
            Vector2 pos = Game1.ScreenCenter;
            Vector2 size = new(300, 100);
            pos.X -= size.X / 2;
            pos.Y += 200;

            StartGame = UIButton.CreateWithText(null, pos, size, "START");
            StartGame.OnClick += () =>
            {
                Game1.Current.SetCursorVisible(true);
                Game1.Current.StartGame();
            };

            StartGame.fill.Color = Color.Gray;
            StartGame.text.Color = Color.Red;
            StartGame.border.Color = Color.Black;


            Quit = UIButton.CreateWithText(null, new(Game1.ScreenSize.X - 100, Game1.ScreenSize.Y - 100), new(80, 40), "QUIT");
            Quit.OnClick += () =>
            {
                Game1.Current.Exit();
            };
            Quit.fill.Color = Color.DarkGray;
            Quit.text.Color = Color.Black;
            Quit.border.Color = Color.Black;
        }

        public override void Initialize()
        {
            Position = new(0, 0);
            Origin = new(0, 0);
            Texture = Game1.GetTexture("Cover");
            base.Initialize();
        }

        public override void Destroy()
        {
            base.Destroy();
            StartGame.Destroy();
            Quit.Destroy();
        }


    }
}
