using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ZombieSurvivorZ
{

    public static class Collision
    {

        private static CollisionComponent CollisionComponent;
        private static readonly HashSet<Collider> ActiveColliders = new();



        public static void Initialize()
        {
            Console.WriteLine($"{Game1.ScreenSize.X}, {Game1.ScreenSize.Y}");
            ////CollisionComponent = new(new RectangleF(0, 0, Game1.ScreenSize.X, Game1.ScreenSize.Y));
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var collider in ActiveColliders)
            {
                collider.Update();
            }

            foreach (var collider in ActiveColliders)
            {
                foreach (var other in ActiveColliders)
                {
                    if (collider == other)
                    {
                        continue;
                    }
                    if (collider.Bounds.Intersects(other.Bounds))
                    {
                        Vector2 penetration = CalculatePenetrationVector(collider.Bounds, other.Bounds);
                        collider.OnCollision(other, penetration);
                    }
                }
            }

            ////CollisionComponent.Update(gameTime);
        }

        public abstract class Collider//// : ICollisionActor
        {

            private GameObject go;
            public abstract IShapeF Bounds { get; protected set; }

            public Collider(GameObject go)
            {
                this.go = go;
                go.ActiveStateChanged += Go_ActiveStateChanged;
                Go_ActiveStateChanged(true);

                go.Destroyed += Destroy;
            }

            private void Go_ActiveStateChanged(bool active)
            {
                if (active)
                {
                    ////CollisionComponent.Insert(this);
                    ActiveColliders.Add(this);
                    Console.WriteLine("Insert");
                    return;
                }
                ////CollisionComponent.Remove(this);
                ActiveColliders.Remove(this);
                Console.WriteLine("Removed");
            }

            public void Update()
            {
                SetBounds(go);
            }

            ////public void OnCollision(CollisionEventArgs collisionInfo)
            ////{
            ////    Console.WriteLine("OnCollision");
            ////    Collider otherCollider = (Collider)collisionInfo.Other;
            ////    go.OnCollision(collisionInfo, this, otherCollider);
            ////}
            public void OnCollision(Collider other, Vector2 penetrationVector)
            {
                go.OnCollision(this, other, penetrationVector);
            }

            private void Destroy()
            {
                go.ActiveStateChanged -= Go_ActiveStateChanged;
                go.Destroyed -= Destroy;
                go = null;
            }

            protected virtual void SetBounds(GameObject go)
            {
                Bounds.Position = go.Position;
            }
        }

        public class CircleCollider : Collider
        {
            public float Radius { get; private set; }
            public override IShapeF Bounds { get; protected set; }

            public CircleCollider(GameObject go, float radius) : base(go)
            {
                Radius = radius;
                Bounds = new CircleF(go.Position, radius);
            }
        }

        public class BoxCollider : Collider
        {
            public float Width { get; private set; }
            public float Height { get; private set; }
            public override IShapeF Bounds { get; protected set; }

            public BoxCollider(GameObject go, float width, float height) : base(go)
            {
                Width = width;
                Height = height;
                Bounds = new RectangleF(go.Position.X - (width / 2), go.Position.Y - (height / 2), width, height);
            }

            protected override void SetBounds(GameObject go)
            {
                Bounds.Position = new(go.Position.X - (Width / 2), go.Position.Y - Height / 2);
            }
        }

























        //
        // Summary:
        //     Calculate a's penetration into b
        //
        // Parameters:
        //   a:
        //     The penetrating shape.
        //
        //   b:
        //     The shape being penetrated.
        //
        // Returns:
        //     The distance vector from the edge of b to a's Position
        private static Vector2 CalculatePenetrationVector(IShapeF a, IShapeF b)
        {
            if (a is RectangleF)
            {
                RectangleF rectangleF = (RectangleF)(object)a;
                if (b is RectangleF)
                {
                    RectangleF rect = (RectangleF)(object)b;
                    return PenetrationVector(rectangleF, rect);
                }

                RectangleF rect2 = rectangleF;
                if (b is CircleF)
                {
                    CircleF circ = (CircleF)(object)b;
                    return PenetrationVector(rect2, circ);
                }
            }
            else if (a is CircleF)
            {
                CircleF circleF = (CircleF)(object)a;
                if (b is CircleF)
                {
                    CircleF circ2 = (CircleF)(object)b;
                    return PenetrationVector(circleF, circ2);
                }

                CircleF circ3 = circleF;
                if (b is RectangleF)
                {
                    RectangleF rect3 = (RectangleF)(object)b;
                    return PenetrationVector(circ3, rect3);
                }
            }

            throw new NotSupportedException("Shapes must be either a CircleF or RectangleF");
        }

        private static Vector2 PenetrationVector(RectangleF rect1, RectangleF rect2)
        {
            RectangleF rectangleF = RectangleF.Intersection(rect1, rect2);
            Vector2 result;
            if (rectangleF.Width < rectangleF.Height)
            {
                float x = ((rect1.Center.X < rect2.Center.X) ? rectangleF.Width : (0f - rectangleF.Width));
                result = new Vector2(x, 0f);
            }
            else
            {
                float y = ((rect1.Center.Y < rect2.Center.Y) ? rectangleF.Height : (0f - rectangleF.Height));
                result = new Vector2(0f, y);
            }

            return result;
        }

        private static Vector2 PenetrationVector(CircleF circ1, CircleF circ2)
        {
            if (!circ1.Intersects(circ2))
            {
                return Vector2.Zero;
            }

            Vector2 vector = Point2.Displacement(circ1.Center, circ2.Center);
            Vector2 vector2 = ((!(vector != Vector2.Zero)) ? (-Vector2.UnitY * (circ1.Radius + circ2.Radius)) : (vector.NormalizedCopy() * (circ1.Radius + circ2.Radius)));
            return vector - vector2;
        }

        private static Vector2 PenetrationVector(CircleF circ, RectangleF rect)
        {
            Vector2 vector = rect.ClosestPointTo(circ.Center) - circ.Center;
            if (rect.Contains(circ.Center) || vector.Equals(Vector2.Zero))
            {
                Vector2 vector2 = Point2.Displacement(circ.Center, rect.Center);
                Vector2 vector5;
                if (vector2 != Vector2.Zero)
                {
                    Vector2 vector3 = new Vector2(vector2.X, 0f);
                    Vector2 vector4 = new Vector2(0f, vector2.Y);
                    vector3.Normalize();
                    vector4.Normalize();
                    vector3 *= circ.Radius + rect.Width / 2f;
                    vector4 *= circ.Radius + rect.Height / 2f;
                    if (vector3.LengthSquared() < vector4.LengthSquared())
                    {
                        vector5 = vector3;
                        vector2.Y = 0f;
                    }
                    else
                    {
                        vector5 = vector4;
                        vector2.X = 0f;
                    }
                }
                else
                {
                    vector5 = -Vector2.UnitY * (circ.Radius + rect.Height / 2f);
                }

                return vector2 - vector5;
            }

            return circ.Radius * vector.NormalizedCopy() - vector;
        }

        private static Vector2 PenetrationVector(RectangleF rect, CircleF circ)
        {
            return -PenetrationVector(circ, rect);
        }
















    }
}