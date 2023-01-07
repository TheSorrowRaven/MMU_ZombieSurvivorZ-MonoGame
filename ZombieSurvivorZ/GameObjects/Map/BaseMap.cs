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
        public static Vector2 TileSize => MapManager.TileSize;

        public BaseMap(TiledMapTileLayer layer)
        {
            Layer = layer;
            Renderer = new TiledMapRenderer(Game1.Current.GraphicsDevice, Map);
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

        public static Vector2 LocalToTileTopLeftPosition(Vector2Int local)
        {
            return MapManager.LocalToTileTopLeftPosition(local.X, local.Y);
        }
        public static Vector2 LocalToTileTopLeftPosition(int x, int y)
        {
            return MapManager.LocalToTileTopLeftPosition(x, y);
        }
        public static Vector2Int PositionToLocal(Vector2 pos)
        {
            return MapManager.PositionToLocal(pos);
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