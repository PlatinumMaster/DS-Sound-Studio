namespace LibDSSound.Hardware
{
	// Token: 0x02000031 RID: 49
	public class Channel
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x0000837C File Offset: 0x0000657C
		public Channel(int Nr)
		{
			ChannelNr = Nr;
			Enabled = false;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000083BC File Offset: 0x000065BC
		public void Evaluate(int NrTicks, out short Left, out short Right)
		{
			if (!Enabled || Timer <= 0)
			{
				CurLeft = CurRight = Left = Right = 0;
			}
			else
			{
				int num = (Counter + NrTicks) / -(short)Timer;
				Counter = (Counter + NrTicks) % -(short)Timer;
				short curLeft = CurLeft;
				short curRight = CurRight;
				for (int i = 0; i < num; i++)
				{
					if (Enabled) {
						GetSample(out curLeft, out curRight);
					}
					else
					{
						curRight = curLeft = 0;
					}
				}
				Left = CurLeft = curLeft;
				Right = CurRight = curRight;
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00008494 File Offset: 0x00006694
		private void GetSample(out short Left, out short Right)
		{
			if (Format == SoundFormat.PCM8 || Format == SoundFormat.PCM16 || Format == SoundFormat.ADPCM)
			{
				if (Format == SoundFormat.ADPCM && ADPCMDecoder != null && ADPCMDecoder.Offset == LoopStart * 4 && !ADPCMDecoder.SecondNibble)
				{
					ADPCMLoopLast = ADPCMDecoder.Last;
					ADPCMLoopIdx = ADPCMDecoder.Index;
				}
				if (Format != SoundFormat.ADPCM && DataPosition >= GetSoundLength() || Format == SoundFormat.ADPCM && ADPCMDecoder != null && ADPCMDecoder.Offset >= GetSoundLength() && !ADPCMDecoder.SecondNibble)
				{
					if (Repeat == RepeatMode.Repeat)
					{
						if (Format == SoundFormat.ADPCM)
						{
							ADPCMDecoder.Offset = LoopStart * 4;
							ADPCMDecoder.Index = ADPCMLoopIdx;
							ADPCMDecoder.Last = ADPCMLoopLast;
							ADPCMDecoder.SecondNibble = false;
						}
						else
						{
							DataPosition = LoopStart * 4;
						}
					}
					else
					{
						DataPosition = 0;
					}
				}
			}
			short num = 0;
			if (DataPosition < 0)
			{
				DataPosition++;
			}
			else
			{
				switch (Format)
				{
				case SoundFormat.PCM8:
					num = (short)((sbyte)Data[DataPosition++] * 256);
					break;
				case SoundFormat.PCM16:
					num = (short)(Data[DataPosition++] | Data[DataPosition++] << 8);
					break;
				case SoundFormat.ADPCM:
					if (ADPCMDecoder == null)
					{
						ADPCMDecoder = new IMAADPCMDecoder(Data, DataPosition);
					}
					num = ADPCMDecoder.GetSample();
					break;
				case SoundFormat.PSG_NOISE:
					if (ChannelNr >= 8 && ChannelNr <= 13)
					{
						num = PSGCounter <= (int)Duty ? short.MinValue : short.MaxValue;
						PSGCounter++;
						if (PSGCounter >= 8)
						{
							PSGCounter = 0;
						}
					}
					else if (ChannelNr == 14 || ChannelNr == 15)
					{
						if ((NoiseCounter & 1) != 0)
						{
							NoiseCounter = (ushort)(NoiseCounter >> 1 ^ 24576);
							num = -32767;
						}
						else
						{
							NoiseCounter = (ushort)(NoiseCounter >> 1);
							num = short.MaxValue;
						}
					}
					break;
				}
			}
			num /= (short)GetVolumeDiv();
			num = (short)(num * (Volume == 127 ? 128 : Volume) / 128);
			Left = (short)(num * (128 - (Pan == 127 ? 128 : Pan)) / 128);
			Right = (short)(num * (Pan == 127 ? 128 : Pan) / 128);
			if (((Format == SoundFormat.PCM8 || Format == SoundFormat.PCM16) && DataPosition >= GetSoundLength() || Format == SoundFormat.ADPCM && ADPCMDecoder != null && ADPCMDecoder.Offset >= GetSoundLength() && !ADPCMDecoder.SecondNibble) && Repeat == RepeatMode.OneShot)
			{
				Enabled = false;
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000088AC File Offset: 0x00006AAC
		private uint GetSoundLength()
		{
			uint result;
			switch (Repeat)
			{
			case RepeatMode.Manual:
				result = (uint)Data.Length;
				break;
			case RepeatMode.Repeat:
			case RepeatMode.OneShot:
				result = (uint)(LoopStart * 4 + Length * 4U);
				break;
			default:
				result = 0U;
				break;
			}
			return result;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000088FC File Offset: 0x00006AFC
		private int GetVolumeDiv()
		{
			int result;
			switch (Shift)
			{
			case DataShift.Shift_0:
				result = 1;
				break;
			case DataShift.Shift_1:
				result = 2;
				break;
			case DataShift.Shift_2:
				result = 4;
				break;
			case DataShift.Shift_4:
				result = 16;
				break;
			default:
				result = 1;
				break;
			}
			return result;
		}

		// Token: 0x04000119 RID: 281
		private int ChannelNr;

		// Token: 0x0400011A RID: 282
		public int Volume;

		// Token: 0x0400011B RID: 283
		public DataShift Shift;

		// Token: 0x0400011C RID: 284
		public bool Hold;

		// Token: 0x0400011D RID: 285
		public int Pan;

		// Token: 0x0400011E RID: 286
		public PSGDuty Duty;

		// Token: 0x0400011F RID: 287
		public RepeatMode Repeat;

		// Token: 0x04000120 RID: 288
		public SoundFormat Format;

		// Token: 0x04000121 RID: 289
		public bool Enabled;

		// Token: 0x04000122 RID: 290
		public byte[] Data;

		// Token: 0x04000123 RID: 291
		public int DataPosition;

		// Token: 0x04000124 RID: 292
		public ushort Timer;

		// Token: 0x04000125 RID: 293
		public ushort LoopStart;

		// Token: 0x04000126 RID: 294
		public uint Length;

		// Token: 0x04000127 RID: 295
		public int Counter;

		// Token: 0x04000128 RID: 296
		private short CurLeft;

		// Token: 0x04000129 RID: 297
		private short CurRight;

		// Token: 0x0400012A RID: 298
		public ushort NoiseCounter = 32767;

		// Token: 0x0400012B RID: 299
		public int PSGCounter;

		// Token: 0x0400012C RID: 300
		public IMAADPCMDecoder ADPCMDecoder;

		// Token: 0x0400012D RID: 301
		private int ADPCMLoopLast;

		// Token: 0x0400012E RID: 302
		private int ADPCMLoopIdx;

		// Token: 0x02000032 RID: 50
		public enum DataShift : byte
		{
			// Token: 0x04000130 RID: 304
			Shift_0 = 0x0,
			// Token: 0x04000131 RID: 305
			Shift_1 = 0x1,
			// Token: 0x04000132 RID: 306
			Shift_2 = 0x2,
			// Token: 0x04000133 RID: 307
			Shift_4 = 0x3,
		}

		// Token: 0x02000033 RID: 51
		public enum PSGDuty
		{
			// Token: 0x04000135 RID: 309
			Duty1_8,
			// Token: 0x04000136 RID: 310
			Duty2_8,
			// Token: 0x04000137 RID: 311
			Duty3_8,
			// Token: 0x04000138 RID: 312
			Duty4_8,
			// Token: 0x04000139 RID: 313
			Duty5_8,
			// Token: 0x0400013A RID: 314
			Duty6_8,
			// Token: 0x0400013B RID: 315
			Duty7_8
		}

		// Token: 0x02000034 RID: 52
		public enum RepeatMode
		{
			// Token: 0x0400013D RID: 317
			Manual,
			// Token: 0x0400013E RID: 318
			Repeat,
			// Token: 0x0400013F RID: 319
			OneShot
		}

		// Token: 0x02000035 RID: 53
		public enum SoundFormat : byte
		{
			// Token: 0x04000141 RID: 321
			PCM8,
			// Token: 0x04000142 RID: 322
			PCM16,
			// Token: 0x04000143 RID: 323
			ADPCM,
			// Token: 0x04000144 RID: 324
			PSG_NOISE
		}
	}
}
