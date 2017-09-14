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
            canvasPlanePanel1.MouseDown += PlanePanel.DefaultMouseDownEventHandler;
            canvasPlanePanel1.MouseUp += PlanePanel.DefaultMouseUpEventHandler;
            canvasPlanePanel1.MouseMove += PlanePanel.DefaultMouseMoveEventHandler;
            //canvasPlanePanel1.MouseClick += PlanePanel.DefaultMouseClickEventHandler;
            canvasPlanePanel1.MouseWheel += PlanePanel.DefaultMouseWheelEventHandler;
            
        }

    }
}
