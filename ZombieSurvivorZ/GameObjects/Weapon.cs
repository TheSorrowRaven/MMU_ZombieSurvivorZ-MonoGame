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

        public float fireTime;
        public float reloadTime;
        public float switchTime;
        public int clipSize;

        public bool canAutoFire;
        public int ammoReserve;
        public int ammoInClip;

        public State state = State.Holstered;
        private float fireTimeCount;
        private float reloadTimeCount;
        private float switchTimeCount;


        public override void Initialize()
        {
            Texture = Game1.TextureBank["cursor_piece"];
        }

        public virtual void HoldWeapon()
        {
            state = State.Switching;
            switchTimeCount = switchTime;
        }

        public virtual void HolsterWeapon()
        {
            state = State.Holstered;
        }

        public virtual void SemiFire()
        {
            Fire();
        }

        public virtual void AutoFire()
        {
            if (canAutoFire)
            {
                Fire();
            }
        }

        protected virtual void Fire()
        {
            if (state == State.Holstered)
            {
                throw new Exception("Weapon is holstered??/");
            }

            if (state == State.Switching)
            {
                //Switching
                Console.WriteLine("IsSwitching!");
                return;
            }

            if (state == State.Reload)
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

            if (ammoInClip == 0)
            {
                //No ammo, try to reload
                Reload();
                return;
            }

            Console.WriteLine("Bang!!");
            ammoInClip--;
            fireTimeCount = fireTime;
        }

        public virtual void Reload()
        {
            if (ammoInClip == clipSize)
            {
                //No need to reload, full
                return;
            }
            if (ammoReserve == 0)
            {
                //No more ammo to reload
                return;
            }
            Console.WriteLine("RELOAD!");
            state = State.Reload;
            reloadTimeCount = reloadTime;
        }

        protected virtual void FinishReload()
        {
            state = State.Ready;
            int initialAmmoInClip = ammoInClip;
            int requiredAmmo = clipSize - initialAmmoInClip;
            int amountReloading = Math.Min(ammoReserve, requiredAmmo);
            ammoReserve -= amountReloading;
            ammoInClip += amountReloading;
        }

        protected virtual void FinishSwitching()
        {
            state = State.Ready;
        }

        public override void Update()
        {
            if (state == State.Holstered)
            {
                return;
            }

            if (state == State.Switching)
            {
                if (switchTimeCount > 0)
                {
                    //Switching...
                    switchTimeCount -= Time.deltaTime;
                    Console.WriteLine(switchTimeCount);
                    return;
                }
                FinishSwitching();
                return;
            }

            if (state == State.Reload)
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