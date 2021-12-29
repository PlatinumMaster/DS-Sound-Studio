using System.IO;
using LibEndianBinaryIO;

namespace LibDSSound.IO
{
	// Token: 0x02000022 RID: 34
	public class SSAR
	{
		// Token: 0x0600008F RID: 143 RVA: 0x00006AEC File Offset: 0x00004CEC
		public SSAR(byte[] Data)
		{
			EndianBinaryReaderEx endianBinaryReaderEx = new EndianBinaryReaderEx(new MemoryStream(Data), Endianness.LittleEndian);
			FileHeader = new BinaryFileHeader(endianBinaryReaderEx);
			if (FileHeader.Signature != "SSAR")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "SSAR", 0L);
			}
			BlockHeader = new BinaryBlockHeader(endianBinaryReaderEx);
			if (BlockHeader.Kind != "DATA")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "DATA", 16L);
			}
			BaseOffset = endianBinaryReaderEx.ReadUInt32();
			NrSequences = endianBinaryReaderEx.ReadUInt32();
			Sequences = new SequenceInfo[NrSequences];
			int num = 0;
			while (num < (long)NrSequences)
			{
				Sequences[num] = new SequenceInfo(endianBinaryReaderEx);
				num++;
			}
			endianBinaryReaderEx.BaseStream.Position = BaseOffset;
			Data = endianBinaryReaderEx.ReadBytes((int)(endianBinaryReaderEx.BaseStream.Length - endianBinaryReaderEx.BaseStream.Position));
			endianBinaryReaderEx.Close();
		}

		// Token: 0x040000BF RID: 191
		private BinaryFileHeader FileHeader;

		// Token: 0x040000C0 RID: 192
		private BinaryBlockHeader BlockHeader;

		// Token: 0x040000C1 RID: 193
		public uint BaseOffset;

		// Token: 0x040000C2 RID: 194
		public uint NrSequences;

		// Token: 0x040000C3 RID: 195
		public SequenceInfo[] Sequences;

		// Token: 0x040000C4 RID: 196
		public byte[] Data;

		// Token: 0x02000023 RID: 35
		public class SequenceInfo
		{
			// Token: 0x06000090 RID: 144 RVA: 0x00006C1B File Offset: 0x00004E1B
			public SequenceInfo(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
			}

			// Token: 0x040000C5 RID: 197
			public uint Offset;

			// Token: 0x040000C6 RID: 198
			public ushort Bank;

			// Token: 0x040000C7 RID: 199
			public byte Volume;

			// Token: 0x040000C8 RID: 200
			public byte ChannelPriority;

			// Token: 0x040000C9 RID: 201
			public byte PlayerPriority;

			// Token: 0x040000CA RID: 202
			public byte PlayerNr;

			// Token: 0x040000CB RID: 203
			public ushort Reserved;
		}
	}
}
