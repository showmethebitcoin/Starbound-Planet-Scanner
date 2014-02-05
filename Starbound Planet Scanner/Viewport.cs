/*
Copyright (c) 2014 Nicholas Tantillo

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
using System.Drawing;
using System.Linq;
using System.Text;

namespace Starbound_Planet_Tagger
{
    class Viewport
    {
        private Bitmap SourceImage;

        private byte[] SourceBytes;

        private int offsetX;
        private int offsetY;
        private int MaxWidth;
        private int MaxHeight;
        private int SourceWidth;
        private int SourceHeight;

        public int Width { get { return MaxWidth; } }
        public int Height { get { return MaxHeight; } }

        public Viewport(int X, int Y, int pWidth, int pHeight, Bitmap Source)
        {
           
            offsetX = X;
            offsetY = Y;
            MaxWidth = pWidth;
            MaxHeight = pHeight;
            

            SourceImage = Source; // little backwards here because GetImage was added later

            SourceImage = GetImage(); // isolate from the whole

            SourceBytes = GetMatrix();

            SourceWidth = MaxWidth;
            SourceHeight = MaxHeight;

            // Crop complete, so reset offsets
            offsetX = 0;
            offsetY = 0;
        }

        public void Offset(int X, int Y)
        {
            offsetX = X;
            offsetY = Y;
        }


        public Bitmap GetSource()
        {
            return SourceImage;
        }

        public Viewport(int X, int Y, int pWidth, int pHeight, Viewport Source)
        {
            SourceImage = Source.SourceImage;
            offsetX = X + Source.offsetX;
            offsetY = Y + Source.offsetY;
            MaxWidth = pWidth;
            MaxHeight = pHeight;

            SourceImage = GetImage(); // isolate from the whole

            SourceBytes = GetMatrix();

            
            SourceWidth = MaxWidth;
            SourceHeight = MaxHeight;

            // Crop complete, so reset offsets
            offsetX = 0;
            offsetY = 0;

        }

        public Color GetPixel(int X, int Y, bool UseBitmap = false)
        {
            if ((offsetX + X) < 0 || (offsetX + X) >= SourceWidth)
                return Color.Transparent;
            if ((offsetY + Y) < 0 || (offsetY + Y) >= SourceHeight)
                return Color.Transparent;

            if (offsetX < 0 || X >= MaxWidth + offsetX)
            {
                return Color.Transparent;
            }

            if (offsetY < 0 || Y >= MaxHeight + offsetY)
            {
                return Color.Transparent;
            }

            if (UseBitmap == true)
                return SourceImage.GetPixel(offsetX + X, offsetY + Y);

            var index = 4 * ((MaxWidth * (offsetY + Y)) + (offsetX + X));

            byte B = SourceBytes[index];
            byte G = SourceBytes[index + 1];
            byte R = SourceBytes[index + 2];
            byte A = SourceBytes[index + 3];
            // BGR

            if (A == 0)
                A = 255; // Screenshot is masking on black!

            Color NewColor = Color.FromArgb(A, R, G, B );

            return NewColor;
        }

        public Bitmap GetImage()
        {
            Rectangle cropRect = new Rectangle(offsetX, offsetY, Width, Height);
            Bitmap src = SourceImage;
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;
        }

        public byte[] GetMatrix()
        {
              // Create a new bitmap.
            Bitmap bmp = SourceImage;

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int bytes  = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

          

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return rgbValues;
        }
    }
}
