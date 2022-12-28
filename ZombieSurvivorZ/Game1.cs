using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class Game1 : Game
    {
        public const bool CollisionDebugging = true;


        public static readonly Game1 Current = new();

        private static readonly Dictionary<string, Texture2D> TextureBank = new();
        public static GameWindow Screen { get; private set; }
        public static Vector2 ScreenCenter => new(Screen.ClientBounds.Width / 2, Screen.ClientBounds.Height / 2);
        public static Vector2 ScreenSize => Screen.ClientBounds.Size.ToVector2();



        public static GraphicsDeviceManager Graphics { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }

        public static Camera Camera { get; private set; }

        public static Player Player { get; private set; }




        private Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Screen = Window;
            IsMouseVisible = false;

            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();


            Camera = new();
            Collision.Initialize();

            base.Initialize();

        }

        public void SetCursorVisible(bool visible)
        {
            IsMouseVisible = visible;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ////_spriteBatch = new(GraphicsDevice);

            Player = new();



            var c = new Crate()
            {
                Position = new(200, 100)
            };

            c = new Crate()
            {
                Position = new(-100, -100)
            };

        }

        public static Texture2D GetTexture(string name)
        {
            if (!TextureBank.TryGetValue(name, out Texture2D texture))
            {
                texture = Current.Content.Load<Texture2D>(name);
                TextureBank.Add(name, texture);
            }
            return texture;
        }

        protected override void Update(GameTime gameTime)
        {
            Time.time = (float)gameTime.TotalGameTime.TotalSeconds;
            Time.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Time.frameCount++;

            Input.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Objects Update
            World.UI.Update(gameTime);
            World.floor.Update(gameTime);
            World.objects.Update(gameTime);

            //Collision Update
            Collision.Update(gameTime);

            //Camera Update
            Camera.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            Matrix transformMatrix = Camera.GetViewMatrix();
            SpriteBatch.Begin(transformMatrix: transformMatrix);
            World.floor.Draw(SpriteBatch, gameTime);
            World.objects.Draw(SpriteBatch, gameTime);
            SpriteBatch.End();


            SpriteBatch.Begin();
            World.UI.Draw(SpriteBatch, gameTime);
            SpriteBatch.End();


            base.Draw(gameTime);
        }
    }
}