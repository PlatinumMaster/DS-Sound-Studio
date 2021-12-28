using System.Text;
using LibEndianBinaryIO;

namespace LibDSSound.IO
{
	// Token: 0x02000026 RID: 38
	internal class BinaryFileHeader
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00006CD6 File Offset: 0x00004ED6
		public BinaryFileHeader(string Signature, int NrBlocks)
		{
			this.Signature = Signature;
			ByteOrder = 65279;
			Version = 256;
			HeaderSize = 16;
			DataBlocks = (ushort)NrBlocks;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006D10 File Offset: 0x00004F10
		public BinaryFileHeader(EndianBinaryReader er)
		{
			Signature = er.ReadString(Encoding.ASCII, 4);
			ByteOrder = er.ReadUInt16();
			Version = er.ReadUInt16();
			FileSize = er.ReadUInt32();
			HeaderSize = er.ReadUInt16();
			DataBlocks = er.ReadUInt16();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006D74 File Offset: 0x00004F74
		public void Write(EndianBinaryWriterEx er)
		{
			er.Write(Signature, Encoding.ASCII, false);
			er.Write(ByteOrder);
			er.Write(Version);
			er.Write(0U);
			er.Write(16);
			er.Write(DataBlocks);
		}

		// Token: 0x040000D4 RID: 212
		public string Signature;

		// Token: 0x040000D5 RID: 213
		public ushort ByteOrder;

		// Token: 0x040000D6 RID: 214
		public ushort Version;

		// Token: 0x040000D7 RID: 215
		public uint FileSize;

		// Token: 0x040000D8 RID: 216
		public ushort HeaderSize;

		// Token: 0x040000D9 RID: 217
		public ushort DataBlocks;
	}
}
