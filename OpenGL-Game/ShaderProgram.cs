using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    abstract class ShaderProgram
    {
        public int ProgramID { get; }
        public int VertexShaderID { get; }
        public int FragmentShaderID { get; }

        protected ShaderProgram(string shaderName)
        {
            var file = "assets/shaders/" + shaderName;

            VertexShaderID = loadShader(ShaderType.VertexShader, file);
            FragmentShaderID = loadShader(ShaderType.FragmentShader, file);

            Console.WriteLine($"vertex: {(VertexShaderID != -1 ? "OK" : "ERR")}");
            Console.WriteLine($"fragment: {(FragmentShaderID != -1 ? "OK" : "ERR")}");

            ProgramID = GL.CreateProgram();

            GL.AttachShader(ProgramID, VertexShaderID);
            GL.AttachShader(ProgramID, FragmentShaderID);

            bindAttributes();

            GL.LinkProgram(ProgramID);
            GL.ValidateProgram(ProgramID);

            getAllUniformLocations();
        }

        protected abstract void getAllUniformLocations();

        public abstract void loadTransformationMatrix(Matrix4 mat);

        public abstract void loadProjectionMatrix(Matrix4 mat);

        public abstract void loadViewMatrix(Camera c);

        protected int getUniformLocation(string uniform)
        {
            return GL.GetUniformLocation(ProgramID, uniform);
        }

        protected void loadMatrix4(int location, Matrix4 mat)
        {
            GL.UniformMatrix4(location, false, ref mat);
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

                int ID = GL.CreateShader(type);

                var text = File.ReadAllText(file + ext);

                GL.ShaderSource(ID, text);
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
}
