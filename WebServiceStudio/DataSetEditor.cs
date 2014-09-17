using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace WebServiceStudio
{
    public class DataSetEditor : UITypeEditor
    {
        private EditForm editForm;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value != null)
            {
                if (editForm == null)
                {
                    editForm = new EditForm();
                }
                editForm.DataSource = ((DataSet) value).Copy();
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    value = editForm.DataSource;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        internal class EditForm : Form
        {
            private static string[] DataSetFileExtenstions = {"xsd", "xml"};
            private Button Cancel;
            private Button LoadXml;
            private Button OK;
            private DataGrid dataGrid1;
            private Label label1;
            private Panel panelBottomMain;
            private Panel panelTopMain;

            internal EditForm()
            {
                InitializeComponent();
            }

            internal DataSet DataSource
            {
                get { return (dataGrid1.DataSource as DataSet); }
                set
                {
                    dataGrid1.DataSource = value;
                    LoadXml.Enabled = value.GetType() == typeof (DataSet);
                }
            }

            private void Cancel_Click(object sender, EventArgs e)
            {
                base.Close();
            }

            private void dataGrid1_Navigate(object sender, NavigateEventArgs ne)
            {
            }

            protected override void Dispose(bool dispose)
            {
                base.Dispose(dispose);
            }

            private void EditForm_Load(object sender, EventArgs e)
            {
                if (DataSource.Tables.Count == 1)
                {
                    dataGrid1.DataMember = DataSource.Tables[0].TableName;
                }
                else
                {
                    dataGrid1.DataMember = "";
                }
                label1.Text = DataSource.DataSetName;
            }

            private void InitializeComponent()
            {
                label1 = new Label();
                Cancel = new Button();
                OK = new Button();
                LoadXml = new Button();
                dataGrid1 = new DataGrid();
                panelTopMain = new Panel();
                panelBottomMain = new Panel();
                panelTopMain.SuspendLayout();
                panelBottomMain.SuspendLayout();
                base.SuspendLayout();
                label1.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
                label1.Location = new Point(0x10, 8);
                label1.Name = "label1";
                label1.Size = new Size(200, 0x18);
                label1.TabIndex = 3;
                label1.Text = "Data Set";
                Cancel.Location = new Point(0xe0, 8);
                Cancel.FlatStyle = FlatStyle.Popup;
                Cancel.Name = "Cancel";
                Cancel.Size = new Size(0x60, 0x18);
                Cancel.TabIndex = 1;
                Cancel.Text = "Cancel";
                Cancel.Click += Cancel_Click;
                OK.Location = new Point(0x148, 8);
                OK.FlatStyle = FlatStyle.Popup;
                OK.Name = "OK";
                OK.Size = new Size(0x60, 0x18);
                OK.TabIndex = 1;
                OK.Text = "OK";
                OK.Click += OK_Click;
                LoadXml.Location = new Point(0x1b0, 8);
                LoadXml.FlatStyle = FlatStyle.Popup;
                LoadXml.Name = "LoadXml";
                LoadXml.Size = new Size(0x60, 0x18);
                LoadXml.TabIndex = 1;
                LoadXml.Text = "Load XML...";
                LoadXml.Click += LoadXml_Click;
                dataGrid1.CaptionVisible = true;
                dataGrid1.DataMember = "";
                dataGrid1.Name = "dataGrid1";
                dataGrid1.Dock = DockStyle.Fill;
                dataGrid1.TabIndex = 4;
                dataGrid1.Navigate += dataGrid1_Navigate;
                panelTopMain.BorderStyle = BorderStyle.None;
                panelTopMain.Controls.AddRange(new Control[] {label1, Cancel, OK, LoadXml});
                panelTopMain.Dock = DockStyle.Top;
                panelTopMain.Name = "panelTopMain";
                panelTopMain.Size = new Size(0, 50);
                panelTopMain.TabIndex = 0;
                panelBottomMain.BorderStyle = BorderStyle.None;
                panelBottomMain.Controls.AddRange(new Control[] {dataGrid1});
                panelBottomMain.Dock = DockStyle.Fill;
                panelBottomMain.Location = new Point(0, 50);
                panelBottomMain.Name = "panelBottomMain";
                panelBottomMain.TabIndex = 1;
                panelBottomMain.SizeChanged += PanelBottomMain_SizeChanged;
                base.AcceptButton = OK;
                base.CancelButton = Cancel;
                AutoScaleBaseSize = new Size(5, 13);
                base.ClientSize = new Size(0x2c0, 0x14d);
                base.Controls.AddRange(new Control[] {panelBottomMain, panelTopMain});
                base.Name = "EditForm";
                Text = "Form1";
                base.FormBorderStyle = FormBorderStyle.Fixed3D;
                base.Load += EditForm_Load;
                dataGrid1.EndInit();
                base.ResumeLayout(false);
            }

            private void LoadXml_Click(object sender, EventArgs e)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Schema Files (*.xsd) |*.xsd|Data Files (*.xml)|*.xml|All Files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DataSource.ReadXml(dialog.FileName);
                    EditForm_Load(sender, e);
                }
            }

            private void OK_Click(object sender, EventArgs e)
            {
                base.DialogResult = DialogResult.OK;
                base.Close();
            }

            private void PanelBottomMain_SizeChanged(object sender, EventArgs e)
            {
                dataGrid1.SetBounds(0, 0, panelBottomMain.Width, panelBottomMain.Height, BoundsSpecified.Size);
            }
        }
    }
}