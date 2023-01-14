﻿using Microsoft.Xna.Framework;
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
                    ClipSize = 12,
                },
                new Level
                {
                    UpgradeCost = 150,
                    ReloadTime = 0.5f,
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

            base.Initialize();
        }



    }
}