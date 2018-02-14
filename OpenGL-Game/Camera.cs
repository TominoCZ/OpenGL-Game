using System;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_Game
{
    class Camera
    {
        private float _pitch, _yaw, speed = 0.1f;

        public Vector3 pos { get; private set; }

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

        public void move()
        {
            if (Keyboard.GetState().IsKeyDown(Key.W))
            {
                var delta = forward;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }
            if (Keyboard.GetState().IsKeyDown(Key.S))
            {
                var delta = -forward;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }
            if (Keyboard.GetState().IsKeyDown(Key.A))
            {
                var delta = left;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }
            if (Keyboard.GetState().IsKeyDown(Key.D))
            {
                var delta = -left;

                pos += new Vector3(delta.X * speed, 0, delta.Y * speed);
            }

            if (Keyboard.GetState().IsKeyDown(Key.Space))
            {
                pos += new Vector3(0, speed, 0);
            }
            else if (Keyboard.GetState().IsKeyDown(Key.LControl))
            {
                pos -= new Vector3(0, speed, 0);
            }
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