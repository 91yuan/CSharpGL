﻿using CSharpGL.FileParser._3DSParser.Chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpGL.FileParser._3DSParser.ToModernOpenGL.ChunkDumpers
{
    public static partial class ChunkDumper
    {
        public static void Dump(this MatShininessChunk chunk, ThreeDSModel4ModernOpengl model, ThreeDSMaterial4ModernOpenGL material)
        {
            material.Shininess = chunk.Shininess;
        }
    }
}
