using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Seurat
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap bmp { get; private set; }
        public UInt32[] pixelBuffer { get; private set; }
        public bool disposed { get; private set; }
        public int height { get; private set; }
        public int width { get; private set; }
        public bool isVisible = true;
        public double alpha;

        protected GCHandle bitsHandle { get; private set; }

        public DirectBitmap(int Width, int Height)
        {
            width = Width;
            height = Height;
            pixelBuffer = new UInt32[width * height];
            bitsHandle = GCHandle.Alloc(pixelBuffer, GCHandleType.Pinned);
            bmp = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, bitsHandle.AddrOfPinnedObject());
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            bmp.Dispose();
            bitsHandle.Free();
        }

        public void clear(UInt32 color)
        {
            for (int i = 0; i < width * height; i++)
            {
                pixelBuffer[i] = color;
            }
        }

        public void SetSize(int NewWidth, int NewHeight)
        {
            UInt32[] NewPixelBuffer = new UInt32[NewWidth * NewHeight];
            GCHandle NewBitsHandle = GCHandle.Alloc(NewPixelBuffer, GCHandleType.Pinned);
            Bitmap NewBmp = new Bitmap(NewWidth, NewHeight, NewWidth * 4, PixelFormat.Format32bppPArgb, NewBitsHandle.AddrOfPinnedObject());
            Graphics g = Graphics.FromImage(NewBmp);
            g.DrawImage(bmp, new Point(0, 0));
            g.Dispose();

            bmp.Dispose();
            bitsHandle.Free();
            disposed = false;

            pixelBuffer = NewPixelBuffer;
            bitsHandle = NewBitsHandle;
            bmp = NewBmp;

            height = NewHeight;
            width = NewWidth;
        }

        public void StretchSetSize(int newWidth, int newHeight, bool smooth)
        {
            UInt32[] NewPixelBuffer = new UInt32[newWidth * newHeight];
            GCHandle NewBitsHandle = GCHandle.Alloc(NewPixelBuffer, GCHandleType.Pinned);
            Bitmap NewBmp = new Bitmap(newWidth, newHeight, newWidth * 4, PixelFormat.Format32bppPArgb, NewBitsHandle.AddrOfPinnedObject());
            Graphics g = Graphics.FromImage(NewBmp);

            if (smooth)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            } else
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            }

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.DrawImage(bmp, 0, 0, newWidth, newHeight);
            g.Dispose();

            bmp.Dispose();
            bitsHandle.Free();
            disposed = false;

            pixelBuffer = NewPixelBuffer;
            bitsHandle = NewBitsHandle;
            bmp = NewBmp;

            height = newHeight;
            width = newWidth;
        }

        public void drawGrid(int x, int y)
        {
            Pen p = new Pen(Color.Silver);
            float widthRatio = (float)width / (float)x;
            float heightRatio = (float)height / (float)y;
            Graphics g = Graphics.FromImage(bmp);

            g.DrawLine(p, 0, 0, width, 0);
            g.DrawLine(p, 0, height, width, height);
            g.DrawLine(p, 0, 0, 0, height);
            g.DrawLine(p, width, 0, width, height);

            for (int i = 1; i < x; i++)
            {
                g.DrawLine(p, i * widthRatio, 0, i * widthRatio, height);
            }
            for (int i = 1; i < y; i++)
            {
                g.DrawLine(p, 0, i * heightRatio, width, i * heightRatio);
            }

            g.Dispose();
            p.Dispose();
        }

        public void drawGrid2(int x, int y)
        {
            Pen p = new Pen(Color.Silver);
            Pen p2 = new Pen(Color.Silver);
            float[] dashValues = { 3, 2 };
            p2.DashPattern = dashValues;
            float widthRatio = (float)width / (float)x;
            float heightRatio = (float)height / (float)y;
            Graphics g = Graphics.FromImage(bmp);

            g.DrawLine(p, 0, height, width, height);
            g.DrawLine(p, width, 0, width, height);

            for (int i = 0; i < x; i++)
            {
                g.DrawLine(p, i * widthRatio, 0, i * widthRatio, height);
                g.DrawLine(p2, (float)((2 * i + 1) * widthRatio / 2.0), 0, (float)((2 * i + 1) * widthRatio / 2.0), height);
            }
            for (int i = 0; i < y; i++)
            {
                g.DrawLine(p, 0, i * heightRatio, width, i * heightRatio);
                g.DrawLine(p2, 0, (float)((2 * i + 1) * heightRatio / 2.0), width, (float)((2 * i + 1) * heightRatio / 2.0));
            }

            g.Dispose();
            p.Dispose();
            p2.Dispose();

        }

        public void drawString(String s, float x, float y)
        {
            Graphics g = Graphics.FromImage(bmp);

            g.DrawString(s,
                new Font(new FontFamily("Times New Roman"), 16, FontStyle.Regular, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, 0, 0, 255)),
                new PointF(x, y));

            g.Dispose();
        }

        public void pSet(int x, int y, UInt32 color)
        {
            pixelBuffer[x + y * width] = color;
        }

        public void drawBresenhamLine(int x1, int y1, int x2, int y2, UInt32 color)
        {
            UInt32 iDestPtrOffset;
            int X_Delta;
            int Y_Delta;
            int X_DeltaX2;
            int Y_DeltaX2;
            int Y_DeltaX2mX_DeltaX2;
            int X_DeltaX2mY_DeltaX2;
            int Error_Term;
            int Y_Screen_Delta;

            int tempx;
            int tempy;

            if (x2 < x1)
            {
                tempx = x1;
                tempy = y1;
                x1 = x2;
                y1 = y2;
                x2 = tempx;
                y2 = tempy;
            }

            X_Delta = x2 - x1;
            Y_Delta = y2 - y1;

            if (Y_Delta < 0)
            {
                Y_Screen_Delta = -width;
            }
            else
            {
                Y_Screen_Delta = width;
            }

            X_Delta = Math.Abs(X_Delta);
            Y_Delta = Math.Abs(Y_Delta);

            iDestPtrOffset = (UInt32)(width * y1 + x1);

            if (X_Delta >= Y_Delta)
            {

                Y_DeltaX2 = Y_Delta << 1;
                Y_DeltaX2mX_DeltaX2 = Y_DeltaX2 - (X_Delta << 1);
                Error_Term = (Y_DeltaX2 - X_Delta);

                for (x1 = 0; x1 <= X_Delta; x1++)
                {

                    pixelBuffer[iDestPtrOffset] = color;
                    iDestPtrOffset += 1;
                    if (Error_Term >= 0)
                    {
                        iDestPtrOffset = (UInt32)(iDestPtrOffset + Y_Screen_Delta);
                        Error_Term += Y_DeltaX2mX_DeltaX2;
                    }
                    else
                    {
                        Error_Term += Y_DeltaX2;
                    }

                }

            }
            else
            {

                X_DeltaX2 = X_Delta << 1;
                X_DeltaX2mY_DeltaX2 = X_DeltaX2 - (Y_Delta << 1);
                Error_Term = (X_DeltaX2 - Y_Delta);

                for (y1 = 0; y1 <= Y_Delta; y1++)
                {

                    pixelBuffer[iDestPtrOffset] = color;
                    iDestPtrOffset = (UInt32)(iDestPtrOffset + Y_Screen_Delta);
                    if (Error_Term >= 0)
                    {
                        iDestPtrOffset += 1;
                        Error_Term += X_DeltaX2mY_DeltaX2;
                    }
                    else
                    {
                        Error_Term += X_DeltaX2;
                    }

                }
            }
        }

    }
}
