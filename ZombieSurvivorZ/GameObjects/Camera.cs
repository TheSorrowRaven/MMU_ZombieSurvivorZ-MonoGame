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
            var viewportAdapter = new BoxingViewportAdapter(Game1.Screen, Game1.Current.GraphicsDevice, 800, 480);
            camera = new(viewportAdapter);
        }

        public void Update()
        {
            Vector2 pos = -Game1.ScreenCenter + Game1.Current.Player.Position;
            camera.Position = pos;
        }

        public Matrix GetViewMatrix()
        {
            return camera.GetViewMatrix();
        }

    }
}