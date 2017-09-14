using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Seurat
{
    class CanvasPlanePanel : PlanePanel
    {
        public List<DirectBitmap> Layers;
        public DirectBitmap DrawingLayer, CompositeLayer;
        public UInt32 CanvasBackgroundColor = 0xffffffff;
        public double[] CanvasNorthWest = new double[] { 0, 0 };
        public double[] CanvasSouthEast = new double[] { 256, -256 };
        private MouseEventBundle ActiveMouseEvents;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            BackgroundColor = 0xffdddddd;
            Layers = new List<DirectBitmap>();
            Layers.Add(new DirectBitmap(256, 256));
            CompositeLayer = new DirectBitmap(256, 256);
            DrawingLayer = new DirectBitmap(256, 256);
            Center = new double[] { 128, -128 };
        }

        public int GetCanvasWidth()
        {
            return (int)(CanvasSouthEast[0] - CanvasNorthWest[0] + 1.0);
        }
        public int GetCanvasHeight()
        {
            return (int)(CanvasSouthEast[1] - CanvasNorthWest[1] + 1.0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            CompositeLayer.clear(CanvasBackgroundColor);
            Graphics c = Graphics.FromImage(CompositeLayer.bmp);
            Point o = new Point(0, 0);
            foreach (DirectBitmap l in Layers)
            {
                if (l.isVisible)
                    c.DrawImage(l.bmp, o);
            }
            c.DrawImage(DrawingLayer.bmp, o);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            double[] NW = PlaneToPanelCoordinates(CanvasNorthWest);
            double[] SE = PlaneToPanelCoordinates(CanvasSouthEast);

            g.Clear(Color.FromArgb((int)BackgroundColor));
            g.DrawImage(CompositeLayer.bmp, (int)(NW[0]), (int)SE[1], (int)(SE[0] - NW[0]), (int)(-(SE[1] - NW[1])));
            DrawGrid(g);
            DrawAxes(g);
        }
    }
}
