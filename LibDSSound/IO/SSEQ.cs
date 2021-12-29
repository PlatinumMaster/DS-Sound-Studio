using System.IO;
using LibEndianBinaryIO;

namespace LibDSSound.IO
{
	// Token: 0x0200002F RID: 47
	public class SSEQ
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x00007DE0 File Offset: 0x00005FE0
		public SSEQ(byte[] Data)
		{
			EndianBinaryReader endianBinaryReader = new EndianBinaryReader(new MemoryStream(Data), Endianness.LittleEndian);
			FileHeader = new BinaryFileHeader(endianBinaryReader);
			if (FileHeader.Signature != "SSEQ")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "SSEQ", 0L);
			}
			BlockHeader = new BinaryBlockHeader(endianBinaryReader);
			if (BlockHeader.Kind != "DATA")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "DATA", 16L);
			}
			BaseOffset = endianBinaryReader.ReadUInt32();
			endianBinaryReader.BaseStream.Position = (long)BaseOffset;
			this.Data = endianBinaryReader.ReadBytes((int)(FileHeader.FileSize - BaseOffset));
			endianBinaryReader.Close();
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00007EC8 File Offset: 0x000060C8
		public byte[] Write()
		{
			MemoryStream memoryStream = new MemoryStream();
			EndianBinaryWriterEx endianBinaryWriterEx = new EndianBinaryWriterEx(memoryStream, Endianness.LittleEndian);
			endianBinaryWriterEx.BeginChunk(8);
			FileHeader.FileSize = 0U;
			FileHeader.DataBlocks = 1;
			FileHeader.Write(endianBinaryWriterEx);
			endianBinaryWriterEx.BeginChunk(4);
			BlockHeader.Size = 0U;
			BlockHeader.Write(endianBinaryWriterEx);
			endianBinaryWriterEx.Write((uint)(endianBinaryWriterEx.BaseStream.Position + 4L));
			endianBinaryWriterEx.Write(Data, 0, Data.Length);
			endianBinaryWriterEx.EndChunk();
			endianBinaryWriterEx.EndChunk();
			byte[] result = memoryStream.ToArray();
			endianBinaryWriterEx.Close();
			return result;
		}

		// Token: 0x0400010E RID: 270
		private BinaryFileHeader FileHeader;

		// Token: 0x0400010F RID: 271
		private BinaryBlockHeader BlockHeader;

		// Token: 0x04000110 RID: 272
		private uint BaseOffset;

		// Token: 0x04000111 RID: 273
		public byte[] Data;
	}
}
