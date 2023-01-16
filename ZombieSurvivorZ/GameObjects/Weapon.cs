using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using static ZombieSurvivorZ.Collision;

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

        public class Level
        {
            public int UpgradeCost; //To NEXT LEVEL

            public float? FireTime;
            public float? RecoilTime;
            public float? ReloadTime;
            public float? SwitchTime;
            public float? MuzzleFlashTime;
            public float? FiringLineFlashTime;
            public int? Damage;
            public int? ClipSize;
            public int? Pellets;
            public float? BaseSpread;
            public bool? CanAutoFire;
            public float? RecoilSpreadIncrease;
            public float? RecoilSpreadDecrease;
            public float? RecoilMaxSpread;
        }


        //State
        public State WeaponState { get; protected set; } = State.Holstered;
        //Sub State
        public bool IsRecoiling { get; protected set; } = false;
        //Visual State
        public bool IsMuzzleFlashing { get; protected set;} = false;

        //Ownership
        public bool IsOwned { get; set; } = false;

        //Properties
        public string WeaponName { get; protected set; }
        public Texture2D WeaponUITexture { get; protected set; }
        public int MaterialsToPurchase { get; protected set; }
        public int MaterialsToPurchaseAmmo { get; protected set; }

        //Upgrades
        public int LevelNumber { get; protected set; } = -1;
        public Level[] Levels { get; protected set; } = Array.Empty<Level>();
        public Level CurrentLevel {
            get
            {
                if (LevelNumber == -1)
                {
                    return null;
                }
                return Levels[LevelNumber];
            }
        }
        public bool IsMaxLevel => LevelNumber + 1 >= Levels.Length;


        //Stats
        public float FireTime { get; protected set; }
        //Recoil time until the spread starts decreasing
        public float RecoilTime { get; protected set; }
        public float ReloadTime { get; protected set; }
        public float SwitchTime { get; protected set; }
        public float MuzzleFlashTime { get; protected set; }
        public float FiringLineFlashTime { get; protected set; }

        public int Damage { get; protected set; }
        public int ClipSize { get; protected set; }
        public int Pellets { get; protected set; }
        public float BaseSpread { get; protected set; }
        public bool CanAutoFire { get; protected set; }
        public float FiringLineStartOffset { get; protected set; }

        public float RecoilSpreadIncrease { get; protected set; }
        public float RecoilSpreadDecrease { get; protected set; }
        public float RecoilMaxSpread { get; protected set; }
        public float RecoilAimFactor { get; protected set; }
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

        private readonly GameObject[] IgnoreObjects = new GameObject[1];
        private WeaponUpgradeUI WeaponUpgradeUIRef;

        // Sound effects
        public SoundEffect WeaponFireSE { get; protected set; }
        public SoundEffect WeaponReloadSE { get; protected set; }
        public SoundEffect WeaponNoAmmoSE { get; protected set; }
        public SoundEffectInstance weaponReloadInstance;

        public void SetWeaponUpgradeUI(WeaponUpgradeUI weaponUpgradeUIRef)
        {
            WeaponUpgradeUIRef = weaponUpgradeUIRef;
        }
        public void UpdateWeaponUpgradeUI()
        {
            WeaponUpgradeUIRef?.WeaponChangedUpdate(this);
        }


        public override void Initialize()
        {
            HolsterWeapon();
        }

        public virtual float GetVisualRecoilSpread()
        {
            return recoilSpread;
        }


        public Level GetNextLevel()
        {
            int l = LevelNumber + 1;
            if (l >= Levels.Length)
            {
                return null;
            }
            return Levels[l];
        }
        public int? GetNextLevelCost()
        {
            Level level = GetNextLevel();
            if (level == null)
            {
                return null;
            }
            return level.UpgradeCost;
        }

        public virtual void AmmoPurchased()
        {
            AmmoReserve += ClipSize;
        }
        public virtual void UpgradeLevel()
        {
            if (IsMaxLevel)
            {
                return;
            }
            LevelNumber++;
            Level level = CurrentLevel;

            if (level.FireTime != null)
            {
                FireTime = level.FireTime.Value;
            }
            if (level.RecoilTime != null)
            {
                RecoilTime = level.RecoilTime.Value;
            }
            if (level.ReloadTime != null)
            {
                ReloadTime = level.ReloadTime.Value;
            }
            if (level.SwitchTime != null)
            {
                SwitchTime = level.SwitchTime.Value;
            }
            if (level.MuzzleFlashTime != null)
            {
                MuzzleFlashTime = level.MuzzleFlashTime.Value;
            }
            if (level.FiringLineFlashTime != null)
            {
                FiringLineFlashTime = level.FiringLineFlashTime.Value;
            }
            if (level.Damage != null)
            {
                Damage = level.Damage.Value;
            }
            if (level.ClipSize != null)
            {
                ClipSize = level.ClipSize.Value;
            }
            if (level.Pellets != null)
            {
                Pellets = level.Pellets.Value;
            }
            if (level.BaseSpread != null)
            {
                BaseSpread = level.BaseSpread.Value;
            }
            if (level.CanAutoFire != null)
            {
                CanAutoFire = level.CanAutoFire.Value;
            }
            if (level.RecoilSpreadIncrease != null)
            {
                RecoilSpreadIncrease = level.RecoilSpreadIncrease.Value;
            }
            if (level.RecoilSpreadDecrease != null)
            {
                RecoilSpreadDecrease = level.RecoilSpreadDecrease.Value;
            }
            if (level.RecoilMaxSpread != null)
            {
                RecoilMaxSpread = level.RecoilMaxSpread.Value;
            }
            AmmoInClip = ClipSize;
        }


        public virtual void HoldWeapon()
        {
            IgnoreObjects[0] = Game1.Player;

            Scale = Game1.Player.Scale;

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
                //Console.WriteLine("IsSwitching!");
                return;
            }

            if (WeaponState == State.Reloading)
            {
                //Reloading
                //Console.WriteLine("IsReloading!");
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
                WeaponNoAmmoSE.Play();
                Reload();
                return;
            }

            TriggerFire();
        }

        protected virtual void TriggerFire()
        {
            Fire();
            WeaponFireSE.Play();

            AmmoInClip--;
            fireTimeCount = FireTime;

            MuzzleFlash();
            Recoil();
        }

        protected virtual void Fire()
        {
            for (int i = 0; i < Pellets; i++)
            {
                FireRaycast(BaseSpread + recoilSpread);
            }
        }

        protected virtual void FireRaycast(float recoil)
        {
            Vector2 originalDirection = Heading;
            float rotation = ((MathF.Atan2(Heading.Y, Heading.X) * (180 / MathF.PI)) + (Random.Shared.NextSingle(-recoil, recoil) * RecoilAimFactor)) * MathF.PI / 180;
            Vector2 direction = new(MathF.Cos(rotation), MathF.Sin(rotation));
            Vector2 start = Position + originalDirection * FiringLineStartOffset * Scale.X;
            Vector2 end;

            if (!Raycast(Position, direction, IgnoreObjects, out Collider collider, out float hitDistance))
            {
                hitDistance = 1000;
            }
            else
            {
                HitCollider(collider, direction);
            }
            end = Position + direction * hitDistance;
            Game1.FiringLines.CreateFiringLine(new(start, end), FiringLineFlashTime);
        }
        private void HitCollider(Collider cl, Vector2 direction)
        {
            if (cl.Go is Zombie zombie)
            {
                HitZombie(zombie, direction);
            }
        }
        private void HitZombie(Zombie zombie, Vector2 direction)
        {
            zombie.DealDamage(Damage, direction);
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
            //Console.WriteLine("RELOAD!");
            if (WeaponState != State.Reloading)
            {
                //WeaponReloadSE.Play();
                ReloadSFX();
            }
            WeaponState = State.Reloading;
            reloadTimeCount = ReloadTime;
        }

        public virtual void ReloadSFX()
        {
            WeaponReloadSE.Play();
        }

        protected virtual void FinishReload()
        {
            weaponReloadInstance.Stop();
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

        }


    }
}