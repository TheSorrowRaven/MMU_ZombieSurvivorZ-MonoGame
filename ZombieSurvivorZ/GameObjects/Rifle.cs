using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivorZ
{
    public class Rifle : Weapon
    {
        public Rifle()
        {
            WeaponName = "Rifle";
            WeaponUITexture = Game1.GetTexture("pistol_texture");
            MaterialsToPurchase = 0;
            MaterialsToPurchaseAmmo = 0;

            FireTime = 0.1f;
            MuzzleFlashTime = 0.1f;
            FiringLineFlashTime = 0.05f;
            ReloadTime = 3.0f;
            SwitchTime = 0.5f;

            ClipSize = 25;
            CanAutoFire = true;
            FiringLineStartOffset = 40;

            AmmoInClip = 20;
            AmmoReserve = 75;

            RecoilSpreadIncrease = 10.0f;
            RecoilSpreadDecrease = 60.0f;
            RecoilMaxSpread = 35.0f;
            RecoilTime = 0.3f;
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
