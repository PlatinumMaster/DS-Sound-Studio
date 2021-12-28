// LibDSSound.Software.SNDWork
using System;
using LibDSSound.Hardware;
using LibDSSound.IO;
using LibDSSound.Software;

public class SNDWork
{
	private class SoundVar
	{
		public short Value { get; set; }
	}

	private static readonly int[] ChannelOrder = new int[16]
	{
		4, 5, 6, 7, 2, 0, 3, 1, 8, 9,
		10, 11, 14, 12, 15, 13
	};

	private uint sLockChannel;

	private uint sWeakLockChannel;

	private byte[] sOrgPan = new byte[16];

	private byte[] sOrgVolume = new byte[16];

	private int sMasterPan = -1;

	private uint sSurroundDecay = 0u;

	private bool sMmlPrintEnable = true;

	public DSSoundHardware Hardware = new DSSoundHardware();

	private SoundVar[] globalVars = new SoundVar[16];

	private SoundVar[][] localVars = new SoundVar[16][];

	private int[] TickCounters = new int[16];

	private int playerStatus;

	private ExChannel[] Channels = new ExChannel[16];

	public Player[] Players = new Player[16];

	private Track[] Tracks = new Track[32];

	public SNDWork()
	{
		for (int i = 0; i < 16; i++)
		{
			Channels[i] = new ExChannel();
			Players[i] = new Player();
			localVars[i] = new SoundVar[16];
			for (int j = 0; j < 16; j++)
			{
				localVars[i][j] = new SoundVar();
			}
		}
		for (int k = 0; k < 32; k++)
		{
			Tracks[k] = new Track();
		}
	}

	public void SeqInit()
	{
		for (int i = 0; i < 16; i++)
		{
			Players[i].Active = false;
			Players[i].MyNo = (byte)i;
		}
		for (int j = 0; j < 32; j++)
		{
			Tracks[j].Active = false;
		}
	}

	public void SeqMain(bool play)
	{
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			if (!Players[i].Active)
			{
				continue;
			}
			if (Players[i].Prepared)
			{
				if (play && !Players[i].Paused)
				{
					PlayerTempoMain(Players[i]);
				}
				UpdatePlayerChannel(Players[i]);
			}
			num |= 1 << i;
		}
		playerStatus = num;
	}

	private void PrepareSeq(int playerNr, byte[] data, int offset, SBNK bank)
	{
		if (Players[playerNr].Active)
		{
			FinishPlayer(Players[playerNr]);
		}
		InitPlayer(Players[playerNr], bank);
		int num = AllocTrack();
		if (num >= 0)
		{
			Tracks[num].Init();
			Tracks[num].Start(data, offset);
			Players[playerNr].Tracks[0] = (byte)num;
			if (data[Tracks[num].DataOffset++] == 254)
			{
				ushort num2 = (ushort)((data[Tracks[num].DataOffset++] | (data[Tracks[num].DataOffset++] << 8)) >> 1);
				int num3 = 1;
				while (num2 != 0)
				{
					if (((uint)num2 & (true ? 1u : 0u)) != 0)
					{
						int num4 = AllocTrack();
						if (num4 < 0)
						{
							break;
						}
						Tracks[num4].Init();
						Players[playerNr].Tracks[num3] = (byte)num4;
					}
					num3++;
					num2 = (ushort)(num2 >> 1);
				}
			}
			else
			{
				Tracks[num].DataOffset--;
			}
			Players[playerNr].Active = true;
			Players[playerNr].Prepared = false;
		}
		playerStatus |= 1 << playerNr;
	}

	private void StartPreparedSeq(int playerNr)
	{
		Players[playerNr].Prepared = true;
	}

	public void StartSeq(int playerNr, byte[] data, int offset, SBNK bank)
	{
		PrepareSeq(playerNr, data, offset, bank);
		StartPreparedSeq(playerNr);
	}

	public void StopSeq(int playerNr)
	{
		if (Players[playerNr].Active)
		{
			FinishPlayer(Players[playerNr]);
			playerStatus &= ~(1 << playerNr);
		}
	}

	public void PauseSeq(int playerNr, bool pause)
	{
		Players[playerNr].Paused = pause;
		if (!pause)
		{
			return;
		}
		for (int i = 0; i < 16; i++)
		{
			Track playerTrack = GetPlayerTrack(Players[playerNr], i);
			if (playerTrack != null)
			{
				playerTrack.ReleaseChannelAll(Players[playerNr], 127);
				playerTrack.FreeChannelAll();
			}
		}
	}

	private void InitPlayer(Player player, SBNK bank)
	{
		player.Paused = false;
		player.Bank = bank;
		player.Tempo = 120;
		player.TempoRatio = 256;
		player.TempoCounter = 240;
		player.Volume = 127;
		player.ExtendedFader = 0;
		player.Priority = 64;
		for (int i = 0; i < 16; i++)
		{
			player.Tracks[i] = byte.MaxValue;
		}
		TickCounters[player.MyNo] = 0;
		for (int j = 0; j < 16; j++)
		{
			localVars[player.MyNo][j].Value = -1;
		}
	}

	private int ReadArg(Track track, Player player, int type)
	{
		switch (type)
		{
		case 0:
			return track.Data[track.DataOffset++];
		case 1:
			return track.Data[track.DataOffset++] | (track.Data[track.DataOffset++] << 8);
		case 2:
		{
			int num3 = 0;
			byte b;
			do
			{
				b = track.Data[track.DataOffset++];
				num3 = (num3 << 7) | (b & 0x7F);
			}
			while ((b & 0x80u) != 0);
			return num3;
		}
		case 3:
		{
			short num = (short)(track.Data[track.DataOffset++] | (track.Data[track.DataOffset++] << 8));
			ushort num2 = (ushort)(track.Data[track.DataOffset++] | (track.Data[track.DataOffset++] << 8));
			return (Util.CalcRandom() * (num2 - (ushort)num + 1) >> 16) + num;
		}
		case 4:
		{
			byte var = track.Data[track.DataOffset++];
			return GetVariablePtr(player, var)?.Value ?? 0;
		}
		default:
			return 0;
		}
	}

	private void PlayerTempoMain(Player player)
	{
		int num = 0;
		while (player.TempoCounter >= 240)
		{
			player.TempoCounter -= 240;
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			if (PlayerSeqMain(player, play: true))
			{
				FinishPlayer(player);
				break;
			}
		}
		TickCounters[player.MyNo] += num;
		player.TempoCounter += (ushort)(player.Tempo * player.TempoRatio / 256);
	}

	private Track GetPlayerTrack(Player player, int trackIdx)
	{
		if (trackIdx > 15 || player.Tracks[trackIdx] == byte.MaxValue)
		{
			return null;
		}
		return Tracks[player.Tracks[trackIdx]];
	}

	private void ClosePlayerTrack(Player player, int track)
	{
		Track playerTrack = GetPlayerTrack(player, track);
		if (playerTrack != null)
		{
			playerTrack.Close(player);
			playerTrack.Active = false;
			player.Tracks[track] = byte.MaxValue;
		}
	}

	private void FinishPlayer(Player player)
	{
		for (int i = 0; i < 16; i++)
		{
			ClosePlayerTrack(player, i);
		}
		player.Active = false;
	}

	private void ChannelCallback(ExChannel Channel, ExChannel.ExChannelCallbackStatus status, object CallbackData)
	{
		if (status == ExChannel.ExChannelCallbackStatus.Finish)
		{
			Channel.Priority = 0;
			Channel.Free();
		}
		Track track = (Track)CallbackData;
		ExChannel exChannel = track.ChannelList;
		if (exChannel == Channel)
		{
			track.ChannelList = Channel.Next;
			return;
		}
		while (exChannel.Next != null)
		{
			if (exChannel.Next == Channel)
			{
				exChannel.Next = Channel.Next;
				break;
			}
			exChannel = exChannel.Next;
		}
	}

	private void UpdatePlayerChannel(Player player)
	{
		for (int i = 0; i < 16; i++)
		{
			GetPlayerTrack(player, i)?.UpdateChannel(player, 1);
		}
	}

	public void NoteOnCommandProc(Track track, Player player, byte key, byte velocity, int length)
	{
		ExChannel exChannel = null;
		if (track.Tie && track.ChannelList != null)
		{
			exChannel = track.ChannelList;
			exChannel.Key = key;
			exChannel.Velocity = velocity;
		}
		else
		{
			InstData instData = player.Bank.ReadInstData(track.ProgramNumber, key);
			if (instData != null)
			{
				ushort num;
				switch (instData.Type)
				{
				default:
					return;
				case InstData.InstType.Pcm:
				case InstData.InstType.DirectPcm:
					num = ushort.MaxValue;
					break;
				case InstData.InstType.Psg:
					num = 16128;
					break;
				case InstData.InstType.Noise:
					num = 49152;
					break;
				}
				exChannel = AllocExChannel((uint)(num & track.ChannelMask), (byte)(track.Priority + player.Priority), track.HasChannelMask, ChannelCallback, track);
				if (exChannel != null)
				{
					if (track.Tie)
					{
						length = -1;
					}
					if (!Sequence.NoteOn(exChannel, key, velocity, length, player.Bank, instData))
					{
						exChannel.Priority = 0;
						exChannel.Free();
						return;
					}
					exChannel.Next = track.ChannelList;
					track.ChannelList = exChannel;
				}
			}
		}
		if (exChannel != null)
		{
			if (track.Attack != byte.MaxValue)
			{
				exChannel.SetAttack(track.Attack);
			}
			if (track.Decay != byte.MaxValue)
			{
				exChannel.SetDecay(track.Decay);
			}
			if (track.Sustain != byte.MaxValue)
			{
				exChannel.SetSustain(track.Sustain);
			}
			if (track.Release != byte.MaxValue)
			{
				exChannel.SetRelease(track.Release);
			}
			exChannel.SweepPitch = track.SweepPitch;
			if (track.Portamento)
			{
				ExChannel exChannel2 = exChannel;
				exChannel2.SweepPitch += (short)(track.PortamentoKey - key << 6);
			}
			if (track.PortamentoTime != 0)
			{
				exChannel.SweepLength = (short)(track.PortamentoTime * track.PortamentoTime) * Math.Abs(exChannel.SweepPitch) >> 11;
			}
			else
			{
				exChannel.SweepLength = length;
				exChannel.AutoSweep = false;
			}
			exChannel.SweepCounter = 0;
		}
	}

	public int TrackSeqMain(Track track, Player player, int trackIdx, bool play)
	{
		for (ExChannel exChannel = track.ChannelList; exChannel != null; exChannel = exChannel.Next)
		{
			if (exChannel.Length > 0)
			{
				exChannel.Length--;
			}
			if (!exChannel.AutoSweep && exChannel.SweepCounter < exChannel.SweepLength)
			{
				exChannel.SweepCounter++;
			}
		}
		if (track.NoteFinishWait)
		{
			if (track.ChannelList != null)
			{
				return 0;
			}
			track.NoteFinishWait = false;
		}
		if (track.Wait > 0)
		{
			track.Wait--;
			if (track.Wait > 0)
			{
				return 0;
			}
		}
		while (track.Wait == 0 && !track.NoteFinishWait)
		{
			bool flag = false;
			bool flag2 = true;
			byte b = track.Data[track.DataOffset++];
			if (b == 162)
			{
				b = track.Data[track.DataOffset++];
				flag2 = track.CompareFlag;
			}
			byte b2 = 0;
			if (b == 160)
			{
				b = track.Data[track.DataOffset++];
				b2 = 3;
				flag = true;
			}
			if (b == 161)
			{
				b = track.Data[track.DataOffset++];
				b2 = 4;
				flag = true;
			}
			if (b < 128)
			{
				byte velocity = track.Data[track.DataOffset++];
				byte type = 2;
				if (flag)
				{
					type = b2;
				}
				int num = ReadArg(track, player, type);
				int num2 = b + (byte)track.Transpose;
				if (!flag2)
				{
					continue;
				}
				if (num2 < 0)
				{
					num2 = 0;
				}
				else if (num2 > 127)
				{
					num2 = 127;
				}
				if (!track.Mute && play)
				{
					NoteOnCommandProc(track, player, (byte)num2, velocity, (num <= 0) ? (-1) : num);
				}
				track.PortamentoKey = (byte)num2;
				if (track.NoteWait)
				{
					track.Wait = num;
					if (num == 0)
					{
						track.NoteFinishWait = true;
					}
				}
				continue;
			}
			int num3 = b & 0xF0;
			if (num3 <= 176)
			{
				switch (num3)
				{
				case 176:
				{
					int var = track.Data[track.DataOffset++];
					int type3 = 1;
					if (flag)
					{
						type3 = b2;
					}
					short num5 = (short)ReadArg(track, player, type3);
					SoundVar variablePtr = GetVariablePtr(player, var);
					if (!flag2 || variablePtr == null)
					{
						break;
					}
					switch (b)
					{
					case 176:
						variablePtr.Value = num5;
						break;
					case 177:
						variablePtr.Value += num5;
						break;
					case 178:
						variablePtr.Value -= num5;
						break;
					case 179:
						variablePtr.Value *= num5;
						break;
					case 180:
						if (num5 != 0)
						{
							variablePtr.Value /= num5;
						}
						break;
					case 181:
						if (num5 < 0)
						{
							SoundVar soundVar = variablePtr;
							soundVar.Value = (short)(soundVar.Value >> -num5);
						}
						else
						{
							variablePtr.Value <<= (int)num5;
						}
						break;
					case 182:
					{
						bool flag3 = false;
						if (num5 < 0)
						{
							flag3 = true;
							num5 = (short)(-num5);
						}
						short num6 = (short)(Util.CalcRandom() * (ushort)(num5 + 1) >> 16);
						if (flag3)
						{
							num6 = (short)(-num6);
						}
						variablePtr.Value = num6;
						break;
					}
					case 184:
						track.CompareFlag = variablePtr.Value == num5;
						break;
					case 185:
						track.CompareFlag = variablePtr.Value >= num5;
						break;
					case 186:
						track.CompareFlag = variablePtr.Value > num5;
						break;
					case 187:
						track.CompareFlag = variablePtr.Value <= num5;
						break;
					case 188:
						track.CompareFlag = variablePtr.Value < num5;
						break;
					case 189:
						track.CompareFlag = variablePtr.Value != num5;
						break;
					}
					break;
				}
				case 144:
					switch (b)
					{
					case 147:
					{
						int trackIdx2 = track.Data[track.DataOffset++];
						int offset = track.Data[track.DataOffset++] | (track.Data[track.DataOffset++] << 8) | (track.Data[track.DataOffset++] << 16);
						if (flag2)
						{
							Track playerTrack = GetPlayerTrack(player, trackIdx2);
							if (playerTrack != null && playerTrack != track)
							{
								playerTrack.Close(player);
								playerTrack.Start(track.Data, offset);
							}
						}
						break;
					}
					case 148:
					{
						int dataOffset2 = track.Data[track.DataOffset++] | (track.Data[track.DataOffset++] << 8) | (track.Data[track.DataOffset++] << 16);
						if (flag2)
						{
							track.DataOffset = dataOffset2;
						}
						break;
					}
					case 149:
					{
						int dataOffset = track.Data[track.DataOffset++] | (track.Data[track.DataOffset++] << 8) | (track.Data[track.DataOffset++] << 16);
						if (flag2 && track.CallStackDepth < 3)
						{
							track.CallStack[track.CallStackDepth] = track.DataOffset;
							track.CallStackDepth++;
							track.DataOffset = dataOffset;
						}
						break;
					}
					}
					break;
				case 128:
				{
					int type2 = 2;
					if (flag)
					{
						type2 = b2;
					}
					int num4 = (short)ReadArg(track, player, type2);
					if (!flag2)
					{
						break;
					}
					switch (b)
					{
					case 128:
						track.Wait = num4;
						break;
					case 129:
						if (num4 < 65536)
						{
							track.ProgramNumber = (ushort)num4;
						}
						break;
					}
					break;
				}
				}
				continue;
			}
			if (num3 <= 208)
			{
				if (num3 != 192 && num3 != 208)
				{
					continue;
				}
				int type4 = 0;
				if (flag)
				{
					type4 = b2;
				}
				int num7 = ReadArg(track, player, type4);
				if (!flag2)
				{
					continue;
				}
				switch (b)
				{
				case 192:
					track.Pan = (sbyte)(num7 - 64);
					break;
				case 193:
					track.Volume = (byte)num7;
					break;
				case 194:
					player.Volume = (byte)num7;
					break;
				case 195:
					track.Transpose = (sbyte)num7;
					break;
				case 196:
					track.PitchBend = (sbyte)num7;
					break;
				case 197:
					track.BendRange = (byte)num7;
					break;
				case 198:
					track.Priority = (byte)num7;
					break;
				case 199:
					track.NoteWait = (num7 & 1) == 1;
					break;
				case 200:
					track.Tie = (num7 & 1) == 1;
					track.ReleaseChannelAll(player, -1);
					track.FreeChannelAll();
					break;
				case 201:
					track.PortamentoKey = (byte)(num7 + track.Transpose);
					track.Portamento = true;
					break;
				case 202:
					track.Modulation.Depth = (byte)num7;
					break;
				case 203:
					track.Modulation.Speed = (byte)num7;
					break;
				case 204:
					track.Modulation.Target = (Lfo.LfoParam.LfoTarget)num7;
					break;
				case 205:
					track.Modulation.Range = (byte)num7;
					break;
				case 206:
					track.Portamento = (num7 & 1) == 1;
					break;
				case 207:
					track.PortamentoTime = (byte)num7;
					break;
				case 208:
					track.Attack = (byte)num7;
					break;
				case 209:
					track.Decay = (byte)num7;
					break;
				case 210:
					track.Sustain = (byte)num7;
					break;
				case 211:
					track.Release = (byte)num7;
					break;
				case 212:
					if (track.CallStackDepth < 3)
					{
						track.CallStack[track.CallStackDepth] = track.DataOffset;
						track.LoopCount[track.CallStackDepth] = (byte)num7;
						track.CallStackDepth++;
					}
					break;
				case 213:
					track.Volume2 = (byte)num7;
					break;
				case 214:
					if (sMmlPrintEnable)
					{
						int value = GetVariablePtr(player, num7).Value;
						Console.WriteLine("#{0}[{1}]: printvar No.{2} = {3}", player.MyNo, trackIdx, num7, value);
					}
					break;
				case 215:
					track.SetMute(player, num7);
					break;
				}
				continue;
			}
			switch (num3)
			{
			case 240:
				if (!flag2)
				{
					break;
				}
				switch (b)
				{
				case 252:
				{
					if (track.CallStackDepth == 0)
					{
						break;
					}
					int num9 = track.LoopCount[track.CallStackDepth - 1];
					if (num9 != 0)
					{
						num9--;
						if (num9 == 0)
						{
							track.CallStackDepth--;
							break;
						}
					}
					track.LoopCount[track.CallStackDepth - 1] = (byte)num9;
					track.DataOffset = track.CallStack[track.CallStackDepth - 1];
					break;
				}
				case 253:
					if (track.CallStackDepth != 0)
					{
						track.CallStackDepth--;
						track.DataOffset = track.CallStack[track.CallStackDepth];
					}
					break;
				case byte.MaxValue:
					return -1;
				}
				break;
			case 224:
			{
				int type5 = 1;
				if (flag)
				{
					type5 = b2;
				}
				short num8 = (short)ReadArg(track, player, type5);
				if (flag2)
				{
					switch (b)
					{
					case 224:
						track.Modulation.Delay = (ushort)num8;
						break;
					case 225:
						player.Tempo = (ushort)num8;
						break;
					case 227:
						track.SweepPitch = num8;
						break;
					}
				}
				break;
			}
			}
		}
		return 0;
	}

	private bool PlayerSeqMain(Player player, bool play)
	{
		bool flag = false;
		for (int i = 0; i < 16; i++)
		{
			Track playerTrack = GetPlayerTrack(player, i);
			if (playerTrack != null && playerTrack.Data != null)
			{
				if (TrackSeqMain(playerTrack, player, i, play) != 0)
				{
					ClosePlayerTrack(player, i);
				}
				else
				{
					flag = true;
				}
			}
		}
		return !flag;
	}

	private SoundVar GetVariablePtr(Player player, int var)
	{
		if (var < 16)
		{
			return localVars[player.MyNo][var];
		}
		return globalVars[var - 16];
	}

	private int AllocTrack()
	{
		for (int i = 0; i < 32; i++)
		{
			if (!Tracks[i].Active)
			{
				Tracks[i].Active = true;
				return i;
			}
		}
		return -1;
	}

	public void ExChannelInit()
	{
		for (int i = 0; i < 16; i++)
		{
			Channels[i].MyNo = (byte)i;
			Channels[i].SyncFlag = 0;
			Channels[i].Active = false;
		}
		sLockChannel = 0u;
		sWeakLockChannel = 0u;
	}

	public void UpdateExChannel()
	{
		for (int i = 0; i < 16; i++)
		{
			if (Channels[i].SyncFlag == 0)
			{
				continue;
			}
			if ((Channels[i].SyncFlag & 2u) != 0)
			{
				StopChannel(i, hold: false);
			}
			if (((uint)Channels[i].SyncFlag & (true ? 1u : 0u)) != 0)
			{
				switch (Channels[i].Type)
				{
				case ExChannel.ExChannelType.Pcm:
					SetupChannelPcm(i, Channels[i].Data, Channels[i].Wave.Format, Channels[i].Wave.Loop ? Channel.RepeatMode.Repeat : Channel.RepeatMode.OneShot, Channels[i].Wave.LoopStart, Channels[i].Wave.LoopLen, (byte)(Channels[i].Volume & 0xFFu), (Channel.DataShift)(Channels[i].Volume >> 8), Channels[i].Timer, Channels[i].Pan);
					break;
				case ExChannel.ExChannelType.Psg:
					SetupChannelPsg(i, Channels[i].Duty, (byte)(Channels[i].Volume & 0xFFu), (Channel.DataShift)(Channels[i].Volume >> 8), Channels[i].Timer, Channels[i].Pan);
					break;
				case ExChannel.ExChannelType.Noise:
					SetupChannelNoise(i, (byte)(Channels[i].Volume & 0xFFu), (Channel.DataShift)(Channels[i].Volume >> 8), Channels[i].Timer, Channels[i].Pan);
					break;
				}
			}
			else
			{
				if ((Channels[i].SyncFlag & 4u) != 0)
				{
					SetChannelTimer(i, Channels[i].Timer);
				}
				if ((Channels[i].SyncFlag & 8u) != 0)
				{
					SetChannelVolume(i, (byte)(Channels[i].Volume & 0xFFu), (Channel.DataShift)(Channels[i].Volume >> 8));
				}
				if ((Channels[i].SyncFlag & 0x10u) != 0)
				{
					SetChannelPan(i, Channels[i].Pan);
				}
			}
		}
		for (int j = 0; j < 16; j++)
		{
			if (Channels[j].SyncFlag != 0)
			{
				if (((uint)Channels[j].SyncFlag & (true ? 1u : 0u)) != 0)
				{
					Hardware.Channels[j].Enabled = true;
				}
				Channels[j].SyncFlag = 0;
			}
		}
	}

	public void ExChannelMain(bool doUpdate)
	{
		for (int i = 0; i < 16; i++)
		{
			int num = 0;
			if (!Channels[i].Active)
			{
				continue;
			}
			if (Channels[i].Started)
			{
				ExChannel exChannel = Channels[i];
				exChannel.SyncFlag |= 1;
				Channels[i].Started = false;
			}
			else if (!IsChannelActive(i))
			{
				if (Channels[i].Callback != null)
				{
					Channels[i].Callback(Channels[i], ExChannel.ExChannelCallbackStatus.Finish, Channels[i].CallbackData);
				}
				else
				{
					Channels[i].Priority = 0;
				}
				Channels[i].Volume = 0;
				Channels[i].Active = false;
				continue;
			}
			int num2 = Channels[i].Key - Channels[i].OriginalKey << 6;
			int num3 = Util.DecibelSquareTable[Channels[i].Velocity] + Channels[i].UpdateEnvelope(doUpdate);
			int num4 = Channels[i].SweepMain(doUpdate);
			int num5 = num3 + Channels[i].UserDecay + Channels[i].UserDecay2;
			int num6 = num2 + num4 + Channels[i].UserPitch;
			int num7 = Channels[i].LfoMain(doUpdate);
			switch (Channels[i].Lfo.Param.Target)
			{
			case Lfo.LfoParam.LfoTarget.Pitch:
				num6 += num7;
				break;
			case Lfo.LfoParam.LfoTarget.Volume:
				if (num5 > -32768)
				{
					num5 += num7;
				}
				break;
			case Lfo.LfoParam.LfoTarget.Pan:
				num = num7;
				break;
			}
			int num8 = num + Channels[i].InitPan;
			if (Channels[i].PanRange != 127)
			{
				num8 = (num8 * Channels[i].PanRange + 64) / 128;
			}
			int num9 = num8 + Channels[i].UserPan;
			if (Channels[i].EnvelopeStatus != ExChannel.SoundEnvelopeStatus.Release || num5 > -723)
			{
				ushort num10 = Util.CalcChannelVolume(num5);
				ushort num11 = Util.CalcTimer(Channels[i].Wave.Timer, num6);
				if (Channels[i].Type == ExChannel.ExChannelType.Psg)
				{
					num11 = (ushort)(num11 & 0xFFFCu);
				}
				int num12 = num9 + 64;
				if (num12 < 0)
				{
					num12 = 0;
				}
				else if (num12 > 127)
				{
					num12 = 127;
				}
				if (num10 != Channels[i].Volume)
				{
					Channels[i].Volume = num10;
					ExChannel exChannel2 = Channels[i];
					exChannel2.SyncFlag |= 8;
				}
				if (num11 != Channels[i].Timer)
				{
					Channels[i].Timer = num11;
					ExChannel exChannel3 = Channels[i];
					exChannel3.SyncFlag |= 4;
				}
				if (num12 != Channels[i].Pan)
				{
					Channels[i].Pan = (byte)num12;
					ExChannel exChannel4 = Channels[i];
					exChannel4.SyncFlag |= 16;
				}
			}
			else
			{
				Channels[i].SyncFlag = 2;
				if (Channels[i].Callback != null)
				{
					Channels[i].Callback(Channels[i], ExChannel.ExChannelCallbackStatus.Finish, Channels[i].CallbackData);
				}
				else
				{
					Channels[i].Priority = 0;
				}
				Channels[i].Volume = 0;
				Channels[i].Active = false;
			}
		}
	}

	public ExChannel AllocExChannel(uint chBitMask, byte prio, bool strongRequest, ExChannel.ExChannelCallback callback, object callbackData)
	{
		uint num = chBitMask & ~sLockChannel;
		if (!strongRequest)
		{
			num &= ~sWeakLockChannel;
		}
		ExChannel exChannel = null;
		for (int i = 0; i < 16; i++)
		{
			int num2 = ChannelOrder[i];
			if ((num & (1L << (num2 & 0x1F))) == 0)
			{
				continue;
			}
			if (exChannel != null)
			{
				int priority = exChannel.Priority;
				int priority2 = Channels[num2].Priority;
				if (priority2 <= priority && (priority2 != priority || ExChannel.CompareVolume(exChannel, Channels[num2]) < 0))
				{
					exChannel = Channels[num2];
				}
			}
			else
			{
				exChannel = Channels[num2];
			}
		}
		if (exChannel != null && prio >= exChannel.Priority)
		{
			if (exChannel.Callback != null)
			{
				exChannel.Callback(exChannel, ExChannel.ExChannelCallbackStatus.Drop, exChannel.CallbackData);
			}
			exChannel.SyncFlag = 1;
			exChannel.Active = false;
			exChannel.InitAlloc(callback, callbackData, prio);
			return exChannel;
		}
		return null;
	}

	public void StopUnlockedChannel(uint chBitMask)
	{
		for (int i = 0; i < 16; i++)
		{
			if ((chBitMask & (true ? 1u : 0u)) != 0 && (sLockChannel & (1L << (i & 0x1F))) == 0)
			{
				if (Channels[i].Callback != null)
				{
					Channels[i].Callback(Channels[i], ExChannel.ExChannelCallbackStatus.Drop, Channels[i].CallbackData);
				}
				StopChannel(i, hold: false);
				Channels[i].Priority = 0;
				Channels[i].Free();
				Channels[i].SyncFlag = 0;
				Channels[i].Active = false;
			}
			chBitMask >>= 1;
			if (chBitMask == 0)
			{
				break;
			}
		}
	}

	public void LockChannel(uint chBitMask, uint flags)
	{
		uint num = chBitMask;
		for (int i = 0; i < 16; i++)
		{
			if ((chBitMask & (true ? 1u : 0u)) != 0 && (sLockChannel & (1L << (i & 0x1F))) == 0)
			{
				if (Channels[i].Callback != null)
				{
					Channels[i].Callback(Channels[i], ExChannel.ExChannelCallbackStatus.Drop, Channels[i].CallbackData);
				}
				StopChannel(i, hold: false);
				Channels[i].Priority = 0;
				Channels[i].Free();
				Channels[i].SyncFlag = 0;
				Channels[i].Active = false;
			}
			chBitMask >>= 1;
			if (chBitMask == 0)
			{
				break;
			}
		}
		if ((flags & (true ? 1u : 0u)) != 0)
		{
			sWeakLockChannel |= num;
		}
		else
		{
			sLockChannel |= num;
		}
	}

	public void UnlockChannel(uint chBitMask, uint flags)
	{
		if ((flags & (true ? 1u : 0u)) != 0)
		{
			sWeakLockChannel &= ~chBitMask;
		}
		else
		{
			sLockChannel &= ~chBitMask;
		}
	}

	public uint GetLockedChannel(uint flags)
	{
		if ((flags & (true ? 1u : 0u)) != 0)
		{
			return sWeakLockChannel;
		}
		return sLockChannel;
	}

	public void SetupChannelPcm(int channel, byte[] data, Channel.SoundFormat format, Channel.RepeatMode repeat, ushort loopStart, uint loopLen, byte volume, Channel.DataShift shift, ushort timer, byte pan)
	{
		sOrgPan[channel] = pan;
		if (sMasterPan >= 0)
		{
			pan = (byte)sMasterPan;
		}
		sOrgVolume[channel] = volume;
		if (sSurroundDecay != 0 && ((uint)(1 << channel) & 0xFFF5u) != 0)
		{
			volume = CalcSurroundDecay(volume, pan);
		}
		Hardware.Channels[channel].Pan = pan;
		Hardware.Channels[channel].Volume = volume;
		Hardware.Channels[channel].Shift = shift;
		Hardware.Channels[channel].Timer = (ushort)(-timer);
		Hardware.Channels[channel].Format = format;
		Hardware.Channels[channel].Data = data;
		switch (format)
		{
		case Channel.SoundFormat.PCM8:
		case Channel.SoundFormat.PCM16:
			Hardware.Channels[channel].DataPosition = -3;
			break;
		case Channel.SoundFormat.ADPCM:
			Hardware.Channels[channel].DataPosition = -11;
			break;
		case Channel.SoundFormat.PSG_NOISE:
			Hardware.Channels[channel].DataPosition = -1;
			break;
		}
		Hardware.Channels[channel].Hold = false;
		Hardware.Channels[channel].LoopStart = loopStart;
		Hardware.Channels[channel].Length = loopLen;
		Hardware.Channels[channel].Repeat = repeat;
		Hardware.Channels[channel].Enabled = false;
		Hardware.Channels[channel].Duty = Channel.PSGDuty.Duty1_8;
		Hardware.Channels[channel].ADPCMDecoder = null;
		Hardware.Channels[channel].Counter = 0;
	}

	public void SetupChannelPsg(int channel, Channel.PSGDuty duty, byte volume, Channel.DataShift shift, ushort timer, byte pan)
	{
		sOrgPan[channel] = pan;
		if (sMasterPan >= 0)
		{
			pan = (byte)sMasterPan;
		}
		sOrgVolume[channel] = volume;
		if (sSurroundDecay != 0 && ((uint)(1 << channel) & 0xFFF5u) != 0)
		{
			volume = CalcSurroundDecay(volume, pan);
		}
		Hardware.Channels[channel].Pan = pan;
		Hardware.Channels[channel].Volume = volume;
		Hardware.Channels[channel].Shift = shift;
		Hardware.Channels[channel].Format = Channel.SoundFormat.PSG_NOISE;
		Hardware.Channels[channel].LoopStart = 0;
		Hardware.Channels[channel].Length = 0u;
		Hardware.Channels[channel].Duty = duty;
		Hardware.Channels[channel].Data = null;
		Hardware.Channels[channel].DataPosition = -1;
		Hardware.Channels[channel].Repeat = Channel.RepeatMode.Manual;
		Hardware.Channels[channel].Hold = false;
		Hardware.Channels[channel].Enabled = false;
		Hardware.Channels[channel].Timer = (ushort)(-timer);
		Hardware.Channels[channel].PSGCounter = 0;
		Hardware.Channels[channel].Counter = 0;
	}

	public void SetupChannelNoise(int channel, byte volume, Channel.DataShift shift, ushort timer, byte pan)
	{
		sOrgPan[channel] = pan;
		if (sMasterPan >= 0)
		{
			pan = (byte)sMasterPan;
		}
		sOrgVolume[channel] = volume;
		if (sSurroundDecay != 0 && ((uint)(1 << channel) & 0xFFF5u) != 0)
		{
			volume = CalcSurroundDecay(volume, pan);
		}
		Hardware.Channels[channel].Pan = pan;
		Hardware.Channels[channel].Volume = volume;
		Hardware.Channels[channel].Shift = shift;
		Hardware.Channels[channel].Format = Channel.SoundFormat.PSG_NOISE;
		Hardware.Channels[channel].LoopStart = 0;
		Hardware.Channels[channel].Length = 0u;
		Hardware.Channels[channel].Duty = Channel.PSGDuty.Duty1_8;
		Hardware.Channels[channel].Data = null;
		Hardware.Channels[channel].DataPosition = -1;
		Hardware.Channels[channel].Repeat = Channel.RepeatMode.Manual;
		Hardware.Channels[channel].Hold = false;
		Hardware.Channels[channel].Enabled = false;
		Hardware.Channels[channel].Timer = (ushort)(-timer);
		Hardware.Channels[channel].NoiseCounter = 32767;
		Hardware.Channels[channel].Counter = 0;
	}

	public void StopChannel(int channel, bool hold)
	{
		Hardware.Channels[channel].Enabled = false;
		Hardware.Channels[channel].Hold = hold;
	}

	public void SetChannelVolume(int channel, byte volume, Channel.DataShift shift)
	{
		sOrgVolume[channel] = volume;
		if (sSurroundDecay != 0 && ((uint)(1 << channel) & 0xFFF5u) != 0)
		{
			volume = CalcSurroundDecay(volume, Hardware.Channels[channel].Pan);
		}
		Hardware.Channels[channel].Volume = volume;
		Hardware.Channels[channel].Shift = shift;
	}

	public void SetChannelTimer(int channel, ushort timer)
	{
		Hardware.Channels[channel].Timer = (ushort)(-timer);
	}

	public void SetChannelPan(int channel, byte pan)
	{
		sOrgPan[channel] = pan;
		if (sMasterPan >= 0)
		{
			pan = (byte)sMasterPan;
		}
		Hardware.Channels[channel].Pan = pan;
		if (sSurroundDecay != 0 && ((uint)(1 << channel) & 0xFFF5u) != 0)
		{
			Hardware.Channels[channel].Volume = CalcSurroundDecay(sOrgVolume[channel], pan);
		}
	}

	public bool IsChannelActive(int channel)
	{
		return Hardware.Channels[channel].Enabled;
	}

	private byte CalcSurroundDecay(int volume, int pan)
	{
		if (pan < 24)
		{
			return (byte)(volume * (sSurroundDecay * (pan + 40) + (long)((ulong)(32767 - sSurroundDecay) << 6)) >> 21);
		}
		if (pan >= 24 && pan <= 104)
		{
			return (byte)volume;
		}
		return (byte)(volume * ((0L - (long)sSurroundDecay) * (pan - 40) + (long)((ulong)(sSurroundDecay + 32767) << 6)) >> 21);
	}
}
