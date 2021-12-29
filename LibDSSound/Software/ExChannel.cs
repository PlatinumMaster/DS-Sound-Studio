using System;
using LibDSSound.Hardware;

namespace LibDSSound.Software
{
	// Token: 0x0200001D RID: 29
	public class ExChannel
	{
		// Token: 0x06000078 RID: 120 RVA: 0x000064D9 File Offset: 0x000046D9
		public void Free()
		{
			Callback = null;
			CallbackData = null;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000064EC File Offset: 0x000046EC
		public bool StartPcm(WaveData.WaveParam waveParam, byte[] data, int length)
		{
			Type = ExChannelType.Pcm;
			Wave.Format = waveParam.Format;
			Wave.Loop = waveParam.Loop;
			Wave.Rate = waveParam.Rate;
			Wave.Timer = waveParam.Timer;
			Wave.LoopStart = waveParam.LoopStart;
			Wave.LoopLen = waveParam.LoopLen;
			Data = data;
			Start(length);
			return true;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000657C File Offset: 0x0000477C
		public bool StartPsg(Channel.PSGDuty duty, int length)
		{
			bool result;
			if (MyNo >= 8 && MyNo <= 13)
			{
				Type = ExChannelType.Psg;
				Duty = duty;
				Wave.Timer = 8006;
				Start(length);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000065D4 File Offset: 0x000047D4
		public bool StartNoise(int length)
		{
			bool result;
			if (MyNo >= 14 && MyNo <= 15)
			{
				Type = ExChannelType.Noise;
				Wave.Timer = 8006;
				Start(length);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006628 File Offset: 0x00004828
		public int UpdateEnvelope(bool doPeriodicProc)
		{
			if (doPeriodicProc)
			{
				switch (EnvelopeStatus)
				{
				case SoundEnvelopeStatus.Attack:
					EnvelopeDecay = -(-EnvelopeDecay * Attack / 256);
					if (EnvelopeDecay == 0)
					{
						EnvelopeStatus = SoundEnvelopeStatus.Decay;
					}
					break;
				case SoundEnvelopeStatus.Decay:
				{
					int num = Util.DecibelSquareTable[Sustain] * 128;
					EnvelopeDecay -= Decay;
					if (EnvelopeDecay <= num)
					{
						EnvelopeDecay = num;
						EnvelopeStatus = SoundEnvelopeStatus.Sustain;
					}
					break;
				}
				case SoundEnvelopeStatus.Release:
					EnvelopeDecay -= Release;
					break;
				}
			}
			return EnvelopeDecay / 128;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00006704 File Offset: 0x00004904
		public void SetAttack(int attack)
		{
			if (attack >= 109)
			{
				Attack = AttackTable[127 - attack];
			}
			else
			{
				Attack = (byte)(255 - attack);
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000673B File Offset: 0x0000493B
		public void SetDecay(int decay)
		{
			Decay = CalcRelease(decay);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000674A File Offset: 0x0000494A
		public void SetSustain(byte sustain)
		{
			Sustain = sustain;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00006754 File Offset: 0x00004954
		public void SetRelease(int release)
		{
			Release = CalcRelease(release);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00006763 File Offset: 0x00004963
		public void ReleaseChannel()
		{
			EnvelopeStatus = SoundEnvelopeStatus.Release;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00006770 File Offset: 0x00004970
		public bool IsActive()
		{
			return Active;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006788 File Offset: 0x00004988
		private static ushort CalcRelease(int a)
		{
			ushort result;
			if (a == 127)
			{
				result = ushort.MaxValue;
			}
			else if (a == 126)
			{
				result = 15360;
			}
			else if (a >= 50)
			{
				result = (ushort)(7680 / (126 - a));
			}
			else
			{
				result = (ushort)(2 * a + 1 & 65535);
			}
			return result;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000067E8 File Offset: 0x000049E8
		public void InitAlloc(ExChannelCallback Callback, object CallbackData, byte Priority)
		{
			Next = null;
			this.Callback = Callback;
			this.CallbackData = CallbackData;
			Length = 0;
			this.Priority = Priority;
			Volume = 127;
			Started = false;
			AutoSweep = true;
			Key = 60;
			OriginalKey = 60;
			Velocity = 127;
			InitPan = 0;
			UserDecay = 0;
			UserDecay2 = 0;
			UserPitch = -127;
			UserPan = 0;
			PanRange = 127;
			SweepPitch = 0;
			SweepLength = 0;
			SweepCounter = 0;
			SetAttack(127);
			SetDecay(127);
			SetSustain(127);
			SetRelease(127);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000068AB File Offset: 0x00004AAB
		public void Start(int length)
		{
			EnvelopeDecay = -92544;
			EnvelopeStatus = SoundEnvelopeStatus.Attack;
			Length = length;
			Lfo.Start();
			Started = true;
			Active = true;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000068E4 File Offset: 0x00004AE4
		public static int CompareVolume(ExChannel a, ExChannel b)
		{
			int num = 16 * (a.Volume & 255) >> Shift[a.Volume >> 8];
			int num2 = 16 * (b.Volume & 255) >> Shift[b.Volume >> 8];
			int result;
			if (num == num2)
			{
				result = 0;
			}
			else if (num >= num2)
			{
				result = -1;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00006958 File Offset: 0x00004B58
		public int SweepMain(bool doUpdate)
		{
			int result;
			if (SweepPitch != 0 && SweepCounter < SweepLength)
			{
				int num = (int)(Math.BigMul(SweepPitch, SweepLength - SweepCounter) / SweepLength);
				if (doUpdate && AutoSweep)
				{
					SweepCounter++;
				}
				result = num;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000069D4 File Offset: 0x00004BD4
		public int LfoMain(bool doUpdate)
		{
			int value = Lfo.GetValue();
			if (Lfo.Param.Target == Lfo.LfoParam.LfoTarget.Volume) {
				value *= 60;
				value >>= 14;
			}
			else {
				value >>= 8;
			}
			if (doUpdate) {
				Lfo.Update();
			}
			return value;
		}

		// Token: 0x0400008D RID: 141
		private static readonly int[] Shift = {
			0,
			1,
			2,
			4
		};

		// Token: 0x0400008E RID: 142
		private static readonly byte[] AttackTable = {
			0,
			1,
			5,
			14,
			26,
			38,
			51,
			63,
			73,
			84,
			92,
			100,
			109,
			116,
			123,
			127,
			132,
			137,
			143
		};

		// Token: 0x0400008F RID: 143
		public byte MyNo;

		// Token: 0x04000090 RID: 144
		public ExChannelType Type;

		// Token: 0x04000091 RID: 145
		public SoundEnvelopeStatus EnvelopeStatus;

		// Token: 0x04000092 RID: 146
		public bool Active;

		// Token: 0x04000093 RID: 147
		public bool Started;

		// Token: 0x04000094 RID: 148
		public bool AutoSweep;

		// Token: 0x04000095 RID: 149
		public byte SyncFlag;

		// Token: 0x04000096 RID: 150
		public byte PanRange;

		// Token: 0x04000097 RID: 151
		public byte OriginalKey;

		// Token: 0x04000098 RID: 152
		public short UserDecay2;

		// Token: 0x04000099 RID: 153
		public byte Key;

		// Token: 0x0400009A RID: 154
		public byte Velocity;

		// Token: 0x0400009B RID: 155
		public sbyte InitPan;

		// Token: 0x0400009C RID: 156
		public sbyte UserPan;

		// Token: 0x0400009D RID: 157
		public short UserDecay;

		// Token: 0x0400009E RID: 158
		public short UserPitch;

		// Token: 0x0400009F RID: 159
		private int EnvelopeDecay;

		// Token: 0x040000A0 RID: 160
		public int SweepCounter;

		// Token: 0x040000A1 RID: 161
		public int SweepLength;

		// Token: 0x040000A2 RID: 162
		private byte Attack;

		// Token: 0x040000A3 RID: 163
		private byte Sustain;

		// Token: 0x040000A4 RID: 164
		private ushort Decay;

		// Token: 0x040000A5 RID: 165
		private ushort Release;

		// Token: 0x040000A6 RID: 166
		public byte Priority;

		// Token: 0x040000A7 RID: 167
		public byte Pan;

		// Token: 0x040000A8 RID: 168
		public ushort Volume;

		// Token: 0x040000A9 RID: 169
		public ushort Timer;

		// Token: 0x040000AA RID: 170
		public Lfo Lfo = new Lfo();

		// Token: 0x040000AB RID: 171
		public short SweepPitch;

		// Token: 0x040000AC RID: 172
		public int Length;

		// Token: 0x040000AD RID: 173
		public WaveData.WaveParam Wave = new WaveData.WaveParam();

		// Token: 0x040000AE RID: 174
		public byte[] Data;

		// Token: 0x040000AF RID: 175
		public Channel.PSGDuty Duty;

		// Token: 0x040000B0 RID: 176
		public ExChannelCallback Callback;

		// Token: 0x040000B1 RID: 177
		public object CallbackData;

		// Token: 0x040000B2 RID: 178
		public ExChannel Next;

		// Token: 0x0200001E RID: 30
		public enum ExChannelType
		{
			// Token: 0x040000B4 RID: 180
			Pcm,
			// Token: 0x040000B5 RID: 181
			Psg,
			// Token: 0x040000B6 RID: 182
			Noise
		}

		// Token: 0x0200001F RID: 31
		public enum ExChannelCallbackStatus
		{
			// Token: 0x040000B8 RID: 184
			Drop,
			// Token: 0x040000B9 RID: 185
			Finish
		}

		// Token: 0x02000020 RID: 32
		public enum SoundEnvelopeStatus
		{
			// Token: 0x040000BB RID: 187
			Attack,
			// Token: 0x040000BC RID: 188
			Decay,
			// Token: 0x040000BD RID: 189
			Sustain,
			// Token: 0x040000BE RID: 190
			Release
		}

		// Token: 0x02000021 RID: 33
		// (Invoke) Token: 0x0600008C RID: 140
		public delegate void ExChannelCallback(ExChannel ch_p, ExChannelCallbackStatus status, object userData);
	}
}
