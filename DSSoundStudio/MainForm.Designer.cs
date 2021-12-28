using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DSSoundStudio.UI;
using MainMenu = System.Windows.Forms.MainMenu;

namespace DSSoundStudio
{
	// Token: 0x02000013 RID: 19
	public partial class MainForm : Form
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00008B74 File Offset: 0x00006D74
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00008BAC File Offset: 0x00006DAC
		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
			this.mainMenu = new UI.MainMenu(this.components);
			this.menuItem1 = new MenuItem();
			this.menuItemOpen = new MenuItem();
			this.menuItem8 = new MenuItem();
			this.menuItemSave = new MenuItem();
			this.menuItemSaveAs = new MenuItem();
			this.menuItemSaveAll = new MenuItem();
			this.menuItem9 = new MenuItem();
			this.menuItemExit = new MenuItem();
			this.menuItem2 = new MenuItem();
			this.menuItem4 = new MenuItem();
			this.menuItem3 = new MenuItem();
			this.toolStrip = new ToolStrip();
			this.toolStripButtonOpen = new ToolStripButton();
			this.toolStripButtonSave = new ToolStripButton();
			this.toolStripButtonSaveAll = new ToolStripButton();
			this.vistaMenu = new VistaMenu(this.components);
			this.panel1 = new Panel();
			this.openFileDialog1 = new OpenFileDialog();
			this.toolStrip.SuspendLayout();
			((ISupportInitialize)this.vistaMenu).BeginInit();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.mainMenu.MenuItems.AddRange(new MenuItem[]
			{
				this.menuItem1,
				this.menuItem2,
				this.menuItem4,
				this.menuItem3
			});
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new MenuItem[]
			{
				this.menuItemOpen,
				this.menuItem8,
				this.menuItemSave,
				this.menuItemSaveAs,
				this.menuItemSaveAll,
				this.menuItem9,
				this.menuItemExit
			});
			this.menuItem1.Text = "File";
			this.vistaMenu.SetImage(this.menuItemOpen, (Image)componentResourceManager.GetObject("menuItemOpen.Image"));
			this.menuItemOpen.Index = 0;
			this.menuItemOpen.Text = "Open";
			this.menuItemOpen.Click += new EventHandler(this.menuOpen);
			this.menuItem8.Index = 1;
			this.menuItem8.Text = "-";
			this.menuItemSave.Enabled = false;
			this.vistaMenu.SetImage(this.menuItemSave, (Image)componentResourceManager.GetObject("menuItemSave.Image"));
			this.menuItemSave.Index = 2;
			this.menuItemSave.Text = "Save";
			this.menuItemSaveAs.Enabled = false;
			this.menuItemSaveAs.Index = 3;
			this.menuItemSaveAs.Text = "Save As...";
			this.menuItemSaveAll.Enabled = false;
			this.vistaMenu.SetImage(this.menuItemSaveAll, (Image)componentResourceManager.GetObject("menuItemSaveAll.Image"));
			this.menuItemSaveAll.Index = 4;
			this.menuItemSaveAll.Text = "Save All";
			this.menuItem9.Index = 5;
			this.menuItem9.Text = "-";
			this.menuItemExit.Index = 6;
			this.menuItemExit.Text = "Exit";
			this.menuItem2.Index = 1;
			this.menuItem2.MergeOrder = 8;
			this.menuItem2.Text = "Tools";
			this.menuItem4.Index = 2;
			this.menuItem4.MergeOrder = 9;
			this.menuItem4.Text = "Window";
			this.menuItem3.Index = 3;
			this.menuItem3.MergeOrder = 10;
			this.menuItem3.Text = "Help";
			this.toolStrip.Items.AddRange(new ToolStripItem[]
			{
				this.toolStripButtonOpen,
				this.toolStripButtonSave,
				this.toolStripButtonSaveAll
			});
			this.toolStrip.Location = new Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new Size(812, 25);
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip1";
			this.toolStripButtonOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOpen.Image = (Image)componentResourceManager.GetObject("toolStripButtonOpen.Image");
			this.toolStripButtonOpen.ImageTransparentColor = Color.Magenta;
			this.toolStripButtonOpen.Name = "toolStripButtonOpen";
			this.toolStripButtonOpen.Size = new Size(23, 22);
			this.toolStripButtonOpen.Text = "toolStripButton1";
			this.toolStripButtonOpen.Click += new EventHandler(this.menuOpen);
			this.toolStripButtonSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSave.Enabled = false;
			this.toolStripButtonSave.Image = (Image)componentResourceManager.GetObject("toolStripButtonSave.Image");
			this.toolStripButtonSave.ImageTransparentColor = Color.Magenta;
			this.toolStripButtonSave.Name = "toolStripButtonSave";
			this.toolStripButtonSave.Size = new Size(23, 22);
			this.toolStripButtonSave.Text = "toolStripButton2";
			this.toolStripButtonSave.Click += new EventHandler(this.toolStripButtonSave_Click);
			this.toolStripButtonSaveAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSaveAll.Enabled = false;
			this.toolStripButtonSaveAll.Image = (Image)componentResourceManager.GetObject("toolStripButtonSaveAll.Image");
			this.toolStripButtonSaveAll.ImageTransparentColor = Color.Magenta;
			this.toolStripButtonSaveAll.Name = "toolStripButtonSaveAll";
			this.toolStripButtonSaveAll.Size = new Size(23, 22);
			this.toolStripButtonSaveAll.Text = "toolStripButton3";
			this.vistaMenu.ContainerControl = this;
			this.panel1.AutoSize = true;
			this.panel1.Controls.Add(this.toolStrip);
			this.panel1.Dock = DockStyle.Top;
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(812, 25);
			this.panel1.TabIndex = 2;
			this.openFileDialog1.DefaultExt = "sdat";
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "NITRO Sound Data(*.sdat)|*.sdat";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(812, 420);
			base.Controls.Add(this.panel1);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.IsMdiContainer = true;
			base.Menu = this.mainMenu;
			base.Name = "MainForm";
			this.Text = "DS Sound Studio";
			base.Load += new EventHandler(this.MainForm_Load);
			base.MdiChildActivate += new EventHandler(this.MainForm_MdiChildActivate);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			((ISupportInitialize)this.vistaMenu).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000080 RID: 128
		private IContainer components = null;

		// Token: 0x04000081 RID: 129
		private MainMenu mainMenu;

		// Token: 0x04000082 RID: 130
		private MenuItem menuItem1;

		// Token: 0x04000083 RID: 131
		private MenuItem menuItem2;

		// Token: 0x04000084 RID: 132
		private MenuItem menuItem4;

		// Token: 0x04000085 RID: 133
		private MenuItem menuItem3;

		// Token: 0x04000086 RID: 134
		private ToolStrip toolStrip;

		// Token: 0x04000087 RID: 135
		private ToolStripButton toolStripButtonOpen;

		// Token: 0x04000088 RID: 136
		private ToolStripButton toolStripButtonSave;

		// Token: 0x04000089 RID: 137
		private ToolStripButton toolStripButtonSaveAll;

		// Token: 0x0400008A RID: 138
		private MenuItem menuItemOpen;

		// Token: 0x0400008B RID: 139
		private MenuItem menuItemSave;

		// Token: 0x0400008C RID: 140
		private MenuItem menuItemSaveAll;

		// Token: 0x0400008D RID: 141
		private VistaMenu vistaMenu;

		// Token: 0x0400008E RID: 142
		private MenuItem menuItem8;

		// Token: 0x0400008F RID: 143
		private MenuItem menuItem9;

		// Token: 0x04000090 RID: 144
		private MenuItem menuItemExit;

		// Token: 0x04000091 RID: 145
		private MenuItem menuItemSaveAs;

		// Token: 0x04000092 RID: 146
		private Panel panel1;

		// Token: 0x04000093 RID: 147
		private OpenFileDialog openFileDialog1;
	}
}
