using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace ZombieSurvivorZ
{
    public class Weapon : SpriteObject
    {
        public enum State
        {
            Ready,
            Reloading,
            Holstered,
            Switching,
        }

        //State
        public State WeaponState { get; protected set; } = State.Holstered;
        //Sub State
        public bool IsRecoiling { get; protected set; } = false;
        //Visual State
        public bool IsMuzzleFlashing { get; protected set;} = false;

        //Stats
        public float FireTime { get; protected set; }
        //Recoil time until the spread starts decreasing
        public float RecoilTime { get; protected set; }
        public float ReloadTime { get; protected set; }
        public float SwitchTime { get; protected set; }
        public float MuzzleFlashTime { get; protected set; }
        public float FiringLineFlashTime { get; protected set; }

        public int ClipSize { get; protected set; }
        public bool CanAutoFire { get; protected set; }

        public float RecoilSpreadIncrease { get; protected set; }
        public float RecoilSpreadDecrease { get; protected set; }
        public float RecoilMaxSpread { get; protected set; }

        //Ammo
        public int AmmoReserve { get; protected set; }
        public int AmmoInClip { get; protected set; }

        //Time control
        private float fireTimeCount;
        private float recoilTimeCount;
        private float reloadTimeCount;
        private float switchTimeCount;
        private float muzzleFlashTimeCount;

        //Recoil control
        private float recoilSpread;

        //Texture
        public Texture2D PlayerBodyTextureHolding { get; protected set; }
        public Texture2D PlayerBodyTextureMuzzleFlashing { get; protected set; }
        public Texture2D PlayerBodyTextureReloading { get; protected set; }

        public Texture2D WeaponTexture { get; protected set; }
        public Texture2D WeaponMuzzleFlashingTexture { get; protected set; }
        public Texture2D WeaponReloadTexture { get; protected set; }

        public Texture2D WeaponFlashTexture { get; protected set; }


        private readonly List<(Line, float)> firingLines = new();


        public override void Initialize()
        {
            HolsterWeapon();
        }

        public virtual float GetVisualRecoilSpread()
        {
            return recoilSpread;
        }


        public virtual void HoldWeapon()
        {
            WeaponState = State.Switching;
            switchTimeCount = SwitchTime;
            FinishRecoil();
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
            FireRaycast();
            AmmoInClip--;
            fireTimeCount = FireTime;

            MuzzleFlash();
            Recoil();
        }

        protected virtual void FireRaycast()
        {
            if (!Collision.Raycast(Position, Heading, out Dictionary<Collision.Collider, float> intersections))
            {
                //Console.WriteLine($"Miss");
                //firingLines.Add(new(Position, Position + direction * 100));
                return;
            }
            Vector2 direction = Heading;
            foreach (var item in intersections)
            {
                Console.WriteLine($"Bang -> {item.Key.Go.GetType()}, {item.Value}");
                firingLines.Add((new(Position, Position + direction * item.Value), FiringLineFlashTime));
            }
        }


        //Doesn't change state, is only visual
        protected virtual void MuzzleFlash()
        {
            IsMuzzleFlashing = true;
            muzzleFlashTimeCount = MuzzleFlashTime;
        }

        protected virtual void FinishMuzzleFlash()
        {
            IsMuzzleFlashing = false;
        }

        protected virtual void Recoil()
        {
            IsRecoiling = true;
            recoilSpread += RecoilSpreadIncrease;
            if (recoilSpread > RecoilMaxSpread)
            {
                recoilSpread = RecoilMaxSpread;
            }
            recoilTimeCount = RecoilTime;
        }

        protected virtual void FinishRecoil()
        {
            IsRecoiling = false;
            recoilSpread = 0;
        }

        public virtual void Reload()
        {
            if (IsMuzzleFlashing)
            {
                IsMuzzleFlashing = false;
            }
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
            for (int i = 0; i < firingLines.Count; i++)
            {
                (Line, float) item = firingLines[i];
                float timeCount = item.Item2 - Time.deltaTime;
                if (timeCount < 0)
                {
                    firingLines.RemoveAt(i);
                    i--;
                    continue;
                }
                item.Item2 = timeCount;
                firingLines[i] = item;
            }

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

            //Decrease Recoil Spread even when the weapon is reloading
            if (IsRecoiling)
            {
                recoilTimeCount -= Time.deltaTime;
                if (recoilTimeCount < 0)
                {
                    if (recoilSpread > 0)
                    {
                        recoilSpread -= RecoilSpreadDecrease * Time.deltaTime;
                    }
                    else
                    {
                        FinishRecoil();
                    }
                }
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

            //state == State.Ready
            if (fireTimeCount > 0)
            {
                fireTimeCount -= Time.deltaTime;
            }


            if (IsMuzzleFlashing)
            {
                if (muzzleFlashTimeCount > 0)
                {
                    //Recoiling...
                    muzzleFlashTimeCount -= Time.deltaTime;
                    return;
                }
                FinishMuzzleFlash();
            }

        }

        public virtual Texture2D GetPlayerBodyTexture()
        {
            Texture2D texture;

            if (IsMuzzleFlashing)
            {
                texture = PlayerBodyTextureMuzzleFlashing;
            }
            else
            {
                texture = WeaponState switch
                {
                    State.Reloading => PlayerBodyTextureReloading,
                    State.Switching => PlayerBodyTextureReloading,
                    _ => PlayerBodyTextureHolding,
                };
            }
            return texture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture;

            if (IsMuzzleFlashing)
            {
                texture = WeaponMuzzleFlashingTexture;
            }
            else
            {
                texture = WeaponState switch
                {
                    State.Reloading => WeaponReloadTexture,
                    State.Switching => WeaponReloadTexture,
                    _ => WeaponTexture,
                };
            }

            spriteBatch.Draw(texture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);

            if (IsMuzzleFlashing)
            {
                spriteBatch.Draw(WeaponFlashTexture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);
            }

            for (int i = 0; i < firingLines.Count; i++)
            {
                Line line = firingLines[i].Item1;
                spriteBatch.DrawLine(line.start, line.end, Color.Red);
            }
        }


    }
}