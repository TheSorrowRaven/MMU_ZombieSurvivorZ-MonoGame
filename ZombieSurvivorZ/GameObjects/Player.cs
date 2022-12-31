using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ZombieSurvivorZ.Collision;

namespace ZombieSurvivorZ
{
    public class Player : SpriteObject
    {

        private float movementSpeed = 100;
        private bool sameKeyHolstersWeapon = true;

        private Reticle reticle;
        private Weapon weapon;

        private Texture2D bodyTexture;

        private readonly List<Weapon> weapons = new();
        private readonly Dictionary<int, Weapon> keyNumToWeapon = new();
        private int keyNumCount = 1;
        public int Materials { get; private set; }

        private readonly CircleDynamicCollider PlayerCL;

        public Player()
        {
            PlayerCL = new(this, 25);
        }

        public override void Initialize()
        {
            Texture = Game1.GetTexture("Player/player_head");
            bodyTexture = Game1.GetTexture("Player/player_body");

            RotationOffset = 90 * MathF.PI / 180;
            Position = new(0, 0);

            //Scale = new(0.5f, 0.5f);

            Pistol pistol = CreateWeapon(new Pistol());
            OwnWeapon(pistol);

            DebugGun debugGun = CreateWeapon(new DebugGun());
            OwnWeapon(debugGun);

            AddWeaponsToWeaponUpgradeUI();

            reticle = new();
            reticle.Disable();

            AddMaterials(100);  //Initial Materials
        }

        protected override void ScaleChanged()
        {
            base.ScaleChanged();
            PlayerCL.Set(25 * Scale.X);
            if (weapon != null)
            {
                weapon.Scale = Scale;
            }
        }

        public override void Update()
        {
            TransformUpdate();
            WeaponOperationUpdate();
            WeaponUpdate();
        }

        private void TransformUpdate()
        {
            Vector2 movement = Input.ConstructAxis2(Keys.S, Keys.W, Keys.D, Keys.A);
            Position += movement * movementSpeed * Time.deltaTime;
            Heading = Game1.Camera.ScreenToWorld(reticle.Position) - Position;

        }

        #region Materials

        private void AddMaterials(int materials)
        {
            Materials += materials;
            Game1.HUDDisplayUI?.MaterialsDisplayUI.UpdateMaterials(Materials);
        }
        private void RemoveMaterials(int materials)
        {
            Materials -= materials;
            Game1.HUDDisplayUI.MaterialsDisplayUI.UpdateMaterials(Materials);
        }
        public bool TryRemoveMaterials(int materials)
        {
            if (Materials >= materials)
            {
                RemoveMaterials(materials);
                return true;
            }
            return false;
        }

        #endregion

        #region Weapon Operation

        private T CreateWeapon<T>(T weapon) where T : Weapon
        {
            weapons.Add(weapon);
            weapon.IsOwned = false;
            return weapon;
        }
        public void OwnWeapon(Weapon weapon)
        {
            keyNumToWeapon.Add(keyNumCount++, weapon);
            weapon.IsOwned = true;
            weapon.UpdateWeaponUpgradeUI();
        }

        private void AddWeaponsToWeaponUpgradeUI()
        {
            foreach (Weapon weapon in weapons)
            {
                Game1.UpgradeWindowUI.Player_AddWeaponToList(weapon);
            }
        }

        private void WeaponOperationUpdate()
        {
            WeaponSwitchUpdate();
            WeaponReloadUpdate();
        }


        private void WeaponSwitchUpdate()
        {
            int numKey = Input.GetNumberKeysFirstDown(1, 2);
            if (numKey == -1)
            {
                return;
            }
            if (!keyNumToWeapon.TryGetValue(numKey, out Weapon weapon))
            {
                SwitchToWeapon(null);
                return;
            }
            SwitchToWeapon(weapon);
        }

        private void WeaponReloadUpdate()
        {
            if (weapon == null)
            {
                return;
            }
            if (Input.IsKeyFirstDown(Keys.R))
            {
                weapon.Reload();
            }
        }


        private void SwitchToWeapon(Weapon weapon)
        {
            Weapon previousWeapon = this.weapon;
            previousWeapon?.HolsterWeapon();
            if (sameKeyHolstersWeapon)
            {
                if (weapon == this.weapon)
                {
                    this.weapon = null;
                    reticle.Disable();
                    Game1.HUDDisplayUI.AmmoDisplayUI.WeaponNotActive();
                    return;
                }
            }
            this.weapon = weapon;

            if (weapon == null)
            {
                reticle.Disable();
                Game1.HUDDisplayUI.AmmoDisplayUI.WeaponNotActive();
                return;
            }
            weapon.Active = true;
            weapon.RotationOffset = 90 * MathF.PI / 180;
            weapon.HoldWeapon();

            reticle.Enable();
            Game1.HUDDisplayUI.AmmoDisplayUI.WeaponActive();
        }

        #endregion

        private void WeaponUpdate()
        {
            if (weapon == null)
            {
                return;
            }

            weapon.Position = Position;
            weapon.Heading = Heading;

            if (Input.IsLMouseFirstDown())
            {
                weapon.SemiFire();
                
            }
            else if (Input.IsLMouseDown())
            {
                weapon.AutoFire();
            }
            reticle.SetSpread(weapon.GetVisualRecoilSpread());
            Game1.HUDDisplayUI.AmmoDisplayUI.UpdateAmmoCount(weapon.AmmoInClip, weapon.AmmoReserve);
        }

        public override void OnCollision(DynamicCollider current, Collider other, Vector2 penetrationVector)
        {
            base.OnCollision(current, other, penetrationVector);
            OnCollision_PushBack(current, other, penetrationVector);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D bodyTexture;
            if (weapon != null)
            {
                bodyTexture = weapon.GetPlayerBodyTexture();
            }
            else
            {
                bodyTexture = this.bodyTexture;
            }
            spriteBatch.Draw(bodyTexture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);

            spriteBatch.Draw(Texture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder); //draw player head

            if (Game1.CollisionDebugging)
            {
                spriteBatch.DrawCircle((CircleF)PlayerCL.Bounds, 20, Color.Red);
            }

        }

    }
}