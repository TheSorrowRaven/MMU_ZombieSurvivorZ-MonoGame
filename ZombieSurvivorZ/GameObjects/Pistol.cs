namespace ZombieSurvivorZ
{
    public class Pistol : Weapon
    {

        public Pistol()
        {
            WeaponName = "Pistol";
            WeaponUITexture = Game1.GetTexture("pistol_texture");
            MaterialsToPurchase = 100;
            MaterialsToPurchaseAmmo = 5;

            FireTime = 0.2f;
            MuzzleFlashTime = 0.05f;
            FiringLineFlashTime = 0.05f;
            ReloadTime = 1.0f;
            SwitchTime = 0.2f;

            Damage = 5;
            ClipSize = 9;
            Pellets = 1;
            BaseSpread = 0;
            CanAutoFire = true;
            FiringLineStartOffset = 20;

            AmmoInClip = 9;
            AmmoReserve = 45;

            RecoilSpreadIncrease = 5.0f;
            RecoilSpreadDecrease = 50.0f;
            RecoilMaxSpread = 25f;
            RecoilTime = 0.3f;
            RecoilAimFactor = 0.2f;

            Levels = new Level[]
            {
                new Level
                {
                    UpgradeCost = 100,
                    ClipSize = 10,
                    ReloadTime = 0.9f,
                },
                new Level
                {
                    UpgradeCost = 125,
                    ClipSize = 11,
                    ReloadTime = 0.8f,
                    FireTime = 0.175f,
                },
                new Level
                {
                    UpgradeCost = 200,
                    ClipSize = 12,
                    ReloadTime = 0.7f,
                    Damage = 7,
                    SwitchTime = 0.15f,
                },
                new Level
                {
                    UpgradeCost = 150,
                    ClipSize = 13,
                    ReloadTime = 0.6f,
                    FireTime = 0.15f,
                },
                new Level
                {
                    UpgradeCost = 175,
                    ClipSize = 14,
                    ReloadTime = 0.5f,
                    SwitchTime = 0.1f,
                },
                new Level
                {
                    UpgradeCost = 5000,
                    ClipSize = 19,
                    Pellets = 2,
                    RecoilMaxSpread = 15f,
                }
            };
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

            WeaponFireSE = Game1.GetSoundEffect("Audio/pistol");
            WeaponReloadSE = Game1.GetSoundEffect("Audio/reload_mag");
            WeaponNoAmmoSE = Game1.GetSoundEffect("Audio/noammo");

            weaponReloadInstance = WeaponReloadSE.CreateInstance();

            base.Initialize();
        }



    }
}