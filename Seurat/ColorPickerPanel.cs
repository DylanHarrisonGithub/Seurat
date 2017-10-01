using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seurat
{
    class ColorPickerPanel : Panel
    {
        public TrackBar TrackBarC1;
        public TrackBar TrackBarC2;
        private TrackBar TrackBarC1R;
        private TrackBar TrackBarC1G;
        private TrackBar TrackBarC1B;
        private TrackBar TrackBarC2R;
        private TrackBar TrackBarC2G;
        private TrackBar TrackBarC2B;
        public TrackBar TrackBarOpacity;
        private Panel PanelColorDisplayC1;
        private Panel PanelColorDisplayC2;
        public VerticalGradientPanel VerticalGradient;
        public UInt32 C1 { get; private set; } = 0xff000000;
        public UInt32 C2 { get; private set; } = 0xffffffff;
        
        public ColorPickerPanel()
        {
            CreateGUI();
            SetupTrackBars();
            DoubleBuffered = true;
        }

        private void ComponentTrackBarChangedEvent(object sender, System.EventArgs e)
        {
            UInt32 CA = BitConverter.ToUInt32(new byte[] {                
                (byte)TrackBarC1B.Value,
                (byte)TrackBarC1G.Value,
                (byte)TrackBarC1R.Value,
                (byte)TrackBarOpacity.Value}, 0);

            UInt32 CB = BitConverter.ToUInt32(new byte[] {
                (byte)TrackBarC2B.Value,
                (byte)TrackBarC2G.Value,
                (byte)TrackBarC2R.Value,
                (byte)TrackBarOpacity.Value}, 0);

            VerticalGradient.c1 = CA;
            VerticalGradient.c2 = CB;
            VerticalGradient.Invalidate();

            C1 = GetInterpolatedColor((byte)(255 - TrackBarC1.Value), CA, CB);
            C2 = GetInterpolatedColor((byte)(255 - TrackBarC2.Value), CA, CB);

            byte[] C1Bytes = BitConverter.GetBytes(C1);
            byte[] C2Bytes = BitConverter.GetBytes(C2);

            PanelColorDisplayC1.BackColor = System.Drawing.Color.FromArgb(C1Bytes[3], C1Bytes[2], C1Bytes[1], C1Bytes[0]);
            PanelColorDisplayC2.BackColor = System.Drawing.Color.FromArgb(C2Bytes[3], C2Bytes[2], C2Bytes[1], C2Bytes[0]);
            this.Invalidate();
        }

        private void SetupTrackBars()
        {
            EventHandler ComponentChangeEvent = new EventHandler(ComponentTrackBarChangedEvent);

            TrackBarC1.ValueChanged += (object sender, System.EventArgs e) =>
            {
                UInt32 CA = BitConverter.ToUInt32(new byte[] {
                    (byte)TrackBarC1B.Value,
                    (byte)TrackBarC1G.Value,
                    (byte)TrackBarC1R.Value,
                    (byte)TrackBarOpacity.Value}, 0);

                UInt32 CB = BitConverter.ToUInt32(new byte[] {
                    (byte)TrackBarC2B.Value,
                    (byte)TrackBarC2G.Value,
                    (byte)TrackBarC2R.Value,
                    (byte)TrackBarOpacity.Value}, 0);

                C1 = GetInterpolatedColor((byte)(255 - TrackBarC1.Value), CA, CB);
                byte[] C1Bytes = BitConverter.GetBytes(C1);

                PanelColorDisplayC1.BackColor = System.Drawing.Color.FromArgb(C1Bytes[3], C1Bytes[2], C1Bytes[1], C1Bytes[0]);
                this.Invalidate();
            };
            TrackBarC2.ValueChanged += (object sender, System.EventArgs e) =>
            {
                UInt32 CA = BitConverter.ToUInt32(new byte[] {
                    (byte)TrackBarC1B.Value,
                    (byte)TrackBarC1G.Value,
                    (byte)TrackBarC1R.Value,
                    (byte)TrackBarOpacity.Value}, 0);

                UInt32 CB = BitConverter.ToUInt32(new byte[] {
                    (byte)TrackBarC2B.Value,
                    (byte)TrackBarC2G.Value,
                    (byte)TrackBarC2R.Value,
                    (byte)TrackBarOpacity.Value}, 0);

                C2 = GetInterpolatedColor((byte)(255-TrackBarC2.Value), CA, CB);

                byte[] C2Bytes = BitConverter.GetBytes(C2);

                PanelColorDisplayC2.BackColor = System.Drawing.Color.FromArgb(C2Bytes[3], C2Bytes[2], C2Bytes[1], C2Bytes[0]);
                this.Invalidate();
            };

            TrackBarC1R.ValueChanged += ComponentChangeEvent;
            TrackBarC1G.ValueChanged += ComponentChangeEvent;
            TrackBarC1B.ValueChanged += ComponentChangeEvent;
            TrackBarC2R.ValueChanged += ComponentChangeEvent;
            TrackBarC2G.ValueChanged += ComponentChangeEvent;
            TrackBarC2B.ValueChanged += ComponentChangeEvent;
            TrackBarOpacity.ValueChanged += ComponentChangeEvent;

        }

        private void CreateGUI()
        {
            //Build GUI

            //Base Table
            TableLayoutPanel BaseTable = new TableLayoutPanel();
            BaseTable.Dock = DockStyle.Fill;
            BaseTable.Margin = new Padding(0, 0, 0, 0);
            BaseTable.Padding = new Padding(0, 0, 0, 0);
            BaseTable.ColumnCount = 1;
            BaseTable.RowCount = 2;
            BaseTable.RowStyles.Add(new RowStyle(SizeType.Percent, 75.0f));
            BaseTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0f));
            BaseTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
            //BaseTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            //C1/C2 selector and Component Selector table
            TableLayoutPanel c1c2AndComponentTable = new TableLayoutPanel();
            c1c2AndComponentTable.Dock = DockStyle.Fill;
            c1c2AndComponentTable.Margin = new Padding(0, 0, 0, 0);
            c1c2AndComponentTable.Padding = new Padding(0, 0, 0, 0);
            //c1c2AndComponentTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            c1c2AndComponentTable.ColumnCount = 2;
            c1c2AndComponentTable.RowCount = 1;
            c1c2AndComponentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            c1c2AndComponentTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            c1c2AndComponentTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.0f));
            BaseTable.Controls.Add(c1c2AndComponentTable, 0, 0);

            //c1/c2 display and opacity selector table
            TableLayoutPanel c1c2DisplayAndOpacityTable = new TableLayoutPanel();
            c1c2DisplayAndOpacityTable.Dock = DockStyle.Fill;
            c1c2DisplayAndOpacityTable.Margin = new Padding(0, 0, 0, 0);
            c1c2DisplayAndOpacityTable.Padding = new Padding(0, 0, 0, 0);
            //c1c2DisplayAndOpacityTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            c1c2DisplayAndOpacityTable.ColumnCount = 2;
            c1c2DisplayAndOpacityTable.RowCount = 2;
            c1c2DisplayAndOpacityTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            c1c2DisplayAndOpacityTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            c1c2DisplayAndOpacityTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            c1c2DisplayAndOpacityTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            BaseTable.Controls.Add(c1c2DisplayAndOpacityTable, 0, 1);

            //c1 and c2 selector table
            TableLayoutPanel c1c2SelectorTable = new TableLayoutPanel();
            c1c2SelectorTable.Dock = DockStyle.Fill;
            c1c2SelectorTable.Margin = new Padding(0, 0, 0, 0);
            c1c2SelectorTable.Padding = new Padding(0, 0, 0, 0);
            //c1c2SelectorTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            c1c2SelectorTable.ColumnCount = 3;
            c1c2SelectorTable.RowCount = 1;
            c1c2SelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c1c2SelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c1c2SelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c1c2AndComponentTable.Controls.Add(c1c2SelectorTable, 0, 0);

            //c1 and c2 component selector table
            TableLayoutPanel c1c2ComponentSelectorTable = new TableLayoutPanel();
            c1c2ComponentSelectorTable.Dock = DockStyle.Fill;
            c1c2ComponentSelectorTable.Margin = new Padding(0, 0, 0, 0);
            c1c2ComponentSelectorTable.Padding = new Padding(0, 0, 0, 0);
            c1c2ComponentSelectorTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            c1c2ComponentSelectorTable.ColumnCount = 1;
            c1c2ComponentSelectorTable.RowCount = 2;
            c1c2ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            c1c2ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            c1c2ComponentSelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
            c1c2AndComponentTable.Controls.Add(c1c2ComponentSelectorTable, 1, 0);

            //TrackbarC1
            TrackBarC1 = new TrackBar();
            TrackBarC1.Orientation = Orientation.Vertical;
            TrackBarC1.TickStyle = TickStyle.None;
            TrackBarC1.Minimum = 0;
            TrackBarC1.Maximum = 255;
            TrackBarC1.Dock = DockStyle.Fill;
            TrackBarC1.Value = 0;
            TrackBarC1.Margin = new Padding(0, 0, 0, 0);
            TrackBarC1.Padding = new Padding(0, 0, 0, 0);
            //GradientPanel
            VerticalGradient = new VerticalGradientPanel();
            VerticalGradient.Dock = DockStyle.Fill;
            VerticalGradient.Padding = new Padding(0, 8, 0, 8);
            //TrackbarC2
            TrackBarC2 = new TrackBar();
            TrackBarC2.Orientation = Orientation.Vertical;
            TrackBarC2.TickStyle = TickStyle.None;
            TrackBarC2.Minimum = 0;
            TrackBarC2.Maximum = 255;
            TrackBarC2.Value = 255;
            TrackBarC2.Dock = DockStyle.Fill;
            TrackBarC2.RightToLeft = RightToLeft.Yes;
            TrackBarC2.RightToLeftLayout = true;
            TrackBarC2.Margin = new Padding(0, 0, 0, 0);
            TrackBarC2.Padding = new Padding(0, 0, 0, 0);
            c1c2SelectorTable.Controls.Add(TrackBarC1, 0, 0);
            c1c2SelectorTable.Controls.Add(VerticalGradient, 1, 0);
            c1c2SelectorTable.Controls.Add(TrackBarC2, 2, 0);

            //c1 components table
            TableLayoutPanel c1ComponentSelectorTable = new TableLayoutPanel();
            c1ComponentSelectorTable.Dock = DockStyle.Fill;
            c1ComponentSelectorTable.Margin = new Padding(0, 0, 0, 0);
            c1ComponentSelectorTable.Padding = new Padding(0, 0, 0, 0);
            //c1ComponentSelectorTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            c1ComponentSelectorTable.ColumnCount = 2;
            c1ComponentSelectorTable.RowCount = 3;
            c1ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c1ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c1ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c1ComponentSelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            c1ComponentSelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.0f));
            c1c2ComponentSelectorTable.Controls.Add(c1ComponentSelectorTable, 0, 0);

            //c2 components table
            TableLayoutPanel c2ComponentSelectorTable = new TableLayoutPanel();
            c2ComponentSelectorTable.Dock = DockStyle.Fill;
            c2ComponentSelectorTable.Margin = new Padding(0, 0, 0, 0);
            c2ComponentSelectorTable.Padding = new Padding(0, 0, 0, 0);
            //c2ComponentSelectorTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            c2ComponentSelectorTable.ColumnCount = 2;
            c2ComponentSelectorTable.RowCount = 3;
            c2ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c2ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c2ComponentSelectorTable.RowStyles.Add(new RowStyle(SizeType.Percent, (float)(100.0 / 3.0)));
            c2ComponentSelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            c2ComponentSelectorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.0f));
            c1c2ComponentSelectorTable.Controls.Add(c2ComponentSelectorTable, 0, 1);

            //c1 and c2 component labels
            Label R1Label = new Label() { Text = "R1", TextAlign = System.Drawing.ContentAlignment.TopCenter };
            Label G1Label = new Label() { Text = "G1", TextAlign = System.Drawing.ContentAlignment.TopCenter };
            Label B1Label = new Label() { Text = "B1", TextAlign = System.Drawing.ContentAlignment.TopCenter };
            Label R2Label = new Label() { Text = "R2", TextAlign = System.Drawing.ContentAlignment.TopCenter };
            Label G2Label = new Label() { Text = "G2", TextAlign = System.Drawing.ContentAlignment.TopCenter };
            Label B2Label = new Label() { Text = "B2", TextAlign = System.Drawing.ContentAlignment.TopCenter };
            c1ComponentSelectorTable.Controls.Add(R1Label, 0, 0);
            c1ComponentSelectorTable.Controls.Add(G1Label, 0, 1);
            c1ComponentSelectorTable.Controls.Add(B1Label, 0, 2);
            c2ComponentSelectorTable.Controls.Add(R2Label, 0, 0);
            c2ComponentSelectorTable.Controls.Add(G2Label, 0, 1);
            c2ComponentSelectorTable.Controls.Add(B2Label, 0, 2);

            //c1 and c2 component trackbars
            TrackBarC1R = new TrackBar() { Maximum = 255, Dock = DockStyle.Fill, TickStyle = TickStyle.None, Value = 255 };
            TrackBarC1G = new TrackBar() { Maximum = 255, Dock = DockStyle.Fill, TickStyle = TickStyle.None, Value = 255 };
            TrackBarC1B = new TrackBar() { Maximum = 255, Dock = DockStyle.Fill, TickStyle = TickStyle.None, Value = 255 };
            TrackBarC2R = new TrackBar() { Maximum = 255, Dock = DockStyle.Fill, TickStyle = TickStyle.None };
            TrackBarC2G = new TrackBar() { Maximum = 255, Dock = DockStyle.Fill, TickStyle = TickStyle.None };
            TrackBarC2B = new TrackBar() { Maximum = 255, Dock = DockStyle.Fill, TickStyle = TickStyle.None };
            c1ComponentSelectorTable.Controls.Add(TrackBarC1R, 1, 0);
            c1ComponentSelectorTable.Controls.Add(TrackBarC1G, 1, 1);
            c1ComponentSelectorTable.Controls.Add(TrackBarC1B, 1, 2);
            c2ComponentSelectorTable.Controls.Add(TrackBarC2R, 1, 0);
            c2ComponentSelectorTable.Controls.Add(TrackBarC2G, 1, 1);
            c2ComponentSelectorTable.Controls.Add(TrackBarC2B, 1, 2);

            //c1 and c2 background panels
            Panel C1BackGroundPanel = new Panel() { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.White };
            Panel C2BackGroundPanel = new Panel() { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.White };
            c1c2DisplayAndOpacityTable.Controls.Add(C1BackGroundPanel, 0, 0);
            c1c2DisplayAndOpacityTable.Controls.Add(C2BackGroundPanel, 1, 0);

            //opacity label and trackbar
            Label OpacityLabel = new Label() { Text = "Opacity", TextAlign = System.Drawing.ContentAlignment.TopCenter };
            TrackBarOpacity = new TrackBar() { Maximum = 255, Dock = DockStyle.Fill, TickStyle = TickStyle.None, Value = 255 };
            c1c2DisplayAndOpacityTable.Controls.Add(OpacityLabel, 0, 1);
            c1c2DisplayAndOpacityTable.Controls.Add(TrackBarOpacity, 1, 1);

            //c1 and c2 display panels
            PanelColorDisplayC1 = new Panel() { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.Black, Padding = new Padding(0, 0, 0, 0), Margin = new Padding(0, 0, 0, 0) };
            PanelColorDisplayC2 = new Panel() { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.White, Padding = new Padding(0, 0, 0, 0), Margin = new Padding(0, 0, 0, 0) };
            C1BackGroundPanel.Controls.Add(PanelColorDisplayC1);
            C2BackGroundPanel.Controls.Add(PanelColorDisplayC2);

            this.Controls.Add(BaseTable);
        }

        public UInt32 GetInterpolatedColor(byte t, UInt32 c1, UInt32 c2)
        {
            byte[] c1Bytes = BitConverter.GetBytes(c1);
            byte[] c2Bytes = BitConverter.GetBytes(c2);
            byte[] newColor = { 0, 0, 0, 0 };

            newColor[0] = (byte)(c1Bytes[0] + ((double)t / 255.0) * (c2Bytes[0] - c1Bytes[0]));
            newColor[1] = (byte)(c1Bytes[1] + ((double)t / 255.0) * (c2Bytes[1] - c1Bytes[1]));
            newColor[2] = (byte)(c1Bytes[2] + ((double)t / 255.0) * (c2Bytes[2] - c1Bytes[2]));
            newColor[3] = (byte)(c1Bytes[3] + ((double)t / 255.0) * (c2Bytes[3] - c1Bytes[3]));

            return BitConverter.ToUInt32(newColor, 0);
        }

        private void setTopColor(UInt32 c)
        {
            byte[] cBytes = BitConverter.GetBytes(c);
            TrackBarC1R.Value = cBytes[0];
            TrackBarC1G.Value = cBytes[1];
            TrackBarC1B.Value = cBytes[2];
            VerticalGradient.c1 = c;
            VerticalGradient.Invalidate();
        }

        private void setBottomColor(UInt32 c)
        {
            byte[] cBytes = BitConverter.GetBytes(c);
            TrackBarC2R.Value = cBytes[0];
            TrackBarC2G.Value = cBytes[1];
            TrackBarC2B.Value = cBytes[2];
            VerticalGradient.c2 = c;
            VerticalGradient.Invalidate();
        }

        public void setC1(UInt32 c)
        {
            setTopColor(c);
            TrackBarC1.Value = 255;
            //C1 = c;
        }
        public void setC2(UInt32 c)
        {
            setBottomColor(c);
            TrackBarC2.Value = 0;
            //C2 = c;
        }
    }
}
