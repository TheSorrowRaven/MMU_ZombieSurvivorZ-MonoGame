using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace ZombieSurvivorZ
{
    public class DebugGun : Weapon
    {

        public DebugGun()
        {
            FireTime = 0.01f;
            MuzzleFlashTime = 0.01f;
            FiringLineFlashTime = 0.05f;
            ReloadTime = 0.1f;
            SwitchTime = 0.1f;

            ClipSize = 100;
            CanAutoFire = true;
            FiringLineStartOffset = 40;

            AmmoInClip = 100;
            AmmoReserve = 100000;

            RecoilSpreadIncrease = 1.0f;
            RecoilSpreadDecrease = 100.0f;
            RecoilMaxSpread = 0f;
            RecoilTime = 0.1f;
        }

        public override void Initialize()
        {
            PlayerBodyTextureHolding = Game1.GetTexture("Player/player_body_holdingpistol");
            PlayerBodyTextureMuzzleFlashing = Game1.GetTexture("Player/player_body_recoilingpistol");
            PlayerBodyTextureReloading = Game1.GetTexture("Player/player_body_reloadingpistol");

            WeaponTexture = Game1.GetTexture("Player/player_pistol");
            WeaponMuzzleFlashingTexture = Game1.GetTexture("Player/player_recoilingpistol");
            WeaponReloadTexture = Game1.GetTexture("Player/player_reloadingpistol");

            WeaponFlashTexture = Game1.GetTexture("Player/player_pistolflash");


            Texture = WeaponTexture;

            base.Initialize();
        }



    }
}