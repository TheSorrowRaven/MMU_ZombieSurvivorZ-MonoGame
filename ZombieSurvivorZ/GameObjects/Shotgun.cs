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
        public Shotgun()
        {
            WeaponName = "Shotgun";
            WeaponUITexture = Game1.GetTexture("shotgun_texture");
            MaterialsToPurchase = 300;
            MaterialsToPurchaseAmmo = 50;

            FireTime = 0.6f;
            MuzzleFlashTime = 0.2f;
            FiringLineFlashTime = 0.1f;
            ReloadTime = 1.2f;
            SwitchTime = 0.8f;

            Damage = 10;
            ClipSize = 6;
            Pellets = 3;
            BaseSpread = 20;
            CanAutoFire = false;
            FiringLineStartOffset = 40;

            AmmoInClip = 6;
            AmmoReserve = 24;

            RecoilSpreadIncrease = 40.0f;
            RecoilSpreadDecrease = 65.0f;
            RecoilMaxSpread = 65.0f;
            RecoilTime = 0.4f;
            RecoilAimFactor = 0.4f;

            Levels = new Level[]
            {
                new Level
                {
                    UpgradeCost = 250,
                    BaseSpread = 18,
                    Pellets = 4
                },
                new Level
                {
                    UpgradeCost = 350,
                    Pellets = 5
                },
                new Level
                {
                    UpgradeCost = 400,
                    BaseSpread = 14,
                    Pellets = 6
                },
                new Level()
                {
                    UpgradeCost = 500,
                    BaseSpread = 12,
                    Pellets = 7
                },
                new Level()
                {
                    UpgradeCost = 800,
                    BaseSpread = 10,
                    Pellets = 8
                }
            };
        }

        public override void Initialize()
        {
            PlayerBodyTextureHolding = Game1.GetTexture("Player/player_body_holdingrifle");
            PlayerBodyTextureMuzzleFlashing = Game1.GetTexture("Player/player_body_recoilingrifle");
            PlayerBodyTextureReloading = Game1.GetTexture("Player/player_body_reloadingpistol");

            WeaponTexture = Game1.GetTexture("Player/player_shotgun");
            WeaponMuzzleFlashingTexture = Game1.GetTexture("Player/player_recoilingshotgun");
            WeaponReloadTexture = Game1.GetTexture("Player/player_reloadingshotgun");

            WeaponFlashTexture = Game1.GetTexture("Player/player_shotgunflash");

            WeaponFireSE = Game1.GetSoundEffect("Audio/shotgun");
            WeaponReloadSE = Game1.GetSoundEffect("Audio/reload");
            WeaponNoAmmoSE = Game1.GetSoundEffect("Audio/noammo");

            weaponReloadInstance = WeaponReloadSE.CreateInstance();

            Texture = WeaponTexture;

            base.Initialize();
        }

        public override void ReloadSFX()
        {
            weaponReloadInstance.IsLooped = true;
            weaponReloadInstance.Play();
        }
    }
}
