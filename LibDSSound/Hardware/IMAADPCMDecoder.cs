namespace LibDSSound.Hardware
{
	// Token: 0x02000030 RID: 48
	public class IMAADPCMDecoder
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00007F80 File Offset: 0x00006180
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00007F97 File Offset: 0x00006197
		public int Last { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00007FA0 File Offset: 0x000061A0
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00007FB7 File Offset: 0x000061B7
		public int Index { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00007FC0 File Offset: 0x000061C0
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00007FD7 File Offset: 0x000061D7
		public int Offset { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00007FE0 File Offset: 0x000061E0
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00007FF7 File Offset: 0x000061F7
		public bool SecondNibble { get; set; }

		// Token: 0x060000C0 RID: 192 RVA: 0x00008000 File Offset: 0x00006200
		public IMAADPCMDecoder(byte[] Data, int Offset)
		{
			Last = (short)(Data[Offset] | Data[Offset + 1] << 8);
			Index = (short)(Data[Offset + 2] | Data[Offset + 3] << 8) & 127;
			Offset += 4;
			this.Offset = Offset;
			this.Data = Data;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00008058 File Offset: 0x00006258
		public short GetSample()
		{
			short sample = GetSample((byte)(Data[Offset] >> ((SecondNibble ? 4 : 0) & 31) & 15));
			if (SecondNibble)
			{
				Offset++;
			}
			SecondNibble = !SecondNibble;
			return sample;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000080C0 File Offset: 0x000062C0
		private short GetSample(byte nibble)
		{
			int num = StepTable[Index] / 8 + StepTable[Index] / 4 * (nibble & 1) + StepTable[Index] / 2 * (nibble >> 1 & 1) + StepTable[Index] * (nibble >> 2 & 1);
			int value = Last + num * ((nibble >> 3 & 1) == 1 ? -1 : 1);
			Last = (short)Clamp(value, -32767, 32767);
			Index = (short)Clamp(Index + IndexTable[nibble & 7], 0, 88);
			return (short)Last;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00008174 File Offset: 0x00006374
		private static int Clamp(int value, int min, int max)
		{
			if (value < min)
			{
				value = min;
			}
			if (value > max)
			{
				value = max;
			}
			return (short)value;
		}

		// Token: 0x04000112 RID: 274
		public static readonly int[] IndexTable = {
			-1,
			-1,
			-1,
			-1,
			2,
			4,
			6,
			8,
			-1,
			-1,
			-1,
			-1,
			2,
			4,
			6,
			8
		};

		// Token: 0x04000113 RID: 275
		public static readonly int[] StepTable = {
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			16,
			17,
			19,
			21,
			23,
			25,
			28,
			31,
			34,
			37,
			41,
			45,
			50,
			55,
			60,
			66,
			73,
			80,
			88,
			97,
			107,
			118,
			130,
			143,
			157,
			173,
			190,
			209,
			230,
			253,
			279,
			307,
			337,
			371,
			408,
			449,
			494,
			544,
			598,
			658,
			724,
			796,
			876,
			963,
			1060,
			1166,
			1282,
			1411,
			1552,
			1707,
			1878,
			2066,
			2272,
			2499,
			2749,
			3024,
			3327,
			3660,
			4026,
			4428,
			4871,
			5358,
			5894,
			6484,
			7132,
			7845,
			8630,
			9493,
			10442,
			11487,
			12635,
			13899,
			15289,
			16818,
			18500,
			20350,
			22385,
			24623,
			27086,
			29794,
			32767
		};

		// Token: 0x04000114 RID: 276
		private byte[] Data;
	}
}
