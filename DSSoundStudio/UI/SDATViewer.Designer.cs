using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DSSoundStudio.UI
{
	// Token: 0x02000003 RID: 3
	public partial class SDATViewer : Form, ISaveable
	{
		// Token: 0x0600000C RID: 12 RVA: 0x000042A0 File Offset: 0x000024A0
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000042D8 File Offset: 0x000024D8
		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SDATViewer));
			this.splitContainer1 = new SplitContainer();
			this.treeView1 = new TreeView();
			this.splitContainer2 = new SplitContainer();
			this.listViewNF1 = new ListViewNF();
			this.toolStrip1 = new ToolStrip();
			this.toolStripButton1 = new ToolStripButton();
			this.toolStripButton2 = new ToolStripButton();
			this.propertyGrid1 = new PropertyGrid();
			this.contextMenu1 = new ContextMenu();
			this.menuItemReplace = new MenuItem();
			this.openFileDialog1 = new OpenFileDialog();
			this.menuItemImportFromSDAT = new MenuItem();
			((ISupportInitialize)this.splitContainer1).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((ISupportInitialize)this.splitContainer2).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.splitContainer1.Dock = DockStyle.Fill;
			this.splitContainer1.FixedPanel = FixedPanel.Panel1;
			this.splitContainer1.Location = new Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new Size(707, 348);
			this.splitContainer1.SplitterDistance = 175;
			this.splitContainer1.TabIndex = 0;
			this.treeView1.Dock = DockStyle.Fill;
			this.treeView1.HideSelection = false;
			this.treeView1.HotTracking = true;
			this.treeView1.Location = new Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new Size(175, 348);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new TreeViewEventHandler(this.treeView1_AfterSelect);
			this.splitContainer2.Dock = DockStyle.Fill;
			this.splitContainer2.FixedPanel = FixedPanel.Panel2;
			this.splitContainer2.Location = new Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Panel1.Controls.Add(this.listViewNF1);
			this.splitContainer2.Panel1.Controls.Add(this.toolStrip1);
			this.splitContainer2.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer2.Size = new Size(528, 348);
			this.splitContainer2.SplitterDistance = 353;
			this.splitContainer2.TabIndex = 0;
			this.listViewNF1.Dock = DockStyle.Fill;
			this.listViewNF1.FullRowSelect = true;
			this.listViewNF1.GridLines = true;
			this.listViewNF1.HideSelection = false;
			this.listViewNF1.Location = new Point(0, 25);
			this.listViewNF1.Name = "listViewNF1";
			this.listViewNF1.Size = new Size(353, 323);
			this.listViewNF1.TabIndex = 1;
			this.listViewNF1.UseCompatibleStateImageBehavior = false;
			this.listViewNF1.View = View.Details;
			this.listViewNF1.ItemActivate += new EventHandler(this.listViewNF1_ItemActivate);
			this.listViewNF1.MouseClick += new MouseEventHandler(this.listViewNF1_MouseClick);
			this.toolStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.toolStripButton1,
				this.toolStripButton2
			});
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(353, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Enabled = false;
			this.toolStripButton1.Image = (Image)componentResourceManager.GetObject("toolStripButton1.Image");
			this.toolStripButton1.ImageTransparentColor = Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new Size(23, 22);
			this.toolStripButton1.Text = "toolStripButton1";
			this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Enabled = false;
			this.toolStripButton2.Image = (Image)componentResourceManager.GetObject("toolStripButton2.Image");
			this.toolStripButton2.ImageTransparentColor = Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new Size(23, 22);
			this.toolStripButton2.Text = "toolStripButton2";
			this.propertyGrid1.Dock = DockStyle.Fill;
			this.propertyGrid1.Location = new Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new Size(171, 348);
			this.propertyGrid1.TabIndex = 0;
			this.contextMenu1.MenuItems.AddRange(new MenuItem[]
			{
				this.menuItemImportFromSDAT,
				this.menuItemReplace
			});
			this.menuItemReplace.Index = 1;
			this.menuItemReplace.Text = "Replace...";
			this.menuItemReplace.Click += new EventHandler(this.menuItemReplace_Click);
			this.openFileDialog1.FileName = "openFileDialog1";
			this.menuItemImportFromSDAT.Index = 0;
			this.menuItemImportFromSDAT.Text = "Import from SDAT...";
			this.menuItemImportFromSDAT.Visible = false;
			this.menuItemImportFromSDAT.Click += new EventHandler(this.menuItemImportFromSDAT_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(707, 348);
			base.Controls.Add(this.splitContainer1);
			base.Name = "SDATViewer";
			this.Text = "SDATViewer";
			base.Load += new EventHandler(this.SDATViewer_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer1).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer2).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
		}

		// Token: 0x04000006 RID: 6
		private IContainer components = null;

		// Token: 0x04000007 RID: 7
		private SplitContainer splitContainer1;

		// Token: 0x04000008 RID: 8
		private TreeView treeView1;

		// Token: 0x04000009 RID: 9
		private SplitContainer splitContainer2;

		// Token: 0x0400000A RID: 10
		private ListViewNF listViewNF1;

		// Token: 0x0400000B RID: 11
		private ToolStrip toolStrip1;

		// Token: 0x0400000C RID: 12
		private PropertyGrid propertyGrid1;

		// Token: 0x0400000D RID: 13
		private ToolStripButton toolStripButton1;

		// Token: 0x0400000E RID: 14
		private ToolStripButton toolStripButton2;

		// Token: 0x0400000F RID: 15
		private ContextMenu contextMenu1;

		// Token: 0x04000010 RID: 16
		private MenuItem menuItemReplace;

		// Token: 0x04000011 RID: 17
		private OpenFileDialog openFileDialog1;

		// Token: 0x04000012 RID: 18
		private MenuItem menuItemImportFromSDAT;
	}
}
