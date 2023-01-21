using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace ZombieSurvivorZ
{
    public class Camera
    {

        public readonly OrthographicCamera camera;

        public event Action OnCameraUpdated = () => { };

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

            OnCameraUpdated.Invoke();
        }

        public Matrix GetViewMatrix()
        {
            return camera.GetViewMatrix();
        }

        public Vector2 ScreenToWorld(Vector2 screen)
        {
            return camera.ScreenToWorld(screen);
        }
        public Vector2 WorldToScreen(Vector2 world)
        {
            return camera.WorldToScreen(world);
        }

    }
}