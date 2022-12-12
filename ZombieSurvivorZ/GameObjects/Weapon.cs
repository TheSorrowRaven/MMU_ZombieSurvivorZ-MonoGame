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
            Recoiling,
            Reloading,
            Holstered,
            Switching,
        }

        //State
        public State WeaponState { get; protected set; } = State.Holstered;

        //Stats
        public float FireTime { get; protected set; }
        public float RecoilTime { get; protected set; }
        public float ReloadTime { get; protected set; }
        public float SwitchTime { get; protected set; }
        public int ClipSize { get; protected set; }
        public bool CanAutoFire { get; protected set; }

        //Ammo
        public int AmmoReserve { get; protected set; }
        public int AmmoInClip { get; protected set; }

        //Time control
        private float fireTimeCount;
        private float recoilTimeCount;
        private float reloadTimeCount;
        private float switchTimeCount;

        //Graphics
        public bool IsRecoiling => WeaponState == State.Recoiling;

        public Texture2D PlayerBodyTextureHolding { get; protected set; }
        public Texture2D PlayerBodyTextureRecoiling { get; protected set; }
        public Texture2D PlayerBodyTextureReloading { get; protected set; }

        public Texture2D WeaponTexture { get; protected set; }
        public Texture2D WeaponRecoilTexture { get; protected set; }
        public Texture2D WeaponReloadTexture { get; protected set; }

        public Texture2D WeaponFlashTexture { get; protected set; }


        public override void Initialize()
        {
            HolsterWeapon();
        }


        public virtual void HoldWeapon()
        {
            WeaponState = State.Switching;
            switchTimeCount = SwitchTime;
        }

        public virtual void HolsterWeapon()
        {
            WeaponState = State.Holstered;
            Active = false;
        }

        public virtual void SemiFire()
        {
            TryFire();
        }

        public virtual void AutoFire()
        {
            if (CanAutoFire)
            {
                TryFire();
            }
        }

        protected virtual void TryFire()
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

            if (WeaponState == State.Reloading)
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

            Fire();
        }

        protected virtual void Fire()
        {
            Console.WriteLine("Bang!!");
            AmmoInClip--;
            fireTimeCount = FireTime;

            Recoil();
        }

        protected virtual void Recoil()
        {
            WeaponState = State.Recoiling;
            recoilTimeCount = RecoilTime;
        }

        protected virtual void FinishRecoil()
        {
            WeaponState = State.Ready;
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
            WeaponState = State.Reloading;
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

            if (WeaponState == State.Reloading)
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

            //state == State.Ready || state == State.Recoiling
            if (fireTimeCount > 0)
            {
                fireTimeCount -= Time.deltaTime;
            }

            if (WeaponState == State.Recoiling)
            {
                if (recoilTimeCount > 0)
                {
                    //Recoiling...
                    recoilTimeCount -= Time.deltaTime;
                    return;
                }
                FinishRecoil();
                return;
            }

        }

        public virtual Texture2D GetPlayerBodyTexture() => WeaponState switch
        {
            State.Recoiling => PlayerBodyTextureRecoiling,
            State.Reloading => PlayerBodyTextureReloading,
            State.Switching => PlayerBodyTextureReloading,
            _ => PlayerBodyTextureHolding,
        };

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = WeaponState switch
            {
                State.Recoiling => WeaponRecoilTexture,
                State.Reloading => WeaponReloadTexture,
                State.Switching => WeaponReloadTexture,
                _ => WeaponTexture,
            };


            spriteBatch.Draw(texture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);

            if (WeaponState == State.Recoiling)
            {
                spriteBatch.Draw(WeaponFlashTexture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);
            }
        }


    }
}