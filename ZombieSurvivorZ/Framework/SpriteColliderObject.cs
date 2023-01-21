using Microsoft.Xna.Framework;
using static ZombieSurvivorZ.Collision;

namespace ZombieSurvivorZ
{
    public class SpriteColliderObject : SpriteObject
    {

        public DynamicCollider CL { get; protected set; }

        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                CL?.UpdatePosition(value);
            }
        }

    }
}