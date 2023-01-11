using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class UpgradeWindowUI : UIBox
    {

        public UIBorder border;
        public UIButton button;
        public UIText title;
        public readonly List<WeaponUpgradeUI> weaponUpgradeUIs = new();

        private int YWeaponUpgradesOffset = 200;
        private int YWeaponUpgradesSpacing = 10;

        public UpgradeWindowUI() : base(null, new(200, 100), new(300, 500))
        {
            Alpha = 0.4f;
            Color = Color.Red;
            border = new(this, Vector2.Zero, Size);
            button = UIButton.CreateWithText(this, new(50, 50), new(100, 50), "Purchase");
            button.OnClick += () =>
            {
                Console.WriteLine("Button Click");
                //this.SetActive(false);
            };
            title = new(this, new(0, 0), new(200, 50));
            title.Text = "Hello World";

        }

        public void Player_AddWeaponToList(Weapon weapon)
        {
            WeaponUpgradeUI weaponUpgradeUI = new(
                this,
                new((Size.X - WeaponUpgradeUI.Size.X) / 2, YWeaponUpgradesOffset + (WeaponUpgradeUI.Size.Y + YWeaponUpgradesSpacing) * weaponUpgradeUIs.Count),
                weapon,
                weaponUpgradeUIs.Count + 1
                );
            weaponUpgradeUIs.Add(weaponUpgradeUI);
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            border.SetActive(active);
            button.SetActive(active);
            title.SetActive(active);
            for (int i = 0; i < weaponUpgradeUIs.Count; i++)
            {
                weaponUpgradeUIs[i].SetActive(active);
            }
        }

        public override void Update()
        {
            base.Update();
            if (!Active)
            {
                return;
            }
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