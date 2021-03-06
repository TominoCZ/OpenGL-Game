﻿namespace OpenGL_Game
{
    interface IRawModel
    {
        int vaoID { get; }
        int vertexCount { get; }
        int[] bufferIDs { get; }
        bool hasUVs { get; }
        bool hasNormals { get; }
    }
}