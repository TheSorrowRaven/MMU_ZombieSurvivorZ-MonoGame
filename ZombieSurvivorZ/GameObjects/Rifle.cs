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
            MaterialsToPurchase = 1000;
            MaterialsToPurchaseAmmo = 20;

            FireTime = 0.1f;
            MuzzleFlashTime = 0.1f;
            FiringLineFlashTime = 0.05f;
            ReloadTime = 1.5f;
            SwitchTime = 0.5f;

            Damage = 5;
            ClipSize = 25;
            CanAutoFire = true;
            FiringLineStartOffset = 40;

            AmmoInClip = 25;
            AmmoReserve = 75;

            RecoilSpreadIncrease = 10.0f;
            RecoilSpreadDecrease = 60.0f;
            RecoilMaxSpread = 35.0f;
            RecoilTime = 0.3f;
            RecoilAimFactor = 0.2f;

            Levels = new Level[]
            {
                new Level
                {
                    UpgradeCost = 100,
                    ClipSize = 30,
                },
                new Level
                {
                    UpgradeCost = 150,
                    Damage = 8,
                }
            };
        }

        public override void Initialize()
        {
            PlayerBodyTextureHolding = Game1.GetTexture("Player/player_body_holdingrifle");
            PlayerBodyTextureMuzzleFlashing = Game1.GetTexture("Player/player_body_recoilingrifle");
            PlayerBodyTextureReloading = Game1.GetTexture("Player/player_body_reloadingpistol");

            WeaponTexture = Game1.GetTexture("Player/player_rifle");
            WeaponMuzzleFlashingTexture = Game1.GetTexture("Player/player_recoilingrifle");
            WeaponReloadTexture = Game1.GetTexture("Player/player_reloadingrifle");

            WeaponFlashTexture = Game1.GetTexture("Player/player_rifleflash");

            Texture = WeaponTexture;

            WeaponFireSE = Game1.GetSoundEffect("Audio/rifle");
            WeaponReloadSE = Game1.GetSoundEffect("Audio/reload_mag");
            WeaponNoAmmoSE = Game1.GetSoundEffect("Audio/noammo");

            weaponReloadInstance = WeaponReloadSE.CreateInstance();

            base.Initialize();
        }
    }
}
