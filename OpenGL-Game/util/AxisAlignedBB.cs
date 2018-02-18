using System;
using OpenTK;

namespace OpenGL_Game
{
    class AxisAlignedBB
    {
        public static AxisAlignedBB BLOCK_FULL { get; } = new AxisAlignedBB(Vector3.Zero, Vector3.One);
        public static AxisAlignedBB NULL { get; } = new AxisAlignedBB(Vector3.Zero, Vector3.Zero);
        public Vector3 min { get; }
        public Vector3 max { get; }

        public AxisAlignedBB(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public AxisAlignedBB(float minX, float minY, float minZ, float maxX, float maxY, float maxZ) : this(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ))
        {

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

        public AxisAlignedBB union(AxisAlignedBB other)
        {
            var minX = (int)Math.Floor(MathUtil.Min(min.X, max.X, other.min.X, other.max.X));
            var minY = (int)Math.Floor(MathUtil.Min(min.Y, max.Y, other.min.Y, other.max.Y));
            var minZ = (int)Math.Floor(MathUtil.Min(min.Z, max.Z, other.min.Z, other.max.Z));

            var maxX = (int)Math.Ceiling(MathUtil.Max(min.X, max.X, other.min.X, other.max.X));
            var maxY = (int)Math.Ceiling(MathUtil.Max(min.Y, max.Y, other.min.Y, other.max.Y));
            var maxZ = (int)Math.Ceiling(MathUtil.Max(min.Z, max.Z, other.min.Z, other.max.Z));

            return new AxisAlignedBB(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public Vector3 calculateOffset(AxisAlignedBB other, Vector3 offset)
        {
            var result = Vector3.One * offset;

            //Y
            if (other.max.X > min.X && other.min.X < max.X && other.max.Z > min.Z && other.min.Z < max.Z)
            {
                if (result.Y > 0.0D && other.max.Y <= min.Y)
                {
                    float d1 = min.Y - other.max.Y;

                    if (d1 < result.Y)
                    {
                        result.Y = d1;
                    }
                }
                else if (result.Y < 0.0D && other.min.Y >= max.Y)
                {
                    float d0 = max.Y - other.min.Y;

                    if (d0 > result.Y)
                    {
                        result.Y = d0;
                    }
                }
            }

            //X
            if (other.max.Y > min.Y && other.min.Y < max.Y && other.max.Z > min.Z && other.min.Z < max.Z)
            {
                if (result.X > 0.0D && other.max.X <= min.X)
                {
                    float d1 = min.X - other.max.X;

                    if (d1 < result.X)
                    {
                        result.X = d1;
                    }
                }
                else if (result.X < 0.0D && other.min.X >= max.X)
                {
                    float d0 = max.X - other.min.X;

                    if (d0 > result.X)
                    {
                        result.X = d0;
                    }
                }
            }

            //Z
            if (other.max.X > min.X && other.min.X < max.X && other.max.Y > min.Y && other.min.Y < max.Y)
            {
                if (result.Z > 0.0D && other.max.Z <= min.Z)
                {
                    float d1 = min.Z - other.max.Z;

                    if (d1 < result.Z)
                    {
                        result.Z = d1;
                    }
                }
                else if (result.Z < 0.0D && other.min.Z >= max.Z)
                {
                    float d0 = max.Z - other.min.Z;

                    if (d0 > result.Z)
                    {
                        result.Z = d0;
                    }
                }
            }

            return result;
        }

        public Vector3 getCenter()
        {
            return (min + max) / 2;
        }

        public bool intersectsWith(AxisAlignedBB other)
        {
            return (min.X <= other.max.X && max.X >= other.min.X) &&
                   (min.Y <= other.max.Y && max.Y >= other.min.Y) &&
                   (min.Z <= other.max.Z && max.Z >= other.min.Z);
        }
    }
}
