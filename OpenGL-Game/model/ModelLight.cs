﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_Game
{
    public class ModelLight
    {
        public Vector3 pos;

        public Vector3 color;

        public ModelLight(Vector3 pos, Vector3 color)
        {
            this.pos = pos;
            this.color = color;
        }
    }
}
