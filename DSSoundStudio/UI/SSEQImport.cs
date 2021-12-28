using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LibDSSound.IO;
using LibDSSound.Software;

namespace DSSoundStudio.UI
{
	// Token: 0x0200001A RID: 26
	public partial class SSEQImport : Form
	{
		// Token: 0x0600007E RID: 126 RVA: 0x000099F6 File Offset: 0x00007BF6
		public SSEQImport()
		{
			InitializeComponent();
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00009A0F File Offset: 0x00007C0F
		private void SSEQImport_Load(object sender, EventArgs e)
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00009A14 File Offset: 0x00007C14
		private void button1_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
			{
				try
				{
					DonorSDAT = new SDAT(File.ReadAllBytes(openFileDialog1.FileName));
				}
				catch
				{
					MessageBox.Show("Invalid SDAT!");
					DonorSDAT = null;
					textBox1.Text = "";
					comboBox1.Items.Clear();
					UpdateEnabled();
					return;
				}
				textBox1.Text = openFileDialog1.FileName;
				comboBox1.BeginUpdate();
				comboBox1.Items.Clear();
				int num = 0;
				foreach (SDAT.INFO.SequenceInfo sequenceInfo in DonorSDAT.InfoBlock.SequenceInfos.Entries)
				{
					if (sequenceInfo != null)
					{
						if (DonorSDAT.SymbolBlock != null && DonorSDAT.SymbolBlock.SequenceSymbols.Entries[num] != null)
						{
							comboBox1.Items.Add(new ComboboxItem
							{
								Id = num,
								Text = num + ": " + DonorSDAT.SymbolBlock.SequenceSymbols.Entries[num]
							});
						}
						else
						{
							comboBox1.Items.Add(new ComboboxItem
							{
								Id = num,
								Text = string.Concat(num)
							});
						}
					}
					num++;
				}
				comboBox1.EndUpdate();
				comboBox1.SelectedIndex = 0;
				UpdateEnabled();
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00009C54 File Offset: 0x00007E54
		private void UpdateEnabled()
		{
			comboBox1.Enabled = textBox1.Text != "" && DonorSDAT != null;
			button2.Enabled = textBox1.Text != "" && DonorSDAT != null && comboBox1.SelectedIndex >= 0;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00009CE8 File Offset: 0x00007EE8
		public void Import(SDAT Target, int SequenceId)
		{
			SDAT.INFO.SequenceInfo sequenceInfo = DonorSDAT.InfoBlock.SequenceInfos[((ComboboxItem)comboBox1.SelectedItem).Id];
			Target.FileAllocationTable.Entries[(int)Target.InfoBlock.SequenceInfos[SequenceId].FileId].Data = DonorSDAT.GetFileData(sequenceInfo.FileId);
			if (checkBox1.Checked)
			{
				Target.InfoBlock.SequenceInfos[SequenceId].Volume = sequenceInfo.Volume;
			}
			SBNK sbnk = new SBNK(DonorSDAT.GetFileData(DonorSDAT.InfoBlock.BankInfos[sequenceInfo.Bank].FileId));
			SWAR[] array = new SWAR[4];
			for (int i = 0; i < 4; i++)
			{
				if (DonorSDAT.InfoBlock.BankInfos[sequenceInfo.Bank].WaveArchives[i] != 65535)
				{
					array[i] = new SWAR(DonorSDAT.GetFileData(DonorSDAT.InfoBlock.WaveArchiveInfos[DonorSDAT.InfoBlock.BankInfos[sequenceInfo.Bank].WaveArchives[i]].FileId));
				}
			}
			SWAR swar = MergeCleanBank(sbnk, array);
			int num = Target.InfoBlock.SequenceInfos[SequenceId].Bank;
			int usageForBank = Target.GetUsageForBank(num);
			if (usageForBank > 1)
			{
				num = Target.InfoBlock.BankInfos.Entries.Count;
				Target.InfoBlock.BankInfos.Entries.Add(new SDAT.INFO.BankInfo
				{
					FileId = (uint)Target.FileAllocationTable.Entries.Count
				});
				Target.FileAllocationTable.Entries.Add(new SDAT.FAT.FATEntry());
				if (Target.SymbolBlock != null)
				{
					Target.SymbolBlock.BankSymbols.Entries.Add("Bank_" + num);
				}
			}
			Target.InfoBlock.SequenceInfos[SequenceId].Bank = (ushort)num;
			Target.FileAllocationTable.Entries[(int)Target.InfoBlock.BankInfos[num].FileId].Data = sbnk.Write();
			if (swar == null)
			{
				Target.InfoBlock.BankInfos[num].WaveArchives = new[]
				{
					ushort.MaxValue,
					ushort.MaxValue,
					ushort.MaxValue,
					ushort.MaxValue
				};
			}
			else
			{
				SDAT.INFO.BankInfo bankInfo = Target.InfoBlock.BankInfos[num];
				ushort[] array2 = {
					0,
					ushort.MaxValue,
					ushort.MaxValue,
					ushort.MaxValue
				};
				array2[0] = (ushort)Target.InfoBlock.WaveArchiveInfos.Entries.Count;
				bankInfo.WaveArchives = array2;
				Target.InfoBlock.WaveArchiveInfos.Entries.Add(new SDAT.INFO.WaveArchiveInfo
				{
					FileId = (uint)Target.FileAllocationTable.Entries.Count
				});
				Target.FileAllocationTable.Entries.Add(new SDAT.FAT.FATEntry());
				Target.FileAllocationTable.Entries[(int)Target.InfoBlock.WaveArchiveInfos[Target.InfoBlock.BankInfos[num].WaveArchives[0]].FileId].Data = swar.Write();
				if (Target.SymbolBlock != null)
				{
					Target.SymbolBlock.WaveArchiveSymbols.Entries.Add("WaveArc_" + Target.InfoBlock.BankInfos[num].WaveArchives[0]);
				}
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000A0EC File Offset: 0x000082EC
		private SWAR MergeCleanBank(SBNK Bank, SWAR[] Waves)
		{
			Dictionary<uint, Tuple<int, WaveData>> dictionary = new Dictionary<uint, Tuple<int, WaveData>>();
			int num = 0;
			foreach (SBNK.Instrument instrument in Bank.Instruments)
			{
				if (instrument.Type == InstData.InstType.Pcm)
				{
					if (!dictionary.ContainsKey((uint)(((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[1] << 16 | ((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[0])))
					{
						dictionary.Add((uint)(((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[1] << 16 | ((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[0]), new Tuple<int, WaveData>(num++, Waves[((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[1]].Waves[((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[0]]));
					}
					((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[0] = (ushort)dictionary[(uint)(((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[1] << 16 | ((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[0])].Item1;
					((SBNK.SimpleInstrumentParam)instrument.Param).Param.Wave[1] = 0;
				}
				else if (instrument.Type == InstData.InstType.DrumSet)
				{
					foreach (InstData instData in ((SBNK.DrumSetParam)instrument.Param).SubInstruments)
					{
						if (instData.Type == InstData.InstType.Pcm)
						{
							if (!dictionary.ContainsKey((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])))
							{
								dictionary.Add((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0]), new Tuple<int, WaveData>(num++, Waves[instData.Param.Wave[1]].Waves[instData.Param.Wave[0]]));
							}
							instData.Param.Wave[0] = (ushort)dictionary[(uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])].Item1;
							instData.Param.Wave[1] = 0;
						}
					}
				}
				else if (instrument.Type == InstData.InstType.KeySplit)
				{
					foreach (InstData instData in ((SBNK.KeySplitParam)instrument.Param).SubInstruments)
					{
						if (instData.Type == InstData.InstType.Pcm)
						{
							if (!dictionary.ContainsKey((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])))
							{
								dictionary.Add((uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0]), new Tuple<int, WaveData>(num++, Waves[instData.Param.Wave[1]].Waves[instData.Param.Wave[0]]));
							}
							instData.Param.Wave[0] = (ushort)dictionary[(uint)(instData.Param.Wave[1] << 16 | instData.Param.Wave[0])].Item1;
							instData.Param.Wave[1] = 0;
						}
					}
				}
			}
			Tuple<int, WaveData>[] array = dictionary.Values.ToArray();
			WaveData[] array2 = new WaveData[array.Length];
			for (int k = 0; k < array.Length; k++)
			{
				array2[array[k].Item1] = array[k].Item2;
			}
			bool flag = 0 == 0;
			SWAR swar = new SWAR();
			swar.Waves = new WaveData[array2.Length];
			for (int k = 0; k < array2.Length; k++)
			{
				swar.Waves[k] = array2[k];
			}
			return swar;
		}

		// Token: 0x040000AB RID: 171
		public SDAT DonorSDAT;

		// Token: 0x0200001B RID: 27
		private struct ComboboxItem
		{
			// Token: 0x06000086 RID: 134 RVA: 0x0000AB2C File Offset: 0x00008D2C
			public override string ToString()
			{
				return Text;
			}

			// Token: 0x040000B6 RID: 182
			public string Text;

			// Token: 0x040000B7 RID: 183
			public int Id;
		}
	}
}
