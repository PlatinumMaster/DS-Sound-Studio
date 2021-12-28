using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DSSoundStudio.IO;
using DSSoundStudio.Util;
using LibDSSound.IO;
using LibDSSound.Software;
using NAudio.Wave;

namespace DSSoundStudio.UI
{
	// Token: 0x02000005 RID: 5
	public partial class SBNKViewer : Form
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00005FF4 File Offset: 0x000041F4
		public SBNKViewer(SDAT SoundArchive, int BnkIdx)
		{
			this.SoundArchive = SoundArchive;
			BnkInfo = SoundArchive.InfoBlock.BankInfos[BnkIdx];
			Bank = new SBNK(SoundArchive.GetFileData(BnkInfo.FileId));
			for (int i = 0; i < 4; i++)
			{
				if (BnkInfo.WaveArchives[i] != 65535)
				{
					Bank.AssignWaveArc(i, new SWAR(SoundArchive.GetFileData(SoundArchive.InfoBlock.WaveArchiveInfos[BnkInfo.WaveArchives[i]].FileId)));
				}
			}
			InitializeComponent();
			Win32Util.SetWindowTheme(treeView1.Handle, "explorer", null);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000060EE File Offset: 0x000042EE
		private void SBNKViewer_Load(object sender, EventArgs e)
		{
			UpdateTree();
			Playing = true;
			Stop = false;
			new Thread(SoundThreadMain).Start();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00006120 File Offset: 0x00004320
		private void UpdateTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			int num = 0;
			foreach (SBNK.Instrument instrument in Bank.Instruments)
			{
				if (instrument.Type != InstData.InstType.Invalid)
				{
					treeView1.Nodes.Add("Instrument " + num).Tag = num;
				}
				num++;
			}
			treeView1.EndUpdate();
			if (treeView1.Nodes.Count > 0)
			{
				treeView1.SelectedNode = treeView1.Nodes[0];
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00006260 File Offset: 0x00004460
		private void SoundThreadMain()
		{
			BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(65456, 16, 2));
			bufferedWaveProvider.DiscardOnBufferOverflow = true;
			bufferedWaveProvider.BufferLength = 21824;
			WaveOut waveOut = new WaveOut();
			waveOut.DesiredLatency = 150;
			waveOut.Init(bufferedWaveProvider);
			waveOut.Play();
			SNDWork sndwork = new SNDWork();
			sndwork.ExChannelInit();
			sndwork.SeqInit();
			List<ExChannel> usechan = new List<ExChannel>();
			List<long> chantag = new List<long>();
			while (!Stop)
			{
				if (Playing && bufferedWaveProvider.BufferedBytes < bufferedWaveProvider.BufferLength && bufferedWaveProvider.BufferLength - bufferedWaveProvider.BufferedBytes > 1364)
				{
					sndwork.UpdateExChannel();
					foreach (ExChannel exChannel in usechan)
					{
						if (exChannel.Length > 0)
						{
							exChannel.Length--;
						}
						else if (exChannel.Length == 0)
						{
							exChannel.Priority = 1;
							exChannel.ReleaseChannel();
						}
					}
					while (commandQueue.Count > 0)
					{
						SimpleSoundCommand c = commandQueue.Dequeue();
						switch (c.cmd)
						{
						case SimpleSoundCommand.Command.NoteOn:
						{
							InstData instData = Bank.ReadInstData(c.arg1, (byte)c.arg2);
							if (instData != null)
							{
								ushort chBitMask;
								switch (instData.Type)
								{
								case InstData.InstType.Pcm:
								case InstData.InstType.DirectPcm:
									chBitMask = ushort.MaxValue;
									break;
								case InstData.InstType.Psg:
									chBitMask = 16128;
									break;
								case InstData.InstType.Noise:
									chBitMask = 49152;
									break;
								default:
									return;
								}
								ExChannel exChannel2 = sndwork.AllocExChannel(chBitMask, 64, false, delegate(ExChannel Channel, ExChannel.ExChannelCallbackStatus status, object CallbackData)
								{
									if (status == ExChannel.ExChannelCallbackStatus.Finish)
									{
										Channel.Priority = 0;
										Channel.Free();
									}
									usechan.Remove(Channel);
									chantag.Remove(c.arg3);
								}, null);
								if (exChannel2 != null)
								{
									if (!Sequence.NoteOn(exChannel2, (byte)c.arg2, 127, -1, Bank, instData))
									{
										exChannel2.Priority = 0;
										exChannel2.Free();
									}
									else
									{
										usechan.Add(exChannel2);
										chantag.Add(c.arg3);
									}
								}
							}
							break;
						}
						case SimpleSoundCommand.Command.Release:
						{
							int num = chantag.IndexOf(c.arg3);
							if (num >= 0)
							{
								usechan[num].Priority = 1;
								usechan[num].ReleaseChannel();
							}
							break;
						}
						}
					}
					sndwork.SeqMain(true);
					sndwork.ExChannelMain(true);
					LibDSSound.Software.Util.CalcRandom();
					for (int i = 0; i < 341; i++)
					{
						short num2;
						short num3;
						sndwork.Hardware.Evaluate(256, out num2, out num3);
						bufferedWaveProvider.AddSamples(new[]
						{
							(byte)(num2 & 255),
							(byte)(num2 >> 8 & 255),
							(byte)(num3 & 255),
							(byte)(num3 >> 8 & 255)
						}, 0, 4);
					}
				}
			}
			waveOut.Stop();
			waveOut.Dispose();
			waveOut = null;
			bufferedWaveProvider = null;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00006640 File Offset: 0x00004840
		private void SBNKViewer_FormClosing(object sender, FormClosingEventArgs e)
		{
			Playing = false;
			Stop = true;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00006654 File Offset: 0x00004854
		private void pianoControl1_NoteDown(int key)
		{
			curtag = DateTime.Now.Ticks;
			commandQueue.Enqueue(new SimpleSoundCommand
			{
				cmd = SimpleSoundCommand.Command.NoteOn,
				arg1 = (int)treeView1.SelectedNode.Tag,
				arg2 = key,
				arg3 = curtag
			});
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000066C4 File Offset: 0x000048C4
		private void pianoControl1_NoteUp(int key)
		{
			commandQueue.Enqueue(new SimpleSoundCommand
			{
				cmd = SimpleSoundCommand.Command.Release,
				arg3 = curtag
			});
			curtag = -1L;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00006704 File Offset: 0x00004904
		private void toolStripButtonExport_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
			{
				SWAR[] array = new SWAR[4];
				for (int i = 0; i < 4; i++)
				{
					if (BnkInfo.WaveArchives[i] != 65535)
					{
						array[i] = new SWAR(SoundArchive.GetFileData(SoundArchive.InfoBlock.WaveArchiveInfos[BnkInfo.WaveArchives[i]].FileId));
					}
				}
				INST inst = new INST(Bank.Instruments[(int)treeView1.SelectedNode.Tag], array);
				File.Create(saveFileDialog1.FileName).Close();
				File.WriteAllBytes(saveFileDialog1.FileName, inst.Write());
			}
		}

		// Token: 0x0400002D RID: 45
		private const int SAMPLE_TIMER = 256;

		// Token: 0x0400002E RID: 46
		private const int SAMPLE_RATE = 65456;

		// Token: 0x0400003A RID: 58
		private SDAT SoundArchive;

		// Token: 0x0400003B RID: 59
		private SDAT.INFO.BankInfo BnkInfo;

		// Token: 0x0400003C RID: 60
		private SBNK Bank;

		// Token: 0x0400003D RID: 61
		private bool Playing;

		// Token: 0x0400003E RID: 62
		private bool Stop = true;

		// Token: 0x0400003F RID: 63
		private Queue<SimpleSoundCommand> commandQueue = new Queue<SimpleSoundCommand>();

		// Token: 0x04000040 RID: 64
		private long curtag = -1L;

		// Token: 0x02000006 RID: 6
		private struct SimpleSoundCommand
		{
			// Token: 0x04000041 RID: 65
			public Command cmd;

			// Token: 0x04000042 RID: 66
			public int arg1;

			// Token: 0x04000043 RID: 67
			public int arg2;

			// Token: 0x04000044 RID: 68
			public long arg3;

			// Token: 0x02000007 RID: 7
			public enum Command
			{
				// Token: 0x04000046 RID: 70
				NoteOn,
				// Token: 0x04000047 RID: 71
				Release
			}
		}
	}
}
