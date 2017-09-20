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
            canvasPlanePanel1.MyColorPicker = colorPickerPanel1;
            canvasPlanePanel1.SetToolStrip(toolStrip1);
            canvasPlanePanel1.MyToolSettingsGroupBox = toolSettingsGroupBox;

            toolStrip1.Items[0].PerformClick();
        }

        private void CanvasPlanePanel1_MouseMove(object sender, MouseEventArgs e)
        {
            int[] c = canvasPlanePanel1.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            mouseCoordinateLabel.Text = "Mouse (" + c[0].ToString() + ", " + c[1].ToString() + ")";
        }
    }
}
