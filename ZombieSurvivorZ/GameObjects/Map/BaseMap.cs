using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace ZombieSurvivorZ
{
    public abstract class BaseMap : SpriteObject
    {
        protected static MapManager MapManager => Game1.MapManager;
        protected static TiledMap Map => MapManager.Map;

        public readonly TiledMapTileLayer Layer;
        protected readonly TiledMapRenderer Renderer;

        public BaseMap(TiledMapTileLayer layer)
        {
            Layer = layer;
            Renderer = new TiledMapRenderer(Game1.Current.GraphicsDevice, Map);
        }

        public override void Initialize()
        {
            base.Initialize();
        }


        public override void Update()
        {
            base.Update();
            Renderer.Update(Time.gameTime);
        }

        public Vector2 ActualPosition => Position + MapManager.Position;

        public override void Draw(SpriteBatch spriteBatch)
        {
            Matrix viewMatrix = Game1.Camera.GetViewMatrix();
            viewMatrix.Translation += new Vector3(Position.X + MapManager.Position.X, Position.Y + MapManager.Position.Y, 0);
            Renderer.Draw(Layer, viewMatrix);
        }

    }
}