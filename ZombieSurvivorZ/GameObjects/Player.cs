using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ZombieSurvivorZ
{
    public class Player : SpriteObject
    {

        private float movementSpeed = 100;

        private Weapon weapon;

        public Player()
        {

        }

        public override void Initialize()
        {
            Texture = Game1.TextureBank["player"];
            RotationOffset = 90 * MathF.PI / 180;
            Position = Game1.ScreenCenter;
            Pistol pistol = new();
            SwitchToWeapon(pistol);
        }

        public override void Update()
        {
            TransformUpdate();
            WeaponUpdate();
        }

        private void TransformUpdate()
        {
            Vector2 movement = Input.ConstructAxis2(Keys.S, Keys.W, Keys.D, Keys.A);
            Position += movement * movementSpeed * Time.deltaTime;
            Heading = Input.MousePos - Position;
        }

        private void SwitchToWeapon(Weapon weapon)
        {
            this.weapon = weapon;
            weapon.HoldWeapon();
        }

        private void WeaponUpdate()
        {
            if (weapon == null)
            {
                return;
            }

            if (Input.IsLMouseFirstDown())
            {
                weapon.SemiFire();
            }
            else if (Input.IsLMouseDown())
            {
                weapon.AutoFire();
            }
            

        }

    }
}