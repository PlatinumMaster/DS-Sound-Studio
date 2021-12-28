using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSSoundStudio.Util
{
	// Token: 0x02000019 RID: 25
	public class Win32Util
	{
		// Token: 0x06000077 RID: 119
		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern int GetWindowLong(IntPtr hWnd, int Index);

		// Token: 0x06000078 RID: 120
		[DllImport("user32")]
		public static extern uint RegisterWindowMessage(string message);

		// Token: 0x06000079 RID: 121
		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern int SetWindowLong(IntPtr hWnd, int Index, int Value);

		// Token: 0x0600007A RID: 122
		[DllImport("user32", ExactSpelling = true)]
		private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		// Token: 0x0600007B RID: 123
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

		// Token: 0x0600007C RID: 124 RVA: 0x00009940 File Offset: 0x00007B40
		public static void SetMDIBorderStyle(MdiClient mdiClient, BorderStyle value)
		{
			int num = GetWindowLong(mdiClient.Handle, -16);
			int num2 = GetWindowLong(mdiClient.Handle, -20);
			switch (value)
			{
			case BorderStyle.None:
				num &= -8388609;
				num2 &= -513;
				break;
			case BorderStyle.FixedSingle:
				num2 &= -513;
				num |= 8388608;
				break;
			case BorderStyle.Fixed3D:
				num2 |= 512;
				num &= -8388609;
				break;
			}
			SetWindowLong(mdiClient.Handle, -16, num);
			SetWindowLong(mdiClient.Handle, -20, num2);
			SetWindowPos(mdiClient.Handle, IntPtr.Zero, 0, 0, 0, 0, 567U);
		}

		// Token: 0x0400009C RID: 156
		private const int GWL_STYLE = -16;

		// Token: 0x0400009D RID: 157
		private const int GWL_EXSTYLE = -20;

		// Token: 0x0400009E RID: 158
		private const int WS_BORDER = 8388608;

		// Token: 0x0400009F RID: 159
		private const int WS_EX_CLIENTEDGE = 512;

		// Token: 0x040000A0 RID: 160
		private const uint SWP_NOSIZE = 1U;

		// Token: 0x040000A1 RID: 161
		private const uint SWP_NOMOVE = 2U;

		// Token: 0x040000A2 RID: 162
		private const uint SWP_NOZORDER = 4U;

		// Token: 0x040000A3 RID: 163
		private const uint SWP_NOREDRAW = 8U;

		// Token: 0x040000A4 RID: 164
		private const uint SWP_NOACTIVATE = 16U;

		// Token: 0x040000A5 RID: 165
		private const uint SWP_FRAMECHANGED = 32U;

		// Token: 0x040000A6 RID: 166
		private const uint SWP_SHOWWINDOW = 64U;

		// Token: 0x040000A7 RID: 167
		private const uint SWP_HIDEWINDOW = 128U;

		// Token: 0x040000A8 RID: 168
		private const uint SWP_NOCOPYBITS = 256U;

		// Token: 0x040000A9 RID: 169
		private const uint SWP_NOOWNERZORDER = 512U;

		// Token: 0x040000AA RID: 170
		private const uint SWP_NOSENDCHANGING = 1024U;
	}
}
