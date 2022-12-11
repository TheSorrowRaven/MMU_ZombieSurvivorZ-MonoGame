using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace ZombieSurvivorZ
{
    public class Pistol : Weapon
    {



        public override void Initialize()
        {
            Texture = Game1.TextureBank["cursor_piece"];
            fireTime = 0.1f;
            reloadTime = 2.2f;
            switchTime = 0.5f;
            clipSize = 9;
            ammoInClip = 9;
            ammoReserve = 14;
            canAutoFire = false;

            base.Initialize();
        }



    }
}