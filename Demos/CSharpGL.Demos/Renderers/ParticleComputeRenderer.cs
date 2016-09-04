﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CSharpGL.Demos
{
    internal class ParticleComputeRenderer : RendererBase
    {
        private ShaderProgram computeProgram;

        //private uint[] textureBufferPosition = new uint[1];
        //private uint[] textureBufferVelocity = new uint[1];
        private Texture positionTexture;

        private Texture velocityTexture;

        //private uint[] attractor_buffer = new uint[1];
        private IndependentBufferPtr attractorBufferPtr;

        private uint positionBufferPtrId;
        private uint velocityBufferPtrId;
        private float time = 0;
        private Random random = new Random();

        public ParticleComputeRenderer(uint positionBufferPtrId, uint velocityBufferPtrId)
        {
            this.positionBufferPtrId = positionBufferPtrId;
            this.velocityBufferPtrId = velocityBufferPtrId;
        }

        protected override void DoInitialize()
        {
            {
                var computeProgram = new ShaderProgram();
                var shaderCode = new ShaderCode(File.ReadAllText(@"shaders\particleSimulator.comp"), ShaderType.ComputeShader);
                var shader = shaderCode.CreateShader();
                computeProgram.Initialize(shader);
                shader.Delete();
                this.computeProgram = computeProgram;
            }
            {
                var texture = new Texture(BindTextureTarget.TextureBuffer,
                    new TexBufferImageFiller(OpenGL.GL_RGBA32F, this.positionBufferPtrId),
                    new NullSampler());
                texture.Initialize();
                this.positionTexture = texture;
            }
            {
                var texture = new Texture(BindTextureTarget.TextureBuffer,
                    new TexBufferImageFiller(OpenGL.GL_RGBA32F, this.velocityBufferPtrId),
                    new NullSampler());
                texture.Initialize();
                this.velocityTexture = texture;
            }
            {
                //OpenGL.GetDelegateFor<OpenGL.glGenBuffers>()(1, attractor_buffer);
                //OpenGL.BindBuffer(BufferTarget.UniformBuffer, attractor_buffer[0]);
                //OpenGL.GetDelegateFor<OpenGL.glBufferData>()(OpenGL.GL_UNIFORM_BUFFER,
                //    64 * Marshal.SizeOf(typeof(vec4)), IntPtr.Zero, OpenGL.GL_DYNAMIC_COPY);
                //OpenGL.BindBufferBase(BindBufferBaseTarget.UniformBuffer, 0, attractor_buffer[0]);
                var buffer = new IndependentBuffer<vec4>(BufferTarget.UniformBuffer, BufferUsage.DynamicCopy, true);
                buffer.Create(64);
                var ptr = buffer.GetBufferPtr() as IndependentBufferPtr;
                ptr.Bind();
                OpenGL.BindBufferBase((BindBufferBaseTarget)BufferTarget.UniformBuffer, 0, ptr.BufferId);
                this.attractorBufferPtr = ptr;
            }
        }

        protected override void DoRender(RenderEventArgs arg)
        {
            float deltaTime = (float)random.NextDouble() * 5;
            time += (float)random.NextDouble() * 5;

            attractorBufferPtr.Bind();
            IntPtr attractors = OpenGL.MapBufferRange(BufferTarget.UniformBuffer,
                0, 64 * Marshal.SizeOf(typeof(vec4)),
                MapBufferRangeAccess.MapWriteBit | MapBufferRangeAccess.MapInvalidateBufferBit);
            unsafe
            {
                var array = (vec4*)attractors.ToPointer();
                for (int i = 0; i < 64; i++)
                {
                    array[i] = new vec4(
                        (float)(Math.Sin(time)) * 50.0f,
                        (float)(Math.Cos(time)) * 50.0f,
                        (float)(Math.Cos(time)) * (float)(Math.Sin(time)) * 5.0f,
                        ParticleModel.attractor_masses[i]);
                }
            }

            OpenGL.UnmapBuffer(BufferTarget.UniformBuffer);
            attractorBufferPtr.Unbind();

            // Activate the compute program and bind the position and velocity buffers
            computeProgram.Bind();
            OpenGL.BindImageTexture(0, this.velocityTexture.Id, 0, false, 0, OpenGL.GL_READ_WRITE, OpenGL.GL_RGBA32F);
            OpenGL.BindImageTexture(1, this.positionTexture.Id, 0, false, 0, OpenGL.GL_READ_WRITE, OpenGL.GL_RGBA32F);
            // Set delta time
            computeProgram.SetUniform("dt", deltaTime);
            // Dispatch
            OpenGL.GetDelegateFor<OpenGL.glDispatchCompute>()(ParticleModel.particleGroupCount, 1, 1);
        }

        protected override void DisposeUnmanagedResources()
        {
            this.computeProgram.Delete();
            this.positionTexture.Dispose();
            this.velocityTexture.Dispose();
            attractorBufferPtr.Dispose();
        }
    }
}