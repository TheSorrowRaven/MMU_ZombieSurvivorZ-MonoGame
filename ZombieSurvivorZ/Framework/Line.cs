using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System;
using System.Diagnostics.SymbolStore;
using static ZombieSurvivorZ.Collision;

namespace ZombieSurvivorZ
{

    public struct Line
    {
        public Vector2 start;
        public Vector2 end;

        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }
    }

}