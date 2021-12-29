using System;
using System.Runtime.InteropServices;

namespace DSSoundStudio.UI
{
	// Token: 0x02000010 RID: 16
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class MENUITEMINFO_T_RW
	{
		// Token: 0x0400006B RID: 107
		public int cbSize = Marshal.SizeOf(typeof(MENUITEMINFO_T_RW));

		// Token: 0x0400006C RID: 108
		public int fMask = 128;

		// Token: 0x0400006D RID: 109
		public int fType;

		// Token: 0x0400006E RID: 110
		public int fState;

		// Token: 0x0400006F RID: 111
		public int wID;

		// Token: 0x04000070 RID: 112
		public IntPtr hSubMenu = IntPtr.Zero;

		// Token: 0x04000071 RID: 113
		public IntPtr hbmpChecked = IntPtr.Zero;

		// Token: 0x04000072 RID: 114
		public IntPtr hbmpUnchecked = IntPtr.Zero;

		// Token: 0x04000073 RID: 115
		public IntPtr dwItemData = IntPtr.Zero;

		// Token: 0x04000074 RID: 116
		public IntPtr dwTypeData = IntPtr.Zero;

		// Token: 0x04000075 RID: 117
		public int cch;

		// Token: 0x04000076 RID: 118
		public IntPtr hbmpItem = IntPtr.Zero;
	}
}
