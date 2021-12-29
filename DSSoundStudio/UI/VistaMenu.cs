using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSSoundStudio.UI
{
	// Token: 0x0200000A RID: 10
	[ProvideProperty("Image", typeof(MenuItem))]
	public class VistaMenu : Component, IExtenderProvider, ISupportInitialize
	{
		// Token: 0x0600002B RID: 43 RVA: 0x0000694B File Offset: 0x00004B4B
		public VistaMenu(ContainerControl parentControl) : this()
		{
			ownerForm = parentControl;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00006960 File Offset: 0x00004B60
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00006978 File Offset: 0x00004B78
		public ContainerControl ContainerControl
		{
			get
			{
				return ownerForm;
			}
			set
			{
				ownerForm = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00006984 File Offset: 0x00004B84
		public override ISite Site
		{
			set
			{
				base.Site = value;
				if (value != null)
				{
					IDesignerHost designerHost = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if (designerHost != null)
					{
						IComponent rootComponent = designerHost.RootComponent;
						ContainerControl = rootComponent as ContainerControl;
					}
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000069DE File Offset: 0x00004BDE
		private void ownerForm_ChangeUICues(object sender, UICuesEventArgs e)
		{
			isUsingKeyboardAccel = e.ShowKeyboard;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000069F0 File Offset: 0x00004BF0
		private static void MenuItem_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			Font font = ((MenuItem)sender).DefaultItem ? menuBoldFont : SystemFonts.MenuFont;
			if (((MenuItem)sender).Text == "-")
			{
				e.ItemHeight = 9;
			}
			else
			{
				e.ItemHeight = (SystemFonts.MenuFont.Height > 16 ? SystemFonts.MenuFont.Height : 16) + 4;
				e.ItemWidth = 26 + TextRenderer.MeasureText(((MenuItem)sender).Text, font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine).Width + 20 + TextRenderer.MeasureText(ShortcutToString(((MenuItem)sender).Shortcut), font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine).Width + (((MenuItem)sender).IsParent ? 12 : 0);
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00006AD8 File Offset: 0x00004CD8
		private void MenuItem_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
			e.Graphics.InterpolationMode = InterpolationMode.Low;
			bool flag = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
			if (flag)
			{
				e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
			}
			else
			{
				e.Graphics.FillRectangle(SystemBrushes.Menu, e.Bounds);
			}
			if (((MenuItem)sender).Text == "-")
			{
				int num = e.Bounds.Top + e.Bounds.Height / 2 - 1;
				e.Graphics.DrawLine(SystemPens.ControlDark, e.Bounds.Left + 1, num, e.Bounds.Left + e.Bounds.Width - 2, num);
				e.Graphics.DrawLine(SystemPens.ControlLightLight, e.Bounds.Left + 1, num + 1, e.Bounds.Left + e.Bounds.Width - 2, num + 1);
			}
			else
			{
				DrawText(sender, e, flag);
				if (((MenuItem)sender).Checked)
				{
					if (((MenuItem)sender).RadioCheck)
					{
						ControlPaint.DrawMenuGlyph(e.Graphics, e.Bounds.Left + (26 - SystemInformation.MenuCheckSize.Width) / 2, e.Bounds.Top + (e.Bounds.Height - SystemInformation.MenuCheckSize.Height) / 2 + 1, SystemInformation.MenuCheckSize.Width, SystemInformation.MenuCheckSize.Height, MenuGlyph.Bullet, flag ? SystemColors.HighlightText : SystemColors.MenuText, flag ? SystemColors.Highlight : SystemColors.Menu);
					}
					else
					{
						ControlPaint.DrawMenuGlyph(e.Graphics, e.Bounds.Left + (26 - SystemInformation.MenuCheckSize.Width) / 2, e.Bounds.Top + (e.Bounds.Height - SystemInformation.MenuCheckSize.Height) / 2 + 1, SystemInformation.MenuCheckSize.Width, SystemInformation.MenuCheckSize.Height, MenuGlyph.Checkmark, flag ? SystemColors.HighlightText : SystemColors.MenuText, flag ? SystemColors.Highlight : SystemColors.Menu);
					}
				}
				else
				{
					Image image = GetImage((MenuItem)sender);
					if (image != null)
					{
						if (((MenuItem)sender).Enabled)
						{
							e.Graphics.DrawImage(image, e.Bounds.Left + 4, e.Bounds.Top + (e.Bounds.Height - 16) / 2, 16, 16);
						}
						else
						{
							ControlPaint.DrawImageDisabled(e.Graphics, image, e.Bounds.Left + 4, e.Bounds.Top + (e.Bounds.Height - 16) / 2, SystemColors.Menu);
						}
					}
				}
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00006E58 File Offset: 0x00005058
		private static string ShortcutToString(Shortcut shortcut)
		{
			string result;
			if (shortcut != Shortcut.None)
			{
				result = TypeDescriptor.GetConverter(((Keys)shortcut).GetType()).ConvertToString((Keys)shortcut);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00006E94 File Offset: 0x00005094
		private void DrawText(object sender, DrawItemEventArgs e, bool isSelected)
		{
			string text = ShortcutToString(((MenuItem)sender).Shortcut);
			int y = e.Bounds.Top + (e.Bounds.Height - SystemFonts.MenuFont.Height) / 2;
			Font font = ((MenuItem)sender).DefaultItem ? menuBoldFont : SystemFonts.MenuFont;
			Size size = TextRenderer.MeasureText(((MenuItem)sender).Text, font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine);
			Rectangle bounds = new Rectangle(e.Bounds.Left + 4 + 16 + 6, y, size.Width, size.Height);
			if (!((MenuItem)sender).Enabled && !isSelected)
			{
				bounds.Offset(1, 1);
				TextRenderer.DrawText(e.Graphics, ((MenuItem)sender).Text, font, bounds, SystemColors.ControlLightLight, TextFormatFlags.SingleLine | (isUsingKeyboardAccel ? TextFormatFlags.Default : TextFormatFlags.HidePrefix) | TextFormatFlags.NoClipping);
				bounds.Offset(-1, -1);
			}
			TextRenderer.DrawText(e.Graphics, ((MenuItem)sender).Text, font, bounds, ((MenuItem)sender).Enabled ? isSelected ? SystemColors.HighlightText : SystemColors.MenuText : SystemColors.GrayText, TextFormatFlags.SingleLine | (isUsingKeyboardAccel ? TextFormatFlags.Default : TextFormatFlags.HidePrefix) | TextFormatFlags.NoClipping);
			if (text != null)
			{
				size = TextRenderer.MeasureText(text, font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine);
				bounds = new Rectangle(e.Bounds.Width - size.Width - 12, y, size.Width, size.Height);
				if (!((MenuItem)sender).Enabled && !isSelected)
				{
					bounds.Offset(1, 1);
					TextRenderer.DrawText(e.Graphics, text, font, bounds, SystemColors.ControlLightLight, TextFormatFlags.SingleLine | (isUsingKeyboardAccel ? TextFormatFlags.Default : TextFormatFlags.HidePrefix) | TextFormatFlags.NoClipping);
					bounds.Offset(-1, -1);
				}
				TextRenderer.DrawText(e.Graphics, text, font, bounds, ((MenuItem)sender).Enabled ? isSelected ? SystemColors.HighlightText : SystemColors.MenuText : SystemColors.GrayText, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine);
			}
		}

		// Token: 0x06000034 RID: 52
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool SetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, MENUITEMINFO_T_RW lpmii);

		// Token: 0x06000035 RID: 53
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool SetMenuInfo(HandleRef hMenu, MENUINFO lpcmi);

		// Token: 0x06000036 RID: 54
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x06000037 RID: 55 RVA: 0x000070E4 File Offset: 0x000052E4
		public VistaMenu()
		{
			isVistaOrLater = Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;
			InitializeComponent();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000714D File Offset: 0x0000534D
		public VistaMenu(IContainer container) : this()
		{
			container.Add(this);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00007160 File Offset: 0x00005360
		private void InitializeComponent()
		{
			components = new Container();
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00007170 File Offset: 0x00005370
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (object obj in properties)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (((Properties)dictionaryEntry.Value).renderBmpHbitmap != IntPtr.Zero)
					{
						DeleteObject(((Properties)dictionaryEntry.Value).renderBmpHbitmap);
					}
				}
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00007238 File Offset: 0x00005438
		bool IExtenderProvider.CanExtend(object o)
		{
			bool result;
			if (o is MenuItem)
			{
				result = ((MenuItem)o).Parent == null || ((MenuItem)o).Parent.GetType() != typeof(MainMenu);
			}
			else
			{
				result = o is Form;
			}
			return result;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000072A8 File Offset: 0x000054A8
		private Properties EnsurePropertiesExists(MenuItem key)
		{
			Properties properties = (Properties)this.properties[key];
			if (properties == null)
			{
				properties = new Properties();
				this.properties[key] = properties;
			}
			return properties;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000072F0 File Offset: 0x000054F0
		[Description("The Image for the MenuItem")]
		[DefaultValue(null)]
		[Category("Appearance")]
		public Image GetImage(MenuItem mnuItem)
		{
			return EnsurePropertiesExists(mnuItem).Image;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00007310 File Offset: 0x00005510
		[DefaultValue(null)]
		public void SetImage(MenuItem mnuItem, Image value)
		{
			Properties properties = EnsurePropertiesExists(mnuItem);
			properties.Image = value;
			if (!DesignMode && isVistaOrLater)
			{
				if (properties.renderBmpHbitmap != IntPtr.Zero)
				{
					DeleteObject(properties.renderBmpHbitmap);
					properties.renderBmpHbitmap = IntPtr.Zero;
				}
				if (value == null)
				{
					return;
				}
				using (Bitmap bitmap = new Bitmap(value.Width, value.Height, PixelFormat.Format32bppPArgb))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						graphics.DrawImage(value, 0, 0, value.Width, value.Height);
					}
					properties.renderBmpHbitmap = bitmap.GetHbitmap(Color.FromArgb(0, 0, 0, 0));
				}
				if (formHasBeenIntialized)
				{
					AddVistaMenuItem(mnuItem);
				}
			}
			if (!DesignMode && !isVistaOrLater && formHasBeenIntialized)
			{
				AddPreVistaMenuItem(mnuItem);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00007458 File Offset: 0x00005658
		void ISupportInitialize.BeginInit()
		{
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000745C File Offset: 0x0000565C
		private void AddVistaMenuItem(MenuItem mnuItem)
		{
			if (menuParents[mnuItem.Parent] == null)
			{
				if (mnuItem.Parent.GetType() == typeof(ContextMenu))
				{
					((ContextMenu)mnuItem.Parent).Popup += MenuItem_Popup;
				}
				else
				{
					((MenuItem)mnuItem.Parent).Popup += MenuItem_Popup;
				}
				SetMenuInfo(new HandleRef(null, mnuItem.Parent.Handle), mnuInfo);
				menuParents[mnuItem.Parent] = true;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000751C File Offset: 0x0000571C
		private void AddPreVistaMenuItem(MenuItem mnuItem)
		{
			if (menuParents[mnuItem.Parent] == null)
			{
				menuParents[mnuItem.Parent] = true;
				if (formHasBeenIntialized)
				{
					foreach (object obj in mnuItem.Parent.MenuItems)
					{
						MenuItem menuItem = (MenuItem)obj;
						menuItem.DrawItem += MenuItem_DrawItem;
						menuItem.MeasureItem += MenuItem_MeasureItem;
						menuItem.OwnerDraw = true;
					}
				}
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000075F8 File Offset: 0x000057F8
		void ISupportInitialize.EndInit()
		{
			if (!DesignMode)
			{
				if (isVistaOrLater)
				{
					foreach (object obj in properties)
					{
						AddVistaMenuItem((MenuItem)((DictionaryEntry)obj).Key);
					}
				}
				else
				{
					menuBoldFont = new Font(SystemFonts.MenuFont, FontStyle.Bold);
					if (ownerForm != null)
					{
						ownerForm.ChangeUICues += ownerForm_ChangeUICues;
					}
					foreach (object obj2 in properties)
					{
						AddPreVistaMenuItem((MenuItem)((DictionaryEntry)obj2).Key);
					}
					foreach (object obj3 in menuParents)
					{
						foreach (object obj4 in ((Menu)((DictionaryEntry)obj3).Key).MenuItems)
						{
							MenuItem menuItem = (MenuItem)obj4;
							menuItem.DrawItem += MenuItem_DrawItem;
							menuItem.MeasureItem += MenuItem_MeasureItem;
							menuItem.OwnerDraw = true;
						}
					}
				}
				formHasBeenIntialized = true;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00007820 File Offset: 0x00005A20
		private void MenuItem_Popup(object sender, EventArgs e)
		{
			MENUITEMINFO_T_RW menuiteminfo_T_RW = new MENUITEMINFO_T_RW();
			Menu.MenuItemCollection menuItemCollection = sender.GetType() == typeof(ContextMenu) ? ((ContextMenu)sender).MenuItems : ((MenuItem)sender).MenuItems;
			int num = 0;
			for (int i = 0; i < menuItemCollection.Count; i++)
			{
				if (menuItemCollection[i].Visible)
				{
					Properties properties = (Properties)this.properties[menuItemCollection[i]];
					if (properties != null)
					{
						menuiteminfo_T_RW.hbmpItem = properties.renderBmpHbitmap;
						SetMenuItemInfo(new HandleRef(null, ((Menu)sender).Handle), num, true, menuiteminfo_T_RW);
					}
					num++;
				}
			}
		}

		// Token: 0x0400004B RID: 75
		private const int SEPARATOR_HEIGHT = 9;

		// Token: 0x0400004C RID: 76
		private const int BORDER_VERTICAL = 4;

		// Token: 0x0400004D RID: 77
		private const int LEFT_MARGIN = 4;

		// Token: 0x0400004E RID: 78
		private const int RIGHT_MARGIN = 6;

		// Token: 0x0400004F RID: 79
		private const int SHORTCUT_MARGIN = 20;

		// Token: 0x04000050 RID: 80
		private const int ARROW_MARGIN = 12;

		// Token: 0x04000051 RID: 81
		private const int ICON_SIZE = 16;

		// Token: 0x04000052 RID: 82
		private ContainerControl ownerForm;

		// Token: 0x04000053 RID: 83
		private bool isUsingKeyboardAccel;

		// Token: 0x04000054 RID: 84
		private static Font menuBoldFont;

		// Token: 0x04000055 RID: 85
		private Container components;

		// Token: 0x04000056 RID: 86
		private readonly Hashtable properties = new Hashtable();

		// Token: 0x04000057 RID: 87
		private readonly Hashtable menuParents = new Hashtable();

		// Token: 0x04000058 RID: 88
		private bool formHasBeenIntialized;

		// Token: 0x04000059 RID: 89
		private readonly bool isVistaOrLater;

		// Token: 0x0400005A RID: 90
		private readonly MENUINFO mnuInfo = new MENUINFO();
	}
}
