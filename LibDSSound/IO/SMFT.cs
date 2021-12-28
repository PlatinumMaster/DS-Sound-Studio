using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibDSSound.IO
{
	// Token: 0x0200003E RID: 62
	public class SMFT
	{
		// Token: 0x060000DF RID: 223 RVA: 0x00009968 File Offset: 0x00007B68
		public static string ToSMFT(SSEQ Seq)
		{
			int[] labels = GetLabels(Seq);
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			while (i < Seq.Data.Length)
			{
				if (labels.Contains(i))
				{
					stringBuilder.AppendFormat("\nLabel_0x{0:X8}:\n", i);
				}
				string text = "";
				bool flag = false;
				byte b = Seq.Data[i++];
				if (b == 162)
				{
					b = Seq.Data[i++];
					text = "_if" + text;
				}
				byte type = 0;
				if (b == 160)
				{
					b = Seq.Data[i++];
					text = "_r" + text;
					type = 3;
					flag = true;
				}
				if (b == 161)
				{
					b = Seq.Data[i++];
					text = "_v" + text;
					type = 4;
					flag = true;
				}
				if (b < 128)
				{
					if (b == 0 && i >= Seq.Data.Length - 4)
					{
						break;
					}
					byte b2 = Seq.Data[i++];
					int num = b % 12;
					int num2 = b / 12 - 1;
					stringBuilder.AppendFormat("    {0}{1} {2},", Notes[num], (num2 < 0 ? "m1" : string.Concat(num2)) + text, b2);
					if (flag)
					{
						WriteArgOverride(stringBuilder, Seq.Data, ref i, type);
					}
					else
					{
						stringBuilder.AppendFormat(" {0}\n", ReadArg(Seq.Data, ref i, 2));
					}
				}
				else
				{
					int num3 = b & 240;
					if (num3 <= 176)
					{
						if (num3 != 128)
						{
							if (num3 != 144)
							{
								if (num3 == 176)
								{
									switch (b)
									{
									case 176:
										stringBuilder.Append("    setvar" + text);
										break;
									case 177:
										stringBuilder.Append("    addvar" + text);
										break;
									case 178:
										stringBuilder.Append("    subvar" + text);
										break;
									case 179:
										stringBuilder.Append("    mulvar" + text);
										break;
									case 180:
										stringBuilder.Append("    divvar" + text);
										break;
									case 181:
										stringBuilder.Append("    shiftvar" + text);
										break;
									case 182:
										stringBuilder.Append("    randvar" + text);
										break;
									case 184:
										stringBuilder.Append("    cmp_eq" + text);
										break;
									case 185:
										stringBuilder.Append("    cmp_ge" + text);
										break;
									case 186:
										stringBuilder.Append("    cmp_gt" + text);
										break;
									case 187:
										stringBuilder.Append("    cmp_le" + text);
										break;
									case 188:
										stringBuilder.Append("    cmp_lt" + text);
										break;
									case 189:
										stringBuilder.Append("    cmp_ne" + text);
										break;
									}
									int num4 = Seq.Data[i++];
									stringBuilder.AppendFormat(" {0},", num4);
									if (flag)
									{
										WriteArgOverride(stringBuilder, Seq.Data, ref i, type);
									}
									else
									{
										stringBuilder.AppendFormat(" {0}\n", (short)ReadArg(Seq.Data, ref i, 1));
									}
								}
							}
							else
							{
								switch (b)
								{
								case 147:
								{
									int num5 = Seq.Data[i++];
									int num6 = Seq.Data[i++] | Seq.Data[i++] << 8 | Seq.Data[i++] << 16;
									stringBuilder.AppendFormat("    opentrack{0} {1}, Label_0x{2:X8}\n", text, num5, num6);
									break;
								}
								case 148:
								{
									int num6 = Seq.Data[i++] | Seq.Data[i++] << 8 | Seq.Data[i++] << 16;
									stringBuilder.AppendFormat("    jump{0} Label_0x{1:X8}\n", text, num6);
									break;
								}
								case 149:
								{
									int num6 = Seq.Data[i++] | Seq.Data[i++] << 8 | Seq.Data[i++] << 16;
									stringBuilder.AppendFormat("    call{0} Label_0x{1:X8}\n", text, num6);
									break;
								}
								}
							}
						}
						else
						{
							short num7 = -1;
							if (!flag)
							{
								num7 = (short)ReadArg(Seq.Data, ref i, 2);
							}
							if (b == 128)
							{
								stringBuilder.Append("    wait" + text);
							}
							else if (b == 129)
							{
								stringBuilder.Append("    prg" + text);
							}
							if (flag)
							{
								WriteArgOverride(stringBuilder, Seq.Data, ref i, type);
							}
							else
							{
								stringBuilder.AppendFormat(" {0}\n", num7);
							}
						}
					}
					else if (num3 <= 208)
					{
						if (num3 == 192 || num3 == 208)
						{
							int num8 = 0;
							if (!flag)
							{
								num8 = ReadArg(Seq.Data, ref i, 0);
							}
							switch (b)
							{
							case 192:
								stringBuilder.Append("    pan" + text);
								break;
							case 193:
								stringBuilder.Append("    volume" + text);
								break;
							case 194:
								stringBuilder.Append("    main_volume" + text);
								break;
							case 195:
								stringBuilder.Append("    transpose" + text);
								break;
							case 196:
								stringBuilder.Append("    pitchbend" + text);
								break;
							case 197:
								stringBuilder.Append("    bendrange" + text);
								break;
							case 198:
								stringBuilder.Append("    prio" + text);
								break;
							case 199:
								stringBuilder.AppendFormat("    notewait_{0}", ((num8 & 1) == 1 ? "on" : "off") + text);
								break;
							case 200:
								stringBuilder.AppendFormat("    tie{0}", ((num8 & 1) == 1 ? "on" : "off") + text);
								break;
							case 201:
								stringBuilder.Append("    porta" + text);
								break;
							case 202:
								stringBuilder.Append("    mod_depth" + text);
								break;
							case 203:
								stringBuilder.Append("    mod_speed" + text);
								break;
							case 204:
								stringBuilder.Append("    mod_type" + text);
								break;
							case 205:
								stringBuilder.Append("    mod_range" + text);
								break;
							case 206:
								stringBuilder.AppendFormat("    porta_{0}", (num8 & 1) == 1 ? "on" : "off");
								break;
							case 207:
								stringBuilder.Append("    porta_time" + text);
								break;
							case 208:
								stringBuilder.Append("    attack" + text);
								break;
							case 209:
								stringBuilder.Append("    decay" + text);
								break;
							case 210:
								stringBuilder.Append("    sustain" + text);
								break;
							case 211:
								stringBuilder.Append("    release" + text);
								break;
							case 212:
								stringBuilder.Append("    loop_start" + text);
								break;
							case 213:
								stringBuilder.Append("    volume2" + text);
								break;
							case 214:
								stringBuilder.Append("    printvar" + text);
								break;
							case 215:
								stringBuilder.Append("    mute" + text);
								break;
							}
							if (flag)
							{
								WriteArgOverride(stringBuilder, Seq.Data, ref i, type);
							}
							else if (b == 195 || b == 196)
							{
								stringBuilder.AppendFormat(" {0}\n", (sbyte)num8);
							}
							else if (b != 199 && b != 200 && b != 206)
							{
								stringBuilder.AppendFormat(" {0}\n", num8);
							}
							else
							{
								stringBuilder.AppendLine();
							}
						}
					}
					else if (num3 != 224)
					{
						if (num3 == 240)
						{
							switch (b)
							{
							case 252:
								stringBuilder.AppendLine("    loop_end" + text);
								break;
							case 253:
								stringBuilder.AppendLine("    ret" + text);
								break;
							case 254:
								stringBuilder.AppendFormat("    alloctrack{0} 0x{1:X4}\n", text, ReadArg(Seq.Data, ref i, 1));
								break;
							case 255:
								stringBuilder.AppendLine("    fin" + text);
								break;
							}
						}
					}
					else
					{
						switch (b)
						{
						case 224:
							stringBuilder.Append("    mod_delay" + text);
							break;
						case 225:
							stringBuilder.Append("    tempo" + text);
							break;
						case 227:
							stringBuilder.Append("    sweep_pitch" + text);
							break;
						}
						if (flag)
						{
							WriteArgOverride(stringBuilder, Seq.Data, ref i, type);
						}
						else if (b == 227)
						{
							stringBuilder.AppendFormat(" {0}\n", (short)ReadArg(Seq.Data, ref i, 1));
						}
						else
						{
							stringBuilder.AppendFormat(" {0}\n", (ushort)ReadArg(Seq.Data, ref i, 1));
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000A40C File Offset: 0x0000860C
		private static int[] GetLabels(SSEQ Seq)
		{
			int i = 0;
			List<int> list = new List<int>();
			while (i < Seq.Data.Length)
			{
				bool flag = false;
				byte b = Seq.Data[i++];
				if (b == 162)
				{
					b = Seq.Data[i++];
				}
				byte b2 = 0;
				if (b == 160)
				{
					b = Seq.Data[i++];
					b2 = 3;
					flag = true;
				}
				if (b == 161)
				{
					b = Seq.Data[i++];
					b2 = 4;
					flag = true;
				}
				if (b < 128)
				{
					if (b == 0 && i >= Seq.Data.Length - 4)
					{
						break;
					}
					byte b3 = Seq.Data[i++];
					byte type = 2;
					if (flag)
					{
						type = b2;
					}
					int num = ReadArg(Seq.Data, ref i, type);
				}
				else
				{
					int num2 = b & 240;
					if (num2 <= 176)
					{
						if (num2 != 128)
						{
							if (num2 != 144)
							{
								if (num2 == 176)
								{
									int num3 = Seq.Data[i++];
									int type2 = 1;
									if (flag)
									{
										type2 = b2;
									}
									short num4 = (short)ReadArg(Seq.Data, ref i, type2);
								}
							}
							else
							{
								switch (b)
								{
								case 147:
								{
									int num5 = Seq.Data[i++];
									int item = Seq.Data[i++] | Seq.Data[i++] << 8 | Seq.Data[i++] << 16;
									if (!list.Contains(item))
									{
										list.Add(item);
									}
									break;
								}
								case 148:
								{
									int item = Seq.Data[i++] | Seq.Data[i++] << 8 | Seq.Data[i++] << 16;
									if (!list.Contains(item))
									{
										list.Add(item);
									}
									break;
								}
								case 149:
								{
									int item = Seq.Data[i++] | Seq.Data[i++] << 8 | Seq.Data[i++] << 16;
									if (!list.Contains(item))
									{
										list.Add(item);
									}
									break;
								}
								}
							}
						}
						else
						{
							int type2 = 2;
							if (flag)
							{
								type2 = b2;
							}
							int num6 = (short)ReadArg(Seq.Data, ref i, type2);
						}
					}
					else if (num2 <= 208)
					{
						if (num2 == 192 || num2 == 208)
						{
							int type2 = 0;
							if (flag)
							{
								type2 = b2;
							}
							int num6 = ReadArg(Seq.Data, ref i, type2);
						}
					}
					else if (num2 != 224)
					{
						if (num2 == 240)
						{
							switch (b)
							{
							case 254:
								i += 2;
								break;
							}
						}
					}
					else
					{
						int type2 = 1;
						if (flag)
						{
							type2 = b2;
						}
						short num4 = (short)ReadArg(Seq.Data, ref i, type2);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000A78C File Offset: 0x0000898C
		private static int ReadArg(byte[] Data, ref int Offset, int type)
		{
			int result;
			switch (type)
			{
			case 0:
				result = Data[Offset++];
				break;
			case 1:
				result = Data[Offset++] | Data[Offset++] << 8;
				break;
			case 2:
			{
				int num = 0;
				byte b;
				do
				{
					b = Data[Offset++];
					num = num << 7 | b & 127;
				}
				while ((b & 128) != 0);
				result = num;
				break;
			}
			case 3:
			{
				short num2 = (short)(Data[Offset++] | Data[Offset++] << 8);
				ushort num3 = (ushort)(Data[Offset++] | Data[Offset++] << 8);
				result = 0;
				break;
			}
			case 4:
			{
				byte b2 = Data[Offset++];
				result = 0;
				break;
			}
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000A888 File Offset: 0x00008A88
		private static void WriteArgOverride(StringBuilder b, byte[] Data, ref int Offset, int type)
		{
			switch (type)
			{
			case 3:
			{
				short num = (short)(Data[Offset++] | Data[Offset++] << 8);
				ushort num2 = (ushort)(Data[Offset++] | Data[Offset++] << 8);
				b.AppendFormat(" {0}, {1}\n", num, num2);
				break;
			}
			case 4:
				b.AppendFormat(" {0}\n", Data[Offset++]);
				break;
			}
		}

		// Token: 0x04000195 RID: 405
		private static readonly string[] Notes = {
			"cn",
			"cs",
			"dn",
			"ds",
			"en",
			"fn",
			"fs",
			"gn",
			"gs",
			"an",
			"as",
			"bn"
		};
	}
}
