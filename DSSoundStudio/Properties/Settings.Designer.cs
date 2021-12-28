using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace DSSoundStudio.Properties
{
	// Token: 0x02000018 RID: 24
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00009908 File Offset: 0x00007B08
		public static Settings Default
		{
			get
			{
				return defaultInstance;
			}
		}

		// Token: 0x0400009B RID: 155
		private static Settings defaultInstance = (Settings)Synchronized(new Settings());
	}
}
