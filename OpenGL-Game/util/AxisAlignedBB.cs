using OpenTK;

namespace OpenGL_Game
{
    class AxisAlignedBB
    {
        public static AxisAlignedBB BLOCK_FULL { get; } = new AxisAlignedBB(Vector3.Zero, Vector3.One);
        public Vector3 min { get; }
        public Vector3 max { get; }

        public AxisAlignedBB(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public AxisAlignedBB offset(Vector3 by)
        {
            return new AxisAlignedBB(min + by, max + by);
        }

        public AxisAlignedBB grow(float f)
        {
            var vec = Vector3.One * f;
            return new AxisAlignedBB(min - vec, max + vec);
        }

        public bool isIntersectingWith(AxisAlignedBB other)
        {
            return (min.X <= other.max.X && max.X >= other.min.X) &&
                   (min.Y <= other.max.Y && max.Y >= other.min.Y) &&
                   (min.Z <= other.max.Z && max.Z >= other.min.Z);
        }
    }
}
