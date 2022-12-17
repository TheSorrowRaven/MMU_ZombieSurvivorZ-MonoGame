using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Gui.Controls;
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

        public static bool Raycast(Vector2 position, Vector2 direction, out Collider collider, out float hitDistance)
        {
            collider = null;
            if (!Raycast(position, direction, out Dictionary<Collider, float> intersections))
            {
                hitDistance = float.NaN;
                return false;
            }
            hitDistance = float.MaxValue;
            foreach (var item in intersections)
            {
                if (item.Value < hitDistance)
                {
                    collider = item.Key;
                    hitDistance = item.Value;
                }
            }
            return true;
        }
        public static bool Raycast(Vector2 position, Vector2 direction, out Dictionary<Collider, float> intersections)
        {
            Ray2 ray = new(position, direction);
            intersections = new();
            foreach (var collider in ActiveColliders)
            {
                if (collider.Bounds is RectangleF rect)
                {
                    if (GetIntersectionPoints(new Line(ray.Position, ray.Direction), rect, out float hitDistance))
                    {
                        intersections.Add(collider, hitDistance);
                    }
                    //if (ray.Intersects(rect, out float hitDistance, out float tt))
                    //{
                    //    if (float.IsNaN(hitDistance))
                    //    {
                    //        Console.WriteLine("NAN?? tt => " + tt);
                    //    }
                    //}
                }
                else if (collider.Bounds is CircleF circle)
                {
                    if (GetIntersectionPoints(ray, circle, out float rayNearDistance))
                    {
                        intersections.Add(collider, rayNearDistance);
                    }
                }
            }
            return intersections.Count > 0;
        }

        public abstract class Collider//// : ICollisionActor
        {

            public GameObject Go { get; private set; }
            public abstract IShapeF Bounds { get; protected set; }

            public Collider(GameObject go)
            {
                this.Go = go;
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
                SetBounds(Go);
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

            private void Destroy()
            {
                Go.ActiveStateChanged -= Go_ActiveStateChanged;
                Go.Destroyed -= Destroy;
                Go = null;
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























        public static bool GetIntersectionPoints(Line ray, RectangleF rect, out float rayNearDistance)
        {
            // Check intersection with each of the four sides of the box
            Line[] sides = new Line[4];
            sides[0] = new Line(new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y));
            sides[1] = new Line(new Vector2(rect.X + rect.Width, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + rect.Height));
            sides[2] = new Line(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), new Vector2(rect.X, rect.Y + rect.Height));
            sides[3] = new Line(new Vector2(rect.X, rect.Y + rect.Height), new Vector2(rect.X, rect.Y));

            bool hit = false;
            rayNearDistance = float.MaxValue;
            foreach (Line side in sides)
            {
                if (!GetIntersectionDistance(ray, side, out float rayDistance))
                {
                    continue;
                }
                if (rayDistance < 0)
                {
                    continue;
                }
                hit = true;
                if (rayDistance < rayNearDistance)
                {
                    rayNearDistance = rayDistance;
                }
            }

            return hit;
        }

        private static bool GetIntersectionDistance(Line line1, Line line2, out float rayDistance)
        {
            // Calculate the intersection point using the formula from https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
            float x1 = line1.start.X;
            float x2 = line1.end.X;
            float x3 = line2.start.X;
            float x4 = line2.end.X;
            float y1 = line1.start.Y;
            float y2 = line1.end.Y;
            float y3 = line2.start.Y;
            float y4 = line2.end.Y;

            float divd = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / divd;
            //float u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / divd;

            Console.WriteLine(t);

            //if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            //{

            //}
            Vector2 t1 = new(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
            //Vector2 t2 = new(x3 + u * (x4 - x3), y3 + u * (y4 - y3));
            rayDistance = float.NaN;
            return false;



            //Vector2 p = line1.Position;
            //Vector2 r = line1.Direction;
            //Vector2 q = line2.Position;
            //Vector2 s = line2.Direction;

            //rayDistance = CrossProduct((q - p), s) / CrossProduct(r, s);
            //if (float.IsNaN(rayDistance))
            //{
            //    // Lines are parallel
            //    return false;
            //}
            //return true;
        }

        private static float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
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