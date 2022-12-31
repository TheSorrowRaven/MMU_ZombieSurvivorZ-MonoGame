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
    public class HUDDisplayUI : UIBox
    {

        private readonly UIBox BottomRightContainer;
        private readonly UIBox BottomRightLineSeparator;
        public readonly MaterialsDisplayUI MaterialsDisplayUI;
        public readonly AmmoDisplayUI AmmoDisplayUI;

        public HUDDisplayUI() : base(null, new(0, 0), new(Game1.ScreenSize.X, Game1.ScreenSize.Y))
        {
            Vector2 size = new(200, 100);
            BottomRightContainer = new(this, new(Game1.ScreenSize.X - size.X, Game1.ScreenSize.Y - size.Y), size);
            //BottomRightContainer.Active = false;
            //Improve this?
            BottomRightContainer.Color = Color.White;
            BottomRightContainer.Alpha = 0.1f;

            Vector2 lSize = new(70, 2);
            BottomRightLineSeparator = new(BottomRightContainer, (size / 2) - (lSize / 2), lSize);
            BottomRightLineSeparator.Color = Color.Black;

            AmmoDisplayUI = new(BottomRightContainer, new(0, 0), new(200, 50));
            MaterialsDisplayUI = new(BottomRightContainer, new(0, 50), new(200, 50));
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            BottomRightContainer.SetActive(active);
            BottomRightLineSeparator.SetActive(active);
            MaterialsDisplayUI.SetActive(active);
            AmmoDisplayUI.SetActive(active);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}