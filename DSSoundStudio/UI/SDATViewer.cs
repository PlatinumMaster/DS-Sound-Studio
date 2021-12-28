using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DSSoundStudio.Properties;
using DSSoundStudio.Util;
using LibDSSound.IO;

namespace DSSoundStudio.UI
{
	// Token: 0x02000003 RID: 3
	public partial class SDATViewer : Form, ISaveable
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public SDATViewer(string SDATPath)
		{
			this.SDATPath = SDATPath;
			SoundArchive = new SDAT(File.ReadAllBytes(SDATPath));
			InitializeComponent();
			Win32Util.SetWindowTheme(treeView1.Handle, "explorer", null);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020B0 File Offset: 0x000002B0
		private void SDATViewer_Load(object sender, EventArgs e)
		{
			ImageList1 = new ImageList();
			ImageList1.ImageSize = new Size(16, 16);
			ImageList1.ColorDepth = ColorDepth.Depth32Bit;
			ImageList1.Images.Add(Resources.folder_open);
			treeView1.ImageList = ImageList1;
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			TreeNode treeNode = new TreeNode("/", 0, 0);
			TreeNode selectedNode = treeNode.Nodes.Add("seq", "Sequences", 0, 0);
			treeNode.Nodes.Add("seqarc", "Sequence Archives", 0, 0);
			treeNode.Nodes.Add("bank", "Banks", 0, 0);
			treeNode.Nodes.Add("wavearc", "Wave Archives", 0, 0);
			treeNode.Nodes.Add("strm", "Streams", 0, 0);
			treeNode.Nodes.Add("player", "Players", 0, 0);
			treeNode.Nodes.Add("strmplayer", "Stream Players", 0, 0);
			treeNode.Nodes.Add("group", "Groups", 0, 0);
			treeView1.Nodes.Add(treeNode);
			treeNode.Expand();
			treeView1.EndUpdate();
			treeView1.SelectedNode = selectedNode;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002230 File Offset: 0x00000430
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			string name = e.Node.Name;
			switch (name)
			{
			case "seq":
			case "seqarc":
			case "bank":
			case "wavearc":
			case "strm":
			case "player":
			case "strmplayer":
			case "group":
				CurSel = e.Node.Name;
				UpdateListView(CurSel);
				break;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002320 File Offset: 0x00000520
		private void UpdateListView(string type)
		{
			listViewNF1.BeginUpdate();
			if (type != null)
			{
				switch (type)
				{
					case "seq":
					{
						listViewNF1.Columns.Clear();
						listViewNF1.Columns.Add("Id");
						if (SoundArchive.SymbolBlock != null)
						{
							listViewNF1.Columns.Add("Label");
						}

						listViewNF1.Columns.Add("File Id");
						listViewNF1.Columns.Add("Bank");
						listViewNF1.Columns.Add("Volume");
						listViewNF1.Columns.Add("Channel Priority");
						listViewNF1.Columns.Add("Player Priority");
						listViewNF1.Columns.Add("Player Nr");
						listViewNF1.Items.Clear();
						int num2 = 0;
						foreach (SDAT.INFO.SequenceInfo sequenceInfo in SoundArchive.InfoBlock.SequenceInfos
							.Entries)
						{
							if (sequenceInfo != null)
							{
								ListViewItem listViewItem = new ListViewItem(num2.ToString());
								if (SoundArchive.SymbolBlock != null)
								{
									listViewItem.SubItems.Add(
										SoundArchive.SymbolBlock.SequenceSymbols.Entries[num2]);
								}

								listViewItem.SubItems.Add(string.Concat(sequenceInfo.FileId));
								listViewItem.SubItems.Add(sequenceInfo.Bank + (SoundArchive.SymbolBlock != null
									? " (" +
									  SoundArchive.SymbolBlock.BankSymbols.Entries[sequenceInfo.Bank] + ")"
									: ""));
								listViewItem.SubItems.Add(string.Concat(sequenceInfo.Volume));
								listViewItem.SubItems.Add(string.Concat(sequenceInfo.ChannelPriority));
								listViewItem.SubItems.Add(string.Concat(sequenceInfo.PlayerPriority));
								listViewItem.SubItems.Add(string.Concat(sequenceInfo.PlayerNr));
								listViewItem.Tag = num2;
								listViewNF1.Items.Add(listViewItem);
							}

							num2++;
						}

						break;
					}
					case "seqarc":
					{
						listViewNF1.Columns.Clear();
						listViewNF1.Columns.Add("Id");
						if (SoundArchive.SymbolBlock != null)
						{
							listViewNF1.Columns.Add("Label");
						}

						listViewNF1.Columns.Add("File Id");
						listViewNF1.Items.Clear();
						int num2 = 0;
						foreach (SDAT.INFO.SequenceArchiveInfo sequenceArchiveInfo in SoundArchive.InfoBlock
							.SequenceArchiveInfos.Entries)
						{
							if (sequenceArchiveInfo != null)
							{
								ListViewItem listViewItem = new ListViewItem(num2.ToString());
								if (SoundArchive.SymbolBlock != null)
								{
									listViewItem.SubItems.Add(SoundArchive.SymbolBlock.SequenceArchiveSymbols
										.Entries[num2].ArchiveName);
								}

								listViewItem.SubItems.Add(string.Concat(sequenceArchiveInfo.FileId));
								listViewItem.Tag = num2;
								listViewNF1.Items.Add(listViewItem);
							}

							num2++;
						}

						break;
					}
					case "bank":
					{
						listViewNF1.Columns.Clear();
						listViewNF1.Columns.Add("Id");
						if (SoundArchive.SymbolBlock != null)
						{
							listViewNF1.Columns.Add("Label");
						}

						listViewNF1.Columns.Add("File Id");
						listViewNF1.Columns.Add("Wave Archive 0");
						listViewNF1.Columns.Add("Wave Archive 1");
						listViewNF1.Columns.Add("Wave Archive 2");
						listViewNF1.Columns.Add("Wave Archive 3");
						listViewNF1.Items.Clear();
						int num2 = 0;
						foreach (SDAT.INFO.BankInfo bankInfo in SoundArchive.InfoBlock.BankInfos.Entries)
						{
							if (bankInfo != null)
							{
								ListViewItem listViewItem = new ListViewItem(num2.ToString());
								if (SoundArchive.SymbolBlock != null)
								{
									listViewItem.SubItems.Add(SoundArchive.SymbolBlock.BankSymbols.Entries[num2]);
								}

								listViewItem.SubItems.Add(string.Concat(bankInfo.FileId));
								listViewItem.SubItems.Add(
									(bankInfo.WaveArchives[0] == ushort.MaxValue
										? -1
										: bankInfo.WaveArchives[0]) +
									(bankInfo.WaveArchives[0] != ushort.MaxValue &&
									 SoundArchive.SymbolBlock != null
										? " (" + SoundArchive.SymbolBlock.WaveArchiveSymbols.Entries[
											bankInfo.WaveArchives[0]] + ")"
										: ""));
								listViewItem.SubItems.Add(
									(bankInfo.WaveArchives[1] == ushort.MaxValue
										? -1
										: bankInfo.WaveArchives[1]) +
									(bankInfo.WaveArchives[1] != ushort.MaxValue &&
									 SoundArchive.SymbolBlock != null
										? " (" + SoundArchive.SymbolBlock.WaveArchiveSymbols.Entries[
											bankInfo.WaveArchives[1]] + ")"
										: ""));
								listViewItem.SubItems.Add(
									(bankInfo.WaveArchives[2] == ushort.MaxValue
										? -1
										: bankInfo.WaveArchives[2]) +
									(bankInfo.WaveArchives[2] != ushort.MaxValue &&
									 SoundArchive.SymbolBlock != null
										? " (" + SoundArchive.SymbolBlock.WaveArchiveSymbols.Entries[
											bankInfo.WaveArchives[2]] + ")"
										: ""));
								listViewItem.SubItems.Add(
									(bankInfo.WaveArchives[3] == ushort.MaxValue
										? -1
										: bankInfo.WaveArchives[3]) +
									(bankInfo.WaveArchives[3] != ushort.MaxValue &&
									 SoundArchive.SymbolBlock != null
										? " (" + SoundArchive.SymbolBlock.WaveArchiveSymbols.Entries[
											bankInfo.WaveArchives[3]] + ")"
										: ""));
								listViewItem.Tag = num2;
								listViewNF1.Items.Add(listViewItem);
							}

							num2++;
						}

						break;
					}
					case "wavearc":
					{
						listViewNF1.Columns.Clear();
						listViewNF1.Columns.Add("Id");
						if (SoundArchive.SymbolBlock != null)
						{
							listViewNF1.Columns.Add("Label");
						}

						listViewNF1.Columns.Add("File Id");
						listViewNF1.Columns.Add("Flags");
						listViewNF1.Items.Clear();
						int num2 = 0;
						foreach (SDAT.INFO.WaveArchiveInfo waveArchiveInfo in SoundArchive.InfoBlock
							.WaveArchiveInfos.Entries)
						{
							if (waveArchiveInfo != null)
							{
								ListViewItem listViewItem = new ListViewItem(num2.ToString());
								if (SoundArchive.SymbolBlock != null)
								{
									listViewItem.SubItems.Add(
										SoundArchive.SymbolBlock.WaveArchiveSymbols.Entries[num2]);
								}

								listViewItem.SubItems.Add(string.Concat(waveArchiveInfo.FileId));
								listViewItem.SubItems.Add(string.Concat(waveArchiveInfo.Flags));
								listViewItem.Tag = num2;
								listViewNF1.Items.Add(listViewItem);
							}

							num2++;
						}

						break;
					}
					case "strm":
					{
						listViewNF1.Columns.Clear();
						listViewNF1.Columns.Add("Id");
						if (SoundArchive.SymbolBlock != null)
						{
							listViewNF1.Columns.Add("Label");
						}

						listViewNF1.Columns.Add("File Id");
						listViewNF1.Columns.Add("Volume");
						listViewNF1.Columns.Add("Player Priority");
						listViewNF1.Columns.Add("Player Nr");
						listViewNF1.Columns.Add("Flags");
						listViewNF1.Items.Clear();
						int num2 = 0;
						foreach (SDAT.INFO.StreamInfo streamInfo in SoundArchive.InfoBlock.StreamInfos.Entries)
						{
							if (streamInfo != null)
							{
								ListViewItem listViewItem = new ListViewItem(num2.ToString());
								if (SoundArchive.SymbolBlock != null)
								{
									listViewItem.SubItems.Add(SoundArchive.SymbolBlock.StreamSymbols
										.Entries[num2]);
								}

								listViewItem.SubItems.Add(string.Concat(streamInfo.FileId));
								listViewItem.SubItems.Add(string.Concat(streamInfo.Volume));
								listViewItem.SubItems.Add(string.Concat(streamInfo.PlayerPriority));
								listViewItem.SubItems.Add(string.Concat(streamInfo.PlayerNr));
								listViewItem.SubItems.Add(string.Concat(streamInfo.Flags));
								listViewItem.Tag = num2;
								listViewNF1.Items.Add(listViewItem);
							}

							num2++;
						}

						break;
					}
					case "player":
					{
						listViewNF1.Columns.Clear();
						listViewNF1.Columns.Add("Id");
						if (SoundArchive.SymbolBlock != null)
						{
							listViewNF1.Columns.Add("Label");
						}

						listViewNF1.Columns.Add("Max Nr Sequences");
						listViewNF1.Columns.Add("Channel Allocation Mask");
						listViewNF1.Columns.Add("Heap Size");
						listViewNF1.Items.Clear();
						int num2 = 0;
						foreach (SDAT.INFO.PlayerInfo playerInfo in SoundArchive.InfoBlock.PlayerInfos.Entries)
						{
							if (playerInfo != null)
							{
								ListViewItem listViewItem = new ListViewItem(num2.ToString());
								if (SoundArchive.SymbolBlock != null)
								{
									listViewItem.SubItems.Add(SoundArchive.SymbolBlock.PlayerSymbols
										.Entries[num2]);
								}

								listViewItem.SubItems.Add(string.Concat(playerInfo.MaxNrSequences));
								listViewItem.SubItems.Add(string.Concat(playerInfo.ChannelAllocationMask));
								listViewItem.SubItems.Add(string.Concat(playerInfo.HeapSize));
								listViewItem.Tag = num2;
								listViewNF1.Items.Add(listViewItem);
							}

							num2++;
						}

						break;
					}
					case "strmplayer":
					{
						listViewNF1.Columns.Clear();
						listViewNF1.Columns.Add("Id");
						if (SoundArchive.SymbolBlock != null)
						{
							listViewNF1.Columns.Add("Label");
						}

						listViewNF1.Columns.Add("Nr Channels");
						for (int i = 0; i < 16; i++)
						{
							listViewNF1.Columns.Add("Channel " + (i + 1));
						}

						listViewNF1.Items.Clear();
						int num2 = 0;
						foreach (SDAT.INFO.StreamPlayerInfo streamPlayerInfo in SoundArchive.InfoBlock
							.StreamPlayerInfos.Entries)
						{
							if (streamPlayerInfo != null)
							{
								ListViewItem listViewItem = new ListViewItem(num2.ToString());
								if (SoundArchive.SymbolBlock != null)
								{
									listViewItem.SubItems.Add(
										SoundArchive.SymbolBlock.StreamPlayerSymbols.Entries[num2]);
								}

								listViewItem.SubItems.Add(string.Concat(streamPlayerInfo.NrChannels));
								for (int i = 0; i < 16; i++)
								{
									listViewItem.SubItems.Add(string.Concat(streamPlayerInfo.ChannelNumbers[i]));
								}

								listViewItem.Tag = num2;
								listViewNF1.Items.Add(listViewItem);
							}

							num2++;
						}

						break;
					}
					case "group":
						break;
					default:
						goto IL_1029;
				}

				foreach (object obj in listViewNF1.Columns)
				{
					ColumnHeader columnHeader = (ColumnHeader) obj;
					columnHeader.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
				}
				IL_1029:
				listViewNF1.EndUpdate();
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00003490 File Offset: 0x00001690
		private void listViewNF1_ItemActivate(object sender, EventArgs e)
		{
			string curSel = CurSel;
			switch (curSel)
			{
			case "seq":
				if (!OpenFiles.ContainsKey(SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag]))
				{
					SSEQViewer sseqviewer = new SSEQViewer(SoundArchive, (int)listViewNF1.SelectedItems[0].Tag);
					sseqviewer.MdiParent = MdiParent;
					sseqviewer.Tag = SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag];
					sseqviewer.FormClosed += v_FormClosed;
					OpenFiles.Add(SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag], sseqviewer);
					sseqviewer.Show();
				}
				else
				{
					MessageBox.Show("This file has already been opened!");
					OpenFiles[SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag]].BringToFront();
				}
				break;
			case "bank":
				if (!OpenFiles.ContainsKey(SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag]))
				{
					SBNKViewer sbnkviewer = new SBNKViewer(SoundArchive, (int)listViewNF1.SelectedItems[0].Tag);
					sbnkviewer.MdiParent = MdiParent;
					sbnkviewer.Tag = SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag];
					sbnkviewer.FormClosed += v_FormClosed;
					OpenFiles.Add(SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag], sbnkviewer);
					sbnkviewer.Show();
				}
				else
				{
					MessageBox.Show("This file has already been opened!");
					OpenFiles[SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag]].BringToFront();
				}
				break;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00003826 File Offset: 0x00001A26
		private void v_FormClosed(object sender, FormClosedEventArgs e)
		{
			OpenFiles.Remove((SDAT.INFO.SDATInfo)((Form)sender).Tag);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00003848 File Offset: 0x00001A48
		public void Save()
		{
			byte[] bytes = SoundArchive.Write();
			File.Create(SDATPath).Close();
			File.WriteAllBytes(SDATPath, bytes);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00003880 File Offset: 0x00001A80
		private void listViewNF1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ListViewItem itemAt = listViewNF1.GetItemAt(e.X, e.Y);
				if (itemAt != null)
				{
					string curSel = CurSel;
					switch (curSel)
					{
					case "seq":
					case "seqarc":
					case "bank":
					case "wavearc":
					case "strm":
						menuItemImportFromSDAT.Visible = CurSel == "seq";
						contextMenu1.Show(listViewNF1, e.Location);
						break;
					}
				}
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000039C0 File Offset: 0x00001BC0
		private void menuItemReplace_Click(object sender, EventArgs e)
		{
			string curSel = CurSel;
			switch (curSel)
			{
			case "seq":
				openFileDialog1.Filter = "NITRO Sound Sequence (*.sseq)|*.sseq";
				if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
				{
					if (OpenFiles.ContainsKey(SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag]))
					{
						if (MessageBox.Show("The file you want to replace is currently open. If you continue it will be closed. Do you still want to replace?", "Replace", MessageBoxButtons.YesNo) == DialogResult.No)
						{
							break;
						}
						OpenFiles[SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag]].Close();
						OpenFiles.Remove(SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag]);
					}
					SoundArchive.FileAllocationTable.Entries[(int)SoundArchive.InfoBlock.SequenceInfos[(int)listViewNF1.SelectedItems[0].Tag].FileId].Data = File.ReadAllBytes(openFileDialog1.FileName);
				}
				break;
			case "seqarc":
				openFileDialog1.Filter = "NITRO Sound Sequence Archive (*.ssar)|*.ssar";
				if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
				{
					if (OpenFiles.ContainsKey(SoundArchive.InfoBlock.SequenceArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag]))
					{
						if (MessageBox.Show("The file you want to replace is currently open. If you continue it will be closed. Do you still want to replace?", "Replace", MessageBoxButtons.YesNo) == DialogResult.No)
						{
							break;
						}
						OpenFiles[SoundArchive.InfoBlock.SequenceArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag]].Close();
						OpenFiles.Remove(SoundArchive.InfoBlock.SequenceArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag]);
					}
					SoundArchive.FileAllocationTable.Entries[(int)SoundArchive.InfoBlock.SequenceArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag].FileId].Data = File.ReadAllBytes(openFileDialog1.FileName);
				}
				break;
			case "bank":
				openFileDialog1.Filter = "NITRO Sound Bank (*.sbnk)|*.sbnk";
				if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
				{
					if (OpenFiles.ContainsKey(SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag]))
					{
						if (MessageBox.Show("The file you want to replace is currently open. If you continue it will be closed. Do you still want to replace?", "Replace", MessageBoxButtons.YesNo) == DialogResult.No)
						{
							break;
						}
						OpenFiles[SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag]].Close();
						OpenFiles.Remove(SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag]);
					}
					SoundArchive.FileAllocationTable.Entries[(int)SoundArchive.InfoBlock.BankInfos[(int)listViewNF1.SelectedItems[0].Tag].FileId].Data = File.ReadAllBytes(openFileDialog1.FileName);
				}
				break;
			case "wavearc":
				openFileDialog1.Filter = "NITRO Sound Wave Archive (*.swar)|*.swar";
				if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
				{
					if (OpenFiles.ContainsKey(SoundArchive.InfoBlock.WaveArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag]))
					{
						if (MessageBox.Show("The file you want to replace is currently open. If you continue it will be closed. Do you still want to replace?", "Replace", MessageBoxButtons.YesNo) == DialogResult.No)
						{
							break;
						}
						OpenFiles[SoundArchive.InfoBlock.WaveArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag]].Close();
						OpenFiles.Remove(SoundArchive.InfoBlock.WaveArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag]);
					}
					SoundArchive.FileAllocationTable.Entries[(int)SoundArchive.InfoBlock.WaveArchiveInfos[(int)listViewNF1.SelectedItems[0].Tag].FileId].Data = File.ReadAllBytes(openFileDialog1.FileName);
				}
				break;
			case "strm":
				openFileDialog1.Filter = "NITRO Sound Stream (*.strm)|*.strm";
				if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
				{
					if (OpenFiles.ContainsKey(SoundArchive.InfoBlock.StreamInfos[(int)listViewNF1.SelectedItems[0].Tag]))
					{
						if (MessageBox.Show("The file you want to replace is currently open. If you continue it will be closed. Do you still want to replace?", "Replace", MessageBoxButtons.YesNo) == DialogResult.No)
						{
							break;
						}
						OpenFiles[SoundArchive.InfoBlock.StreamInfos[(int)listViewNF1.SelectedItems[0].Tag]].Close();
						OpenFiles.Remove(SoundArchive.InfoBlock.StreamInfos[(int)listViewNF1.SelectedItems[0].Tag]);
					}
					SoundArchive.FileAllocationTable.Entries[(int)SoundArchive.InfoBlock.StreamInfos[(int)listViewNF1.SelectedItems[0].Tag].FileId].Data = File.ReadAllBytes(openFileDialog1.FileName);
				}
				break;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00004244 File Offset: 0x00002444
		private void menuItemImportFromSDAT_Click(object sender, EventArgs e)
		{
			SSEQImport sseqimport = new SSEQImport();
			if (sseqimport.ShowDialog() == DialogResult.OK)
			{
				sseqimport.Import(SoundArchive, (int)listViewNF1.SelectedItems[0].Tag);
				UpdateListView(CurSel);
			}
		}

		// Token: 0x04000001 RID: 1
		private Dictionary<SDAT.INFO.SDATInfo, Form> OpenFiles = new Dictionary<SDAT.INFO.SDATInfo, Form>();

		// Token: 0x04000002 RID: 2
		private string CurSel;

		// Token: 0x04000003 RID: 3
		private string SDATPath;

		// Token: 0x04000004 RID: 4
		private SDAT SoundArchive;

		// Token: 0x04000005 RID: 5
		private ImageList ImageList1;
	}
}
