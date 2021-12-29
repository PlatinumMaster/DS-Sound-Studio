using System.IO;
using LibDSSound.Software;
using LibEndianBinaryIO;

namespace LibDSSound.IO
{
	// Token: 0x02000029 RID: 41
	public class SBNK
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x000072A4 File Offset: 0x000054A4
		public SBNK(byte[] Data)
		{
			EndianBinaryReaderEx endianBinaryReaderEx = new EndianBinaryReaderEx(new MemoryStream(Data), Endianness.LittleEndian);
			FileHeader = new BinaryFileHeader(endianBinaryReaderEx);
			if (FileHeader.Signature != "SBNK")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "SBNK", 0L);
			}
			BlockHeader = new BinaryBlockHeader(endianBinaryReaderEx);
			if (BlockHeader.Kind != "DATA")
			{
				throw new SignatureNotCorrectException(FileHeader.Signature, "DATA", 16L);
			}
			endianBinaryReaderEx.ReadBytes(32);
			NrInstruments = endianBinaryReaderEx.ReadUInt32();
			Instruments = new Instrument[NrInstruments];
			int num = 0;
			while (num < (long)NrInstruments)
			{
				Instruments[num] = new Instrument(endianBinaryReaderEx);
				num++;
			}
			endianBinaryReaderEx.Close();
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000073AC File Offset: 0x000055AC
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
			endianBinaryWriterEx.Write(new byte[32], 0, 32);
			endianBinaryWriterEx.Write((uint)Instruments.Length);
			long position = endianBinaryWriterEx.BaseStream.Position;
			for (int i = 0; i < Instruments.Length; i++)
			{
				Instruments[i].Write(endianBinaryWriterEx);
			}
			for (int i = 0; i < Instruments.Length; i++)
			{
				if (Instruments[i].Type != InstData.InstType.Invalid)
				{
					long position2 = endianBinaryWriterEx.BaseStream.Position;
					endianBinaryWriterEx.BaseStream.Position = position + 4 * i + 1L;
					endianBinaryWriterEx.Write((ushort)position2);
					endianBinaryWriterEx.BaseStream.Position = position2;
					Instruments[i].Param.Write(endianBinaryWriterEx);
				}
			}
			endianBinaryWriterEx.EndChunk();
			endianBinaryWriterEx.EndChunk();
			byte[] result = memoryStream.ToArray();
			endianBinaryWriterEx.Close();
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00007518 File Offset: 0x00005718
		public void AssignWaveArc(int index, SWAR waveArc)
		{
			if (WaveArcLink[index] == null)
			{
				WaveArcLink[index] = new WaveArcLink();
			}
			if (WaveArcLink[index].WaveArc != null)
			{
				if (WaveArcLink[index].WaveArc == waveArc)
				{
					return;
				}
				if (WaveArcLink[index] == WaveArcLink[index].WaveArc.TopLink)
				{
					WaveArcLink[index].WaveArc.TopLink = WaveArcLink[index].Next;
				}
				else
				{
					WaveArcLink waveArcLink;
					for (waveArcLink = WaveArcLink[index].WaveArc.TopLink; waveArcLink != null; waveArcLink = waveArcLink.Next)
					{
						if (WaveArcLink[index] == waveArcLink.Next)
						{
							break;
						}
					}
					waveArcLink.Next = WaveArcLink[index].Next;
				}
			}
			WaveArcLink topLink = waveArc.TopLink;
			waveArc.TopLink = WaveArcLink[index];
			WaveArcLink[index].Next = topLink;
			WaveArcLink[index].WaveArc = waveArc;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00007648 File Offset: 0x00005848
		public InstData ReadInstData(int prgNo, int key)
		{
			InstData result;
			if (prgNo < 0)
			{
				result = null;
			}
			else if (prgNo >= (long)NrInstruments)
			{
				result = null;
			}
			else
			{
				InstData instData = new InstData();
				instData.Type = Instruments[prgNo].Type;
				InstData.InstType type = instData.Type;
				switch (type)
				{
				case InstData.InstType.Pcm:
				case InstData.InstType.Psg:
				case InstData.InstType.Noise:
				case InstData.InstType.DirectPcm:
				case InstData.InstType.Null:
					instData.Param.Wave = new ushort[2];
					instData.Param.Wave[0] = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.Wave[0];
					instData.Param.Wave[1] = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.Wave[1];
					instData.Param.OriginalKey = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.OriginalKey;
					instData.Param.Attack = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.Attack;
					instData.Param.Decay = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.Decay;
					instData.Param.Sustain = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.Sustain;
					instData.Param.Release = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.Release;
					instData.Param.Pan = ((SimpleInstrumentParam)Instruments[prgNo].Param).Param.Pan;
					result = instData;
					break;
				default:
					switch (type)
					{
					case InstData.InstType.DrumSet:
						if (key < ((DrumSetParam)Instruments[prgNo].Param).Min || key > ((DrumSetParam)Instruments[prgNo].Param).Max)
						{
							result = null;
						}
						else
						{
							InstData instData2 = ((DrumSetParam)Instruments[prgNo].Param).SubInstruments[key - ((DrumSetParam)Instruments[prgNo].Param).Min];
							instData.Type = instData2.Type;
							instData.Param.Wave = new ushort[2];
							instData.Param.Wave[0] = instData2.Param.Wave[0];
							instData.Param.Wave[1] = instData2.Param.Wave[1];
							instData.Param.OriginalKey = instData2.Param.OriginalKey;
							instData.Param.Attack = instData2.Param.Attack;
							instData.Param.Decay = instData2.Param.Decay;
							instData.Param.Sustain = instData2.Param.Sustain;
							instData.Param.Release = instData2.Param.Release;
							instData.Param.Pan = instData2.Param.Pan;
							result = instData;
						}
						break;
					case InstData.InstType.KeySplit:
					{
						int num = 0;
						while (key > ((KeySplitParam)Instruments[prgNo].Param).Key[num])
						{
							num++;
							if (num >= 8)
							{
								return null;
							}
						}
						InstData instData2 = ((KeySplitParam)Instruments[prgNo].Param).SubInstruments[num];
						instData.Type = instData2.Type;
						instData.Param.Wave = new ushort[2];
						instData.Param.Wave[0] = instData2.Param.Wave[0];
						instData.Param.Wave[1] = instData2.Param.Wave[1];
						instData.Param.OriginalKey = instData2.Param.OriginalKey;
						instData.Param.Attack = instData2.Param.Attack;
						instData.Param.Decay = instData2.Param.Decay;
						instData.Param.Sustain = instData2.Param.Sustain;
						instData.Param.Release = instData2.Param.Release;
						instData.Param.Pan = instData2.Param.Pan;
						result = instData;
						break;
					}
					default:
						result = null;
						break;
					}
					break;
				}
			}
			return result;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00007ABC File Offset: 0x00005CBC
		internal WaveData GetWaveData(int waveArc, int wave)
		{
			WaveData result;
			if (WaveArcLink[waveArc] != null && WaveArcLink[waveArc].WaveArc != null && wave < (long)WaveArcLink[waveArc].WaveArc.WaveCount)
			{
				result = WaveArcLink[waveArc].WaveArc.GetWaveDataAddress(wave);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000100 RID: 256
		private BinaryFileHeader FileHeader;

		// Token: 0x04000101 RID: 257
		private BinaryBlockHeader BlockHeader;

		// Token: 0x04000102 RID: 258
		internal WaveArcLink[] WaveArcLink = new WaveArcLink[4];

		// Token: 0x04000103 RID: 259
		public uint NrInstruments;

		// Token: 0x04000104 RID: 260
		public Instrument[] Instruments;

		// Token: 0x0200002A RID: 42
		public class Instrument
		{
			// Token: 0x060000A9 RID: 169 RVA: 0x00007B20 File Offset: 0x00005D20
			public Instrument(EndianBinaryReaderEx er)
			{
				Type = (InstData.InstType)er.ReadByte();
				Offset = er.ReadUInt16();
				er.ReadByte();
				long position = er.BaseStream.Position;
				er.BaseStream.Position = Offset;
				if (Type > InstData.InstType.Invalid && Type < InstData.InstType.DrumSet)
				{
					Param = new SimpleInstrumentParam(er);
				}
				else if (Type == InstData.InstType.DrumSet)
				{
					Param = new DrumSetParam(er);
				}
				else if (Type == InstData.InstType.KeySplit)
				{
					Param = new KeySplitParam(er);
				}
				er.BaseStream.Position = position;
			}

			// Token: 0x060000AA RID: 170 RVA: 0x00007BE8 File Offset: 0x00005DE8
			public void Write(EndianBinaryWriterEx er)
			{
				er.Write((byte)Type);
				er.Write(0);
				er.Write(0);
			}

			// Token: 0x04000105 RID: 261
			public InstData.InstType Type;

			// Token: 0x04000106 RID: 262
			public ushort Offset;

			// Token: 0x04000107 RID: 263
			public InstrumentParam Param;
		}

		// Token: 0x0200002B RID: 43
		public abstract class InstrumentParam
		{
			// Token: 0x060000AB RID: 171
			public abstract void Write(EndianBinaryWriterEx er);
		}

		// Token: 0x0200002C RID: 44
		public class SimpleInstrumentParam : InstrumentParam
		{
			// Token: 0x060000AD RID: 173 RVA: 0x00007C10 File Offset: 0x00005E10
			public SimpleInstrumentParam()
			{
			}

			// Token: 0x060000AE RID: 174 RVA: 0x00007C1B File Offset: 0x00005E1B
			public SimpleInstrumentParam(EndianBinaryReaderEx er)
			{
				Param = new InstData.InstParam(er);
			}

			// Token: 0x060000AF RID: 175 RVA: 0x00007C32 File Offset: 0x00005E32
			public override void Write(EndianBinaryWriterEx er)
			{
				Param.Write(er);
			}

			// Token: 0x04000108 RID: 264
			public InstData.InstParam Param;
		}

		// Token: 0x0200002D RID: 45
		public class DrumSetParam : InstrumentParam
		{
			// Token: 0x060000B0 RID: 176 RVA: 0x00007C42 File Offset: 0x00005E42
			public DrumSetParam()
			{
			}

			// Token: 0x060000B1 RID: 177 RVA: 0x00007C50 File Offset: 0x00005E50
			public DrumSetParam(EndianBinaryReaderEx er)
			{
				Min = er.ReadByte();
				Max = er.ReadByte();
				SubInstruments = new InstData[Max - Min + 1];
				for (int i = 0; i < SubInstruments.Length; i++)
				{
					SubInstruments[i] = new InstData(er);
				}
			}

			// Token: 0x060000B2 RID: 178 RVA: 0x00007CC0 File Offset: 0x00005EC0
			public override void Write(EndianBinaryWriterEx er)
			{
				er.Write(Min);
				er.Write(Max);
				for (int i = 0; i < SubInstruments.Length; i++)
				{
					SubInstruments[i].Write(er);
				}
			}

			// Token: 0x04000109 RID: 265
			public byte Min;

			// Token: 0x0400010A RID: 266
			public byte Max;

			// Token: 0x0400010B RID: 267
			public InstData[] SubInstruments;
		}

		// Token: 0x0200002E RID: 46
		public class KeySplitParam : InstrumentParam
		{
			// Token: 0x060000B3 RID: 179 RVA: 0x00007D10 File Offset: 0x00005F10
			public KeySplitParam()
			{
			}

			// Token: 0x060000B4 RID: 180 RVA: 0x00007D1C File Offset: 0x00005F1C
			public KeySplitParam(EndianBinaryReaderEx er)
			{
				Key = er.ReadBytes(8);
				int num = 0;
				for (int i = 0; i < 8; i++)
				{
					num++;
					if (Key[i] == 0)
					{
						break;
					}
				}
				SubInstruments = new InstData[num];
				for (int i = 0; i < num; i++)
				{
					SubInstruments[i] = new InstData(er);
				}
			}

			// Token: 0x060000B5 RID: 181 RVA: 0x00007D98 File Offset: 0x00005F98
			public override void Write(EndianBinaryWriterEx er)
			{
				er.Write(Key, 0, 8);
				for (int i = 0; i < SubInstruments.Length; i++)
				{
					SubInstruments[i].Write(er);
				}
			}

			// Token: 0x0400010C RID: 268
			public byte[] Key;

			// Token: 0x0400010D RID: 269
			public InstData[] SubInstruments;
		}
	}
}
