using System.ComponentModel;

namespace DSSoundStudio.UI
{
	// Token: 0x02000009 RID: 9
	public class MainMenu : System.Windows.Forms.MainMenu
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00006939 File Offset: 0x00004B39
		public MainMenu(IContainer iContainer)
		{
			this.iContainer = iContainer;
		}

		// Token: 0x0400004A RID: 74
		private IContainer iContainer;
	}
}
