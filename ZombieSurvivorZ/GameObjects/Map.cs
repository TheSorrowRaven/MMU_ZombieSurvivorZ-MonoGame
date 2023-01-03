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
    public class Map : SpriteObject
    {

        private TiledMap map;
        private TiledMapLayer layer;
        private TiledMapRenderer tiledMapRenderer;

        public Map()
        {
            map = Game1.GetContent<TiledMap>("zombie-survivorz");
            layer = map.GetLayer("Tile Layer 1");
            tiledMapRenderer = new TiledMapRenderer(Game1.Current.GraphicsDevice, map);
            Position = new(map.TileWidth * map.Width * -0.5f, map.TileHeight * map.Height * -0.5f);
        }

        public override void Initialize()
        {
            base.Initialize();
        }


        public override void Update()
        {
            base.Update();
            tiledMapRenderer.Update(Time.gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Matrix viewMatrix = Game1.Camera.GetViewMatrix();
            viewMatrix.Translation += new Vector3(Position.X, Position.Y, 0);
            tiledMapRenderer.Draw(layer, viewMatrix);
        }

    }
}