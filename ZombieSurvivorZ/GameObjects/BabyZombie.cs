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
    public class BabyZombie : Zombie
    {

        private const float BabyMovementSpeed = 400f;

        public override int MaxHealth => 10;
        public override float MovementSpeed => BabyMovementSpeed;
        public override float ColliderSize => 15f;
        public override float ScaleF => 0.15f;

    }
}