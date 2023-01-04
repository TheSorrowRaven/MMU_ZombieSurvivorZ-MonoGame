using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivorZ
{
    public struct Vector2Int : IEqualityComparer<Vector2Int>
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Vector2Int x, Vector2Int y)
        {
            return X == y.X && Y == x.Y;
        }

        public int GetHashCode([DisallowNull] Vector2Int obj)
        {
            return X << 16 | Y;
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new(a.X - b.X, a.Y - b.Y);
        }


    }
}
