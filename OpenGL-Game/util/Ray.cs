using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace OpenGL_Game
{
    class Ray
    {
        public static bool RayAABB(Vector3 /*ray*/origin, Vector3 /*ray*/direction, AxisAlignedBB bb, out Vector3 hitPosition, out Vector3 hitNormal)
        {
            direction = direction.Normalized();
            hitNormal = Vector3.One.Normalized();
            hitPosition = Vector3.Zero;

            float tmin, tmax, tymin, tymax, tzmin, tzmax;
            Vector3 invrd = direction;
            invrd.X = 1.0f / invrd.X;
            invrd.Y = 1.0f / invrd.Y;
            invrd.Z = 1.0f / invrd.Z;

            if (invrd.X >= 0.0f)
            {
                tmin = (bb.min.X - origin.X) * invrd.X;
                tmax = (bb.max.X - origin.X) * invrd.X;
            }
            else
            {
                tmin = (bb.max.X - origin.X) * invrd.X;
                tmax = (bb.min.X - origin.X) * invrd.X;
            }

            if (invrd.Y >= 0.0f)
            {
                tymin = (bb.min.Y - origin.Y) * invrd.Y;
                tymax = (bb.max.Y - origin.Y) * invrd.Y;
            }
            else
            {
                tymin = (bb.max.Y - origin.Y) * invrd.Y;
                tymax = (bb.min.Y - origin.Y) * invrd.Y;
            }

            if ((tmin > tymax) || (tymin > tmax))
            {
                return false;
            }
            if (tymin > tmin) tmin = tymin;
            if (tymax < tmax) tmax = tymax;

            if (invrd.Z >= 0.0f)
            {
                tzmin = (bb.min.Z - origin.Z) * invrd.Z;
                tzmax = (bb.max.Z - origin.Z) * invrd.Z;
            }
            else
            {
                tzmin = (bb.max.Z - origin.Z) * invrd.Z;
                tzmax = (bb.min.Z - origin.Z) * invrd.Z;
            }

            if ((tmin > tzmax) || (tzmin > tmax))
            {
                return false;
            }
            if (tzmin > tmin) tmin = tzmin;
            if (tzmax < tmax) tmax = tzmax;

            if (tmin < 0) tmin = tmax;
            if (tmax < 0)
            {
                return false;
            }

            float t = tmin;
            hitPosition = origin + t * direction;

            Vector3 AABBCenter = (bb.min + bb.max) * 0.5f;

            Vector3 dir = hitPosition - AABBCenter;

            Vector3 width = bb.max - bb.min;
            width.X = Math.Abs(width.X);
            width.Y = Math.Abs(width.Y);
            width.Z = Math.Abs(width.Z);

            Vector3 ratio = Vector3.One;
            ratio.X = Math.Abs(dir.X / width.X);
            ratio.Y = Math.Abs(dir.Y / width.Y);
            ratio.Z = Math.Abs(dir.Z / width.Z);

            hitNormal = Vector3.Zero;
            int maxDir = 0; // x
            if (ratio.X >= ratio.Y && ratio.X >= ratio.Z)
            { // x is the greatest	
                maxDir = 0;
            }
            else if (ratio.Y >= ratio.X && ratio.Y >= ratio.Z)
            { // y is the greatest	
                maxDir = 1;
            }
            else if (ratio.Z >= ratio.X && ratio.Z >= ratio.Y)
            { // z is the greatest
                maxDir = 2;
            }

            if (dir[maxDir] > 0)
                hitNormal[maxDir] = 1.0f;
            else hitNormal[maxDir] = -1.0f;

            return true;
        }
        /*
        public string intersects(BoundingBox bb)
        {
            float tmin = (bb.min.X - orig.X) / dir.X;
            float tmax = (bb.max.X - orig.X) / dir.X;

            if (tmin > tmax)
            {
                var _tmin = tmin;

                tmin = tmax;
                tmax = _tmin;
            }

            float tymin = (bb.min.Y - orig.Y) / dir.Y;
            float tymax = (bb.max.Y - orig.Y) / dir.Y;

            if (tymin > tymax)
            {
                var _tymin = tymin;

                tymin = tymax;
                tymax = _tymin;
            }

            if ((tmin > tymax) || (tymin > tmax))
                return "";//Vector3.Zero;

            if (tymin > tmin)
                tmin = tymin;

            if (tymax < tmax)
                tmax = tymax;

            float tzmin = (bb.min.Z - orig.Z) / dir.Z;
            float tzmax = (bb.max.Z - orig.Z) / dir.Z;

            if (tzmin > tzmax)
            {
                var _tzmin = tzmin;

                tzmin = tzmax;
                tzmax = _tzmin;
            }

            if ((tmin > tzmax) || (tzmin > tmax))
                return "";//Vector3.Zero;
            
            if (tzmin > tmin)
                tmin = tzmin;

            if (tzmax < tmax)
                tmax = tzmax;

            return $"vector: {tmin.ToString("##.###")}, {tmax.ToString("##.###")}, {tymin.ToString("##.###")}, {tymax.ToString("##.###")}, {tzmin.ToString("##.###")}, {tzmin.ToString("##.###")}, {tzmax.ToString("##.###")}"; //new Vector3(tmax, tymax, tmax);
        }*/
    }

    class MouseOverObject
    {
        public EnumFacing sideHit;

        public Vector3 hitVec;

        public object hit;
    }

    class AxisAlignedBB
    {
        public static AxisAlignedBB BLOCK_FULL = new AxisAlignedBB(Vector3.Zero, Vector3.One);

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

        public bool isIntersectingWith(AxisAlignedBB other)
        {
            return (min.X <= other.max.X && max.X >= other.min.X) &&
                   (min.Y <= other.max.Y && max.Y >= other.min.Y) &&
                   (min.Z <= other.max.Z && max.Z >= other.min.Z);
        }
    }
}
