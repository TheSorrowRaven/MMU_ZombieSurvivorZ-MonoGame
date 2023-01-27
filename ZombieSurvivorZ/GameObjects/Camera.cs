/********************************************
Course : TGD3351 Game Algorithms
Session: Trimester 1, 2022/23
ID and Name #1 : 1191101213 RavenLimZheXuan
Contacts #1 : 601155873318 1191101213@student.mmu.edu.my
ID and Name #2 : 1181103109 EuwernYongChernJun
Contacts #2 : 60163371078 1181103109@student.mmu.edu.my
********************************************/
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