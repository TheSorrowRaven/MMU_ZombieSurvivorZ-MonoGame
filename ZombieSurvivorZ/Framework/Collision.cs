using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{

    // This is a very simple Collision Checker class

    public static class Collision
    {
        public interface ICollisionBody : IDisposable
        {
            public GameObject GameObject { get; }

            public string Tag { get; }
        }

        public class CollisionBody : ICollisionBody
        {
            private GameObject gameObject;

            private string tag = "";

            public CollisionBody(GameObject gameObject, string tag)
            {
                this.gameObject = gameObject;
                this.tag = tag;
            }

            public GameObject GameObject { get => gameObject; }

            public string Tag { get => tag; }

            public void Dispose()
            {
                gameObject = null;
            }
        }

        private static readonly List<ICollisionBody> bodies = new();

        public static ICollisionBody CreateBody(GameObject go, string tag = "")
        {
            if (go == null)
            {
                throw new Exception("[Collision.CreateBody] GameObject cannot be null!");
            }

            CollisionBody body = new(go, tag);
            bodies.Add(body);
            return body;
        }

        public static void RemoveBody(ICollisionBody body)
        {
            bodies.Remove(body);
        }

        public static void Simulate()
        {
            for (int i = 0; i < bodies.Count - 1; i++)
            {
                ICollisionBody go1 = bodies[i];

                // GameObject may be flagged for deletion due to previous GameObject
                // calling OnCollision
                if (!go1.GameObject.Alive)
                {
                    throw new Exception($"{bodies[i].GameObject.Name} is dead, remove this!");
                }

                if (!go1.GameObject.Active)
                {
                    continue;
                }

                for (int j = i + 1; j < bodies.Count; j++)
                {
                    ICollisionBody go2 = bodies[j];

                    if (!go2.GameObject.Active)
                    {
                        continue;
                    }

                    // Don't check for collision between GameObjects with same tag
                    if (go1.Tag == go2.Tag) continue;

                    if (IsCollided_AABB(go1, go2))
                    {
                        go1.GameObject.OnCollision(go2.GameObject);
                        go2.GameObject.OnCollision(go1.GameObject);
                    }
                }
            }

        }

        private static bool IsCollided_AABB(ICollisionBody obj1, ICollisionBody obj2)
        {
            GameObject go1 = obj1.GameObject;
            GameObject go2 = obj2.GameObject;
            // TL = top left, BR = bottom right
            Rectangle obj1Bounds = go1.Bounds();
            Rectangle obj2Bounds = go2.Bounds();

            Vector2 obj1_TL = new(obj1Bounds.X, obj1Bounds.Y);
            Vector2 obj2_TL = new(obj2Bounds.X, obj2Bounds.Y);
            Vector2 obj1_BR = go1.Position + new Vector2(obj1Bounds.Width, obj1Bounds.Height);
            Vector2 obj2_BR = go2.Position + new Vector2(obj2Bounds.Width, obj2Bounds.Height);

            if (obj1_BR.X < obj2_TL.X || obj2_BR.X < obj1_TL.X ||
                obj1_BR.Y < obj2_TL.Y || obj2_BR.Y < obj1_TL.Y)
                return false;
            else
                return true;
        }
    }
}