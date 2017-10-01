using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Seurat
{
    class TextBrush : BrushType
    {
        public override Cursor MyCursor { get => Cursors.Arrow; }
        private TrackBar myWidthTrackBar;
        private string caption = "caption";
        private SolidBrush myBrush = new SolidBrush(Color.Black);
        private Font myFont = new Font("Arial", 24, FontStyle.Regular, GraphicsUnit.Pixel);

        public TextBrush()
        {
            Image = Properties.Resources.if_Character_728921;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();
            ControlPanelControlStack s = new ControlPanelControlStack();
            s.AddHeaderLabel("Text", System.Drawing.Color.White, System.Drawing.Color.DarkViolet);
            s.AddLabelTrackBar("Size", 12, 120, 24);
            s.AddLabelTextBox("Text", "Caption");

            myWidthTrackBar = (TrackBar)s.StackControls[1];
            ((TrackBar)s.StackControls[1]).ValueChanged += (object sender, EventArgs e) =>
            {
                myFont = new Font("Arial", ((TrackBar)sender).Value, FontStyle.Regular, GraphicsUnit.Pixel);
            };
            ((TextBox)s.StackControls[2]).TextChanged += (object sender, EventArgs e) =>
            {
                caption = ((TextBox)sender).Text;
            };

            p.Controls.Add(s);
            p.AutoSize = true;
            return p;
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            int[] thisCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            if (thisCoord[0] >= 0 && thisCoord[1] >= 0 && thisCoord[0] < cpp.CanvasWidth && thisCoord[1] < cpp.CanvasHeight)
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    switch(e.Button)
                    {
                        case MouseButtons.Left:
                            myBrush.Color = Color.FromArgb((int) cpp.MyColorPicker.C1);
                            break;
                        case MouseButtons.Right:
                            myBrush.Color = Color.FromArgb((int)cpp.MyColorPicker.C2);
                            break;
                    }

                    Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
                    g.DrawString(caption, myFont, myBrush, thisCoord[0], thisCoord[1]);
                    g.Dispose();
                    cpp.Invalidate();
                }
            }
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            int[] thisCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            if (thisCoord[0] >= 0 && thisCoord[1] >= 0 && thisCoord[0] < cpp.CanvasWidth && thisCoord[1] < cpp.CanvasHeight)
            {
                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                g.Clear(Color.FromArgb(0));
                g.DrawString(caption, myFont, Brushes.White, thisCoord[0], thisCoord[1]);
                g.Dispose();
                cpp.Invalidate();
            }
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (myWidthTrackBar != null)
            {
                if (e.Delta > 0)
                {
                    if (myWidthTrackBar.Value < myWidthTrackBar.Maximum)
                    {
                        myWidthTrackBar.Value = myWidthTrackBar.Value + 1;
                    }
                }
                else
                {
                    if (myWidthTrackBar.Value > myWidthTrackBar.Minimum)
                    {
                        myWidthTrackBar.Value = myWidthTrackBar.Value - 1;
                    }
                }
            }
        }
    }
}
