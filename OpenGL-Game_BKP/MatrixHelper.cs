using System;
using System.Runtime.Remoting.Messaging;
using OpenTK;

namespace OpenGL_Game
{
    internal class MatrixHelper
    {
        public static Matrix4 createTransformationMatrix(Vector3 translation, float rx, float ry, float rz, float scale)
        {
            var x = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rx));
            var y = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(ry));
            var z = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rz));

            var s = Matrix4.CreateScale(scale);
            var t = Matrix4.CreateTranslation(translation);

            return x * y * z * s * t;
        }

        public static Matrix4 createTransformationMatrix(Vector3 translation, float scale)
        {
            var s = Matrix4.CreateScale(scale);
            var t = Matrix4.CreateTranslation(translation);

            return s * t;
        }

        public static Matrix4 createTransformationMatrix(Vector3 translation)
        {
            return Matrix4.CreateTranslation(translation);
        }

        public static Matrix4 createViewMatrix(Camera c)
        {
            var x = Matrix4.CreateRotationX(c.pitch);
            var y = Matrix4.CreateRotationY(c.yaw);

            var t = Matrix4.CreateTranslation(-c.pos);

            return t * y * x;
        }
    }
}