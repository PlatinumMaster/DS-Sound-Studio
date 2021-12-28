using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DSSoundStudio.UI
{
	// Token: 0x02000014 RID: 20
	public class PianoControl : UserControl
	{
		// Token: 0x0600005D RID: 93 RVA: 0x0000936C File Offset: 0x0000756C
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000093A4 File Offset: 0x000075A4
		private void InitializeComponent()
		{
			SuspendLayout();
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			DoubleBuffered = true;
			Name = "PianoControl";
			Size = new Size(401, 197);
			Paint += PianoControl_Paint;
			MouseClick += PianoControl_MouseClick;
			MouseDown += PianoControl_MouseDown;
			MouseMove += PianoControl_MouseMove;
			MouseUp += PianoControl_MouseUp;
			Resize += PianoControl_Resize;
			ResumeLayout(false);
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600005F RID: 95 RVA: 0x0000947C File Offset: 0x0000767C
		// (remove) Token: 0x06000060 RID: 96 RVA: 0x000094B8 File Offset: 0x000076B8
		public event NoteDownEventHandler NoteDown;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000061 RID: 97 RVA: 0x000094F4 File Offset: 0x000076F4
		// (remove) Token: 0x06000062 RID: 98 RVA: 0x00009530 File Offset: 0x00007730
		public event NoteUpEventHandler NoteUp;

		// Token: 0x06000063 RID: 99 RVA: 0x000095A4 File Offset: 0x000077A4
		public PianoControl()
		{
			InitializeComponent();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00009618 File Offset: 0x00007818
		private void PianoControl_Paint(object sender, PaintEventArgs e)
		{
			float num = Width / 75f;
			int num2 = 0;
			for (int i = 0; i < 128; i++)
			{
				int num3 = i % 12;
				int num4 = i / 12;
				if (!blackkey[num3])
				{
					e.Graphics.FillRectangle(i == SelKey ? Brushes.Blue : Brushes.White, num2 * num, 0f, num, Height);
					e.Graphics.DrawRectangle(Pens.Black, num2 * num, 0f, num, Height);
					num2++;
				}
			}
			num2 = 0;
			for (int i = 0; i < 128; i++)
			{
				int num3 = i % 12;
				int num4 = i / 12;
				if (!blackkey[num3])
				{
					num2++;
				}
				else
				{
					e.Graphics.FillRectangle(i == SelKey ? Brushes.Blue : Brushes.Black, num2 * num - num / 4f, 0f, num / 2f, Height / 2);
				}
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00009743 File Offset: 0x00007943
		private void PianoControl_Resize(object sender, EventArgs e)
		{
			Invalidate();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000974D File Offset: 0x0000794D
		private void PianoControl_MouseClick(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00009750 File Offset: 0x00007950
		private void PianoControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				SelKey = GetKeyFromPos(e.Location);
				Invalidate();
				if (NoteDown != null)
				{
					NoteDown(SelKey);
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000097AC File Offset: 0x000079AC
		private int GetKeyFromPos(Point Location)
		{
			float num = Width / 75f;
			float num2 = Location.X / num;
			int num3 = (int)Math.Floor(num2);
			int num4 = num3 % 7;
			int num5 = num3 / 7;
			int num6 = 0;
			if (Location.Y < Height / 2 && num2 - num3 > 0.75f && simplenotehasblack[num4])
			{
				num6 = 1;
			}
			else if (num4 != 0 && Location.Y < Height / 2 && num2 - num3 < 0.25f && simplenotehasblack[num4 - 1])
			{
				num6 = -1;
			}
			int num7 = num5 * 12 + simplenotenumber[num4] + num6;
			if (num7 < 0)
			{
				num7 = 0;
			}
			else if (num7 > 127)
			{
				num7 = 127;
			}
			return num7;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00009898 File Offset: 0x00007A98
		private void PianoControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (NoteUp != null)
				{
					NoteUp(SelKey);
				}
				SelKey = -1;
				Invalidate();
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000098E9 File Offset: 0x00007AE9
		private void PianoControl_MouseMove(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x04000094 RID: 148
		private IContainer components = null;

		// Token: 0x04000097 RID: 151
		private readonly bool[] blackkey = {
			false,
			true,
			false,
			true,
			false,
			false,
			true,
			false,
			true,
			false,
			true,
			false
		};

		// Token: 0x04000098 RID: 152
		private readonly bool[] simplenotehasblack = {
			true,
			true,
			false,
			true,
			true,
			true,
			false
		};

		// Token: 0x04000099 RID: 153
		private readonly int[] simplenotenumber = {
			0,
			2,
			4,
			5,
			7,
			9,
			11
		};

		// Token: 0x0400009A RID: 154
		private int SelKey = -1;

		// Token: 0x02000015 RID: 21
		// (Invoke) Token: 0x0600006C RID: 108
		public delegate void NoteDownEventHandler(int key);

		// Token: 0x02000016 RID: 22
		// (Invoke) Token: 0x06000070 RID: 112
		public delegate void NoteUpEventHandler(int key);
	}
}
