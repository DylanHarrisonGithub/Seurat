using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Seurat
{
    class VerticalGradientPanel : Panel
    {
        public UInt32 c1, c2;
        private DirectBitmap Display;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Display = new DirectBitmap(this.ClientSize.Width, this.ClientSize.Height);
            c1 = 0xffffffff;
            c2 = 0xff000000;
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (Display != null)
            {
                Display.SetSize(this.ClientSize.Width, this.ClientSize.Height);
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.Draw();

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.DrawImage(Display.bmp, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
        }

        private void Draw()
        {
            byte[] c1Bytes = BitConverter.GetBytes(c1);
            byte[] c2Bytes = BitConverter.GetBytes(c2);
            byte[] cBytes = { 0, 0, 0, 255 };
            UInt32 c = c1;

            //linear interpolation between c1 and c2
            for (int t = 0; t < Display.height; t++)
            {
                cBytes[0] = (byte)(c1Bytes[0] + (byte)(((float)t / (Display.height)) * (c2Bytes[0] - c1Bytes[0])));
                cBytes[1] = (byte)(c1Bytes[1] + (byte)(((float)t / (Display.height)) * (c2Bytes[1] - c1Bytes[1])));
                cBytes[2] = (byte)(c1Bytes[2] + (byte)(((float)t / (Display.height)) * (c2Bytes[2] - c1Bytes[2])));
                
                Display.drawBresenhamLine(0, t, Display.width - 1, t, BitConverter.ToUInt32(cBytes, 0));
            }
        }
    }
}
