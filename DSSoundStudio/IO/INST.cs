using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibDSSound.IO;
using LibDSSound.Software;
using LibEndianBinaryIO;
using LibEndianBinaryIO.Serialization;

namespace DSSoundStudio.IO
{
	// Token: 0x0200000B RID: 11
	public class INST
	{
		// Token: 0x06000044 RID: 68 RVA: 0x000078E8 File Offset: 0x00005AE8
		public INST(SBNK.Instrument Instrument, SWAR[] Waves)
		{
			Header = new INSTHeader();
			InstrumentData = new INSD(Instrument);
			if (InstrumentData.Type == InstData.InstType.Pcm)
			{
				WaveData = new WAVD(Waves[((SBNK.SimpleInstrumentParam)InstrumentData.Param).Param.Wave[1]].Waves[((SBNK.SimpleInstrumentParam)InstrumentData.Param).Param.Wave[0]]);
				((SBNK.SimpleInstrumentParam)InstrumentData.Param).Param.Wave[0] = 0;
				((SBNK.SimpleInstrumentParam)InstrumentData.Param).Param.Wave[1] = 0;
			}
			else if (InstrumentData.Type == InstData.InstType.DrumSet)
			{
				Dictionary<uint, Tuple<int, WaveData>> dictionary = new Dictionary<uint, Tuple<int, WaveData>>();
				int num = 0;
				foreach (InstData instData in ((SBNK.DrumSetParam)InstrumentData.Param).SubInstruments)
				{
					if (instData.Type == InstData.InstType.Pcm)
					{
						if (!dictionary.ContainsKey((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])))
						{
							dictionary.Add((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0]), new Tuple<int, WaveData>(num++, Waves[instData.Param.Wave[1]].Waves[instData.Param.Wave[0]]));
						}
						instData.Param.Wave[0] = (ushort)dictionary[(uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])].Item1;
						instData.Param.Wave[1] = 0;
					}
				}
				Tuple<int, WaveData>[] array = dictionary.Values.ToArray();
				WaveData[] array2 = new WaveData[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[array[j].Item1] = array[j].Item2;
				}
				WaveData = new WAVD(array2);
			}
			else if (InstrumentData.Type == InstData.InstType.KeySplit)
			{
				Dictionary<uint, Tuple<int, WaveData>> dictionary = new Dictionary<uint, Tuple<int, WaveData>>();
				int num = 0;
				foreach (InstData instData in ((SBNK.KeySplitParam)InstrumentData.Param).SubInstruments)
				{
					if (instData.Type == InstData.InstType.Pcm)
					{
						if (!dictionary.ContainsKey((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])))
						{
							dictionary.Add((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0]), new Tuple<int, WaveData>(num++, Waves[instData.Param.Wave[1]].Waves[instData.Param.Wave[0]]));
						}
						instData.Param.Wave[0] = (ushort)dictionary[(uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])].Item1;
						instData.Param.Wave[1] = 0;
					}
				}
				Tuple<int, WaveData>[] array = dictionary.Values.ToArray();
				WaveData[] array2 = new WaveData[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[array[j].Item1] = array[j].Item2;
				}
				WaveData = new WAVD(array2);
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00007CE8 File Offset: 0x00005EE8
		public INST(byte[] Data)
		{
			EndianBinaryReaderEx endianBinaryReaderEx = new EndianBinaryReaderEx(new MemoryStream(Data));
			Header = new INSTHeader(endianBinaryReaderEx);
			InstrumentData = new INSD(endianBinaryReaderEx);
			WaveData = new WAVD(endianBinaryReaderEx);
			endianBinaryReaderEx.Close();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00007D38 File Offset: 0x00005F38
		public byte[] Write()
		{
			MemoryStream memoryStream = new MemoryStream();
			EndianBinaryWriterEx endianBinaryWriterEx = new EndianBinaryWriterEx(memoryStream, Endianness.LittleEndian);
			endianBinaryWriterEx.BeginChunk(4);
			Header.Write(endianBinaryWriterEx);
			InstrumentData.Write(endianBinaryWriterEx);
			WaveData.Write(endianBinaryWriterEx);
			endianBinaryWriterEx.EndChunk();
			byte[] result = memoryStream.ToArray();
			endianBinaryWriterEx.Close();
			return result;
		}

		// Token: 0x0400005B RID: 91
		private INSTHeader Header;

		// Token: 0x0400005C RID: 92
		public INSD InstrumentData;

		// Token: 0x0400005D RID: 93
		public WAVD WaveData;

		// Token: 0x0200000C RID: 12
		public class INSTHeader
		{
			// Token: 0x06000047 RID: 71 RVA: 0x00007D9D File Offset: 0x00005F9D
			public INSTHeader()
			{
				Signature = "INST";
			}

			// Token: 0x06000048 RID: 72 RVA: 0x00007DB3 File Offset: 0x00005FB3
			public INSTHeader(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
			}

			// Token: 0x06000049 RID: 73 RVA: 0x00007DC6 File Offset: 0x00005FC6
			public void Write(EndianBinaryWriterEx er)
			{
				er.WriteObject(this);
			}

			// Token: 0x0400005E RID: 94
			[BinaryFixedSize(4)]
			[BinaryStringSignature("INST")]
			public string Signature;

			// Token: 0x0400005F RID: 95
			public uint FileSize;
		}

		// Token: 0x0200000D RID: 13
		public class INSD
		{
			// Token: 0x0600004A RID: 74 RVA: 0x00007DD4 File Offset: 0x00005FD4
			public INSD(SBNK.Instrument Instrument)
			{
				Signature = "INSD";
				Type = Instrument.Type;
				if (Type > InstData.InstType.Invalid && Type < InstData.InstType.DrumSet)
				{
					Param = new SBNK.SimpleInstrumentParam();
					((SBNK.SimpleInstrumentParam)Param).Param = new InstData.InstParam();
					((SBNK.SimpleInstrumentParam)Param).Param.Wave = new ushort[2];
					((SBNK.SimpleInstrumentParam)Param).Param.Wave[0] = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.Wave[0];
					((SBNK.SimpleInstrumentParam)Param).Param.Wave[1] = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.Wave[1];
					((SBNK.SimpleInstrumentParam)Param).Param.OriginalKey = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.OriginalKey;
					((SBNK.SimpleInstrumentParam)Param).Param.Attack = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.Attack;
					((SBNK.SimpleInstrumentParam)Param).Param.Decay = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.Decay;
					((SBNK.SimpleInstrumentParam)Param).Param.Sustain = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.Sustain;
					((SBNK.SimpleInstrumentParam)Param).Param.Release = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.Release;
					((SBNK.SimpleInstrumentParam)Param).Param.Pan = ((SBNK.SimpleInstrumentParam)Instrument.Param).Param.Pan;
				}
				else if (Type == InstData.InstType.DrumSet)
				{
					Param = new SBNK.DrumSetParam();
					((SBNK.DrumSetParam)Param).Min = ((SBNK.DrumSetParam)Instrument.Param).Min;
					((SBNK.DrumSetParam)Param).Max = ((SBNK.DrumSetParam)Instrument.Param).Max;
					((SBNK.DrumSetParam)Param).SubInstruments = new InstData[((SBNK.DrumSetParam)Instrument.Param).SubInstruments.Length];
					for (int i = 0; i < ((SBNK.DrumSetParam)Instrument.Param).SubInstruments.Length; i++)
					{
						((SBNK.DrumSetParam)Param).SubInstruments[i] = new InstData();
						((SBNK.DrumSetParam)Param).SubInstruments[i].Type = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Type;
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Wave = new ushort[2];
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Wave[0] = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.Wave[0];
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Wave[1] = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.Wave[1];
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.OriginalKey = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.OriginalKey;
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Attack = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.Attack;
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Decay = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.Decay;
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Sustain = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.Sustain;
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Release = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.Release;
						((SBNK.DrumSetParam)Param).SubInstruments[i].Param.Pan = ((SBNK.DrumSetParam)Instrument.Param).SubInstruments[i].Param.Pan;
					}
				}
				else if (Type == InstData.InstType.KeySplit)
				{
					Param = new SBNK.KeySplitParam();
					((SBNK.KeySplitParam)Param).Key = new byte[8];
					Array.Copy(((SBNK.KeySplitParam)Instrument.Param).Key, ((SBNK.KeySplitParam)Param).Key, 8);
					((SBNK.KeySplitParam)Param).SubInstruments = new InstData[((SBNK.KeySplitParam)Instrument.Param).SubInstruments.Length];
					for (int i = 0; i < ((SBNK.KeySplitParam)Instrument.Param).SubInstruments.Length; i++)
					{
						((SBNK.KeySplitParam)Param).SubInstruments[i] = new InstData();
						((SBNK.KeySplitParam)Param).SubInstruments[i].Type = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Type;
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Wave = new ushort[2];
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Wave[0] = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.Wave[0];
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Wave[1] = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.Wave[1];
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.OriginalKey = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.OriginalKey;
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Attack = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.Attack;
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Decay = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.Decay;
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Sustain = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.Sustain;
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Release = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.Release;
						((SBNK.KeySplitParam)Param).SubInstruments[i].Param.Pan = ((SBNK.KeySplitParam)Instrument.Param).SubInstruments[i].Param.Pan;
					}
				}
			}

			// Token: 0x0600004B RID: 75 RVA: 0x00008580 File Offset: 0x00006780
			public INSD(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
				er.ReadBytes(3);
				if (Type > InstData.InstType.Invalid && Type < InstData.InstType.DrumSet)
				{
					Param = new SBNK.SimpleInstrumentParam(er);
				}
				else if (Type == InstData.InstType.DrumSet)
				{
					Param = new SBNK.DrumSetParam(er);
				}
				else if (Type == InstData.InstType.KeySplit)
				{
					Param = new SBNK.KeySplitParam(er);
				}
			}

			// Token: 0x0600004C RID: 76 RVA: 0x00008610 File Offset: 0x00006810
			public void Write(EndianBinaryWriterEx er)
			{
				er.BeginChunk(4);
				er.WriteObject(this);
				er.Write(new byte[3], 0, 3);
				if (Param != null)
				{
					Param.Write(er);
				}
				er.EndChunk();
			}

			// Token: 0x04000060 RID: 96
			[BinaryFixedSize(4)]
			[BinaryStringSignature("INSD")]
			public string Signature;

			// Token: 0x04000061 RID: 97
			public uint SectionSize;

			// Token: 0x04000062 RID: 98
			public InstData.InstType Type;

			// Token: 0x04000063 RID: 99
			[BinaryIgnore]
			public SBNK.InstrumentParam Param;
		}

		// Token: 0x0200000E RID: 14
		public class WAVD
		{
			// Token: 0x0600004D RID: 77 RVA: 0x0000865E File Offset: 0x0000685E
			public WAVD(params WaveData[] Waves)
			{
				Signature = "WAVD";
				this.Waves = Waves;
			}

			// Token: 0x0600004E RID: 78 RVA: 0x0000867C File Offset: 0x0000687C
			public WAVD(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
				Offsets = er.ReadUInt32s((int)NrWaves);
				Waves = new WaveData[NrWaves];
				int num = 0;
				while (num < NrWaves)
				{
					er.BaseStream.Position = Offsets[num];
					Waves[num] = new WaveData(er);
					num++;
				}
			}

			// Token: 0x0600004F RID: 79 RVA: 0x000086FC File Offset: 0x000068FC
			public void Write(EndianBinaryWriterEx er)
			{
				er.BeginChunk(4);
				er.Write(Signature, Encoding.ASCII, false);
				er.Write(0U);
				er.Write((uint)Waves.Length);
				long position = er.BaseStream.Position;
				er.Write(new uint[Waves.Length], 0, Waves.Length);
				for (int i = 0; i < Waves.Length; i++)
				{
					long position2 = er.BaseStream.Position;
					er.BaseStream.Position = position + i * 4;
					er.Write((uint)position2);
					er.BaseStream.Position = position2;
					Waves[i].Write(er);
				}
				er.EndChunk();
			}

			// Token: 0x04000064 RID: 100
			[BinaryStringSignature("WAVD")]
			[BinaryFixedSize(4)]
			public string Signature;

			// Token: 0x04000065 RID: 101
			public uint SectionSize;

			// Token: 0x04000066 RID: 102
			public uint NrWaves;

			// Token: 0x04000067 RID: 103
			[BinaryIgnore]
			public uint[] Offsets;

			// Token: 0x04000068 RID: 104
			[BinaryIgnore]
			public WaveData[] Waves;
		}
	}
}
