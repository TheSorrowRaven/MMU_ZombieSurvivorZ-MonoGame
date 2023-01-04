using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ZombieSurvivorZ
{

    public static class Collision
    {

        private static CollisionComponent CollisionComponent;

        private static readonly HashSet<Collider> AllColliders = new();
        private static readonly HashSet<DynamicCollider> DynamicColliders = new();
        private static readonly HashSet<StaticCollider> StaticColliders = new();



        public static void Initialize()
        {
            Console.WriteLine($"{Game1.ScreenSize.X}, {Game1.ScreenSize.Y}");
            ////CollisionComponent = new(new RectangleF(0, 0, Game1.ScreenSize.X, Game1.ScreenSize.Y));
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var collider in DynamicColliders)
            {
                collider.Update();
            }

            foreach (var collider in DynamicColliders)
            {
                foreach (var other in DynamicColliders)
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

                foreach (var other in StaticColliders)
                {
                    if (collider.Bounds.Intersects(other.Bounds))
                    {
                        Vector2 penetration = CalculatePenetrationVector(collider.Bounds, other.Bounds);
                        collider.OnCollision(other, penetration);
                    }
                }

            }

            ////CollisionComponent.Update(gameTime);
        }

        public static bool Raycast(Vector2 position, Vector2 direction, GameObject[] ignoreObjects, out Collider collider, out float hitDistance)
        {
            collider = null;
            if (!Raycast(position, direction, out List<(Collider, float)> intersections))
            {
                hitDistance = float.NaN;
                return false;
            }
            hitDistance = float.MaxValue;
            bool hit = false;
            foreach (var item in intersections)
            {
                if (ignoreObjects.Contains(item.Item1.Go))
                {
                    continue;
                }
                if (item.Item2 < hitDistance)
                {
                    collider = item.Item1;
                    hitDistance = item.Item2;
                    hit = true;
                }
            }
            return hit;
        }
        public static bool Raycast(Vector2 position, Vector2 direction, out List<(Collider, float)> intersections)
        {
            intersections = new();
            foreach (var collider in AllColliders)
            {
                if (collider.Bounds is RectangleF rect)
                {
                    //There's a bug in Ray2 intersection, thus 3D check is used.
                    //Custom function doesn't seem to work too
                    Ray ray = new(new(position.X, position.Y, 0), new(direction.X, direction.Y, 0));
                    Vector3 min = new(rect.Left, rect.Top, -1);
                    Vector3 max = new(rect.Right, rect.Bottom, 1);
                    BoundingBox box = new(min, max);
                    float? hitDistance = ray.Intersects(box);
                    if (hitDistance != null)
                    {
                        intersections.Add((collider, (float)hitDistance));
                    }
                }
                else if (collider.Bounds is CircleF circle)
                {
                    //Custom function to detect it works perfectly
                    Ray2 ray = new(position, direction);
                    if (GetIntersectionPoints(ray, circle, out float rayNearDistance))
                    {
                        intersections.Add((collider, rayNearDistance));
                    }
                }
            }
            return intersections.Count > 0;
        }

        public abstract class Collider
        {
            public GameObject Go { get; private set; }
            public abstract IShapeF Bounds { get; protected set; }

            public Collider(GameObject go)
            {
                Go = go;
                go.ActiveStateChanged += Go_ActiveStateChanged;
                Go_ActiveStateChanged(true);

                go.Destroyed += Destroy;
            }


            private void Destroy()
            {
                Go.ActiveStateChanged -= Go_ActiveStateChanged;
                Go.Destroyed -= Destroy;
                Go = null;
            }

            protected abstract void Go_ActiveStateChanged(bool active);

        }

        /// <summary>
        /// Static Colliders won't be tested against each other, will not move, and will not call the OnCollision of the GameObject
        /// </summary>
        public abstract class StaticCollider : Collider
        {

            public StaticCollider(GameObject go) : base(go)
            {
            }

            protected override void Go_ActiveStateChanged(bool active)
            {
                if (active)
                {
                    ////CollisionComponent.Insert(this);
                    StaticColliders.Add(this);
                    AllColliders.Add(this);
                    Console.WriteLine("Staic Collider inserted");
                    return;
                }
                ////CollisionComponent.Remove(this);
                StaticColliders.Remove(this);
                AllColliders.Remove(this);
                Console.WriteLine("Staic Collider Removed");
            }

        }

        public class CircleStaticCollider : StaticCollider
        {
            public float Radius { get; private set; }
            public override IShapeF Bounds { get; protected set; }

            public CircleStaticCollider(GameObject go, float radius) : base(go)
            {
                Set(radius);
            }
            public void Set(float radius)
            {
                Radius = radius;
                Bounds = new CircleF(Go.Position, radius);
            }
        }

        public class BoxStaticCollider : StaticCollider
        {
            public float Width { get; private set; }
            public float Height { get; private set; }
            public override IShapeF Bounds { get; protected set; }

            public BoxStaticCollider(GameObject go, float width, float height) : base(go)
            {
                Set(width, height, Go.Position.X - (width / 2), Go.Position.Y - (height / 2));
            }
            public BoxStaticCollider(GameObject go, float width, float height, float xPos, float yPos) : base(go)
            {
                Set(width, height, xPos, yPos);
            }
            public void Set(float width, float height, float xPos, float yPos)
            {
                Width = width;
                Height = height;
                Bounds = new RectangleF(xPos, yPos, width, height);
            }
        }



        public abstract class DynamicCollider : Collider
        {


            public DynamicCollider(GameObject go) : base(go)
            {
            }

            protected override void Go_ActiveStateChanged(bool active)
            {
                if (active)
                {
                    ////CollisionComponent.Insert(this);
                    DynamicColliders.Add(this);
                    AllColliders.Add(this);
                    Console.WriteLine("Dynamic Collider Inserted");
                    return;
                }
                ////CollisionComponent.Remove(this);
                DynamicColliders.Remove(this);
                AllColliders.Remove(this);
                Console.WriteLine("Dynamic Collider Removed");
            }

            public void Update()
            {
                SetBoundsUpdate(Go);
            }

            ////public void OnCollision(CollisionEventArgs collisionInfo)
            ////{
            ////    Console.WriteLine("OnCollision");
            ////    Collider otherCollider = (Collider)collisionInfo.Other;
            ////    go.OnCollision(collisionInfo, this, otherCollider);
            ////}
            public void OnCollision(Collider other, Vector2 penetrationVector)
            {
                Go.OnCollision(this, other, penetrationVector);
            }

            protected virtual void SetBoundsUpdate(GameObject go)
            {
                Bounds.Position = go.Position;
            }
        }

        public class CircleDynamicCollider : DynamicCollider
        {
            public float Radius { get; private set; }
            public override IShapeF Bounds { get; protected set; }

            public CircleDynamicCollider(GameObject go, float radius) : base(go)
            {
                Set(radius);
            }
            public void Set(float radius)
            {
                Radius = radius;
                Bounds = new CircleF(Go.Position, radius);
            }
        }

        public class BoxDynamicCollider : DynamicCollider
        {
            public float Width { get; private set; }
            public float Height { get; private set; }
            public override IShapeF Bounds { get; protected set; }

            public BoxDynamicCollider(GameObject go, float width, float height) : base(go)
            {
                Set(width, height);
            }
            protected override void SetBoundsUpdate(GameObject go)
            {
                Bounds.Position = new(go.Position.X - (Width / 2), go.Position.Y - Height / 2);
            }
            public void Set(float width, float height)
            {
                Width = width;
                Height = height;
                Bounds = new RectangleF(Go.Position.X - (width / 2), Go.Position.Y - (height / 2), width, height);
            }
        }





        public static bool GetIntersectionPoints(Ray2 ray, CircleF circle, out float rayNearDistance)
        {
            // Calculate the intersection points using the formula from https://en.wikipedia.org/wiki/Line%E2%80%93circle_intersection
            Vector2 p = ray.Position;
            Vector2 d = ray.Direction;
            Vector2 c = circle.Center;
            float r = circle.Radius;

            float a = d.X * d.X + d.Y * d.Y;
            float b = 2 * (d.X * (p.X - c.X) + d.Y * (p.Y - c.Y));
            float cc = c.X * c.X + c.Y * c.Y + p.X * p.X + p.Y * p.Y - 2 * (c.X * p.X + c.Y * p.Y) - r * r;

            float discriminant = b * b - 4 * a * cc;
            if (discriminant < 0)
            {
                // No intersections
                rayNearDistance = float.NaN;
                return false;
            }
            else if (discriminant == 0)
            {
                // One intersection
                float t = -b / (2 * a);
                rayNearDistance = t;
                return true;
            }
            // Two intersections
            float t1 = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);
            rayNearDistance = t2;
            return true;
        }



        #region Penetration Vector (Monogame.Extended.Collisions)

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


        #endregion




    }
}