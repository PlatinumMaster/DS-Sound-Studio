using System;
using System.Windows.Forms;

namespace DSSoundStudio
{
	// Token: 0x02000017 RID: 23
	internal static class Program
	{
		// Token: 0x06000073 RID: 115 RVA: 0x000098EC File Offset: 0x00007AEC
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
