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

        private const float BaseColliderSize = 24;

        private float ClosestDistanceToPlayer => Game1.Player.ColliderSize + BaseColliderSize;

        private float movementSpeed = 100;
        private int health;

        private readonly Stack<Vector2Int> Path = new();
        private bool hasNextTargetPos = false;
        private Vector2 nextTargetPos = new(float.MinValue, float.MinValue);

        public Zombie()
        {
            health = 10;
            Scale = new(0.25f, 0.25f);
            CL = new CircleDynamicCollider(this, BaseColliderSize);
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

        private void CalculatePathToPlayer()
        {
            Game1.MapManager.Pathfind(Game1.MapManager.PositionToLocal(Position), Game1.Player.CellPosition, Path);
            SetNextTargetPos();
        }

        public override void Update()
        {
            base.Update();

            if (hasNextTargetPos)
            {
                Vector2 direction = nextTargetPos - Position;
                float distance = direction.Length();
                if (distance == 0)
                {
                    SetNextTargetPos();
                    return;
                }
                direction /= distance;  //Normalize
                Heading = direction;

                float move = movementSpeed * Time.deltaTime;
                if (distance < move)
                {
                    SetNextTargetPos();
                }

                Vector2 movement = direction * move;
                Position += movement;
            }
            else
            {
                CalculatePathToPlayer();
            }

        }

        private void SetNextTargetPos()
        {
            if (Path.TryPop(out Vector2Int cell))
            {
                nextTargetPos = Game1.MapManager.LocalToTileCenterPosition(cell);
                hasNextTargetPos = true;
                return;
            }
            hasNextTargetPos = false;
        }

        public override void OnCollision(DynamicCollider current, Collider other, Vector2 penetrationVector)
        {
            OnCollision_PushBack(current, other, penetrationVector);
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

            //Stack<Vector2Int> path = new(Path);
            //Vector2Int firstPos = Game1.MapManager.PositionToLocal(Position);
            //while (path.TryPop(out Vector2Int pos))
            //{
            //    spriteBatch.DrawLine(Game1.MapManager.LocalToTileCenterPosition(firstPos.X, firstPos.Y), Game1.MapManager.LocalToTileCenterPosition(pos.X, pos.Y), Color.Yellow, 2);
            //    firstPos = pos;
            //}
        }

    }
}