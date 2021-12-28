using System;
using System.Runtime.InteropServices;

namespace DSSoundStudio.UI
{
	// Token: 0x02000011 RID: 17
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class MENUINFO
	{
		// Token: 0x04000077 RID: 119
		public int cbSize = Marshal.SizeOf(typeof(MENUINFO));

		// Token: 0x04000078 RID: 120
		public int fMask = 16;

		// Token: 0x04000079 RID: 121
		public int dwStyle = 67108864;

		// Token: 0x0400007A RID: 122
		public uint cyMax;

		// Token: 0x0400007B RID: 123
		public IntPtr hbrBack = IntPtr.Zero;

		// Token: 0x0400007C RID: 124
		public int dwContextHelpID;

		// Token: 0x0400007D RID: 125
		public IntPtr dwMenuData = IntPtr.Zero;
	}
}
