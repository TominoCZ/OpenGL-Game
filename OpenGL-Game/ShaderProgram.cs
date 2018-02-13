using System.IO;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
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

                int ID = GL.CreateShader(type);

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
}
