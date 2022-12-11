using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ZombieSurvivorZ
{
    public class Player : SpriteObject
    {

        private float movementSpeed;

        public override void Initialize()
        {
            movementSpeed = 100;

            Texture = Game1.TextureBank["player"];
            RotationOffset = 90 * MathF.PI / 180;
            Position = Game1.ScreenCenter;
        }

        public override void Update()
        {
            Vector2 movement = new();
            if (Input.KeyboardState.IsKeyDown(Keys.W))
            {
                movement.Y -= 1;
            }
            if (Input.KeyboardState.IsKeyDown(Keys.S))
            {
                movement.Y += 1;
            }
            if (Input.KeyboardState.IsKeyDown(Keys.A))
            {
                movement.X -= 1;
            }
            if (Input.KeyboardState.IsKeyDown(Keys.D))
            {
                movement.X += 1;
            }
            Position += movement * movementSpeed * Time.deltaTime;
            Heading = Input.MouseState.Position.ToVector2() - Position;
        }

    }
}