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
        public List<CanvasLayer> Layers;
        public CanvasLayer DrawingLayer, CompositeLayer;
        public int CanvasWidth = 256;
        public int CanvasHeight = 256;
        public UInt32 CanvasBackgroundColor = 0xffffffff;
        public double[] CanvasNorthWest = new double[] { 0, 0 };
        public double[] CanvasSouthEast = new double[] { 256, -256 };
        private MouseEventBundle ActiveMouseEvents;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            BackgroundColor = 0xffdddddd;
            Layers = new List<CanvasLayer>();
            Layers.Add(new CanvasLayer("Layer1", true, 256, 256));
            Layers.Add(new CanvasLayer("Layer2", true, 256, 256));

            Graphics g1 = Graphics.FromImage(Layers[0].DBMP.bmp);
            Graphics g2 = Graphics.FromImage(Layers[1].DBMP.bmp);
            g1.FillRectangle(new SolidBrush(System.Drawing.Color.Blue), new Rectangle(0, 0, 200, 200));
            g2.FillRectangle(new SolidBrush(System.Drawing.Color.Green), new Rectangle(128, 128, 200, 200));

            CompositeLayer = new CanvasLayer("Composite Layer", true, 256, 256);
            DrawingLayer = new CanvasLayer("Drawing Layer", true, 256, 256);
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
            CompositeLayer.DBMP.clear(CanvasBackgroundColor);
            Graphics c = Graphics.FromImage(CompositeLayer.DBMP.bmp);
            Point o = new Point(0, 0);
            foreach (CanvasLayer l in Layers)
            {
                if (l.isVisible)
                    c.DrawImage(l.DBMP.bmp, o);
                if (l.isActiveLayer)
                {
                    c.DrawImage(DrawingLayer.DBMP.bmp, o);
                }
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            double[] NW = PlaneToPanelCoordinates(CanvasNorthWest);
            double[] SE = PlaneToPanelCoordinates(CanvasSouthEast);

            g.Clear(Color.FromArgb((int)BackgroundColor));
            g.DrawImage(CompositeLayer.DBMP.bmp, (int)(NW[0]), (int)NW[1], (int)(SE[0] - NW[0]), (int)((SE[1] - NW[1])));
            
            DrawGrid(g);
            DrawAxes(g);
        }
    }
}
