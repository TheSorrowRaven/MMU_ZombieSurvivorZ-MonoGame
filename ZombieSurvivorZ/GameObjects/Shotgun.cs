using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivorZ
{
    public class Shotgun : Weapon
    {

        public const int Pellets = 8;
        public const float baseSpread = 16;

        public Shotgun()
        {
            WeaponName = "Shotgun";
            WeaponUITexture = Game1.GetTexture("pistol_texture");
            MaterialsToPurchase = 2500;
            MaterialsToPurchaseAmmo = 40;

            FireTime = 0.6f;
            MuzzleFlashTime = 0.2f;
            FiringLineFlashTime = 0.1f;
            ReloadTime = 1.5f;
            SwitchTime = 0.8f;

            Damage = 10;
            ClipSize = 8;
            CanAutoFire = false;
            FiringLineStartOffset = 40;

            AmmoInClip = 8;
            AmmoReserve = 32;

            RecoilSpreadIncrease = 20.0f;
            RecoilSpreadDecrease = 75.0f;
            RecoilMaxSpread = 40.0f;
            RecoilTime = 0.4f;
            RecoilAimFactor = 0.4f;
        }

        protected override void Fire()
        {
            for (int i = 0; i < Pellets; i++)
            {
                FireRaycast(baseSpread);
            }
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
