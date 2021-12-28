using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DSSoundStudio.UI
{
	// Token: 0x0200001A RID: 26
	public partial class SSEQImport : Form
	{
		// Token: 0x06000084 RID: 132 RVA: 0x0000A564 File Offset: 0x00008764
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000A59C File Offset: 0x0000879C
		private void InitializeComponent()
		{
			this.textBox1 = new TextBox();
			this.button1 = new Button();
			this.label1 = new Label();
			this.comboBox1 = new ComboBox();
			this.label2 = new Label();
			this.checkBox1 = new CheckBox();
			this.button2 = new Button();
			this.button3 = new Button();
			this.openFileDialog1 = new OpenFileDialog();
			base.SuspendLayout();
			this.textBox1.Location = new Point(82, 12);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new Size(295, 20);
			this.textBox1.TabIndex = 0;
			this.button1.FlatStyle = FlatStyle.System;
			this.button1.Location = new Point(383, 12);
			this.button1.Name = "button1";
			this.button1.Size = new Size(20, 20);
			this.button1.TabIndex = 1;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.button1_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new Size(64, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "SDAT Path:";
			this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox1.Enabled = false;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new Point(82, 38);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new Size(295, 21);
			this.comboBox1.TabIndex = 3;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(12, 41);
			this.label2.Name = "label2";
			this.label2.Size = new Size(59, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Sequence:";
			this.checkBox1.AutoSize = true;
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = CheckState.Checked;
			this.checkBox1.Location = new Point(15, 65);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new Size(187, 17);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Text = "Transfer player volume information";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.button2.DialogResult = DialogResult.OK;
			this.button2.Enabled = false;
			this.button2.Location = new Point(247, 88);
			this.button2.Name = "button2";
			this.button2.Size = new Size(75, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "OK";
			this.button2.UseVisualStyleBackColor = true;
			this.button3.DialogResult = DialogResult.Cancel;
			this.button3.Location = new Point(328, 88);
			this.button3.Name = "button3";
			this.button3.Size = new Size(75, 23);
			this.button3.TabIndex = 8;
			this.button3.Text = "Cancel";
			this.button3.UseVisualStyleBackColor = true;
			this.openFileDialog1.DefaultExt = "sdat";
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "NITRO Sound Data(*.sdat)|*.sdat";
			base.AcceptButton = this.button2;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.button3;
			base.ClientSize = new Size(415, 123);
			base.ControlBox = false;
			base.Controls.Add(this.button3);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.checkBox1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.comboBox1);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.textBox1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SSEQImport";
			this.Text = "Import SSEQ from SDAT";
			base.Load += new EventHandler(this.SSEQImport_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x040000AC RID: 172
		private IContainer components = null;

		// Token: 0x040000AD RID: 173
		private TextBox textBox1;

		// Token: 0x040000AE RID: 174
		private Button button1;

		// Token: 0x040000AF RID: 175
		private Label label1;

		// Token: 0x040000B0 RID: 176
		private ComboBox comboBox1;

		// Token: 0x040000B1 RID: 177
		private Label label2;

		// Token: 0x040000B2 RID: 178
		private CheckBox checkBox1;

		// Token: 0x040000B3 RID: 179
		private Button button2;

		// Token: 0x040000B4 RID: 180
		private Button button3;

		// Token: 0x040000B5 RID: 181
		private OpenFileDialog openFileDialog1;
	}
}
