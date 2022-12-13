using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace ZombieSurvivorZ
{
    public class Pistol : Weapon
    {

        public Pistol()
        {
            FireTime = 0.2f;
            RecoilTime = 0.05f;
            ReloadTime = 2.2f;
            SwitchTime = 0.3f;
            ClipSize = 9;
            CanAutoFire = false;

            AmmoInClip = 9;
            AmmoReserve = 14;

            CursorSpreadIncrease = 5.0f;
            CursorSpreadDecrease = 50.0f;
            CursorSpreadTime = 0.3f;
        }

        public override void Initialize()
        {
            PlayerBodyTextureHolding = Game1.GetTexture("Player/player_body_holdingpistol");
            PlayerBodyTextureRecoiling = Game1.GetTexture("Player/player_body_recoilingpistol");
            PlayerBodyTextureReloading = Game1.GetTexture("Player/player_body_reloadingpistol");

            WeaponTexture = Game1.GetTexture("Player/player_pistol");
            WeaponRecoilTexture = Game1.GetTexture("Player/player_recoilingpistol");
            WeaponReloadTexture = Game1.GetTexture("Player/player_reloadingpistol");

            WeaponFlashTexture = Game1.GetTexture("Player/player_pistolflash");


            Texture = WeaponTexture;

            base.Initialize();
        }



    }
}