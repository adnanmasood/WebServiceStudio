using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WebServiceStudio
{
    public class SearchDialog : Form
    {
        private readonly Container components = null;
        public bool MatchCase;
        public string SearchStr;
        public bool WholeWord;
        private Button buttonCancel;
        private Button buttonOk;
        private CheckBox checkMatchCase;
        private CheckBox checkWholeWord;
        private Label label1;
        private TextBox textSearch;

        public SearchDialog()
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
            SearchStr = textSearch.Text;
            MatchCase = checkMatchCase.Checked;
            WholeWord = checkWholeWord.Checked;
            base.DialogResult = DialogResult.OK;
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
            label1 = new Label();
            textSearch = new TextBox();
            checkMatchCase = new CheckBox();
            checkWholeWord = new CheckBox();
            buttonOk = new Button();
            buttonCancel = new Button();
            base.SuspendLayout();
            label1.Location = new Point(0x18, 0x10);
            label1.Name = "label1";
            label1.Size = new Size(40, 0x10);
            label1.TabIndex = 0;
            label1.Text = "Search";
            textSearch.Location = new Point(0x40, 0x10);
            textSearch.Name = "textSearch";
            textSearch.Size = new Size(0xd0, 20);
            textSearch.TabIndex = 1;
            textSearch.Text = "";
            checkMatchCase.Location = new Point(0x18, 0x30);
            checkMatchCase.Name = "checkMatchCase";
            checkMatchCase.Size = new Size(0x58, 0x18);
            checkMatchCase.TabIndex = 2;
            checkMatchCase.Text = "Match Case";
            checkWholeWord.Location = new Point(0x18, 0x48);
            checkWholeWord.Name = "checkWholeWord";
            checkWholeWord.Size = new Size(0x58, 0x18);
            checkWholeWord.TabIndex = 2;
            checkWholeWord.Text = "Whole Word";
            buttonOk.Location = new Point(0x80, 0x38);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(0x40, 0x18);
            buttonOk.TabIndex = 3;
            buttonOk.Text = "OK";
            buttonOk.Click += buttonOk_Click;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(0xd0, 0x38);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(0x40, 0x18);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += buttonCancel_Click;
            base.AcceptButton = buttonOk;
            base.CancelButton = buttonCancel;
            AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x124, 0x6d);
            base.ControlBox = false;
            base.Controls.AddRange(new Control[]
            {buttonOk, checkMatchCase, textSearch, label1, checkWholeWord, buttonCancel});
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Name = "SearchDialog";
            Text = "SearchDialog";
            base.ResumeLayout(false);
        }
    }
}