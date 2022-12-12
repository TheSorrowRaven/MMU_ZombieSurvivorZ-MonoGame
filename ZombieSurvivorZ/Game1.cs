using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class Game1 : Game
    {
        public static readonly Game1 Current = new();

        private static readonly Dictionary<string, Texture2D> TextureBank = new();
        public static GameWindow Screen { get; private set; }
        public static Vector2 ScreenCenter => new(Screen.ClientBounds.Width / 2, Screen.ClientBounds.Height / 2);

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        //UI render order in the world (affected by camera)
        public const int RenderOrder_WorldUI = 0;


        public Camera Camera { get; private set; }

        public Player Player { get; private set; }


        private Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Screen = Window;
            IsMouseVisible = false;

            Camera = new();

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //TextureBank.Add("player", Content.Load<Texture2D>("player"));
            //TextureBank.Add("cursor_piece", Content.Load<Texture2D>("cursor_piece"));

            Player = new();

            new Crate()
            {
                Position = new(200, 100)
            };

            new Crate()
            {
                Position = new(-400, -300)
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

            //Collision.Simulate();
            World.UI.Update(gameTime);
            World.floor.Update(gameTime);
            World.objects.Update(gameTime);

            Camera.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            Matrix transformMatrix = Camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            World.floor.Draw(_spriteBatch, gameTime);
            World.objects.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();


            _spriteBatch.Begin();
            World.UI.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}