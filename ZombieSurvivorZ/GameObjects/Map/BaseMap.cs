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


        public Vector2 ActualPosition => Position + MapManager.Position;

        private Vector2 tileSize;
        public Vector2 TileSize => tileSize;

        public BaseMap(TiledMapTileLayer layer)
        {
            Layer = layer;
            Renderer = new TiledMapRenderer(Game1.Current.GraphicsDevice, Map);
            tileSize = new(Layer.TileWidth, Layer.TileHeight);
        }

        public override void Initialize()
        {
            base.Initialize();
            for (ushort y = 0; y < Layer.Height; y++)
            {
                for (ushort x = 0; x < Layer.Width; x++)
                {
                    TiledMapTile tile = Layer.GetTile(x, y);
                    if (tile.GlobalIdentifier == 0)
                    {
                        continue;
                    }
                    InitializeTile(x, y, tile);
                }
            }
        }

        protected virtual void InitializeTile(int x, int y, TiledMapTile tile)
        {

        }

        public Vector2 LocalToTileTopLeftPosition(int x, int y)
        {
            return new(x * Layer.TileWidth + ActualPosition.X, y * Layer.TileHeight + ActualPosition.Y);
        }

        public override void Update()
        {
            base.Update();
            Renderer.Update(Time.gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Matrix viewMatrix = Game1.Camera.GetViewMatrix();
            Vector2 actualPosition = ActualPosition;
            viewMatrix.Translation += new Vector3(actualPosition.X, actualPosition.Y, 0);
            Renderer.Draw(Layer, viewMatrix);
        }

    }
}