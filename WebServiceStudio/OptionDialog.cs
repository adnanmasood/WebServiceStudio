using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WebServiceStudio
{
    internal class OptionDialog : Form
    {
        private readonly Container components = null;
        private Button buttonCancel;
        private Button buttonOk;
        private Panel panelBottomMain;
        private Panel panelTopMain;
        private PropertyGrid propertyOptions;

        public OptionDialog()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
            var selectedObject = propertyOptions.SelectedObject as Configuration;
            if (selectedObject != null)
            {
                Configuration.MasterConfig = selectedObject;
            }
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            propertyOptions = new PropertyGrid();
            buttonOk = new Button();
            buttonCancel = new Button();
            panelTopMain = new Panel();
            panelBottomMain = new Panel();
            panelTopMain.SuspendLayout();
            panelBottomMain.SuspendLayout();
            base.SuspendLayout();
            propertyOptions.CommandsVisibleIfAvailable = true;
            propertyOptions.HelpVisible = false;
            propertyOptions.LargeButtons = false;
            propertyOptions.LineColor = SystemColors.ScrollBar;
            propertyOptions.Location = new Point(8, 8);
            propertyOptions.Name = "propertyOptions";
            propertyOptions.PropertySort = PropertySort.Alphabetical;
            propertyOptions.Dock = DockStyle.Fill;
            propertyOptions.TabIndex = 0;
            propertyOptions.Text = "PropertyGrid";
            propertyOptions.ToolbarVisible = false;
            propertyOptions.ViewBackColor = SystemColors.Window;
            propertyOptions.ViewForeColor = SystemColors.WindowText;
            propertyOptions.SelectedObject = Configuration.MasterConfig.Copy();
            buttonOk.DialogResult = DialogResult.Cancel;
            buttonOk.FlatStyle = FlatStyle.Popup;
            buttonOk.Location = new Point(8, 5);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(50, 20);
            buttonOk.TabIndex = 1;
            buttonOk.Text = "OK";
            buttonOk.Click += buttonOk_Click;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.FlatStyle = FlatStyle.Popup;
            buttonCancel.Location = new Point(70, 5);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(50, 20);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += buttonCancel_Click;
            panelTopMain.BorderStyle = BorderStyle.None;
            panelTopMain.Controls.AddRange(new Control[] {propertyOptions});
            panelTopMain.Dock = DockStyle.Fill;
            panelTopMain.Name = "panelTopMain";
            panelTopMain.Size = new Size(0, 250);
            panelTopMain.TabIndex = 0;
            panelBottomMain.BorderStyle = BorderStyle.None;
            panelBottomMain.Controls.AddRange(new Control[] {buttonOk, buttonCancel});
            panelBottomMain.Dock = DockStyle.Bottom;
            panelBottomMain.Size = new Size(0, 30);
            panelBottomMain.Name = "panelBottomMain";
            panelBottomMain.TabIndex = 1;
            base.AcceptButton = buttonOk;
            base.CancelButton = buttonCancel;
            AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x110, 0x12b);
            base.Controls.AddRange(new Control[] {panelTopMain, panelBottomMain});
            base.Name = "OptionDialog";
            Text = "Options ";
            panelTopMain.ResumeLayout(false);
            panelBottomMain.ResumeLayout(false);
            base.ResumeLayout(false);
            propertyOptions.ExpandAllGridItems();
        }
    }
}