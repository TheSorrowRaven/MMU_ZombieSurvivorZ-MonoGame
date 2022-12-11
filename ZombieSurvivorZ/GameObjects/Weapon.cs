using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace ZombieSurvivorZ
{
    public class Weapon : SpriteObject
    {
        public enum State
        {
            Ready,
            Reload,
            Holstered,
            Switching,
        }

        public State WeaponState { get; protected set; } = State.Holstered;

        // Stats
        protected float _fireTime;
        protected float _reloadTime;
        protected float _switchTime;
        protected int _clipSize;
        protected bool _canAutoFire;

        public float FireTime => _fireTime;
        public float ReloadTime => _reloadTime;
        public float SwitchTime => _switchTime;
        public int ClipSize => _clipSize;
        public bool CanAutoFire => _canAutoFire;

        // Ammo
        public int AmmoReserve { get; protected set; }
        public int AmmoInClip { get; protected set; }

        // Time control
        private float fireTimeCount;
        private float reloadTimeCount;
        private float switchTimeCount;


        public override void Initialize()
        {
            Texture = Game1.TextureBank["cursor_piece"];
        }

        public virtual void HoldWeapon()
        {
            WeaponState = State.Switching;
            switchTimeCount = SwitchTime;
        }

        public virtual void HolsterWeapon()
        {
            WeaponState = State.Holstered;
        }

        public virtual void SemiFire()
        {
            Fire();
        }

        public virtual void AutoFire()
        {
            if (CanAutoFire)
            {
                Fire();
            }
        }

        protected virtual void Fire()
        {
            if (WeaponState == State.Holstered)
            {
                throw new Exception("Weapon is holstered??/");
            }

            if (WeaponState == State.Switching)
            {
                //Switching
                Console.WriteLine("IsSwitching!");
                return;
            }

            if (WeaponState == State.Reload)
            {
                //Reloading
                Console.WriteLine("IsReloading!");
                return;
            }

            //state == State.Ready

            if (fireTimeCount > 0)
            {
                //Cannot Fire, cooling down from fire speed
                return;
            }

            if (AmmoInClip == 0)
            {
                //No ammo, try to reload
                Reload();
                return;
            }

            Console.WriteLine("Bang!!");
            AmmoInClip--;
            fireTimeCount = FireTime;
        }

        public virtual void Reload()
        {
            if (AmmoInClip == ClipSize)
            {
                //No need to reload, full
                return;
            }
            if (AmmoReserve == 0)
            {
                //No more ammo to reload
                return;
            }
            Console.WriteLine("RELOAD!");
            WeaponState = State.Reload;
            reloadTimeCount = ReloadTime;
        }

        protected virtual void FinishReload()
        {
            WeaponState = State.Ready;
            int initialAmmoInClip = AmmoInClip;
            int requiredAmmo = ClipSize - initialAmmoInClip;
            int amountReloading = Math.Min(AmmoReserve, requiredAmmo);
            AmmoReserve -= amountReloading;
            AmmoInClip += amountReloading;
        }

        protected virtual void FinishSwitching()
        {
            WeaponState = State.Ready;
        }

        public override void Update()
        {
            if (WeaponState == State.Holstered)
            {
                return;
            }

            if (WeaponState == State.Switching)
            {
                if (switchTimeCount > 0)
                {
                    //Switching...
                    switchTimeCount -= Time.deltaTime;
                    return;
                }
                FinishSwitching();
                return;
            }

            if (WeaponState == State.Reload)
            {
                if (reloadTimeCount > 0)
                {
                    //Reloading...
                    reloadTimeCount -= Time.deltaTime;
                    return;
                }
                //Finished reloading
                FinishReload();
                return;
            }

            //state == State.Ready
            if (fireTimeCount > 0)
            {
                fireTimeCount -= Time.deltaTime;
            }
        }

    }
}