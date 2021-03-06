﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Seurat
{
    class BoxBrush : BrushType
    {
        private int[] upperLeftCorner;
        private Pen myPen = new Pen(Color.Black);
        private SolidBrush myBrush = new SolidBrush(Color.Black);
        private bool isFilled = true;
        public override Cursor MyCursor { get => Cursors.Arrow; }

        public BoxBrush()
        {
            Image = Properties.Resources.if_ic_crop_square_48px_351984;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();
            ControlPanelControlStack s = new ControlPanelControlStack();
            s.AddHeaderLabel("Box", System.Drawing.Color.White, System.Drawing.Color.DarkViolet);
            s.AddLabelCheckBox("Filled", isFilled);
            s.AddLabelTrackBar("Border", 1, 50, 1);

            ((CheckBox)s.StackControls[1]).CheckedChanged += (object sender, EventArgs e) =>
            {
                isFilled = ((CheckBox)sender).Checked;
            };
            ((TrackBar)s.StackControls[2]).ValueChanged += (object sender, EventArgs e) =>
            {
                myPen.Width = ((TrackBar)sender).Value;
            };

            p.Controls.Add(s);
            p.AutoSize = true;
            return p;
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            upperLeftCorner = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
            {
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        myPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C1);
                        myBrush.Color = myPen.Color;
                        break;
                    case MouseButtons.Right:
                        myPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C2);
                        myBrush.Color = myPen.Color;
                        break;
                    default:
                        break;
                }

                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                g.Clear(Color.FromArgb(0));
                if (isFilled)
                {
                    g.FillRectangle(myBrush,
                        Math.Min(upperLeftCorner[0], currentCoordinates[0]),
                        Math.Min(upperLeftCorner[1], currentCoordinates[1]),
                        Math.Abs(currentCoordinates[0] - upperLeftCorner[0]),
                        Math.Abs(currentCoordinates[1] - upperLeftCorner[1]));
                } else
                {
                    g.DrawRectangle(myPen,
                        Math.Min(upperLeftCorner[0], currentCoordinates[0]),
                        Math.Min(upperLeftCorner[1], currentCoordinates[1]),
                        Math.Abs(currentCoordinates[0] - upperLeftCorner[0]),
                        Math.Abs(currentCoordinates[1] - upperLeftCorner[1]));
                }

                cpp.Invalidate();
                g.Dispose();
            }

        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
            g.DrawImage(cpp.DrawingLayer.DBMP.bmp, new Point(0, 0));
            cpp.DrawingLayer.DBMP.clear(0x00000000);
            g.Dispose();
            cpp.Invalidate();
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
