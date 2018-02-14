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

            return Matrix4.Identity * x * y * z * s * t;
        }

        public static Matrix4 createViewMatrix(Camera c)
        {
            var negativeCameraPos = -c.pos;

            var x = Matrix4.CreateRotationX(c.pitch);
            var y = Matrix4.CreateRotationY(c.yaw);

            var t = Matrix4.CreateTranslation(negativeCameraPos);

            return Matrix4.Identity * t * y * x;
        }
    }
}