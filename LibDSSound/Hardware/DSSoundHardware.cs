namespace LibDSSound.Hardware
{
	// Token: 0x02000038 RID: 56
	public class DSSoundHardware
	{
		// Token: 0x060000CC RID: 204 RVA: 0x00008A90 File Offset: 0x00006C90
		public DSSoundHardware()
		{
			Channels = new Channel[16];
			for (int i = 0; i < 16; i++)
			{
				Channels[i] = new Channel(i);
			}
			MasterVolume = 127;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00008ADC File Offset: 0x00006CDC
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00008AF3 File Offset: 0x00006CF3
		public Channel[] Channels { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00008AFC File Offset: 0x00006CFC
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00008B13 File Offset: 0x00006D13
		public byte MasterVolume { get; set; }

		// Token: 0x060000D1 RID: 209 RVA: 0x00008B1C File Offset: 0x00006D1C
		public void Evaluate(int NrTicks, out short Left, out short Right)
		{
			int num = 0;
			int num2 = 0;
			foreach (Channel channel in Channels)
			{
				if (channel != null && channel.Enabled)
				{
					short num3;
					short num4;
					channel.Evaluate(NrTicks, out num3, out num4);
					num += num3;
					num2 += num4;
				}
			}
			num = num * (MasterVolume == 127 ? 128 : MasterVolume) / 128;
			num2 = num2 * (MasterVolume == 127 ? 128 : MasterVolume) / 128;
			if (num < -32768)
			{
				num = -32768;
			}
			else if (num > 32767)
			{
				num = 32767;
			}
			if (num2 < -32768)
			{
				num2 = -32768;
			}
			else if (num2 > 32767)
			{
				num2 = 32767;
			}
			Left = (short)num;
			Right = (short)num2;
		}
	}
}
