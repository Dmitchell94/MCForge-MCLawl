/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCForge.Gui.Utils {
    /// <summary>
    /// Native Rectangle
    /// </summary>
    public struct RECT {
        private int left;
        private int top;
        private int right;
        private int bottom;

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public int Top {
            get { return top; }
            set { top = value; }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public int Right {
            get { return right; }
            set { right = value; }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        public int Bottom {
            get { return bottom; }
            set { bottom = value; }
        }
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public int Left {
            get { return left; }
            set { left = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RECT"/> struct.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="top">The top.</param>
        /// <param name="bottom">The bottom.</param>
        public RECT( int left, int right, int top, int bottom ) {
            this.top = top;
            this.bottom = bottom;
            this.right = right;
            this.left = left;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height {
            get {
                return Bottom - Top + 1;
            }
        }
        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width {
            get {
                return Right - Left + 1;
            }
        }
        /// <summary>
        /// Gets the size.
        /// </summary>
        public Size Size {
            get {
                return new Size( Width, Height );
            }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        public Point Location {
            get {
                return new Point( Left, Top );
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MCForge.Gui.Utils.RECT"/> to <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="margs">The margs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Rectangle( RECT margs ) {
            return new Margins( margs.Left, margs.Right, margs.Top, margs.Bottom );
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Drawing.Rectangle"/> to <see cref="MCForge.Gui.Utils.RECT"/>.
        /// </summary>
        /// <param name="margs">The margs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator RECT( Rectangle margs ) {
            return new Margins( margs.Left, margs.Right, margs.Top, margs.Bottom );
        }



        /// <summary>
        /// Inflates the rectangle.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Inflate( int width, int height ) {
            this.Left -= width;
            this.Top -= height;
            this.Right += width;
            this.Bottom += height;
        }
    }
}
