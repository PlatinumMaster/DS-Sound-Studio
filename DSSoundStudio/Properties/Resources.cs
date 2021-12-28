using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DSSoundStudio.Properties
{
	// Token: 0x02000008 RID: 8
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	internal class Resources
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00006802 File Offset: 0x00004A02

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00006810 File Offset: 0x00004A10
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("DSSoundStudio.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = resourceManager;
				}
				return resourceMan;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000685C File Offset: 0x00004A5C
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00006873 File Offset: 0x00004A73
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000026 RID: 38 RVA: 0x0000687C File Offset: 0x00004A7C
		internal static Bitmap control
		{
			get
			{
				object @object = ResourceManager.GetObject("control", resourceCulture);
				return (Bitmap)@object;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000068AC File Offset: 0x00004AAC
		internal static Bitmap control_pause
		{
			get
			{
				object @object = ResourceManager.GetObject("control_pause", resourceCulture);
				return (Bitmap)@object;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000068DC File Offset: 0x00004ADC
		internal static Bitmap control_stop_square
		{
			get
			{
				object @object = ResourceManager.GetObject("control_stop_square", resourceCulture);
				return (Bitmap)@object;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000690C File Offset: 0x00004B0C
		internal static Bitmap folder_open
		{
			get
			{
				object @object = ResourceManager.GetObject("folder_open", resourceCulture);
				return (Bitmap)@object;
			}
		}

		// Token: 0x04000048 RID: 72
		private static ResourceManager resourceMan;

		// Token: 0x04000049 RID: 73
		private static CultureInfo resourceCulture;
	}
}
