using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibEndianBinaryIO;
using LibEndianBinaryIO.Serialization;

namespace LibDSSound.IO
{
	// Token: 0x02000006 RID: 6
	public class SDAT
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00004D88 File Offset: 0x00002F88
		public SDAT(byte[] Data)
		{
			EndianBinaryReaderEx endianBinaryReaderEx = new EndianBinaryReaderEx(new MemoryStream(Data), Endianness.LittleEndian);
			try
			{
				Header = new SDATHeader(endianBinaryReaderEx);
				if (Header.SYMBOffset != 0U && Header.SYMBLength != 0U)
				{
					endianBinaryReaderEx.BaseStream.Position = Header.SYMBOffset;
					SymbolBlock = new SYMB(endianBinaryReaderEx);
				}
				endianBinaryReaderEx.BaseStream.Position = Header.INFOOffset;
				InfoBlock = new INFO(endianBinaryReaderEx);
				endianBinaryReaderEx.BaseStream.Position = Header.FATOffset;
				FileAllocationTable = new FAT(endianBinaryReaderEx);
			}
			finally
			{
				endianBinaryReaderEx.Close();
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004E68 File Offset: 0x00003068
		public byte[] Write()
		{
			MemoryStream memoryStream = new MemoryStream();
			EndianBinaryWriterEx endianBinaryWriterEx = new EndianBinaryWriterEx(memoryStream, Endianness.LittleEndian);
			endianBinaryWriterEx.BeginChunk(8);
			Header.HeaderSize = 64;
			Header.SYMBOffset = 0U;
			Header.SYMBLength = 0U;
			Header.INFOOffset = 0U;
			Header.INFOLength = 0U;
			Header.FATOffset = 0U;
			Header.FATLength = 0U;
			Header.FILEOffset = 0U;
			Header.FILELength = 0U;
			Header.NrBlocks = (ushort) (SymbolBlock != null ? 4 : 3);
			Header.Write(endianBinaryWriterEx);
			long position;
			long position2;
			if (SymbolBlock != null)
			{
				endianBinaryWriterEx.WriteCurposRelative(16);
				position = endianBinaryWriterEx.BaseStream.Position;
				SymbolBlock.Write(endianBinaryWriterEx);
				position2 = endianBinaryWriterEx.BaseStream.Position;
				endianBinaryWriterEx.BaseStream.Position = 20L;
				endianBinaryWriterEx.Write((uint)(position2 - position));
				endianBinaryWriterEx.BaseStream.Position = position2;
			}
			endianBinaryWriterEx.WriteCurposRelative(24);
			position = endianBinaryWriterEx.BaseStream.Position;
			InfoBlock.Write(endianBinaryWriterEx);
			position2 = endianBinaryWriterEx.BaseStream.Position;
			endianBinaryWriterEx.BaseStream.Position = 28L;
			endianBinaryWriterEx.Write((uint)(position2 - position));
			endianBinaryWriterEx.BaseStream.Position = position2;
			endianBinaryWriterEx.WriteCurposRelative(32);
			position = endianBinaryWriterEx.BaseStream.Position;
			FileAllocationTable.Write(endianBinaryWriterEx);
			position2 = endianBinaryWriterEx.BaseStream.Position;
			endianBinaryWriterEx.BaseStream.Position = 36L;
			endianBinaryWriterEx.Write((uint)(position2 - position));
			endianBinaryWriterEx.BaseStream.Position = position2;
			endianBinaryWriterEx.WriteCurposRelative(40);
			position = endianBinaryWriterEx.BaseStream.Position;
			endianBinaryWriterEx.Write("FILE", Encoding.ASCII, false);
			endianBinaryWriterEx.Write((uint)(endianBinaryWriterEx.BaseStream.Length - position));
			endianBinaryWriterEx.Write((uint)FileAllocationTable.Entries.Count);
			endianBinaryWriterEx.BaseStream.Position = 44L;
			endianBinaryWriterEx.Write((uint)(endianBinaryWriterEx.BaseStream.Length - position));
			endianBinaryWriterEx.BaseStream.Position = endianBinaryWriterEx.BaseStream.Length;
			endianBinaryWriterEx.EndChunk();
			byte[] result = memoryStream.ToArray();
			endianBinaryWriterEx.Close();
			return result;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000050DC File Offset: 0x000032DC
		public byte[] GetFileData(uint FileId)
		{
			byte[] result;
			if (FileId >= (ulong)FileAllocationTable.Entries.Count)
			{
				result = null;
			}
			else if (FileAllocationTable.Entries[(int)FileId] == null)
			{
				result = null;
			}
			else
			{
				result = FileAllocationTable.Entries[(int)FileId].Data;
			}
			return result;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00005140 File Offset: 0x00003340
		public int GetUsageForBank(int Id)
		{
			int num = 0;
			foreach (INFO.SequenceInfo sequenceInfo in InfoBlock.SequenceInfos.Entries)
			{
				if (sequenceInfo != null)
				{
					if (sequenceInfo.Bank == Id)
					{
						num++;
					}
				}
			}
			foreach (INFO.SequenceArchiveInfo sequenceArchiveInfo in InfoBlock.SequenceArchiveInfos.Entries)
			{
				if (sequenceArchiveInfo != null)
				{
					SSAR ssar = new SSAR(GetFileData(sequenceArchiveInfo.FileId));
					foreach (SSAR.SequenceInfo sequenceInfo2 in ssar.Sequences)
					{
						if (sequenceInfo2.Bank == Id)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00005284 File Offset: 0x00003484
		public int GetUsageForWaveArchive(int Id)
		{
			int num = 0;
			foreach (INFO.BankInfo bankInfo in InfoBlock.BankInfos.Entries)
			{
				if (bankInfo != null)
				{
					if (bankInfo.WaveArchives[0] == Id)
					{
						num++;
					}
					else if (bankInfo.WaveArchives[1] == Id)
					{
						num++;
					}
					else if (bankInfo.WaveArchives[2] == Id)
					{
						num++;
					}
					else if (bankInfo.WaveArchives[3] == Id)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x04000025 RID: 37
		public SDATHeader Header;

		// Token: 0x04000026 RID: 38
		public SYMB SymbolBlock;

		// Token: 0x04000027 RID: 39
		public INFO InfoBlock;

		// Token: 0x04000028 RID: 40
		public FAT FileAllocationTable;

		// Token: 0x02000007 RID: 7
		public class SDATHeader
		{
			// Token: 0x06000035 RID: 53 RVA: 0x00005364 File Offset: 0x00003564
			internal SDATHeader(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
			}

			// Token: 0x06000036 RID: 54 RVA: 0x00005377 File Offset: 0x00003577
			internal void Write(EndianBinaryWriterEx er)
			{
				er.WriteObject(this);
			}

			// Token: 0x04000029 RID: 41
			[BinaryFixedSize(4)]
			[BinaryStringSignature("SDAT")]
			public string Signature;

			// Token: 0x0400002A RID: 42
			[BinaryBOM(65534U)]
			public ushort Endianness;

			// Token: 0x0400002B RID: 43
			public ushort Version;

			// Token: 0x0400002C RID: 44
			public uint FileSize;

			// Token: 0x0400002D RID: 45
			public ushort HeaderSize;

			// Token: 0x0400002E RID: 46
			public ushort NrBlocks;

			// Token: 0x0400002F RID: 47
			public uint SYMBOffset;

			// Token: 0x04000030 RID: 48
			public uint SYMBLength;

			// Token: 0x04000031 RID: 49
			public uint INFOOffset;

			// Token: 0x04000032 RID: 50
			public uint INFOLength;

			// Token: 0x04000033 RID: 51
			public uint FATOffset;

			// Token: 0x04000034 RID: 52
			public uint FATLength;

			// Token: 0x04000035 RID: 53
			public uint FILEOffset;

			// Token: 0x04000036 RID: 54
			public uint FILELength;

			// Token: 0x04000037 RID: 55
			[BinaryFixedSize(16)]
			public byte[] Padding;
		}

		// Token: 0x02000008 RID: 8
		public class SYMB
		{
			// Token: 0x06000037 RID: 55 RVA: 0x00005384 File Offset: 0x00003584
			internal SYMB(EndianBinaryReaderEx er)
			{
				er.BeginChunk();
				Signature = er.ReadString(Encoding.ASCII, 4);
				if (Signature != "SYMB")
				{
					throw new SignatureNotCorrectException(Signature, "SYMB", er.BaseStream.Position - 4L);
				}
				SectionSize = er.ReadUInt32();
				RecordOffsets = er.ReadUInt32s(8);
				Padding = er.ReadBytes(24);
				long position = er.BaseStream.Position;
				er.JumpRelative(RecordOffsets[0]);
				SequenceSymbols = new SymbolRecord(er);
				er.JumpRelative(RecordOffsets[1]);
				SequenceArchiveSymbols = new ArchiveSymbolRecord(er);
				er.JumpRelative(RecordOffsets[2]);
				BankSymbols = new SymbolRecord(er);
				er.JumpRelative(RecordOffsets[3]);
				WaveArchiveSymbols = new SymbolRecord(er);
				er.JumpRelative(RecordOffsets[4]);
				PlayerSymbols = new SymbolRecord(er);
				er.JumpRelative(RecordOffsets[5]);
				GroupSymbols = new SymbolRecord(er);
				er.JumpRelative(RecordOffsets[6]);
				StreamPlayerSymbols = new SymbolRecord(er);
				er.JumpRelative(RecordOffsets[7]);
				StreamSymbols = new SymbolRecord(er);
				er.EndChunk(SectionSize);
			}

			// Token: 0x06000038 RID: 56 RVA: 0x0000550C File Offset: 0x0000370C
			internal void Write(EndianBinaryWriterEx er)
			{
				er.BeginChunk(4);
				er.Write(Signature, Encoding.ASCII, false);
				er.Write(0U);
				er.Write(new uint[8], 0, 8);
				er.Write(new byte[24], 0, 24);
				er.WriteCurposRelative(8);
				SequenceSymbols.Write(er);
				er.WriteCurposRelative(12);
				SequenceArchiveSymbols.Write(er);
				er.WriteCurposRelative(16);
				BankSymbols.Write(er);
				er.WriteCurposRelative(20);
				WaveArchiveSymbols.Write(er);
				er.WriteCurposRelative(24);
				PlayerSymbols.Write(er);
				er.WriteCurposRelative(28);
				GroupSymbols.Write(er);
				er.WriteCurposRelative(32);
				StreamPlayerSymbols.Write(er);
				er.WriteCurposRelative(36);
				StreamSymbols.Write(er);
				er.EndChunk();
			}

			// Token: 0x04000038 RID: 56
			public string Signature;

			// Token: 0x04000039 RID: 57
			public uint SectionSize;

			// Token: 0x0400003A RID: 58
			public uint[] RecordOffsets;

			// Token: 0x0400003B RID: 59
			public byte[] Padding;

			// Token: 0x0400003C RID: 60
			public SymbolRecord SequenceSymbols;

			// Token: 0x0400003D RID: 61
			public ArchiveSymbolRecord SequenceArchiveSymbols;

			// Token: 0x0400003E RID: 62
			public SymbolRecord BankSymbols;

			// Token: 0x0400003F RID: 63
			public SymbolRecord WaveArchiveSymbols;

			// Token: 0x04000040 RID: 64
			public SymbolRecord PlayerSymbols;

			// Token: 0x04000041 RID: 65
			public SymbolRecord GroupSymbols;

			// Token: 0x04000042 RID: 66
			public SymbolRecord StreamPlayerSymbols;

			// Token: 0x04000043 RID: 67
			public SymbolRecord StreamSymbols;

			// Token: 0x02000009 RID: 9
			public class SymbolRecord
			{
				// Token: 0x06000039 RID: 57 RVA: 0x00005614 File Offset: 0x00003814
				internal SymbolRecord(EndianBinaryReaderEx er)
				{
					NrEntries = er.ReadUInt32();
					EntryOffsets = er.ReadUInt32s((int)NrEntries);
					long position = er.BaseStream.Position;
					Entries = new List<string>();
					int num = 0;
					while (num < (long)NrEntries)
					{
						if (EntryOffsets[num] != 0U)
						{
							er.JumpRelative(EntryOffsets[num]);
							Entries.Add(er.ReadStringNT(Encoding.ASCII));
						}
						else
						{
							Entries.Add(null);
						}
						num++;
					}
					er.BaseStream.Position = position;
				}

				// Token: 0x0600003A RID: 58 RVA: 0x000056CC File Offset: 0x000038CC
				internal void Write(EndianBinaryWriterEx er)
				{
					er.Write((uint)Entries.Count);
					int curposRelative = er.GetCurposRelative();
					er.Write(new uint[Entries.Count], 0, Entries.Count);
					for (int i = 0; i < Entries.Count; i++)
					{
						if (Entries[i] != null)
						{
							er.WriteCurposRelative(curposRelative + i * 4);
							er.Write(Entries[i], Encoding.ASCII, true);
						}
					}
					er.WritePadding(4);
				}

				// Token: 0x04000044 RID: 68
				public uint NrEntries;

				// Token: 0x04000045 RID: 69
				public uint[] EntryOffsets;

				// Token: 0x04000046 RID: 70
				public List<string> Entries;
			}

			// Token: 0x0200000A RID: 10
			public class ArchiveSymbolRecord
			{
				// Token: 0x0600003B RID: 59 RVA: 0x00005778 File Offset: 0x00003978
				internal ArchiveSymbolRecord(EndianBinaryReaderEx er)
				{
					NrEntries = er.ReadUInt32();
					Entries = new ArchiveSymbolRecordEntry[NrEntries];
					int num = 0;
					while (num < (long)NrEntries)
					{
						Entries[num] = new ArchiveSymbolRecordEntry(er);
						num++;
					}
				}

				// Token: 0x0600003C RID: 60 RVA: 0x000057D4 File Offset: 0x000039D4
				internal void Write(EndianBinaryWriterEx er)
				{
					er.Write((uint)Entries.Length);
					int curposRelative = er.GetCurposRelative();
					for (int i = 0; i < Entries.Length; i++)
					{
						Entries[i].Write(er);
					}
					for (int i = 0; i < Entries.Length; i++)
					{
						if (Entries[i].ArchiveName != null)
						{
							er.WriteCurposRelative(curposRelative + i * 8);
							er.Write(Entries[i].ArchiveName, Encoding.ASCII, true);
							er.WritePadding(4);
						}
						if (Entries[i].ArchiveSubRecord != null)
						{
							er.WriteCurposRelative(curposRelative + i * 8 + 4);
							Entries[i].ArchiveSubRecord.Write(er);
						}
					}
				}

				// Token: 0x04000047 RID: 71
				public uint NrEntries;

				// Token: 0x04000048 RID: 72
				public ArchiveSymbolRecordEntry[] Entries;

				// Token: 0x0200000B RID: 11
				public class ArchiveSymbolRecordEntry
				{
					// Token: 0x0600003D RID: 61 RVA: 0x000058BC File Offset: 0x00003ABC
					internal ArchiveSymbolRecordEntry(EndianBinaryReaderEx er)
					{
						ArchiveNameOffset = er.ReadUInt32();
						ArchiveSubRecordOffset = er.ReadUInt32();
						if (ArchiveNameOffset != 0U)
						{
							long position = er.BaseStream.Position;
							er.JumpRelative(ArchiveNameOffset);
							ArchiveName = er.ReadStringNT(Encoding.ASCII);
							er.BaseStream.Position = position;
						}
						if (ArchiveSubRecordOffset != 0U)
						{
							long position = er.BaseStream.Position;
							er.JumpRelative(ArchiveSubRecordOffset);
							ArchiveSubRecord = new SymbolRecord(er);
							er.BaseStream.Position = position;
						}
					}

					// Token: 0x0600003E RID: 62 RVA: 0x00005973 File Offset: 0x00003B73
					internal void Write(EndianBinaryWriterEx er)
					{
						er.Write(0U);
						er.Write(0U);
					}

					// Token: 0x04000049 RID: 73
					public uint ArchiveNameOffset;

					// Token: 0x0400004A RID: 74
					public uint ArchiveSubRecordOffset;

					// Token: 0x0400004B RID: 75
					public string ArchiveName;

					// Token: 0x0400004C RID: 76
					public SymbolRecord ArchiveSubRecord;
				}
			}
		}

		// Token: 0x0200000C RID: 12
		public class INFO
		{
			// Token: 0x0600003F RID: 63 RVA: 0x00005988 File Offset: 0x00003B88
			internal INFO(EndianBinaryReaderEx er)
			{
				er.BeginChunk();
				Signature = er.ReadString(Encoding.ASCII, 4);
				if (Signature != "INFO")
				{
					throw new SignatureNotCorrectException(Signature, "INFO", er.BaseStream.Position - 4L);
				}
				SectionSize = er.ReadUInt32();
				InfoOffsets = er.ReadUInt32s(8);
				er.ReadBytes(24);
				er.JumpRelative(InfoOffsets[0]);
				SequenceInfos = new InfoRecord<SequenceInfo>(er);
				er.JumpRelative(InfoOffsets[1]);
				SequenceArchiveInfos = new InfoRecord<SequenceArchiveInfo>(er);
				er.JumpRelative(InfoOffsets[2]);
				BankInfos = new InfoRecord<BankInfo>(er);
				er.JumpRelative(InfoOffsets[3]);
				WaveArchiveInfos = new InfoRecord<WaveArchiveInfo>(er);
				er.JumpRelative(InfoOffsets[4]);
				PlayerInfos = new InfoRecord<PlayerInfo>(er);
				er.JumpRelative(InfoOffsets[5]);
				GroupInfos = new InfoRecord<GroupInfo>(er);
				er.JumpRelative(InfoOffsets[6]);
				StreamPlayerInfos = new InfoRecord<StreamPlayerInfo>(er);
				er.JumpRelative(InfoOffsets[7]);
				StreamInfos = new InfoRecord<StreamInfo>(er);
				er.EndChunk(SectionSize);
			}

			// Token: 0x06000040 RID: 64 RVA: 0x00005B00 File Offset: 0x00003D00
			internal void Write(EndianBinaryWriterEx er)
			{
				er.BeginChunk(4);
				er.Write(Signature, Encoding.ASCII, false);
				er.Write(0U);
				long position = er.BaseStream.Position;
				er.Write(new uint[8], 0, 8);
				er.Write(new byte[24], 0, 24);
				er.WriteCurposRelative(8);
				SequenceInfos.Write(er);
				er.WriteCurposRelative(12);
				SequenceArchiveInfos.Write(er);
				er.WriteCurposRelative(16);
				BankInfos.Write(er);
				er.WriteCurposRelative(20);
				WaveArchiveInfos.Write(er);
				er.WriteCurposRelative(24);
				PlayerInfos.Write(er);
				er.WriteCurposRelative(28);
				GroupInfos.Write(er);
				er.WriteCurposRelative(32);
				StreamPlayerInfos.Write(er);
				er.WriteCurposRelative(36);
				StreamInfos.Write(er);
				er.EndChunk();
			}

			// Token: 0x0400004D RID: 77
			public string Signature;

			// Token: 0x0400004E RID: 78
			public uint SectionSize;

			// Token: 0x0400004F RID: 79
			public uint[] InfoOffsets;

			// Token: 0x04000050 RID: 80
			public InfoRecord<SequenceInfo> SequenceInfos;

			// Token: 0x04000051 RID: 81
			public InfoRecord<SequenceArchiveInfo> SequenceArchiveInfos;

			// Token: 0x04000052 RID: 82
			public InfoRecord<BankInfo> BankInfos;

			// Token: 0x04000053 RID: 83
			public InfoRecord<WaveArchiveInfo> WaveArchiveInfos;

			// Token: 0x04000054 RID: 84
			public InfoRecord<PlayerInfo> PlayerInfos;

			// Token: 0x04000055 RID: 85
			public InfoRecord<GroupInfo> GroupInfos;

			// Token: 0x04000056 RID: 86
			public InfoRecord<StreamPlayerInfo> StreamPlayerInfos;

			// Token: 0x04000057 RID: 87
			public InfoRecord<StreamInfo> StreamInfos;

			// Token: 0x0200000D RID: 13
			public class InfoRecord<T> where T : SDATInfo, new()
			{
				// Token: 0x06000041 RID: 65 RVA: 0x00005C14 File Offset: 0x00003E14
				internal InfoRecord(EndianBinaryReaderEx er)
				{
					NrEntries = er.ReadUInt32();
					EntryOffsets = er.ReadUInt32s((int)NrEntries);
					long position = er.BaseStream.Position;
					Entries = new List<T>();
					int num = 0;
					while (num < (long)NrEntries)
					{
						if (EntryOffsets[num] == 0U)
						{
							Entries.Add(default(T));
						}
						else
						{
							er.JumpRelative(EntryOffsets[num]);
							Entries.Add(Activator.CreateInstance<T>());
							T t = Entries[num];
							t.Read(er);
						}
						num++;
					}
					er.BaseStream.Position = position;
				}

				// Token: 0x06000042 RID: 66 RVA: 0x00005CEC File Offset: 0x00003EEC
				internal void Write(EndianBinaryWriterEx er)
				{
					er.Write((uint)Entries.Count);
					int curposRelative = er.GetCurposRelative();
					er.Write(new uint[Entries.Count], 0, Entries.Count);
					for (int i = 0; i < Entries.Count; i++)
					{
						if (Entries[i] != null)
						{
							er.WriteCurposRelative(curposRelative + i * 4);
							T t = Entries[i];
							t.Write(er);
						}
					}
				}

				// Token: 0x17000002 RID: 2
				public T this[int i]
				{
					get
					{
						return Entries[i];
					}
					set
					{
						Entries[i] = value;
					}
				}

				// Token: 0x04000058 RID: 88
				public uint NrEntries;

				// Token: 0x04000059 RID: 89
				public uint[] EntryOffsets;

				// Token: 0x0400005A RID: 90
				public List<T> Entries;
			}

			// Token: 0x0200000E RID: 14
			public abstract class SDATInfo
			{
				// Token: 0x06000045 RID: 69
				internal abstract void Read(EndianBinaryReaderEx er);

				// Token: 0x06000046 RID: 70
				internal abstract void Write(EndianBinaryWriterEx er);

				// Token: 0x06000047 RID: 71
				internal abstract int GetLength();
			}

			// Token: 0x0200000F RID: 15
			public class SequenceInfo : SDATInfo
			{
				// Token: 0x06000049 RID: 73 RVA: 0x00005DCC File Offset: 0x00003FCC
				internal override int GetLength()
				{
					return 12;
				}

				// Token: 0x0600004A RID: 74 RVA: 0x00005DE0 File Offset: 0x00003FE0
				internal override void Read(EndianBinaryReaderEx er)
				{
					er.ReadObject(this);
				}

				// Token: 0x0600004B RID: 75 RVA: 0x00005DEB File Offset: 0x00003FEB
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.WriteObject(this);
				}

				// Token: 0x0400005B RID: 91
				public uint FileId;

				// Token: 0x0400005C RID: 92
				public ushort Bank;

				// Token: 0x0400005D RID: 93
				public byte Volume;

				// Token: 0x0400005E RID: 94
				public byte ChannelPriority;

				// Token: 0x0400005F RID: 95
				public byte PlayerPriority;

				// Token: 0x04000060 RID: 96
				public byte PlayerNr;

				// Token: 0x04000061 RID: 97
				public ushort Reserved;
			}

			// Token: 0x02000010 RID: 16
			public class SequenceArchiveInfo : SDATInfo
			{
				// Token: 0x0600004D RID: 77 RVA: 0x00005E00 File Offset: 0x00004000
				internal override int GetLength()
				{
					return 4;
				}

				// Token: 0x0600004E RID: 78 RVA: 0x00005E13 File Offset: 0x00004013
				internal override void Read(EndianBinaryReaderEx er)
				{
					FileId = er.ReadUInt32();
				}

				// Token: 0x0600004F RID: 79 RVA: 0x00005E22 File Offset: 0x00004022
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.Write(FileId);
				}

				// Token: 0x04000062 RID: 98
				public uint FileId;
			}

			// Token: 0x02000011 RID: 17
			public class BankInfo : SDATInfo
			{
				// Token: 0x06000051 RID: 81 RVA: 0x00005E3C File Offset: 0x0000403C
				internal override int GetLength()
				{
					return 12;
				}

				// Token: 0x06000052 RID: 82 RVA: 0x00005E50 File Offset: 0x00004050
				internal override void Read(EndianBinaryReaderEx er)
				{
					FileId = er.ReadUInt32();
					WaveArchives = er.ReadUInt16s(4);
				}

				// Token: 0x06000053 RID: 83 RVA: 0x00005E6C File Offset: 0x0000406C
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.Write(FileId);
					er.Write(WaveArchives, 0, 4);
				}

				// Token: 0x04000063 RID: 99
				public uint FileId;

				// Token: 0x04000064 RID: 100
				public ushort[] WaveArchives;
			}

			// Token: 0x02000012 RID: 18
			public class WaveArchiveInfo : SDATInfo
			{
				// Token: 0x06000055 RID: 85 RVA: 0x00005E94 File Offset: 0x00004094
				internal override int GetLength()
				{
					return 4;
				}

				// Token: 0x06000056 RID: 86 RVA: 0x00005EA8 File Offset: 0x000040A8
				internal override void Read(EndianBinaryReaderEx er)
				{
					uint num = er.ReadUInt32();
					FileId = num & 16777215U;
					Flags = (byte)(num >> 24);
				}

				// Token: 0x06000057 RID: 87 RVA: 0x00005ED5 File Offset: 0x000040D5
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.Write((uint)(Flags << 24 | (int)(FileId & 16777215U)));
				}

				// Token: 0x04000065 RID: 101
				public uint FileId;

				// Token: 0x04000066 RID: 102
				public byte Flags;
			}

			// Token: 0x02000013 RID: 19
			public class PlayerInfo : SDATInfo
			{
				// Token: 0x06000059 RID: 89 RVA: 0x00005F00 File Offset: 0x00004100
				internal override int GetLength()
				{
					return 8;
				}

				// Token: 0x0600005A RID: 90 RVA: 0x00005F13 File Offset: 0x00004113
				internal override void Read(EndianBinaryReaderEx er)
				{
					er.ReadObject(this);
				}

				// Token: 0x0600005B RID: 91 RVA: 0x00005F1E File Offset: 0x0000411E
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.WriteObject(this);
				}

				// Token: 0x04000067 RID: 103
				public byte MaxNrSequences;

				// Token: 0x04000068 RID: 104
				public byte Padding;

				// Token: 0x04000069 RID: 105
				public ushort ChannelAllocationMask;

				// Token: 0x0400006A RID: 106
				public uint HeapSize;
			}

			// Token: 0x02000014 RID: 20
			public class GroupInfo : SDATInfo
			{
				// Token: 0x0600005D RID: 93 RVA: 0x00005F34 File Offset: 0x00004134
				internal override int GetLength()
				{
					return 4 + SubItems.Count * 8;
				}

				// Token: 0x0600005E RID: 94 RVA: 0x00005F58 File Offset: 0x00004158
				internal override void Read(EndianBinaryReaderEx er)
				{
					NrSubItems = er.ReadUInt32();
					SubItems = new List<GroupItem>();
					int num = 0;
					while (num < (long)NrSubItems)
					{
						SubItems.Add(new GroupItem(er));
						num++;
					}
				}

				// Token: 0x0600005F RID: 95 RVA: 0x00005FAC File Offset: 0x000041AC
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.Write((uint)SubItems.Count);
					foreach (GroupItem groupItem in SubItems)
					{
						groupItem.Write(er);
					}
				}

				// Token: 0x17000003 RID: 3
				// (get) Token: 0x06000060 RID: 96 RVA: 0x00006018 File Offset: 0x00004218
				// (set) Token: 0x06000061 RID: 97 RVA: 0x0000602F File Offset: 0x0000422F
				public List<GroupItem> SubItems { get; set; }

				// Token: 0x0400006B RID: 107
				public uint NrSubItems;

				// Token: 0x02000015 RID: 21
				public class GroupItem
				{
					// Token: 0x06000063 RID: 99 RVA: 0x00006040 File Offset: 0x00004240
					internal GroupItem(EndianBinaryReaderEx er)
					{
						er.ReadObject(this);
					}

					// Token: 0x06000064 RID: 100 RVA: 0x00006053 File Offset: 0x00004253
					internal void Write(EndianBinaryWriterEx er)
					{
						er.WriteObject(this);
					}

					// Token: 0x0400006D RID: 109
					public byte Type;

					// Token: 0x0400006E RID: 110
					public byte LoadFlag;

					// Token: 0x0400006F RID: 111
					public ushort Padding;

					// Token: 0x04000070 RID: 112
					public uint Index;
				}
			}

			// Token: 0x02000016 RID: 22
			public class StreamPlayerInfo : SDATInfo
			{
				// Token: 0x06000065 RID: 101 RVA: 0x00006060 File Offset: 0x00004260
				internal override int GetLength()
				{
					return 24;
				}

				// Token: 0x06000066 RID: 102 RVA: 0x00006074 File Offset: 0x00004274
				internal override void Read(EndianBinaryReaderEx er)
				{
					er.ReadObject(this);
				}

				// Token: 0x06000067 RID: 103 RVA: 0x0000607F File Offset: 0x0000427F
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.WriteObject(this);
				}

				// Token: 0x04000071 RID: 113
				public byte NrChannels;

				// Token: 0x04000072 RID: 114
				[BinaryFixedSize(16)]
				public byte[] ChannelNumbers;

				// Token: 0x04000073 RID: 115
				[BinaryFixedSize(7)]
				public byte[] Padding;
			}

			// Token: 0x02000017 RID: 23
			public class StreamInfo : SDATInfo
			{
				// Token: 0x06000069 RID: 105 RVA: 0x00006094 File Offset: 0x00004294
				internal override int GetLength()
				{
					return 8;
				}

				// Token: 0x0600006A RID: 106 RVA: 0x000060A7 File Offset: 0x000042A7
				internal override void Read(EndianBinaryReaderEx er)
				{
					er.ReadObject(this);
				}

				// Token: 0x0600006B RID: 107 RVA: 0x000060B2 File Offset: 0x000042B2
				internal override void Write(EndianBinaryWriterEx er)
				{
					er.WriteObject(this);
				}

				// Token: 0x04000074 RID: 116
				public uint FileId;

				// Token: 0x04000075 RID: 117
				public byte Volume;

				// Token: 0x04000076 RID: 118
				public byte PlayerPriority;

				// Token: 0x04000077 RID: 119
				public byte PlayerNr;

				// Token: 0x04000078 RID: 120
				public byte Flags;
			}
		}

		// Token: 0x02000018 RID: 24
		public class FAT
		{
			// Token: 0x0600006D RID: 109 RVA: 0x000060C8 File Offset: 0x000042C8
			internal FAT(EndianBinaryReaderEx er)
			{
				long position = er.BaseStream.Position;
				Signature = er.ReadString(Encoding.ASCII, 4);
				if (Signature != "FAT ")
				{
					throw new SignatureNotCorrectException(Signature, "FAT ", er.BaseStream.Position - 4L);
				}
				SectionSize = er.ReadUInt32();
				NrEntries = er.ReadUInt32();
				Entries = new List<FATEntry>();
				int num = 0;
				while (num < (long)NrEntries)
				{
					Entries.Add(new FATEntry(er));
					num++;
				}
				er.BaseStream.Position = position + SectionSize;
			}

			// Token: 0x0600006E RID: 110 RVA: 0x00006198 File Offset: 0x00004398
			internal void Write(EndianBinaryWriterEx er)
			{
				er.BeginChunk(4);
				er.Write(Signature, Encoding.ASCII, false);
				er.Write(0U);
				er.Write((uint)Entries.Count);
				long num = er.BaseStream.Position + Entries.Count * 16 + 12L;
				while (num % 32L != 0L)
				{
					num += 1L;
				}
				for (int i = 0; i < Entries.Count; i++)
				{
					Entries[i].Write(er, ref num);
				}
				er.EndChunk();
			}

			// Token: 0x04000079 RID: 121
			public string Signature;

			// Token: 0x0400007A RID: 122
			public uint SectionSize;

			// Token: 0x0400007B RID: 123
			public uint NrEntries;

			// Token: 0x0400007C RID: 124
			public List<FATEntry> Entries;

			// Token: 0x02000019 RID: 25
			public class FATEntry
			{
				// Token: 0x0600006F RID: 111 RVA: 0x0000624A File Offset: 0x0000444A
				public FATEntry()
				{
				}

				// Token: 0x06000070 RID: 112 RVA: 0x00006258 File Offset: 0x00004458
				internal FATEntry(EndianBinaryReaderEx er)
				{
					Offset = er.ReadUInt32();
					Length = er.ReadUInt32();
					Padding = er.ReadBytes(8);
					long position = er.BaseStream.Position;
					er.BaseStream.Position = Offset;
					Data = er.ReadBytes((int)Length);
					er.BaseStream.Position = position;
				}

				// Token: 0x06000071 RID: 113 RVA: 0x000062D4 File Offset: 0x000044D4
				internal void Write(EndianBinaryWriterEx er, ref long FileOffset)
				{
					er.Write((uint)FileOffset);
					er.Write((uint)Data.Length);
					er.Write(new byte[8], 0, 8);
					long position = er.BaseStream.Position;
					er.BaseStream.Position = FileOffset;
					er.Write(Data, 0, Data.Length);
					int num = 0;
					while (er.BaseStream.Position % 32L != 0L)
					{
						er.Write(0);
						num++;
					}
					er.BaseStream.Position = position;
					FileOffset += Data.Length + num;
				}

				// Token: 0x0400007D RID: 125
				public uint Offset;

				// Token: 0x0400007E RID: 126
				public uint Length;

				// Token: 0x0400007F RID: 127
				public byte[] Padding;

				// Token: 0x04000080 RID: 128
				public byte[] Data;
			}
		}
	}
}
