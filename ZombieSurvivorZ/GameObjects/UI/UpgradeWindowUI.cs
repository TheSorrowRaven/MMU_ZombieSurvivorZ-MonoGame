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
        public UIText title;
        public readonly List<WeaponUpgradeUI> weaponUpgradeUIs = new();

        private int YWeaponUpgradesOffset = 60;
        private int YWeaponUpgradesSpacing = 10;

        public UpgradeWindowUI() : base(null, new(Game1.ScreenSize.X / 2 - 350, Game1.ScreenSize.Y / 2 - 300), new(300, 500))
        {
            Alpha = 0.8f;
            Color = Color.Black;

            border = new(this, Vector2.Zero, Size);
            border.Color = Color.Gray;

            title = new(this, new(0, 0), new(200, 50));
            title.Align = new(0.5f, 0.5f);
            title.Text = "Weapon Purchase";
            title.Color = Color.White;

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

        public override void Destroy()
        {
            base.Destroy();
            border.Destroy();
            title.Destroy();
            for (int i = 0; i < weaponUpgradeUIs.Count; i++)
            {
                weaponUpgradeUIs[i].Destroy();
            }
        }

    }
}