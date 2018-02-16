namespace OpenGL_Game
{
    interface IModel
    {
        IRawModel rawModel { get; }
        ShaderProgram shader { get; }
    }
}