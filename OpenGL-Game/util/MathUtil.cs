using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_Game
{
    class MathUtil
    {
        public static float Min(params float[] values)
        {
            var min = float.MaxValue;

            for (int i = 0; i < values.Length; i++)
            {
                min = Math.Min(min, values[i]);
            }

            return min;
        }

        public static float Max(params float[] values)
        {
            var max = float.MinValue;

            for (int i = 0; i < values.Length; i++)
            {
                max = Math.Max(max, values[i]);
            }

            return max;
        }

        public static float distance(Vector2 v1, Vector2 v2)
        {
            var x = v1.X - v2.X;
            var y = v1.Y - v2.Y;

            return (float)Math.Sqrt(x * x + y * y);
        }
    }
}
