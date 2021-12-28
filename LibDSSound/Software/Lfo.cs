namespace LibDSSound.Software
{
	// Token: 0x0200001A RID: 26
	public class Lfo
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00006383 File Offset: 0x00004583
		public void Start()
		{
			DelayCounter = 0;
			Counter = 0;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006394 File Offset: 0x00004594
		public void Update()
		{
			if (DelayCounter >= Param.Delay)
			{
				int num = Param.Speed << 6;
				int i;
				for (i = Counter + num >> 8; i >= 128; i -= 128)
				{
				}
				Counter += (ushort)num;
				Counter &= 255;
				Counter |= (ushort)(i << 8);
			}
			else
			{
				DelayCounter += 1;
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00006434 File Offset: 0x00004634
		public int GetValue()
		{
			int result;
			if (Param.Depth != 0 && DelayCounter >= Param.Delay)
			{
				result = Param.Range * Param.Depth * Util.SinIdx(Counter >> 8);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x04000081 RID: 129
		public LfoParam Param = new LfoParam();

		// Token: 0x04000082 RID: 130
		public ushort DelayCounter;

		// Token: 0x04000083 RID: 131
		public ushort Counter;

		// Token: 0x0200001B RID: 27
		public class LfoParam
		{
			// Token: 0x06000076 RID: 118 RVA: 0x000064AA File Offset: 0x000046AA
			public void Init()
			{
				Target = LfoTarget.Pitch;
				Depth = 0;
				Range = 1;
				Speed = 16;
				Delay = 0;
			}

			// Token: 0x04000084 RID: 132
			public LfoTarget Target;

			// Token: 0x04000085 RID: 133
			public byte Speed;

			// Token: 0x04000086 RID: 134
			public byte Depth;

			// Token: 0x04000087 RID: 135
			public byte Range;

			// Token: 0x04000088 RID: 136
			public ushort Delay;

			// Token: 0x0200001C RID: 28
			public enum LfoTarget : byte
			{
				// Token: 0x0400008A RID: 138
				Pitch,
				// Token: 0x0400008B RID: 139
				Volume,
				// Token: 0x0400008C RID: 140
				Pan
			}
		}
	}
}
