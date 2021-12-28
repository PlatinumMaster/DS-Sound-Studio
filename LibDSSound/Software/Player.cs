using LibDSSound.IO;

namespace LibDSSound.Software
{
	// Token: 0x02000004 RID: 4
	public class Player
	{
		// Token: 0x04000012 RID: 18
		public bool Active;

		// Token: 0x04000013 RID: 19
		public bool Prepared;

		// Token: 0x04000014 RID: 20
		public bool Paused;

		// Token: 0x04000015 RID: 21
		public byte MyNo;

		// Token: 0x04000016 RID: 22
		public byte Priority;

		// Token: 0x04000017 RID: 23
		public byte Volume;

		// Token: 0x04000018 RID: 24
		public short ExtendedFader;

		// Token: 0x04000019 RID: 25
		public byte[] Tracks = new byte[16];

		// Token: 0x0400001A RID: 26
		public ushort Tempo;

		// Token: 0x0400001B RID: 27
		public ushort TempoRatio;

		// Token: 0x0400001C RID: 28
		public ushort TempoCounter;

		// Token: 0x0400001D RID: 29
		public SBNK Bank;
	}
}
