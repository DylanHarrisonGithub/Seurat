using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seurat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();            
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            layerTabPanel1.SetDelegateCanvasPlanePanel(canvasPlanePanel1);
            canvasPlanePanel1.MouseMove += CanvasPlanePanel1_MouseMove;

            toolStrip1.Items.Add(new PencilBrush());
            toolStrip1.Items.Add(new HandBrush());
            toolStrip1.Items.Add(new BoxBrush());
            canvasPlanePanel1.MyColorPicker = colorPickerPanel1;
            canvasPlanePanel1.SetToolStrip(toolStrip1);
            canvasPlanePanel1.MyToolSettingsPanel = panel4;
            toolStrip1.Items[0].PerformClick();
        }

        private void CanvasPlanePanel1_MouseMove(object sender, MouseEventArgs e)
        {
            int[] c = canvasPlanePanel1.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            mouseCoordinateLabel.Text = "Mouse (" + c[0].ToString() + ", " + c[1].ToString() + ")";
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.FileName = "untitled.bmp";

            string f = "Bitmap Image (*.bmp)|*.bmp";
            f += "|Graphics Interchange Format (*.gif)|*.gif";
            f += "|Windows Icon Format (*.ico)|*.ico";
            f += "|Joint Photographic Experts Group (*.jpeg)|*.jpeg";
            f += "|Portable Network Graphics (*.png)|*.png";
            f += "|Tagged Image File Format (*.tiff)|*.tiff";
            sf.Filter = f; 

            if (sf.ShowDialog() == DialogResult.OK)
            {
                var extension = System.IO.Path.GetExtension(sf.FileName);

                switch (extension.ToLower())
                {
                    case ".bmp":
                        canvasPlanePanel1.SaveFileImage.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".gif":
                        canvasPlanePanel1.SaveFileImage.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case ".ico":
                        canvasPlanePanel1.SaveFileImage.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Icon);
                        break;
                    case ".jpeg":
                        canvasPlanePanel1.SaveFileImage.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".png":
                        canvasPlanePanel1.SaveFileImage.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".tiff":
                        canvasPlanePanel1.SaveFileImage.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    default:
                        MessageBox.Show("unsupported format, " + extension);
                        break;
                }
            }
        }
    }
}
