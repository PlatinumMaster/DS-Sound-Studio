using System.Text;
using LibEndianBinaryIO;

namespace LibDSSound.IO
{
	// Token: 0x02000028 RID: 40
	internal class BinaryBlockHeader
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00007244 File Offset: 0x00005444
		public BinaryBlockHeader(string Signature)
		{
			Kind = Signature;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00007256 File Offset: 0x00005456
		public BinaryBlockHeader(EndianBinaryReader er)
		{
			Kind = er.ReadString(Encoding.ASCII, 4);
			Size = er.ReadUInt32();
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000727F File Offset: 0x0000547F
		public void Write(EndianBinaryWriterEx er)
		{
			er.Write(Kind, Encoding.ASCII, false);
			er.Write(Size);
		}

		// Token: 0x040000FE RID: 254
		public string Kind;

		// Token: 0x040000FF RID: 255
		public uint Size;
	}
}
