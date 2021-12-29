using System.Windows.Forms;

namespace DSSoundStudio.UI
{
	// Token: 0x02000012 RID: 18
	public class ListViewNF : ListView
	{
		// Token: 0x06000053 RID: 83 RVA: 0x000088A6 File Offset: 0x00006AA6
		public ListViewNF()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.EnableNotifyMessage, true);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000088CC File Offset: 0x00006ACC
		protected override void OnNotifyMessage(Message m)
		{
			if (m.Msg != 20)
			{
				base.OnNotifyMessage(m);
			}
		}
	}
}
