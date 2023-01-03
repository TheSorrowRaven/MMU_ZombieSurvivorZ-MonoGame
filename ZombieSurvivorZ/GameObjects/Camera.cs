using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;

namespace ZombieSurvivorZ
{
    public class Camera
    {

        public readonly OrthographicCamera camera;

        public Camera()
        {
            var viewportAdapter = new BoxingViewportAdapter(Game1.Screen, Game1.Current.GraphicsDevice, (int)Game1.ScreenSize.X, (int)Game1.ScreenSize.Y);
            camera = new(viewportAdapter);
        }

        public void Update()
        {
            Vector2 pos = -Game1.ScreenCenter + Game1.Player.Position;
            pos = new((int)pos.X, (int)pos.Y);
            camera.Position = pos;
        }

        public Matrix GetViewMatrix()
        {
            return camera.GetViewMatrix();
        }

        public Vector2 ScreenToWorld(Vector2 screen)
        {
            return camera.ScreenToWorld(screen);
        }

    }
}