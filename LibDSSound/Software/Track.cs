namespace LibDSSound.Software
{
	// Token: 0x02000027 RID: 39
	public class Track
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00006DD0 File Offset: 0x00004FD0
		public void Init()
		{
			Data = null;
			DataOffset = 0;
			NoteWait = true;
			Mute = false;
			Tie = false;
			NoteFinishWait = false;
			Portamento = false;
			CompareFlag = true;
			HasChannelMask = false;
			CallStackDepth = 0;
			ProgramNumber = 0;
			Priority = 64;
			Volume = 127;
			Volume2 = 127;
			ExtendedFader = 0;
			Pan = 0;
			ExtendedPan = 0;
			PitchBend = 0;
			ExtendedPitch = 0;
			Attack = byte.MaxValue;
			Decay = byte.MaxValue;
			Sustain = byte.MaxValue;
			Release = byte.MaxValue;
			PanRange = 127;
			BendRange = 2;
			PortamentoKey = 60;
			PortamentoTime = 0;
			SweepPitch = 0;
			Transpose = 0;
			ChannelMask = ushort.MaxValue;
			Modulation.Init();
			Wait = 0;
			ChannelList = null;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006EE3 File Offset: 0x000050E3
		public void Close(Player player)
		{
			ReleaseChannelAll(player, -1);
			FreeChannelAll();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006EF6 File Offset: 0x000050F6
		public void Start(byte[] data, int offset)
		{
			Data = data;
			DataOffset = offset;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006F08 File Offset: 0x00005108
		public void SetMute(Player player, int type)
		{
			switch (type)
			{
			case 0:
				Mute = false;
				break;
			case 1:
				Mute = true;
				break;
			case 2:
				Mute = true;
				ReleaseChannelAll(player, -1);
				break;
			case 3:
				Mute = true;
				ReleaseChannelAll(player, 127);
				FreeChannelAll();
				break;
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006F70 File Offset: 0x00005170
		public void ReleaseChannelAll(Player player, int release)
		{
			UpdateChannel(player, release);
			for (ExChannel exChannel = ChannelList; exChannel != null; exChannel = exChannel.Next)
			{
				if (exChannel.IsActive())
				{
					if (release >= 0)
					{
						exChannel.SetRelease(release);
					}
					exChannel.Priority = 1;
					exChannel.ReleaseChannel();
				}
			} 
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006FD4 File Offset: 0x000051D4
		public void FreeChannelAll()
		{
			for (ExChannel exChannel = ChannelList; exChannel != null; exChannel = exChannel.Next)
			{
				exChannel.Free();
			}
			ChannelList = null;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00007010 File Offset: 0x00005210
		public void UpdateChannel(Player player, int release)
		{
			int num = Util.DecibelSquareTable[player.Volume] + Util.DecibelSquareTable[Volume] + Util.DecibelSquareTable[Volume2];
			int num2 = ExtendedFader + player.ExtendedFader;
			
			int num3 = ExtendedPitch + PitchBend * BendRange / 2;
			int num4 = Pan;
			if (PanRange != 127)
			{
				num4 = (num4 * PanRange + 64) / 128;
			}
			num4 += ExtendedPan;
			if (num < -32768)
			{
				num = -32768;
			}
			if (num2 < -32768)
			{
				num2 = -32768;
			}
			if (num4 < -128)
			{
				num4 = -128;
			}
			else if (num4 > 127)
			{
				num4 = 127;
			}
			for (ExChannel exChannel = ChannelList; exChannel != null; exChannel = exChannel.Next)
			{
				exChannel.UserDecay2 = (short)num2;
				if (exChannel.EnvelopeStatus != ExChannel.SoundEnvelopeStatus.Release)
				{
					exChannel.UserDecay = (short)num;
					exChannel.UserPitch = (short)num3;
					exChannel.UserPan = (sbyte)num4;
					exChannel.PanRange = PanRange;
					exChannel.Lfo.Param.Target = Modulation.Target;
					exChannel.Lfo.Param.Speed = Modulation.Speed;
					exChannel.Lfo.Param.Depth = Modulation.Depth;
					exChannel.Lfo.Param.Range = Modulation.Range;
					exChannel.Lfo.Param.Delay = Modulation.Delay;
					if (exChannel.Length == 0 && release != 0)
					{
						exChannel.Priority = 1;
						exChannel.ReleaseChannel();
					}
				}
			}
		}

		// Token: 0x040000DA RID: 218
		public bool Active;

		// Token: 0x040000DB RID: 219
		public bool NoteWait;

		// Token: 0x040000DC RID: 220
		public bool Mute;

		// Token: 0x040000DD RID: 221
		public bool Tie;

		// Token: 0x040000DE RID: 222
		public bool NoteFinishWait;

		// Token: 0x040000DF RID: 223
		public bool Portamento;

		// Token: 0x040000E0 RID: 224
		public bool CompareFlag;

		// Token: 0x040000E1 RID: 225
		public bool HasChannelMask;

		// Token: 0x040000E2 RID: 226
		public byte PanRange;

		// Token: 0x040000E3 RID: 227
		public ushort ProgramNumber;

		// Token: 0x040000E4 RID: 228
		public byte Volume;

		// Token: 0x040000E5 RID: 229
		public byte Volume2;

		// Token: 0x040000E6 RID: 230
		public sbyte PitchBend;

		// Token: 0x040000E7 RID: 231
		public byte BendRange;

		// Token: 0x040000E8 RID: 232
		public sbyte Pan;

		// Token: 0x040000E9 RID: 233
		public sbyte ExtendedPan;

		// Token: 0x040000EA RID: 234
		public short ExtendedFader;

		// Token: 0x040000EB RID: 235
		public short ExtendedPitch;

		// Token: 0x040000EC RID: 236
		public byte Attack;

		// Token: 0x040000ED RID: 237
		public byte Decay;

		// Token: 0x040000EE RID: 238
		public byte Sustain;

		// Token: 0x040000EF RID: 239
		public byte Release;

		// Token: 0x040000F0 RID: 240
		public byte Priority;

		// Token: 0x040000F1 RID: 241
		public sbyte Transpose;

		// Token: 0x040000F2 RID: 242
		public byte PortamentoKey;

		// Token: 0x040000F3 RID: 243
		public byte PortamentoTime;

		// Token: 0x040000F4 RID: 244
		public short SweepPitch;

		// Token: 0x040000F5 RID: 245
		public Lfo.LfoParam Modulation = new Lfo.LfoParam();

		// Token: 0x040000F6 RID: 246
		public ushort ChannelMask;

		// Token: 0x040000F7 RID: 247
		public int Wait;

		// Token: 0x040000F8 RID: 248
		public byte[] Data;

		// Token: 0x040000F9 RID: 249
		public int DataOffset;

		// Token: 0x040000FA RID: 250
		public int[] CallStack = new int[3];

		// Token: 0x040000FB RID: 251
		public byte[] LoopCount = new byte[3];

		// Token: 0x040000FC RID: 252
		public byte CallStackDepth;

		// Token: 0x040000FD RID: 253
		public ExChannel ChannelList;
	}
}
