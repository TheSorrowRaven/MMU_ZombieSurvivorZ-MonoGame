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
            SwitchTime = 0.5f;
            ClipSize = 9;
            CanAutoFire = false;

            AmmoInClip = 9;
            AmmoReserve = 14;
        }

        public override void Initialize()
        {
            PlayerBodyTextureHolding = Game1.GetTexture("Player/player_body_holdingpistol");
            PlayerBodyTextureRecoiling = Game1.GetTexture("Player/player_body_firingpistol");

            WeaponTexture = Game1.GetTexture("Player/player_pistol");
            WeaponRecoilTexture = Game1.GetTexture("Player/player_recoilingpistol");

            Texture = WeaponTexture;

            base.Initialize();
        }



    }
}