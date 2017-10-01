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
        public Bitmap SaveFileImage { get => GenerateSaveFileImage(); }
        public BrushType ActiveBrush;
        public ColorPickerPanel MyColorPicker;
        private ToolStrip MyToolStrip;
        public Panel MyToolSettingsPanel;
        private Cursor SavedCursor;
        private bool inResizeState;
        public bool showGrid = true;
        public bool showAxes = true;

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

            //brush tool mouse listeners
            MouseUp += (object sender, MouseEventArgs e) =>
            {
                if ((ActiveBrush != null) && !inResizeState)
                {
                    ActiveBrush.MouseUp(this, e);
                }
            };
            MouseDown += (object sender, MouseEventArgs e) =>
            {
                if ((ActiveBrush != null) && !inResizeState)
                {
                    ActiveBrush.MouseDown(this, e);
                }
            };
            MouseMove += (object sender, MouseEventArgs e) =>
            {
                if ((ActiveBrush != null) && !inResizeState)
                {
                    ActiveBrush.MouseMove(this, e);
                }
            };
            MouseClick += (object sender, MouseEventArgs e) =>
            {
                if ((ActiveBrush != null) && !inResizeState)
                {
                    ActiveBrush.MouseClick(this, e);
                }
            };
            MouseWheel += (object sender, MouseEventArgs e) =>
            {
                if ((ActiveBrush != null) && !inResizeState)
                {
                    ActiveBrush.MouseWheel(this, e);
                }
            };

            MouseEnter += (object sender, EventArgs e) =>
            {
                if (ActiveBrush != null)
                {
                    Cursor = ActiveBrush.MyCursor;
                }
            };
            MouseLeave += (object sender, EventArgs e) =>
            {
                Cursor = Cursors.Arrow;
            };

            //switch to resize state mouse events
            MouseMove += (object sender, MouseEventArgs e) =>
            {
                if (MouseIsInWidthResizeArea(e))
                {
                    if ((Cursor != Cursors.SizeWE) && (e.Button != MouseButtons.Right) && (e.Button != MouseButtons.Left))
                    {
                        if ((Cursor != Cursors.SizeNS) && (Cursor != Cursors.SizeNWSE)) {
                            SavedCursor = Cursor;
                        }
                        inResizeState = true;
                        Cursor = Cursors.SizeWE;
                    }
                } else
                {
                    if ((Cursor == Cursors.SizeWE) && (e.Button != MouseButtons.Left))
                    {
                        Cursor = SavedCursor;
                        inResizeState = false;
                    }
                }
                if (MouseIsInHeightResizeArea(e))
                {
                    if ((Cursor != Cursors.SizeNS) && (e.Button != MouseButtons.Right) && (e.Button != MouseButtons.Left))
                    {
                        if ((Cursor != Cursors.SizeWE) && (Cursor != Cursors.SizeNWSE))
                        {
                            SavedCursor = Cursor;
                            inResizeState = true;
                        }
                        Cursor = Cursors.SizeNS;
                    }
                }
                else
                {
                    if (Cursor == Cursors.SizeNS && (e.Button != MouseButtons.Left))
                    {
                        Cursor = SavedCursor;
                        inResizeState = false;
                    }
                }
                if (MouseIsInBothResizeArea(e))
                {
                    if ((Cursor != Cursors.SizeNWSE) && (e.Button != MouseButtons.Right) && (e.Button != MouseButtons.Left))
                    {
                        if ((Cursor != Cursors.SizeNS) && (Cursor != Cursors.SizeWE))
                        {
                            SavedCursor = Cursor;
                            inResizeState = true;
                        }
                        Cursor = Cursors.SizeNWSE;
                    }
                }
                else
                {
                    if (Cursor == Cursors.SizeNWSE && (e.Button != MouseButtons.Left))
                    {
                        Cursor = SavedCursor;
                        inResizeState = false;
                    }
                }
            };

            //resize mouse events
            MouseMove += (object sender, MouseEventArgs e) =>
            {
                if (inResizeState)
                {
                    Invalidate();
                }
            };
            MouseUp += (object sender, MouseEventArgs e) =>
            {
                if (inResizeState)
                {
                    int[] d = CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                    if (d[0] > 0 && d[1] > 0)
                    {
                        if (Cursor == Cursors.SizeWE)
                        {
                            d[1] = CanvasHeight;
                        }
                        if (Cursor == Cursors.SizeNS)
                        {
                            d[0] = CanvasWidth;
                        }
                        CropCanvas(d[0], d[1]);
                        Invalidate();
                    }
                }
            };
        }

        public void ResetCanvas()
        {
            foreach (CanvasLayer l in Layers)
            {
                l.DBMP.Dispose();
            }
            Layers = new List<CanvasLayer>();
            Layers.Add(new CanvasLayer("Layer1", true, CanvasWidth, CanvasHeight));
            Layers[0].isActiveLayer = true;
            DrawingLayer.DBMP.SetSize(CanvasWidth, CanvasHeight);
            CompositeLayer.DBMP.SetSize(CanvasWidth, CanvasHeight);
            CanvasSouthEast = new double[] { CanvasWidth, -CanvasHeight };
            Invalidate();
        }

        public void CropCanvas(int newWidth, int newHeight)
        {
            if (newWidth > 0 && newHeight > 0)
            {
                CompositeLayer.DBMP.SetSize(newWidth, newHeight);
                DrawingLayer.DBMP.SetSize(newWidth, newHeight);
                foreach (CanvasLayer l in this.Layers)
                {
                    l.DBMP.SetSize(newWidth, newHeight);
                }
                CanvasWidth = newWidth;
                CanvasHeight = newHeight;
                CanvasSouthEast = new double[] { CanvasWidth, -CanvasHeight };
            }
        }

        public void StretchCanvas(int newWidth, int newHeight, bool smooth)
        {
            if (newWidth > 0 && newHeight > 0)
            {
                if (smooth)
                {
                    CompositeLayer.DBMP.SetSize(newWidth, newHeight);
                    DrawingLayer.DBMP.SetSize(newWidth, newHeight);
                    foreach (CanvasLayer l in this.Layers)
                    {
                        l.DBMP.StretchSetSize(newWidth, newHeight, false);
                        MessageBox.Show(newWidth.ToString() + ", " + newHeight.ToString());
                    }
                } else
                {
                    CompositeLayer.DBMP.SetSize(newWidth, newHeight);
                    DrawingLayer.DBMP.SetSize(newWidth, newHeight);
                    foreach (CanvasLayer l in this.Layers)
                    {
                        l.DBMP.StretchSetSize(newWidth, newHeight, false);
                    }
                }
                CanvasWidth = newWidth;
                CanvasHeight = newHeight;
                CanvasSouthEast = new double[] { CanvasWidth, -CanvasHeight };
            }
        }

        public void SetToolStrip(ToolStrip ts)
        {
            MyToolStrip = ts;
            MyToolStrip.ItemClicked += (object sender, ToolStripItemClickedEventArgs e) => {
                ActiveBrush = (BrushType)e.ClickedItem;
                if (MyToolSettingsPanel != null)
                {
                    MyToolSettingsPanel.Controls.Clear();
                    MyToolSettingsPanel.Controls.Add(ActiveBrush.ControlPanel());                    
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

        private bool MouseIsInImageCanvas(MouseEventArgs e)
        {
            int[] PlaneCoords = CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });

            return (PlaneCoords[0] >= 0) && (PlaneCoords[1] >= 0) && (PlaneCoords[0] < CanvasWidth) && (PlaneCoords[1] < CanvasHeight);
        }

        private bool MouseIsInWidthResizeArea(MouseEventArgs e)
        {
            int[] PlaneCoords = CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            bool inResize = (PlaneCoords[0] >= 0) && (PlaneCoords[1] >= 0) && (PlaneCoords[0] < CanvasWidth + 3) && (PlaneCoords[1] < CanvasHeight);

            return inResize && !MouseIsInImageCanvas(e);
        }

        private bool MouseIsInHeightResizeArea(MouseEventArgs e)
        {
            int[] PlaneCoords = CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            bool inResize = (PlaneCoords[0] >= 0) && (PlaneCoords[1] >= 0) && (PlaneCoords[0] < CanvasWidth) && (PlaneCoords[1] < CanvasHeight + 3);

            return inResize && !MouseIsInImageCanvas(e);
        }

        private bool MouseIsInBothResizeArea(MouseEventArgs e)
        {
            int[] PlaneCoords = CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            bool inResize = (PlaneCoords[0] >= 0) && (PlaneCoords[1] >= 0) && (PlaneCoords[0] < CanvasWidth + 3) && (PlaneCoords[1] < CanvasHeight + 3);

            return inResize && !MouseIsInWidthResizeArea(e) && !MouseIsInHeightResizeArea(e) && !MouseIsInImageCanvas(e);
        }

        private Bitmap GenerateSaveFileImage()
        {
            Bitmap sfi = new Bitmap(CanvasWidth, CanvasHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics s = Graphics.FromImage(sfi);
            Point o = new Point(0, 0);

            foreach (CanvasLayer l in Layers)
            {
                if (l.isVisible)
                    s.DrawImage(l.DBMP.bmp, o);
            }

            return sfi;
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
                    if (DrawingLayer.isVisible)
                    {
                        c.DrawImage(DrawingLayer.DBMP.bmp, o);
                    }
                }
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            double[] NW = PlaneToPanelCoordinates(CanvasNorthWest);
            double[] SE = PlaneToPanelCoordinates(CanvasSouthEast);

            g.Clear(Color.FromArgb((int)BackgroundColor));
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle((int)(NW[0]) + 3, (int)NW[1] + 3, (int)(SE[0] - NW[0]) + 3, (int)((SE[1] - NW[1])) + 3));
            g.DrawImage(CompositeLayer.DBMP.bmp, (int)(NW[0]), (int)NW[1], (int)(SE[0] - NW[0]), (int)((SE[1] - NW[1])));
            
            if (showGrid)
            {
                DrawGrid(g);
            }
            if (showAxes)
            {
                DrawAxes(g);
            }

            if (inResizeState && (MouseButtons == MouseButtons.Left))
            {
                Pen p = new Pen(Color.GhostWhite);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                Point mc = PointToClient(Cursor.Position);
                if (Cursor == Cursors.SizeNWSE)
                {
                    g.DrawRectangle(p, (int)(NW[0]), (int)NW[1], mc.X - (int)(NW[0]), mc.Y - (int)NW[1]);
                }
                if (Cursor == Cursors.SizeWE)
                {
                    g.DrawRectangle(p, (int)(NW[0]), (int)NW[1], mc.X - (int)(NW[0]), (int)((SE[1] - NW[1])));
                }
                if (Cursor == Cursors.SizeNS)
                {
                    g.DrawRectangle(p, (int)(NW[0]), (int)NW[1], (int)(SE[0] - NW[0]), mc.Y - (int)NW[1]);
                }
            }
        }
    }
}
