using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LibDSSound.Software;

namespace DSSoundStudio.UI
{
	// Token: 0x0200001C RID: 28
	public class ADSRGraph : UserControl
	{
		// Token: 0x06000087 RID: 135 RVA: 0x0000AB44 File Offset: 0x00008D44
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000AB7C File Offset: 0x00008D7C
		private void InitializeComponent()
		{
			SuspendLayout();
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			DoubleBuffered = true;
			Name = "ADSRGraph";
			Size = new Size(236, 232);
			Load += ADSRGraph_Load;
			Paint += ADSRGraph_Paint;
			ResumeLayout(false);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000AC07 File Offset: 0x00008E07
		public ADSRGraph()
		{
			InitializeComponent();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000AC20 File Offset: 0x00008E20
		private void ADSRGraph_Load(object sender, EventArgs e)
		{
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000AC24 File Offset: 0x00008E24
		private void ADSRGraph_Paint(object sender, PaintEventArgs e)
		{
			ExChannel exChannel = new ExChannel();
			exChannel.InitAlloc(null, null, 64);
			exChannel.SetAttack(127);
			exChannel.SetDecay(92);
			exChannel.SetSustain(0);
			exChannel.SetRelease(92);
			for (int i = 0; i < Width; i++)
			{
				int num = LibDSSound.Software.Util.DecibelSquareTable[exChannel.Velocity] + exChannel.UpdateEnvelope(true);
				e.Graphics.FillRectangle(Brushes.Black, i, (int)(num / -723f * Height), 1, 1);
			}
		}

		// Token: 0x040000B8 RID: 184
		private IContainer components = null;
	}
}
