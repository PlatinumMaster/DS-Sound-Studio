using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DSSoundStudio.UI
{
	// Token: 0x02000005 RID: 5
	public partial class SBNKViewer : Form
	{
		// Token: 0x06000018 RID: 24 RVA: 0x0000588C File Offset: 0x00003A8C
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000058C4 File Offset: 0x00003AC4
		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SBNKViewer));
			this.toolStrip1 = new ToolStrip();
			this.splitContainer1 = new SplitContainer();
			this.treeView1 = new TreeView();
			this.splitContainer2 = new SplitContainer();
			this.panel1 = new Panel();
			this.panel2 = new Panel();
			this.toolStrip2 = new ToolStrip();
			this.toolStripButtonExport = new ToolStripButton();
			this.saveFileDialog1 = new SaveFileDialog();
			this.pianoControl1 = new PianoControl();
			((ISupportInitialize)this.splitContainer1).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((ISupportInitialize)this.splitContainer2).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(774, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.splitContainer1.Dock = DockStyle.Fill;
			this.splitContainer1.FixedPanel = FixedPanel.Panel1;
			this.splitContainer1.Location = new Point(0, 25);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			this.splitContainer1.Panel1.Controls.Add(this.toolStrip2);
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new Size(774, 322);
			this.splitContainer1.SplitterDistance = 175;
			this.splitContainer1.TabIndex = 1;
			this.treeView1.Dock = DockStyle.Fill;
			this.treeView1.HideSelection = false;
			this.treeView1.HotTracking = true;
			this.treeView1.Location = new Point(0, 25);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new Size(175, 297);
			this.treeView1.TabIndex = 0;
			this.splitContainer2.Dock = DockStyle.Fill;
			this.splitContainer2.FixedPanel = FixedPanel.Panel2;
			this.splitContainer2.Location = new Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = Orientation.Horizontal;
			this.splitContainer2.Panel1.Controls.Add(this.panel1);
			this.splitContainer2.Panel2.Controls.Add(this.panel2);
			this.splitContainer2.Size = new Size(595, 322);
			this.splitContainer2.SplitterDistance = 208;
			this.splitContainer2.TabIndex = 0;
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(595, 208);
			this.panel1.TabIndex = 0;
			this.panel2.Controls.Add(this.pianoControl1);
			this.panel2.Dock = DockStyle.Fill;
			this.panel2.Location = new Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(595, 110);
			this.panel2.TabIndex = 0;
			this.toolStrip2.Items.AddRange(new ToolStripItem[]
			{
				this.toolStripButtonExport
			});
			this.toolStrip2.Location = new Point(0, 0);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new Size(175, 25);
			this.toolStrip2.TabIndex = 1;
			this.toolStrip2.Text = "toolStrip2";
			this.toolStripButtonExport.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonExport.Image = (Image)componentResourceManager.GetObject("toolStripButtonExport.Image");
			this.toolStripButtonExport.ImageTransparentColor = Color.Magenta;
			this.toolStripButtonExport.Name = "toolStripButtonExport";
			this.toolStripButtonExport.Size = new Size(23, 22);
			this.toolStripButtonExport.Text = "Export Instrument";
			this.toolStripButtonExport.Click += new EventHandler(this.toolStripButtonExport_Click);
			this.saveFileDialog1.Filter = "DS Instrument (*.inst)|*.inst";
			this.pianoControl1.Dock = DockStyle.Fill;
			this.pianoControl1.Location = new Point(0, 0);
			this.pianoControl1.Name = "pianoControl1";
			this.pianoControl1.Size = new Size(595, 110);
			this.pianoControl1.TabIndex = 1;
			this.pianoControl1.NoteDown += new PianoControl.NoteDownEventHandler(this.pianoControl1_NoteDown);
			this.pianoControl1.NoteUp += new PianoControl.NoteUpEventHandler(this.pianoControl1_NoteUp);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(774, 347);
			base.Controls.Add(this.splitContainer1);
			base.Controls.Add(this.toolStrip1);
			base.Name = "SBNKViewer";
			this.Text = "SBNKViewer";
			base.FormClosing += new FormClosingEventHandler(this.SBNKViewer_FormClosing);
			base.Load += new EventHandler(this.SBNKViewer_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer1).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer2).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x0400002F RID: 47
		private IContainer components = null;

		// Token: 0x04000030 RID: 48
		private ToolStrip toolStrip1;

		// Token: 0x04000031 RID: 49
		private SplitContainer splitContainer1;

		// Token: 0x04000032 RID: 50
		private TreeView treeView1;

		// Token: 0x04000033 RID: 51
		private SplitContainer splitContainer2;

		// Token: 0x04000034 RID: 52
		private Panel panel1;

		// Token: 0x04000035 RID: 53
		private Panel panel2;

		// Token: 0x04000036 RID: 54
		private PianoControl pianoControl1;

		// Token: 0x04000037 RID: 55
		private ToolStrip toolStrip2;

		// Token: 0x04000038 RID: 56
		private ToolStripButton toolStripButtonExport;

		// Token: 0x04000039 RID: 57
		private SaveFileDialog saveFileDialog1;
	}
}
