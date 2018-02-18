using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    public abstract class ShaderProgram
    {
        public int ProgramID { get; }
        public int VertexShaderID { get; }
        public int FragmentShaderID { get; }

        protected ShaderProgram(string shaderName)
        {
            var file = "assets/shaders/" + shaderName;

            VertexShaderID = loadShader(ShaderType.VertexShader, file);
            FragmentShaderID = loadShader(ShaderType.FragmentShader, file);

            Console.WriteLine($"{shaderName} vertex shader: {(VertexShaderID != -1 ? "OK" : "ERR")}");
            Console.WriteLine($"{shaderName} fragment shader: {(FragmentShaderID != -1 ? "OK" : "ERR")}");

            ProgramID = GL.CreateProgram();

            GL.AttachShader(ProgramID, VertexShaderID);
            GL.AttachShader(ProgramID, FragmentShaderID);

            bindAttributes();

            GL.LinkProgram(ProgramID);
            GL.ValidateProgram(ProgramID);

            getAllUniformLocations();
        }
        
        protected int getUniformLocation(string uniform)
        {
            return GL.GetUniformLocation(ProgramID, uniform);
        }

        protected void loadMatrix(int location, Matrix4 mat)
        {
            GL.UniformMatrix4(location, false, ref mat);
        }

        protected void loadVector(int location, Vector3 vec)
        {
            GL.Uniform3(location, ref vec);
        }

        public void start()
        {
            GL.UseProgram(ProgramID);
        }

        public void stop()
        {
            GL.UseProgram(0);
        }

        public void cleanUp()
        {
            stop();

            GL.DetachShader(ProgramID, VertexShaderID);
            GL.DetachShader(ProgramID, FragmentShaderID);
            GL.DeleteShader(VertexShaderID);
            GL.DeleteShader(FragmentShaderID);
            GL.DeleteProgram(ProgramID);
        }

        protected void bindAttributes(int attrib, string variable)
        {
            GL.BindAttribLocation(ProgramID, attrib, variable);
        }

        private int loadShader(ShaderType type, string file)
        {
            try
            {
                string ext = type == ShaderType.VertexShader ? ".vsh" : ".fsh";

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

        public abstract void loadTransformationMatrix(Matrix4 mat);
        public abstract void loadViewMatrix(Matrix4 mat);
        public abstract void loadLight(ModelLight light);
        protected abstract void getAllUniformLocations();
        protected abstract void bindAttributes();
    }
}
