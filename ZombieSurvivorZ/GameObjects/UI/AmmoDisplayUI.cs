using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;

namespace ZombieSurvivorZ
{
    public class AmmoDisplayUI : UIText
    {

        private bool isDashed = false;
        private int lastAmmoInClip;
        private int lastAmmoReserve;

        public AmmoDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            isDashed = true;
            Text = "-";
            Align = new(0.5f, 1f);
        }

        public void UpdateAmmoCount(int ammoInClip, int ammoReserve)
        {
            if (ammoInClip == lastAmmoInClip && ammoReserve == lastAmmoReserve)
            {
                return;
            }
            lastAmmoInClip = ammoInClip;
            lastAmmoReserve = ammoReserve;
            Text = $"{ammoInClip} / {ammoReserve}";
        }

        public void WeaponNotActive()
        {
            isDashed = true;
            Text = "-";
        }
        public void WeaponActive()
        {
            isDashed = false;
            Text = $"{lastAmmoInClip} / {lastAmmoReserve}";
        }

    }
}