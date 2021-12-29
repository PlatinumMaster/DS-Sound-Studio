using System.IO;
using LibDSSound.Software;
using LibEndianBinaryIO;

namespace LibDSSound.IO
{
	// Token: 0x02000005 RID: 5
	public class SWAR
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00004AA4 File Offset: 0x00002CA4
		public SWAR()
		{
			FileHeader = new BinaryFileHeader("SWAR", 1);
			BlockHeader = new BinaryBlockHeader("DATA");
			WaveCount = 0U;
			WaveOffset = new uint[0];
			Waves = new WaveData[0];
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00004AFC File Offset: 0x00002CFC
		public SWAR(byte[] Data)
		{
			EndianBinaryReaderEx endianBinaryReaderEx = new EndianBinaryReaderEx(new MemoryStream(Data), Endianness.LittleEndian);
			FileHeader = new BinaryFileHeader(endianBinaryReaderEx);
			if (FileHeader.Signature != "SWAR")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "SWAR", 0L);
			}
			BlockHeader = new BinaryBlockHeader(endianBinaryReaderEx);
			if (BlockHeader.Kind != "DATA")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "DATA", 16L);
			}
			endianBinaryReaderEx.ReadUInt32();
			Reserved = endianBinaryReaderEx.ReadUInt32s(7);
			WaveCount = endianBinaryReaderEx.ReadUInt32();
			WaveOffset = endianBinaryReaderEx.ReadUInt32s((int)WaveCount);
			Waves = new WaveData[WaveCount];
			int num = 0;
			while (num < (long)WaveCount)
			{
				endianBinaryReaderEx.BaseStream.Position = WaveOffset[num];
				Waves[num] = new WaveData(endianBinaryReaderEx);
				num++;
			}
			endianBinaryReaderEx.Close();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00004C28 File Offset: 0x00002E28
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
			endianBinaryWriterEx.Write(0U);
			endianBinaryWriterEx.Write(new uint[7], 0, 7);
			endianBinaryWriterEx.Write((uint)Waves.Length);
			long position = endianBinaryWriterEx.BaseStream.Position;
			endianBinaryWriterEx.Write(new uint[Waves.Length], 0, Waves.Length);
			for (int i = 0; i < Waves.Length; i++)
			{
				long position2 = endianBinaryWriterEx.BaseStream.Position;
				endianBinaryWriterEx.BaseStream.Position = position + 4 * i;
				endianBinaryWriterEx.Write((uint)position2);
				endianBinaryWriterEx.BaseStream.Position = position2;
				Waves[i].Write(endianBinaryWriterEx);
			}
			endianBinaryWriterEx.EndChunk();
			endianBinaryWriterEx.EndChunk();
			byte[] result = memoryStream.ToArray();
			endianBinaryWriterEx.Close();
			return result;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004D6C File Offset: 0x00002F6C
		public WaveData GetWaveDataAddress(int wave)
		{
			return Waves[wave];
		}

		// Token: 0x0400001E RID: 30
		private BinaryFileHeader FileHeader;

		// Token: 0x0400001F RID: 31
		private BinaryBlockHeader BlockHeader;

		// Token: 0x04000020 RID: 32
		internal WaveArcLink TopLink;

		// Token: 0x04000021 RID: 33
		private uint[] Reserved;

		// Token: 0x04000022 RID: 34
		public uint WaveCount;

		// Token: 0x04000023 RID: 35
		public uint[] WaveOffset;

		// Token: 0x04000024 RID: 36
		public WaveData[] Waves;
	}
}
