using Microsoft.Xna.Framework;
using System;

namespace ZombieSurvivorZ
{
    public static class MathUtils
    {
        /// <summary>
        /// <para>Compares two floating point values if they are similar.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static bool Approximately(float a, float b)
        {
            return (double)MathF.Abs(b - a) < (double)MathF.Max(1E-06f * MathF.Max(MathF.Abs(a),
                MathF.Abs(b)), float.Epsilon * 8.0f);
        }

        public static float Lerp(float from, float to, float t)
        {
            return from + t * (to - from);
        }

    }

    public static class Vector2Utils
    {
        // Returns a copy of /vector/ with its magnitude clamped to /maxLength/.
        public static Vector2 Truncate(Vector2 vector, float maxLength)
        {
            float sqrmag = vector.LengthSquared();
            if (sqrmag > maxLength * maxLength)
            {
                float mag = (float)Math.Sqrt(sqrmag);
                //these intermediate variables force the intermediate result to be
                //of float precision. without this, the intermediate result can be of higher
                //precision, which changes behavior.
                float normalized_x = vector.X / mag;
                float normalized_y = vector.Y / mag;
                return new Vector2(normalized_x * maxLength,
                    normalized_y * maxLength);
            }
            return vector;
        }
    }
}