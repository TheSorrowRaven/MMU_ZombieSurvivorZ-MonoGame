using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class Player : SpriteObject
    {

        private float movementSpeed = 100;
        private bool sameKeyHolstersWeapon = true;

        private Weapon weapon;

        private Texture2D bodyTexture;

        private readonly Dictionary<int, Weapon> keyNumToWeapon = new();

        public Player()
        {

        }

        public override void Initialize()
        {
            Texture = Game1.GetTexture("Player/player_head");
            bodyTexture = Game1.GetTexture("Player/player_body");

            RotationOffset = 90 * MathF.PI / 180;
            Position = Game1.ScreenCenter;
            Pistol pistol = new();
            keyNumToWeapon.Add(1, pistol);
        }

        public override void Update()
        {
            TransformUpdate();
            WeaponUpdate();
            WeaponSwitchUpdate();
        }

        private void TransformUpdate()
        {
            Vector2 movement = Input.ConstructAxis2(Keys.S, Keys.W, Keys.D, Keys.A);
            Position += movement * movementSpeed * Time.deltaTime;
            Heading = Input.MousePos - Position;

        }

        private void SwitchToWeapon(Weapon weapon)
        {
            Weapon previousWeapon = this.weapon;
            if (previousWeapon != null)
            {
                previousWeapon.HolsterWeapon();
            }
            if (sameKeyHolstersWeapon)
            {
                if (weapon == this.weapon)
                {
                    this.weapon = null;
                    return;
                }
            }
            this.weapon = weapon;

            if (weapon == null)
            {
                return;
            }
            weapon.Active = true;
            weapon.RotationOffset = 90 * MathF.PI / 180;
            weapon.HoldWeapon();
        }

        private void WeaponSwitchUpdate()
        {
            int numKey = Input.GetNumberKeysFirstDown(1, 1);
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

        }

    }
}