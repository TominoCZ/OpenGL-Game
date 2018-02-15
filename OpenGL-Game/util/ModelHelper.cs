using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_Game
{
    class ModelHelper
    {
        private static Vector3 V0, V1, V2, NORMAL;

        public static float[] calculateNormals(float[] vertices)
        {
            float[] normals = new float[vertices.Length];

            for (int i = 0; i < vertices.Length; i += 12)
            {
                V0.Z = vertices[i];
                V0.Y = vertices[i + 1];
                V0.X = vertices[i + 2];

                V1.Z = vertices[i + 3];
                V1.Y = vertices[i + 4];
                V1.X = vertices[i + 5];

                V2.Z = vertices[i + 6];
                V2.Y = vertices[i + 7];
                V2.X = vertices[i + 8];

                NORMAL = Vector3.Cross(V1 - V2, V0 - V1);

                for (int j = 0; j < 4; j++)
                {
                    normals[i + j * 3] = NORMAL.X;
                    normals[i + j * 3 + 1] = NORMAL.Y;
                    normals[i + j * 3 + 2] = NORMAL.Z;
                }
            }

            return normals;
        }

        public static Vector3 getFacingVector(EnumFacing dir)
        {
            switch (dir)
            {
                case EnumFacing.NORTH:
                    return -Vector3.UnitZ;
                case EnumFacing.SOUTH:
                    return Vector3.UnitZ;
                case EnumFacing.EAST:
                    return Vector3.UnitX;
                case EnumFacing.WEST:
                    return -Vector3.UnitX;
                case EnumFacing.UP:
                    return Vector3.UnitY;
                case EnumFacing.DOWN:
                    return -Vector3.UnitY;
                default:
                    return Vector3.Zero;
            }
        }
    }
}
