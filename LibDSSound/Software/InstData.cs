using LibEndianBinaryIO;
using LibEndianBinaryIO.Serialization;

namespace LibDSSound.Software
{
	// Token: 0x02000039 RID: 57
	public class InstData
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x00008C2C File Offset: 0x00006E2C
		public InstData()
		{
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00008C42 File Offset: 0x00006E42
		public InstData(EndianBinaryReaderEx er)
		{
			Type = (InstType)er.ReadByte();
			er.ReadByte();
			Param = new InstParam(er);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00008C77 File Offset: 0x00006E77
		public void Write(EndianBinaryWriterEx er)
		{
			er.Write((byte)Type);
			er.Write(0);
			Param.Write(er);
		}

		// Token: 0x0400017C RID: 380
		public InstType Type;

		// Token: 0x0400017D RID: 381
		public InstParam Param = new InstParam();

		// Token: 0x0200003A RID: 58
		public enum InstType : byte
		{
			// Token: 0x0400017F RID: 383
			Invalid,
			// Token: 0x04000180 RID: 384
			Pcm,
			// Token: 0x04000181 RID: 385
			Psg,
			// Token: 0x04000182 RID: 386
			Noise,
			// Token: 0x04000183 RID: 387
			DirectPcm,
			// Token: 0x04000184 RID: 388
			Null,
			// Token: 0x04000185 RID: 389
			DrumSet = 16,
			// Token: 0x04000186 RID: 390
			KeySplit
		}

		// Token: 0x0200003B RID: 59
		public class InstParam
		{
			// Token: 0x060000D5 RID: 213 RVA: 0x00008C9C File Offset: 0x00006E9C
			public InstParam()
			{
			}

			// Token: 0x060000D6 RID: 214 RVA: 0x00008CA7 File Offset: 0x00006EA7
			public InstParam(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
			}

			// Token: 0x060000D7 RID: 215 RVA: 0x00008CBA File Offset: 0x00006EBA
			public void Write(EndianBinaryWriterEx er)
			{
				er.WriteObject(this);
			}

			// Token: 0x04000187 RID: 391
			[BinaryFixedSize(2)]
			public ushort[] Wave;

			// Token: 0x04000188 RID: 392
			public byte OriginalKey;

			// Token: 0x04000189 RID: 393
			public byte Attack;

			// Token: 0x0400018A RID: 394
			public byte Decay;

			// Token: 0x0400018B RID: 395
			public byte Sustain;

			// Token: 0x0400018C RID: 396
			public byte Release;

			// Token: 0x0400018D RID: 397
			public byte Pan;
		}
	}
}
