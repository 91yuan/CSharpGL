﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpGL
{
    public partial class TextBillboardNode
    {
        /// <summary>
        /// height / width.
        /// </summary>
        private float heightByWidth;
        /// <summary>
        /// width / height.
        /// </summary>
        private float widthByHeight;

        private float _width;// TODO: make this an int type!
        /// <summary>
        /// Billboard's width(in pixels).
        /// </summary>
        public int Width
        {
            get { return (int)this._width; }
            set
            {
                if (this._width != value)
                {
                    this._width = value;

                    this._height = (value * this.heightByWidth);

                    ModernRenderUnit unit = this.RenderUnit;
                    if (unit == null) { return; }
                    RenderMethod method = unit.Methods[0];
                    if (method == null) { return; }
                    ShaderProgram program = method.Program;
                    if (program == null) { return; }

                    program.SetUniform(width, this._width);
                    program.SetUniform(height, this._height);
                }
            }
        }

        private float _height;
        /// <summary>
        /// Billboard's height(in pixels).
        /// </summary>
        public int Height
        {
            get { return (int)this._height; }
            set
            {
                if (this._height != value)
                {
                    this._height = value;
                    this._width = (value * this.widthByHeight);

                    ModernRenderUnit unit = this.RenderUnit;
                    if (unit == null) { return; }
                    RenderMethod method = unit.Methods[0];
                    if (method == null) { return; }
                    ShaderProgram program = method.Program;
                    if (program == null) { return; }

                    program.SetUniform(width, this._width);
                    program.SetUniform(height, this._height);
                }
            }
        }
    }
}
