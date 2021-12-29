using LibDSSound.Hardware;
using LibDSSound.IO;

namespace LibDSSound.Software
{
	// Token: 0x02000036 RID: 54
	public class Sequence
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00008940 File Offset: 0x00006B40
		public static bool NoteOn(ExChannel channel, byte key, byte velocity, int length, SBNK bank, InstData inst)
		{
			int num = inst.Param.Release;
			if (num == 255)
			{
				length = -1;
				num = 0;
			}
			bool flag = false;
			switch (inst.Type)
			{
			case InstData.InstType.Pcm:
			{
				WaveData waveData = bank.GetWaveData(inst.Param.Wave[1], inst.Param.Wave[0]);
				if (waveData != null)
				{
					flag = channel.StartPcm(waveData.Param, waveData.Samples, length);
				}
				break;
			}
			case InstData.InstType.Psg:
				flag = channel.StartPsg((Channel.PSGDuty)inst.Param.Wave[0], length);
				break;
			case InstData.InstType.Noise:
				flag = channel.StartNoise(length);
				break;
			}
			bool result;
			if (flag)
			{
				channel.Key = key;
				channel.OriginalKey = inst.Param.OriginalKey;
				channel.Velocity = velocity;
				channel.SetAttack(inst.Param.Attack);
				channel.SetDecay(inst.Param.Decay);
				channel.SetSustain(inst.Param.Sustain);
				channel.SetRelease(num);
				channel.InitPan = (sbyte)(inst.Param.Pan - 64);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x02000037 RID: 55
		public enum MmlCommand : byte
		{
			// Token: 0x04000146 RID: 326
			Wait = 128,
			// Token: 0x04000147 RID: 327
			Prg,
			// Token: 0x04000148 RID: 328
			OpenTrack = 147,
			// Token: 0x04000149 RID: 329
			Jump,
			// Token: 0x0400014A RID: 330
			Call,
			// Token: 0x0400014B RID: 331
			Random = 160,
			// Token: 0x0400014C RID: 332
			Variable,
			// Token: 0x0400014D RID: 333
			If,
			// Token: 0x0400014E RID: 334
			Setvar = 176,
			// Token: 0x0400014F RID: 335
			Addvar,
			// Token: 0x04000150 RID: 336
			Subvar,
			// Token: 0x04000151 RID: 337
			Mulvar,
			// Token: 0x04000152 RID: 338
			Divvar,
			// Token: 0x04000153 RID: 339
			Shiftvar,
			// Token: 0x04000154 RID: 340
			Randvar,
			// Token: 0x04000155 RID: 341
			CmpEq = 184,
			// Token: 0x04000156 RID: 342
			CmpGe,
			// Token: 0x04000157 RID: 343
			CmpGt,
			// Token: 0x04000158 RID: 344
			CmpLe,
			// Token: 0x04000159 RID: 345
			CmpLt,
			// Token: 0x0400015A RID: 346
			CmpNe,
			// Token: 0x0400015B RID: 347
			Pan = 192,
			// Token: 0x0400015C RID: 348
			Volume,
			// Token: 0x0400015D RID: 349
			MainVolume,
			// Token: 0x0400015E RID: 350
			Transpose,
			// Token: 0x0400015F RID: 351
			PitchBend,
			// Token: 0x04000160 RID: 352
			BendRange,
			// Token: 0x04000161 RID: 353
			Prio,
			// Token: 0x04000162 RID: 354
			NoteWait,
			// Token: 0x04000163 RID: 355
			Tie,
			// Token: 0x04000164 RID: 356
			Porta,
			// Token: 0x04000165 RID: 357
			ModDepth,
			// Token: 0x04000166 RID: 358
			ModSpeed,
			// Token: 0x04000167 RID: 359
			ModType,
			// Token: 0x04000168 RID: 360
			ModRange,
			// Token: 0x04000169 RID: 361
			PortaSw,
			// Token: 0x0400016A RID: 362
			PortaTime,
			// Token: 0x0400016B RID: 363
			Attack,
			// Token: 0x0400016C RID: 364
			Decay,
			// Token: 0x0400016D RID: 365
			Sustain,
			// Token: 0x0400016E RID: 366
			Release,
			// Token: 0x0400016F RID: 367
			LoopStart,
			// Token: 0x04000170 RID: 368
			Volume2,
			// Token: 0x04000171 RID: 369
			Printvar,
			// Token: 0x04000172 RID: 370
			Mute,
			// Token: 0x04000173 RID: 371
			ModDelay = 224,
			// Token: 0x04000174 RID: 372
			Tempo,
			// Token: 0x04000175 RID: 373
			SweepPitch = 227,
			// Token: 0x04000176 RID: 374
			LoopEnd = 252,
			// Token: 0x04000177 RID: 375
			Ret,
			// Token: 0x04000178 RID: 376
			AllocTrack,
			// Token: 0x04000179 RID: 377
			Fin
		}
	}
}
