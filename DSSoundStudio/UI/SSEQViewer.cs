using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using DSSoundStudio.Properties;
using FastColoredTextBoxNS;
using LibDSSound.IO;
using LibDSSound.Software;

namespace DSSoundStudio.UI
{
	// Token: 0x02000004 RID: 4
	public partial class SSEQViewer : Form
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00004A80 File Offset: 0x00002C80
		public SSEQViewer(SDAT SoundArchive, int SeqIdx)
		{
			this.SoundArchive = SoundArchive;
			SeqInfo = SoundArchive.InfoBlock.SequenceInfos[SeqIdx];
			Sequence = new SSEQ(SoundArchive.GetFileData(SeqInfo.FileId));
			BnkInfo = SoundArchive.InfoBlock.BankInfos[SeqInfo.Bank];
			Bank = new SBNK(SoundArchive.GetFileData(BnkInfo.FileId));
			for (int i = 0; i < 4; i++)
			{
				if (BnkInfo.WaveArchives[i] != 65535)
				{
					Bank.AssignWaveArc(i, new SWAR(SoundArchive.GetFileData(SoundArchive.InfoBlock.WaveArchiveInfos[BnkInfo.WaveArchives[i]].FileId)));
				}
			}
			InitializeComponent();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00004BF8 File Offset: 0x00002DF8
		private void SSEQViewer_Load(object sender, EventArgs e)
		{
			fastColoredTextBox1.Text = SMFT.ToSMFT(Sequence);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00004C14 File Offset: 0x00002E14
		private void SoundThreadMain()
		{
			MainForm.waveOut.Play();
			SNDWork sndwork = new SNDWork();
			sndwork.ExChannelInit();
			sndwork.SeqInit();
			sndwork.StartSeq(0, Sequence.Data, 0, Bank);
			Player player = sndwork.Players[0];
			player.Volume = SeqInfo.Volume;
			while (!Stop)
			{
				if (Playing && MainForm.bufferedWaveProvider.BufferedBytes < MainForm.bufferedWaveProvider.BufferLength && MainForm.bufferedWaveProvider.BufferLength - MainForm.bufferedWaveProvider.BufferedBytes > 1364)
				{
					sndwork.UpdateExChannel();
					sndwork.SeqMain(play: true);
					sndwork.ExChannelMain(doUpdate: true);
					LibDSSound.Software.Util.CalcRandom();
					for (int i = 0; i < 341; i++)
					{
						sndwork.Hardware.Evaluate(256, out var Left, out var Right);
						MainForm.bufferedWaveProvider.AddSamples(new byte[4]
						{
							(byte)((uint)Left & 0xFFu),
							(byte)((uint)(Left >> 8) & 0xFFu),
							(byte)((uint)Right & 0xFFu),
							(byte)((uint)(Right >> 8) & 0xFFu)
						}, 0, 4);
					}
				}
			}
			MainForm.waveOut.Stop();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00004DB4 File Offset: 0x00002FB4
		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (!Playing)
			{
				if (Stop)
				{
					Stop = false;
					new Thread(SoundThreadMain).Start();
				}
				toolStripButton1.Image = Resources.control_pause;
			}
			else
			{
				toolStripButton1.Image = Resources.control;
			}
			Playing = !Playing;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00004E30 File Offset: 0x00003030
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			Playing = false;
			Stop = true;
			toolStripButton1.Image = Resources.control;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00004E52 File Offset: 0x00003052
		private void SSEQViewer_FormClosing(object sender, FormClosingEventArgs e)
		{
			Playing = false;
			Stop = true;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00004E64 File Offset: 0x00003064
		private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
		{
			e.ChangedRange.ClearStyle(CommentStyle, Label, Keyword, Red, Magenta);
			e.ChangedRange.SetStyle(CommentStyle, ";.*$", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(Label, "^.*:", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(Label, "(?<=^\\s*(opentrack\\s*\\d*\\s*,\\s*|call|jump_if|jump)).*$", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(Keyword, "(?<=^\\s*)(alloctrack|opentrack|call|jump|ret|fin)", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(Red, "(?<=^\\s*)(pan|bendrange|pitchbend|main_volume|volume2|volume)", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(Magenta, "(?<=^\\s*)(notewait_off|notewait_on|wait|tempo)", RegexOptions.Multiline);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00004F44 File Offset: 0x00003144
		private void fastColoredTextBox1_AutoIndentNeeded(object sender, AutoIndentEventArgs e)
		{
			if (new Regex("^.*:", RegexOptions.Multiline).Matches(e.LineText).Count > 0)
			{
				e.AbsoluteIndentation = 0;
				e.ShiftNextLines = 4;
			}
		}

		// Token: 0x04000013 RID: 19
		private const int SAMPLE_TIMER = 256;

		// Token: 0x04000014 RID: 20
		private const int SAMPLE_RATE = 65456;

		// Token: 0x04000015 RID: 21
		private bool Playing;

		// Token: 0x04000016 RID: 22
		private bool Stop = true;

		// Token: 0x04000017 RID: 23
		private SDAT SoundArchive;

		// Token: 0x04000018 RID: 24
		private SDAT.INFO.SequenceInfo SeqInfo;

		// Token: 0x04000019 RID: 25
		private SSEQ Sequence;

		// Token: 0x0400001A RID: 26
		private SDAT.INFO.BankInfo BnkInfo;

		// Token: 0x0400001B RID: 27
		private SBNK Bank;

		// Token: 0x0400001C RID: 28
		private Style CommentStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);

		// Token: 0x0400001D RID: 29
		private Style Label = new TextStyle(new SolidBrush(Color.FromArgb(43, 145, 175)), null, FontStyle.Regular);

		// Token: 0x0400001E RID: 30
		private Style Keyword = new TextStyle(Brushes.Blue, null, FontStyle.Regular);

		// Token: 0x0400001F RID: 31
		private Style Red = new TextStyle(Brushes.Red, null, FontStyle.Regular);

		// Token: 0x04000020 RID: 32
		private Style Magenta = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
	}
}
