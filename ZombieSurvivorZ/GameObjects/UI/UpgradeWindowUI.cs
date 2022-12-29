using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;

namespace ZombieSurvivorZ
{
    public class UpgradeWindowUI : UIBox
    {

        public UpgradeWindowUI() : base(null, new(400, 200), new(200, 500))
        {
            Alpha = 0.4f;
            Color = Color.Red;
            UIBorder border = new(this, new(0, 0), new(200, 500));
            UIButton button = UIButton.CreateWithText(this, new(50, 50), new(100, 50), "Purchase");
            button.OnClick += () =>
            {
                Console.WriteLine("Button Click");
                Game1.Player.RemoveMaterials(5);
            };
            UIText title = new(this, new(0, 0), new(200, 50));
            title.Text = "Hello World";
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();
            if (MouseWithinBounds())
            {
                Game1.UISuppressClick = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

    }
}