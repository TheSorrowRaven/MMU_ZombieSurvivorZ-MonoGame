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




        ////public interface IEntity : ICollisionActor
        ////{
        ////    public void Update(GameTime gameTime);
        ////    public void Draw(SpriteBatch spriteBatch);
        ////}
        ////public class CubeEntity : IEntity
        ////{
        ////    private readonly Game1 _game;
        ////    public Vector2 Velocity;
        ////    public IShapeF Bounds { get; }
        //
        ////    public CubeEntity(Game1 game, RectangleF rectangleF)
        ////    {
        ////        _game = game;
        ////        Bounds = rectangleF;
        ////        RandomizeVelocity();
        ////    }
        //
        ////    public virtual void Draw(SpriteBatch spriteBatch)
        ////    {
        ////        spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 3);
        ////    }
        //
        ////    public virtual void Update(GameTime gameTime)
        ////    {
        ////        Bounds.Position += Velocity * gameTime.GetElapsedSeconds() * 50;
        ////    }
        //
        ////    public void OnCollision(CollisionEventArgs collisionInfo)
        ////    {
        ////        Velocity.X *= -1;
        ////        Velocity.Y *= -1;
        ////        Bounds.Position -= collisionInfo.PenetrationVector;
        ////    }
        //
        ////    private void RandomizeVelocity()
        ////    {
        ////        Velocity.X = _game.Random.Next(-1, 2);
        ////        Velocity.Y = _game.Random.Next(-1, 2);
        ////    }
        ////}
        //
        ////public class BallEntity : IEntity
        ////{
        ////    private readonly Game1 _game;
        ////    public Vector2 Velocity;
        ////    public IShapeF Bounds { get; }
        //
        ////    public BallEntity(Game1 game, CircleF circleF)
        ////    {
        ////        _game = game;
        ////        Bounds = circleF;
        ////        RandomizeVelocity();
        ////    }
        //
        ////    public void Draw(SpriteBatch spriteBatch)
        ////    {
        ////        spriteBatch.DrawCircle((CircleF)Bounds, 8, Color.Red, 3f);
        ////    }
        //
        ////    public void Update(GameTime gameTime)
        ////    {
        ////        Bounds.Position += Velocity * gameTime.GetElapsedSeconds() * 30;
        ////    }
        //
        ////    public void OnCollision(CollisionEventArgs collisionInfo)
        ////    {
        ////        Console.WriteLine("OnCollision");
        ////        RandomizeVelocity();
        ////        Bounds.Position -= collisionInfo.PenetrationVector;
        ////    }
        //
        //
        ////    private void RandomizeVelocity()
        ////    {
        ////        Velocity.X = _game.Random.Next(-1, 2);
        ////        Velocity.Y = _game.Random.Next(-1, 2);
        ////    }
        ////}
        //
        ////public readonly Random Random = new Random(Guid.NewGuid().GetHashCode());
        ////private readonly List<IEntity> _entities = new List<IEntity>();
        ////private readonly CollisionComponent _collisionComponent;
        ////const int MapWidth = 500;
        ////const int MapHeight = 500;
        ////GraphicsDeviceManager _graphics;
        ////SpriteBatch _spriteBatch;
        ////public Game1()
        ////{
        ////    _graphics = new GraphicsDeviceManager(this);
        ////    _collisionComponent = new CollisionComponent(new RectangleF(0, 0, MapWidth, MapHeight));
        //
        ////    Content.RootDirectory = "Content";
        ////    IsMouseVisible = true;
        ////}
        //
        ////protected override void Initialize()
        ////{
        ////    base.Initialize();
        ////    _graphics.PreferredBackBufferHeight = MapHeight;
        ////    _graphics.PreferredBackBufferWidth = MapWidth;
        ////    _graphics.ApplyChanges();
        //
        ////    for (var i = 0; i < 50; i++)
        ////    {
        ////        var size = Random.Next(20, 40);
        ////        var position = new Point2(Random.Next(-MapWidth, MapWidth * 2), Random.Next(0, MapHeight));
        ////        if (i % 2 == 0)
        ////        {
        ////            _entities.Add(new BallEntity(this, new CircleF(position, size)));
        ////        }
        ////        else
        ////        {
        ////            _entities.Add(new CubeEntity(this, new RectangleF(position, new Size2(size, size))));
        ////        }
        ////    }
        //
        ////    foreach (IEntity entity in _entities)
        ////    {
        ////        _collisionComponent.Insert(entity);
        ////    }
        ////}
        ////protected override void Update(GameTime gameTime)
        ////{
        ////    foreach (IEntity entity in _entities)
        ////    {
        ////        entity.Update(gameTime);
        ////    }
        //
        ////    _collisionComponent.Update(gameTime);
        //
        ////    base.Update(gameTime);
        ////}
        ////protected override void Draw(GameTime gameTime)
        ////{
        ////    GraphicsDevice.Clear(Color.CornflowerBlue);
        //
        ////    _spriteBatch.Begin();
        ////    foreach (IEntity entity in _entities)
        ////    {
        ////        entity.Draw(_spriteBatch);
        ////    }
        //
        ////    _spriteBatch.End();
        //
        ////    base.Draw(gameTime);
        ////}






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
            Player.PlayerCL = new Collision.CircleCollider(Player, 25);



            var c = new Crate()
            {
                Position = new(200, 100)
            };
            c.CrateCL = new Collision.BoxCollider(c, 40, 40);

            c = new Crate()
            {
                Position = new(-400, -300)
            };
            c.CrateCL = new Collision.BoxCollider(c, 40, 40);

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