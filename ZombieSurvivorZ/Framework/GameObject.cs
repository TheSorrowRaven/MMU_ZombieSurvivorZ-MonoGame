using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System;
using System.Diagnostics.SymbolStore;
using static ZombieSurvivorZ.Collision;

namespace ZombieSurvivorZ
{
    public abstract class GameObject
    {
        public virtual World World => World.objects;

        private bool active = true;
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                if (value == active)
                {
                    return;
                }
                active = value;
                ActiveStateChanged.Invoke(value);
            }
        }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        private Vector2 scale = Vector2.One;
        public Vector2 Scale
        {
            get => scale;
            set
            {
                scale = value;
                ScaleChanged();
            }
        }

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

        public GameObject() : this("New GameObject")
        {

        }

        public GameObject(string name)
        {
            Name = name;
            World.AddGameObject(this);
        }


        public event Action<bool> ActiveStateChanged = _ => { };
        public event Action Destroyed = () => { };




        //Used in collisions
        public abstract Rectangle Bounds();

        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }

        public virtual void OnCollision(Collider current, Collider other, Vector2 penetrationVector)
        { }

        public virtual void OnDestroy()
        { }

        public virtual void OnCollision_PushBack(Collider current, Collider other, Vector2 penetrationVector)
        {
            Position -= penetrationVector;
        }

        protected virtual void ScaleChanged()
        {

        }

        public void Destroy()
        {
            Destroy(this);
        }

        public static void Destroy(GameObject go)
        {
            go.Active = false;
            go._alive = false;
            go.Destroyed.Invoke();
            go.World.RemoveGameObject(go);
        }
    }
}