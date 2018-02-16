using System;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_Game
{
    class Camera
    {
        public static Camera INSTANCE;

        private float _pitch = MathHelper.PiOver6, _yaw = MathHelper.PiOver2 + MathHelper.PiOver4, speed = 0.15f;

        public Vector3 pos;

        public float pitch
        {
            get => _pitch;

            set => _pitch = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
        }

        public float yaw
        {
            get => _yaw;

            set => _yaw = value;
        }

        public Camera()
        {
            INSTANCE = this;
        }

        public void move()
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Key.W))
            {
                var delta = forward;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }
            if (state.IsKeyDown(Key.S))
            {
                var delta = -forward;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }
            if (state.IsKeyDown(Key.A))
            {
                var delta = left;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }
            if (state.IsKeyDown(Key.D))
            {
                var delta = -left;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }

            if (state.IsKeyDown(Key.Space))
            {
                pos += Vector3.UnitY * speed;
            }
            if (state.IsKeyDown(Key.LControl) || state.IsKeyDown(Key.LShift))
            {
                pos -= Vector3.UnitY * speed;
            }
        }

        public Vector3 getLookVec()
        {
            return rotate(Vector3.UnitZ, _pitch, _yaw, 0);
        }

        public static Vector3 rotate(Vector3 vec, float AngleX, float AngleY, float AngleZ)
        {
            Vector3 sin = new Vector3((float)Math.Sin(AngleX), (float)Math.Sin(AngleY), (float)Math.Sin(AngleZ));
            Vector3 cos = new Vector3((float)Math.Cos(AngleX), (float)Math.Cos(AngleY), (float)Math.Cos(AngleZ));

            vec = new Vector3(vec.X, vec.Y * cos.X - vec.Z * sin.X, vec.Y * sin.X + vec.Z * cos.X);
            vec = new Vector3(vec.X * cos.Y + vec.Z * sin.Y, vec.Y, vec.X * sin.Y - vec.Z * cos.Y);
            vec = new Vector3(vec.X * cos.Z - vec.Y * sin.Z, vec.X * sin.Z + vec.Y * cos.Z, vec.Z);

            return vec;
        }

        Vector2 left
        {
            get
            {
                var s = (float)Math.Sin(-(_yaw + MathHelper.PiOver2));
                var c = (float)Math.Cos((_yaw + MathHelper.PiOver2));

                return new Vector2(s, c);
            }
        }

        Vector2 forward
        {
            get
            {
                var s = -(float)Math.Sin(-_yaw);
                var c = -(float)Math.Cos(_yaw);

                return new Vector2(s, c);
            }
        }
    }
}