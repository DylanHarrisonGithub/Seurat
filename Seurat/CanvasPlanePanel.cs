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
        public BrushType ActiveBrush;
        public ColorPickerPanel MyColorPicker;
        private ToolStrip MyToolStrip;
        public GroupBox MyToolSettingsGroupBox;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            BackgroundColor = 0xffdddddd;
            Layers = new List<CanvasLayer>();
            Layers.Add(new CanvasLayer("Layer1", true, CanvasWidth, CanvasHeight));
            Layers[0].isActiveLayer = true;
            
            CompositeLayer = new CanvasLayer("Composite Layer", true, CanvasWidth, CanvasHeight);
            DrawingLayer = new CanvasLayer("Drawing Layer", true, CanvasWidth, CanvasHeight);
            Center = new double[] { CanvasWidth / 2, -(CanvasHeight / 2) };

            MouseUp += (object sender, MouseEventArgs e) =>
            {
                if (ActiveBrush != null)
                {
                    ActiveBrush.MouseUp(this, e);
                }
            };
            MouseDown += (object sender, MouseEventArgs e) =>
            {
                if (ActiveBrush != null)
                {
                    ActiveBrush.MouseDown(this, e);
                }
            };
            MouseMove += (object sender, MouseEventArgs e) =>
            {
                if (ActiveBrush != null)
                {
                    ActiveBrush.MouseMove(this, e);
                }
            };
            MouseClick += (object sender, MouseEventArgs e) =>
            {
                if (ActiveBrush != null)
                {
                    ActiveBrush.MouseClick(this, e);
                }
            };
            MouseWheel += (object sender, MouseEventArgs e) =>
            {
                if (ActiveBrush != null)
                {
                    ActiveBrush.MouseWheel(this, e);
                }
            };

        }

        public void SetToolStrip(ToolStrip ts)
        {
            MyToolStrip = ts;
            MyToolStrip.ItemClicked += (object sender, ToolStripItemClickedEventArgs e) => {
                ActiveBrush = (BrushType)e.ClickedItem;
                if (MyToolSettingsGroupBox != null)
                {
                    MyToolSettingsGroupBox.Controls.Clear();
                    MyToolSettingsGroupBox.Controls.Add(ActiveBrush.ControlPanel());
                }
            };
        }

        public CanvasLayer GetActiveLayer()
        {
            CanvasLayer al = null;

            foreach (CanvasLayer l in Layers)
            {
                if (l.isActiveLayer)
                {
                    al = l;
                }
            }

            return al;
        }

        public int[] CanvasPlanePanelToCanvasCoordinates(double[] PlanePanelCoordinates)
        {
            int[] coords = new int[] { 0, 0 };

            double[] planeCoords = PanelToPlaneCoordinates(PlanePanelCoordinates);
            coords[0] = (int)(planeCoords[0] - CanvasNorthWest[0]);
            coords[1] = (int)-(planeCoords[1] - CanvasNorthWest[1]);

            return coords;
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
