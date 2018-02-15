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
        private static Vector3 V0, V1, V2, V01, V12, NORMAL;

        public static float[] calculateNormals(float[] vertices, int[] indices)
        {
            float[] normals = new float[vertices.Length];

            for (int i = 0; i < indices.Length; i += 3)
            {
                int i0 = indices[i + 2] * 3;
                int i1 = indices[i + 1] * 3;
                int i2 = indices[i + 0] * 3;

                V0.X = vertices[i0];
                V0.Y = vertices[i0 + 1];
                V0.Z = vertices[i0 + 2];

                V1.X = vertices[i1];
                V1.Y = vertices[i1 + 1];
                V1.Z = vertices[i1 + 2];

                V2.X = vertices[i2];
                V2.Y = vertices[i2 + 1];
                V2.Z = vertices[i2 + 2];

                Vector3.Subtract(ref V1, ref V2, out V12);
                Vector3.Subtract(ref V0, ref V1, out V01);

                Vector3.Cross(ref V12, ref V01, out NORMAL);

                normals[i0 + 0] = normals[i1 + 0] = normals[i2 + 0] = NORMAL.X;
                normals[i0 + 1] = normals[i1 + 1] = normals[i2 + 1] = NORMAL.Y;
                normals[i0 + 2] = normals[i1 + 2] = normals[i2 + 2] = NORMAL.Z;
            }

            return normals;
        }
    }
}
