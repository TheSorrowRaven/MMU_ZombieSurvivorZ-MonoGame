using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class Game1 : Game
    {
        public static Dictionary<string, Texture2D> TextureBank = new();
        public static GameWindow Screen { get; private set; }
        public static Vector2 ScreenCenter => new(Screen.ClientBounds.Width / 2, Screen.ClientBounds.Height / 2);

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Screen = Window;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureBank.Add("player", Content.Load<Texture2D>("player"));
            TextureBank.Add("cursor_piece", Content.Load<Texture2D>("cursor_piece"));

            Player player = new()
            {
                Position = new(100, 100)
            };

            Cursor cursor = new();

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
            World.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            World.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}