using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZombieSurvivorZ
{

    public static class Collision
    {

        private static readonly HashSet<Collider> AllColliders = new();

        private static readonly HashSet<DynamicCollider> DynamicColliders = new();
        private static readonly HashSet<StaticCollider> StaticColliders = new();
        private static readonly HashSet<StaticCollider> RaycastIgnoredStaticColliders = new();


        public static void Initialize()
        {

        }

        public static void Update(GameTime gameTime)
        {
            foreach (var collider in DynamicColliders)
            {
                //Dynamic Colliders collision
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

                //Other colliders
                foreach (var other in StaticColliders)
                {
                    if (collider.Bounds.Intersects(other.Bounds))
                    {
                        Vector2 penetration = CalculatePenetrationVector(collider.Bounds, other.Bounds);
                        collider.OnCollision(other, penetration);
                    }
                }

                foreach (var other in RaycastIgnoredStaticColliders)
                {
                    if (collider.Bounds.Intersects(other.Bounds))
                    {
                        Vector2 penetration = CalculatePenetrationVector(collider.Bounds, other.Bounds);
                        collider.OnCollision(other, penetration);
                    }
                }

            }
        }

        //Raycast all, including the raycast ignored colliders
        public static bool RaycastAll(Vector2 position, Vector2 direction, GameObject[] ignoreObjects, out Collider collider, out float hitDistance)
        {
            collider = null;
            if (!RaycastAll(position, direction, out List<(Collider, float)> intersections))
            {
                hitDistance = float.NaN;
                return false;
            }
            hitDistance = float.MaxValue;
            bool hit = false;
            foreach (var item in intersections)
            {
                if (ignoreObjects != null)
                {
                    if (ignoreObjects.Contains(item.Item1.Go))
                    {
                        continue;
                    }
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

        //Normal raycast, excluding the raycast ignored colliders
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
                if (ignoreObjects != null)
                {
                    if (ignoreObjects.Contains(item.Item1.Go))
                    {
                        continue;
                    }
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
            RaycastSet(position, direction, DynamicColliders, intersections);
            RaycastSet(position, direction, StaticColliders, intersections);
            return intersections.Count > 0;
        }
        public static bool RaycastAll(Vector2 position, Vector2 direction, out List<(Collider, float)> intersections)
        {
            intersections = new();
            RaycastSet(position, direction, DynamicColliders, intersections);
            RaycastSet(position, direction, StaticColliders, intersections);
            RaycastSet(position, direction, RaycastIgnoredStaticColliders, intersections);
            return intersections.Count > 0;
        }

        //Do raycast on a hashset
        private static void RaycastSet<T>(Vector2 position, Vector2 direction, HashSet<T> colliders, List<(Collider, float)> intersections) where T : Collider
        {
            Ray ray = new(new(position.X, position.Y, 0), new(direction.X, direction.Y, 0));
            foreach (var collider in colliders)
            {
                if (collider.Bounds is RectangleF rect)
                {
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
                    BoundingSphere sphere = new(new(circle.Center, 0), circle.Radius);
                    float? hitDistance = ray.Intersects(sphere);
                    if (hitDistance != null)
                    {
                        intersections.Add((collider, (float)hitDistance));
                    }
                }
            }
        }

        #region Collider Objects

        public abstract class Collider
        {
            public GameObject Go { get; private set; }
            public IShapeF Bounds { get; protected set; }

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

            public void DestroyCollider()
            {
                Go_ActiveStateChanged(false);
                Destroy();
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
                    StaticColliders.Add(this);
                    AllColliders.Add(this);
                    return;
                }
                StaticColliders.Remove(this);
                AllColliders.Remove(this);
            }

        }
        public class CircleStaticCollider : StaticCollider
        {
            public CircleStaticCollider(GameObject go, float radius) : base(go)
            {
                Set(radius);
            }
            public void Set(float radius)
            {
                Bounds = new CircleF(Go.Position, radius);
            }
        }
        public class BoxStaticCollider : StaticCollider
        {
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
                Bounds = new RectangleF(xPos, yPos, width, height);
            }
        }

        public abstract class RaycastIgnoredStaticCollider : StaticCollider
        {
            public RaycastIgnoredStaticCollider(GameObject go) : base(go)
            {
            }

            protected override void Go_ActiveStateChanged(bool active)
            {
                if (active)
                {
                    RaycastIgnoredStaticColliders.Add(this);
                    AllColliders.Add(this);
                    return;
                }
                RaycastIgnoredStaticColliders.Remove(this);
                AllColliders.Remove(this);
            }
        }
        public class RaycastIgnoredCircleStaticCollider : RaycastIgnoredStaticCollider
        {
            public RaycastIgnoredCircleStaticCollider(GameObject go, float radius) : base(go)
            {
                Set(radius);
            }
            public void Set(float radius)
            {
                Bounds = new CircleF(Go.Position, radius);
            }
        }
        public class RaycastIgnoredBoxStaticCollider : RaycastIgnoredStaticCollider
        {
            public RaycastIgnoredBoxStaticCollider(GameObject go, float width, float height) : base(go)
            {
                Set(width, height, Go.Position.X - (width / 2), Go.Position.Y - (height / 2));
            }
            public RaycastIgnoredBoxStaticCollider(GameObject go, float width, float height, float xPos, float yPos) : base(go)
            {
                Set(width, height, xPos, yPos);
            }
            public void Set(float width, float height, float xPos, float yPos)
            {
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
                    DynamicColliders.Add(this);
                    AllColliders.Add(this);
                    return;
                }
                DynamicColliders.Remove(this);
                AllColliders.Remove(this);
            }

            public void OnCollision(Collider other, Vector2 penetrationVector)
            {
                Go.OnCollision(this, other, penetrationVector);
            }

            public virtual void UpdatePosition(Vector2 pos)
            {
                Bounds.Position = pos;
            }
        }
        public class CircleDynamicCollider : DynamicCollider
        {
            public float Radius { get; private set; }

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

            public BoxDynamicCollider(GameObject go, float width, float height) : base(go)
            {
                Set(width, height);
            }
            public override void UpdatePosition(Vector2 pos)
            {
                Bounds.Position = new(pos.X - (Width / 2), pos.Y - Height / 2);
            }
            public void Set(float width, float height)
            {
                Width = width;
                Height = height;
                Bounds = new RectangleF(Go.Position.X - (width / 2), Go.Position.Y - (height / 2), width, height);
            }
        }

        #endregion

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