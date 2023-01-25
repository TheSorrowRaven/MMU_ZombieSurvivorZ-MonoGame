using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class Game1 : Game
    {
        public const bool CollisionDebugging = true;


        public static readonly Game1 Current = new();

        private static readonly Dictionary<string, Texture2D> TextureBank = new();
        private static readonly Dictionary<string, SoundEffect> SoundEffects = new();
        public static GameWindow Screen { get; private set; }
        public static Vector2 ScreenCenter => new(Screen.ClientBounds.Width / 2, Screen.ClientBounds.Height / 2);
        public static Vector2 ScreenSize => Screen.ClientBounds.Size.ToVector2();



        public static GraphicsDeviceManager Graphics { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }

        public static Camera Camera { get; private set; }

        public static MapManager MapManager { get; private set; }
        public static Player Player { get; private set; }

        public static FiringLines FiringLines { get; private set; }

        public static BloodManager BloodManager { get; private set; }

        public static UpgradeWindowUI UpgradeWindowUI { get; private set; }
        public static HUDDisplayUI HUDDisplayUI { get; private set; }
        public static ScoreDisplayUI ScoreDisplayUI { get; private set; }
        public static bool UISuppressClick { get; set; } = false;

        public static Menu MainMenu { get; private set; }

        public bool GameStarted = false;

        private Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Screen = Window;
            IsMouseVisible = false;

            //Graphics.PreferredBackBufferWidth = 1280;
            //Graphics.PreferredBackBufferHeight = 720;
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.IsFullScreen = true;
            Graphics.HardwareModeSwitch = false;
            Graphics.ApplyChanges();

            SoundEffect.MasterVolume = 0.6f;

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
            Font.Load();

            ShowMainMenu();
            //StartGame();
        }

        public void ShowMainMenu()
        {
            CleanGame();
            MainMenu = new();
            GameStarted = false;
        }

        public void CleanGame()
        {
            Player?.Destroy();
            MapManager?.Destroy();
            BloodManager?.Destroy();
            FiringLines?.Destroy();
            UpgradeWindowUI?.Destroy();
            HUDDisplayUI?.Destroy();
            ScoreDisplayUI?.Destroy();

            Player = null;
            MapManager = null;
            BloodManager = null;
            FiringLines = null;
            UpgradeWindowUI = null;
            HUDDisplayUI = null;
            ScoreDisplayUI = null;
            Camera = null;
        }

        public void StartGame()
        {
            if (GameStarted)
            {
                CleanGame();
            }
            else
            {
                MainMenu?.Destroy();
                MainMenu = null;
            }

            Camera = new();

            Player = new();
            MapManager = new();
            BloodManager = new();

            FiringLines = new();
            UpgradeWindowUI = new();
            HUDDisplayUI = new();
            ScoreDisplayUI = new();

            GameStarted = true;
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

        public static SoundEffect GetSoundEffect(string name)
        {
            if (!SoundEffects.TryGetValue(name, out SoundEffect soundeffect))
            {
                soundeffect = Current.Content.Load<SoundEffect>(name);
                SoundEffects.Add(name, soundeffect);
            }

            return soundeffect;
        }

        public static T GetContent<T>(string name)
        {
            return Current.Content.Load<T>(name);
        }


        protected override void Update(GameTime gameTime)
        {
            Time.gameTime = gameTime;
            Time.time = (float)gameTime.TotalGameTime.TotalSeconds;
            Time.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Time.frameCount++;

            Input.Update(gameTime);
            if (Input.IsKeyFirstDown(Keys.Escape))
            {
                Exit();
            }

            UISuppressClick = false;
            //Objects Update
            World.UI.Update(gameTime);
            World.floor.Update(gameTime);
            World.objects.Update(gameTime);

            //Collision Update
            Collision.Update(gameTime);

            //Camera Update
            Camera?.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (Camera != null)
            {
                Matrix transformMatrix = Camera.GetViewMatrix();
                SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.AnisotropicWrap);
            }
            else
            {
                SpriteBatch.Begin();
            }
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