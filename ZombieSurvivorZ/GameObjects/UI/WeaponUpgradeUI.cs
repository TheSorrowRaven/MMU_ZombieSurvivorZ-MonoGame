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
    public class WeaponUpgradeUI : UIBox
    {

        public readonly UITexture WeaponTexture;
        public readonly UIText WeaponNumber;
        public readonly UIText WeaponName;

        public readonly UIButton AmmoPurchase;
        public readonly UIText AmmoPurchaseMaterials;
        public readonly UITexture AmmoPurchaseTexture;

        public readonly UIButton WeaponPurchase;
        public readonly UIText WeaponPurchaseMaterials;
        public readonly UITexture WeaponPurchaseTexture;

        //Forces Size
        public static new Vector2 Size = new(275, 100);

        public WeaponUpgradeUI(UIBase parent, Vector2 pos) : base(parent, pos, Size)
        {
            Alpha = 0.4f;
            Color = Color.LightSlateGray;

            //UIBorder border = new(this, new(0, 0), Size);
            //border.Color = Color.DarkSlateGray;

            //80,100 size before purchase & ammo
            WeaponTexture = new(this, new(16, 8), new(64, 64), Game1.GetTexture("pistol_texture"));

            WeaponNumber = new(this, new(12, 4), new(16, 16));
            WeaponNumber.Text = "1";

            WeaponName = new(WeaponTexture, new(0, WeaponTexture.Size.Y), new(WeaponTexture.Size.X, 20));
            WeaponName.Text = "Pistol";
            WeaponName.Align = new(0.5f, 0f);

            WeaponPurchase = UIButton.Create(this, new(88, 10), new(104, 80));
            WeaponPurchaseMaterials = new(WeaponPurchase, new(5, 16), new(48, 48));
            WeaponPurchaseMaterials.Text = "500";
            WeaponPurchaseTexture = new(WeaponPurchase, new(51, 16), new(48, 48), Game1.GetTexture("icon_purchase"));

            AmmoPurchase = UIButton.Create(this, new(192, 29), new(75, 42));
            AmmoPurchaseTexture = new(AmmoPurchase, new(5, 5), new(32, 32), Game1.GetTexture("icon_ammo"));
            AmmoPurchaseMaterials = new(AmmoPurchase, new(37, 5), new(32, 32));
            AmmoPurchaseMaterials.Text = "50";


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