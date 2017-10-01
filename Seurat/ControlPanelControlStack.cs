using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seurat
{
    class ControlPanelControlStack : TableLayoutPanel
    {
        public List<Control> StackControls { get; }
        public List<Label> StackLabels { get; }

        public ControlPanelControlStack()
        {
            StackControls = new List<Control>();
            StackLabels = new List<Label>();
            Margin = new Padding(0, 0, 0, 0);
            Padding = new Padding(0, 0, 0, 0);
            ColumnCount = 1;
            RowCount = 0;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
            Dock = DockStyle.Top;
            AutoSize = true;
        }

        public void AddHeaderLabel(string labelText, System.Drawing.Color textColor, System.Drawing.Color backgroundColor)
        {
            Panel header = new Panel();
            header.BackColor = backgroundColor;
            header.MinimumSize = new System.Drawing.Size(175, 20);
            header.MaximumSize = new System.Drawing.Size(175, 20);

            Label l = new Label();            
            l.Text = labelText;            
            l.BackColor = backgroundColor;
            l.ForeColor = textColor;
            l.Dock = DockStyle.Fill;
            l.AutoSize = false;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            header.Controls.Add(l);

            StackLabels.Add(l);
            StackControls.Add(null);

            RowCount += 1;
            RowStyles.Add(new RowStyle(SizeType.Absolute, 20));           
            Controls.Add(header, 0, RowCount - 1);
        }
        public void AddLabelTrackBar(string LabelText, int TrackBarMin, int TrackBarMax, int TrackBarInitVal)
        {
            TableLayoutPanel lsb = new TableLayoutPanel();
            lsb.Margin = new Padding(0, 0, 0, 0);
            lsb.Padding = new Padding(0, 0, 0, 0);
            lsb.ColumnCount = 2;
            lsb.RowCount = 1;
            lsb.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.0f));
            lsb.MaximumSize = new System.Drawing.Size(175, 20);
            lsb.MinimumSize = new System.Drawing.Size(175, 20);

            Label l = new Label();
            l.Text = LabelText;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            l.MinimumSize = new System.Drawing.Size(0, 20);
            l.MaximumSize = new System.Drawing.Size(9999999, 20);

            TrackBar s = new TrackBar();
            s.Minimum = TrackBarMin;
            s.Maximum = TrackBarMax;
            s.Value = TrackBarInitVal;
            s.TickStyle = TickStyle.None;
            s.Padding = new Padding(0, 0, 0, 0);
            s.Margin = new Padding(0, 0, 0, 0);
            s.MinimumSize = new System.Drawing.Size(130, 20);
            s.MaximumSize = new System.Drawing.Size(130, 20);

            lsb.Controls.Add(l, 0, 0);
            lsb.Controls.Add(s, 1, 0);

            StackLabels.Add(l);
            StackControls.Add(s);

            RowCount += 1;
            RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            Controls.Add(lsb, 0, RowCount-1);
        }

        public void AddLabelCheckBox(string labelText, bool initChecked)
        {
            TableLayoutPanel lsb = new TableLayoutPanel();
            lsb.Margin = new Padding(0, 0, 0, 0);
            lsb.Padding = new Padding(0, 0, 0, 0);
            lsb.ColumnCount = 2;
            lsb.RowCount = 1;
            lsb.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            lsb.MaximumSize = new System.Drawing.Size(175, 20);
            lsb.MinimumSize = new System.Drawing.Size(175, 20);

            Label l = new Label();
            l.Text = labelText;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            l.MinimumSize = new System.Drawing.Size(0, 20);
            l.MaximumSize = new System.Drawing.Size(9999999, 20);

            CheckBox s = new CheckBox();
            s.Checked = initChecked;
            s.MinimumSize = new System.Drawing.Size(0, 20);
            s.MaximumSize = new System.Drawing.Size(9999999, 20);

            lsb.Controls.Add(l, 0, 0);
            lsb.Controls.Add(s, 1, 0);

            StackLabels.Add(l);
            StackControls.Add(s);

            RowCount += 1;
            RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            Controls.Add(lsb, 0, RowCount - 1);
        }

        public void AddLabelComboBox(string labelText, string[] options, string initSelected)
        {
            TableLayoutPanel lsb = new TableLayoutPanel();
            lsb.Margin = new Padding(0, 0, 0, 0);
            lsb.Padding = new Padding(0, 0, 0, 0);
            lsb.ColumnCount = 2;
            lsb.RowCount = 1;
            lsb.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.0f));
            lsb.MaximumSize = new System.Drawing.Size(175, 20);
            lsb.MinimumSize = new System.Drawing.Size(175, 20);

            Label l = new Label();
            l.Text = labelText;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            l.MinimumSize = new System.Drawing.Size(0, 20);
            l.MaximumSize = new System.Drawing.Size(9999999, 20);

            ComboBox s = new ComboBox();
            s.MinimumSize = new System.Drawing.Size(0, 20);
            s.MaximumSize = new System.Drawing.Size(9999999, 20);
            s.Padding = new Padding(0, 0, 0, 0);
            s.Margin = new Padding(0, 0, 0, 0);
            s.Items.AddRange(options);
            s.SelectedItem = initSelected;

            lsb.Controls.Add(l, 0, 0);
            lsb.Controls.Add(s, 1, 0);

            StackLabels.Add(l);
            StackControls.Add(s);

            RowCount += 1;
            RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            Controls.Add(lsb, 0, RowCount - 1);
        }

        public void AddLabelTextBox(string labelText, string defaultText)
        {
            TableLayoutPanel lsb = new TableLayoutPanel();
            lsb.Margin = new Padding(0, 0, 0, 0);
            lsb.Padding = new Padding(0, 0, 0, 0);
            lsb.ColumnCount = 2;
            lsb.RowCount = 1;
            lsb.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));
            lsb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.0f));
            lsb.MaximumSize = new System.Drawing.Size(175, 20);
            lsb.MinimumSize = new System.Drawing.Size(175, 20);

            Label l = new Label();
            l.Text = labelText;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            l.MinimumSize = new System.Drawing.Size(0, 20);
            l.MaximumSize = new System.Drawing.Size(9999999, 20);

            TextBox s = new TextBox();
            s.Text = defaultText;
            s.MinimumSize = new System.Drawing.Size(0, 20);
            s.MaximumSize = new System.Drawing.Size(9999999, 20);

            lsb.Controls.Add(l, 0, 0);
            lsb.Controls.Add(s, 1, 0);

            StackLabels.Add(l);
            StackControls.Add(s);

            RowCount += 1;
            RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            Controls.Add(lsb, 0, RowCount - 1);
        }
    }
}
