using LibDSSound.Hardware;
using LibEndianBinaryIO;
using LibEndianBinaryIO.Serialization;

namespace LibDSSound.Software
{
	// Token: 0x02000024 RID: 36
	public class WaveData
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00006C30 File Offset: 0x00004E30
		public WaveData(EndianBinaryReaderEx er)
		{
			Param = new WaveParam(er);
			uint count = (uint)(Param.LoopStart * 4 + (long)(Param.LoopLen * 4U));
			Samples = er.ReadBytes((int)count);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006C7E File Offset: 0x00004E7E
		public void Write(EndianBinaryWriterEx er)
		{
			Param.Write(er);
			er.Write(Samples, 0, Samples.Length);
			er.WritePadding(4);
		}

		// Token: 0x040000CC RID: 204
		public WaveParam Param;

		// Token: 0x040000CD RID: 205
		public byte[] Samples;

		// Token: 0x02000025 RID: 37
		public class WaveParam
		{
			// Token: 0x06000093 RID: 147 RVA: 0x00006CAD File Offset: 0x00004EAD
			public WaveParam()
			{
			}

			// Token: 0x06000094 RID: 148 RVA: 0x00006CB8 File Offset: 0x00004EB8
			public WaveParam(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
			}

			// Token: 0x06000095 RID: 149 RVA: 0x00006CCB File Offset: 0x00004ECB
			public void Write(EndianBinaryWriterEx er)
			{
				er.WriteObject(this);
			}

			// Token: 0x040000CE RID: 206
			public Channel.SoundFormat Format;

			// Token: 0x040000CF RID: 207
			[BinaryBooleanSize(BooleanSize.U8)]
			public bool Loop;

			// Token: 0x040000D0 RID: 208
			public ushort Rate;

			// Token: 0x040000D1 RID: 209
			public ushort Timer;

			// Token: 0x040000D2 RID: 210
			public ushort LoopStart;

			// Token: 0x040000D3 RID: 211
			public uint LoopLen;
		}
	}
}
