using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ZombieSurvivorZ
{
    public abstract class GameObject
    {
        public bool Active { get; set; } = true;
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }

        private Vector2 _heading = new(1, 0);
        private bool _headingDirty = true;

        public Vector2 Heading
        {
            get => _heading;
            set
            {
                _heading = value;
                // We ALWAYS normalize heading
                _heading.Normalize();

                // Also, when heading is changed, we flag it
                // to ensure rotation is recalculated.
                _headingDirty = true;
            }
        }

        private float _rotation;
        public float RotationOffset { get; set; }

        protected float Rotation
        {
            get
            {
                // Atan2 is expensive, so only calculate when needed
                if (_headingDirty)
                {
                    _rotation = MathF.Atan2(Heading.Y, Heading.X) + RotationOffset;
                    _headingDirty = false;
                }
                return _rotation;
            }
        }

        private bool _alive = true;
        public bool Alive => _alive;

        public GameObject()
        {
            Name = "New GameObject";
        }

        public GameObject(string name)
        {
            Name = name;
        }

        public abstract Rectangle Bounds();

        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }

        public virtual void OnCollision(GameObject other)
        { }

        public virtual void OnDestroy()
        { }

        public void Destroy()
        {
            Destroy(this);
        }

        public static void Destroy(GameObject go)
        {
            go.Active = false;
            go._alive = false;
            World.RemoveGameObject(go);
        }
    }
}