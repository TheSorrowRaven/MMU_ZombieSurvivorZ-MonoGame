using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ZombieSurvivorZ.Collision;

namespace ZombieSurvivorZ
{
    public class Zombie : SpriteColliderObject
    {

        private int health;

        public Zombie()
        {
            health = 10;
            Scale = new(0.25f, 0.25f);
            CL = new CircleDynamicCollider(this, 24);
        }

        public override void Initialize()
        {
            Texture = Game1.GetTexture("Zombie/skeleton-idle_0");


        }

        public void DealDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            CL.DestroyCollider();
            CL = null;
        }

        public override void Update()
        {
            base.Update();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);


            if (Game1.CollisionDebugging && CL != null)
            {
                spriteBatch.DrawCircle((CircleF)CL.Bounds, 20, Color.Red);
            }

            if (Time.frameCount == 0)
            {
                return;
            }

            LinkedList<Vector2Int> path = Game1.MapManager.Pathfind(Game1.MapManager.PositionToLocal(Position), Game1.Player.CellPosition);
            Vector2Int firstPos = Game1.MapManager.PositionToLocal(Position);
            while (path.Count > 0)
            {
                Vector2Int pos = path.First.Value;
                path.RemoveFirst();

                spriteBatch.DrawLine(Game1.MapManager.LocalToTileCenterPosition(firstPos.X, firstPos.Y), Game1.MapManager.LocalToTileCenterPosition(pos.X, pos.Y), Color.Yellow, 2);
                firstPos = pos;
            }
        }

    }
}