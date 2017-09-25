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

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to start a new image?", "New Image", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                canvasPlanePanel1.ResetCanvas();
                layerTabPanel1.SetDelegateCanvasPlanePanel(canvasPlanePanel1);
            }
        }

        private void loadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string f = "Bitmap Image (*.bmp)|*.bmp";
            f += "|Graphics Interchange Format (*.gif)|*.gif";
            f += "|Windows Icon Format (*.ico)|*.ico";
            f += "|Joint Photographic Experts Group (*.jpeg)|*.jpeg";
            f += "|Portable Network Graphics (*.png)|*.png";
            f += "|Tagged Image File Format (*.tiff)|*.tiff";
            ofd.Filter = f;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image loadedBMP = Bitmap.FromFile(ofd.FileName);
                canvasPlanePanel1.CanvasWidth = loadedBMP.Width;
                canvasPlanePanel1.CanvasHeight = loadedBMP.Height;
                canvasPlanePanel1.ResetCanvas();
                
                Graphics g = Graphics.FromImage(canvasPlanePanel1.Layers[0].DBMP.bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(Bitmap.FromFile(ofd.FileName), new Point(0, 0));
                g.Dispose();
                canvasPlanePanel1.Invalidate();
            }
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

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Quit Seurat", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form renameDialog = new Form() {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Rename " + canvasPlanePanel1.GetActiveLayer().Name,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = "Enter a new layer name" };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (object s, EventArgs eargs) => {
                if (textBox.Text == "")
                {
                    MessageBox.Show("Invalid Name");
                } else
                {
                    canvasPlanePanel1.GetActiveLayer().Name = textBox.Text;
                    layerTabPanel1.SetDelegateCanvasPlanePanel(canvasPlanePanel1);
                    renameDialog.Close();
                }                
            };
            renameDialog.Controls.Add(textBox);
            renameDialog.Controls.Add(confirmation);
            renameDialog.Controls.Add(textLabel);
            renameDialog.AcceptButton = confirmation;

            renameDialog.ShowDialog();
        }

        private void visibleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            canvasPlanePanel1.GetActiveLayer().isVisible = !canvasPlanePanel1.GetActiveLayer().isVisible;
            canvasPlanePanel1.Invalidate();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to delete " + canvasPlanePanel1.GetActiveLayer().Name +"?", 
                "Delete Layer", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (canvasPlanePanel1.Layers.Count > 1)
                {
                    int index = canvasPlanePanel1.Layers.IndexOf(canvasPlanePanel1.GetActiveLayer());
                    if (canvasPlanePanel1.Layers.Count == (index+1))
                    {
                        //make first layer active
                        canvasPlanePanel1.Layers[0].isActiveLayer = true;                        
                    } else
                    {
                        //make next layer active
                        canvasPlanePanel1.Layers[index + 1].isActiveLayer = true;
                    }
                    //dispose of layer
                    canvasPlanePanel1.Layers[index].DBMP.Dispose();
                    canvasPlanePanel1.Layers.RemoveAt(index);
                    layerTabPanel1.SetDelegateCanvasPlanePanel(canvasPlanePanel1);
                    canvasPlanePanel1.Invalidate();
                } else
                {
                    MessageBox.Show("Cannot Delete " + canvasPlanePanel1.GetActiveLayer().Name + "!");
                }
            }
        }

        private void loadToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string f = "Bitmap Image (*.bmp)|*.bmp";
            f += "|Graphics Interchange Format (*.gif)|*.gif";
            f += "|Windows Icon Format (*.ico)|*.ico";
            f += "|Joint Photographic Experts Group (*.jpeg)|*.jpeg";
            f += "|Portable Network Graphics (*.png)|*.png";
            f += "|Tagged Image File Format (*.tiff)|*.tiff";
            ofd.Filter = f;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Graphics g = Graphics.FromImage(canvasPlanePanel1.GetActiveLayer().DBMP.bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(Bitmap.FromFile(ofd.FileName), new Point(0,0));
                g.Dispose();
                canvasPlanePanel1.Invalidate();
            }
        }

        private void saveToolStripMenuItem2_Click(object sender, EventArgs e)
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
                        canvasPlanePanel1.GetActiveLayer().DBMP.bmp.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".gif":
                        canvasPlanePanel1.GetActiveLayer().DBMP.bmp.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case ".ico":
                        canvasPlanePanel1.GetActiveLayer().DBMP.bmp.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Icon);
                        break;
                    case ".jpeg":
                        canvasPlanePanel1.GetActiveLayer().DBMP.bmp.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".png":
                        canvasPlanePanel1.GetActiveLayer().DBMP.bmp.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".tiff":
                        canvasPlanePanel1.GetActiveLayer().DBMP.bmp.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    default:
                        MessageBox.Show("unsupported format, " + extension);
                        break;
                }
            }
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (canvasPlanePanel1 != null)
            {
                CanvasLayer acl = canvasPlanePanel1.GetActiveLayer();
                if (acl != null)
                {
                    visibleToolStripMenuItem1.Checked = acl.isVisible;
                }
            }
        }

        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            canvasPlanePanel1.showGrid = !canvasPlanePanel1.showGrid;
            canvasPlanePanel1.Invalidate();
        }

        private void showAxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            canvasPlanePanel1.showAxes = !canvasPlanePanel1.showAxes;
            canvasPlanePanel1.Invalidate();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            showGridToolStripMenuItem.Checked = canvasPlanePanel1.showGrid;
            showAxesToolStripMenuItem.Checked = canvasPlanePanel1.showAxes;
        }
    }
}
