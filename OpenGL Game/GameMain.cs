using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using LightModelParameter = OpenTK.Graphics.OpenGL.LightModelParameter;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace OpenGL_Game
{
    class GameMain
    {
        [STAThread]
        static void Main()
        {
            new MainWindow().Run(60);
        }
    }

    public sealed class MainWindow : GameWindow
    {
        private static Vector3 hue;

        public MainWindow()
        {
            Title = "OpenGL Game";

            MakeCurrent();

            new Thread(() =>
                  {
                      double a = 0;

                      while (true)
                      {
                          hue = Hue(a);

                          if (a >= 360)
                              a = 0;

                          a++;

                          Thread.Sleep(16);
                      }
                  })
            { IsBackground = true }.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Begin(PrimitiveType.Quads);

            GL.Color3(hue.X, hue.Y, hue.Z);

            GL.Enable(EnableCap.Lighting);
            GL.LightModel(LightModelParameter.LightModelAmbient, 1);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);

            GL.Vertex3(-1, -1, -1);
            GL.Vertex3(-1, 1, -1);
            GL.Vertex3(1, 1, -1);
            GL.Vertex3(1, -1, -1);

            GL.End();

            SwapBuffers();
            ProcessEvents(true);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Exit();
        }

        static Vector3 Hue(double angle)
        {
            double rad = Math.PI / 180 * angle;
            double third = Math.PI / 3;

            float x = (float)(Math.Sin(rad) * 0.5 + 0.5);
            float y = (float)(Math.Sin(rad + 2 * third) * 0.5 + 0.5);
            float z = (float)(Math.Sin(rad + 4 * third) * 0.5 + 0.5);

            return new Vector3 { X = x, Y = y, Z = z };
        }
    }

    abstract class ShaderProgram
    {
        public int ProgramID;
        public int VertexShaderID;
        public int FragmentShaderID;

        public ShaderProgram(string file)
        {
            VertexShaderID = loadShader(ShaderType.VertexShader, file);
            FragmentShaderID = loadShader(ShaderType.FragmentShader, file);

            ProgramID = GL.CreateProgram();

            GL.AttachShader(ProgramID, VertexShaderID);
            GL.AttachShader(ProgramID, FragmentShaderID);

            GL.LinkProgram(ProgramID);
            GL.ValidateProgram(ProgramID);

            bindAttributes();
        }

        public void start()
        {
            GL.UseProgram(ProgramID);
        }

        public void stop()
        {
            GL.UseProgram(0);
        }

        public void DetachShader()
        {
            stop();

            GL.DetachShader(ProgramID, VertexShaderID);
            GL.DetachShader(ProgramID, FragmentShaderID);
            GL.DeleteShader(VertexShaderID);
            GL.DeleteShader(FragmentShaderID);
            GL.DeleteProgram(ProgramID);
        }

        protected abstract void bindAttributes();

        protected void bindAttributes(int attrib, string variable)
        {
            GL.BindAttribLocation(ProgramID, attrib, variable);
        }

        private int loadShader(ShaderType type, string file)
        {
            try
            {
                string ext = type == ShaderType.VertexShader ? ".vs" : ".fs";

                int ID = GL.CreateShaderProgram(type, 0, new[] { "" });

                GL.ShaderSource(ID, File.ReadAllText(file + ext));
                GL.CompileShader(ID);

                GL.GetShader(ID, ShaderParameter.CompileStatus, out var status);

                return status != -1 ? ID : -1;
            }
            catch
            {
                return -1;
            }
        }
    }

    class StaticShader : ShaderProgram
    {
        public static string FileName = "Shaders/shader";

        public StaticShader() : base(FileName)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
        }
    }
}
