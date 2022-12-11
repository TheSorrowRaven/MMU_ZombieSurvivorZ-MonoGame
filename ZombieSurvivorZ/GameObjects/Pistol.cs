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
            _fireTime = 0.1f;
            _reloadTime = 2.2f;
            _switchTime = 0.5f;
            _clipSize = 9;
            _canAutoFire = false;

            AmmoInClip = 9;
            AmmoReserve = 14;
        }

        public override void Initialize()
        {
            Texture = Game1.TextureBank["cursor_piece"];

            base.Initialize();
        }



    }
}