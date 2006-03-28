using System;
using Gdk;

namespace UltimateCommander {

	public static class Util {

		static int icon_size = 16;

		public static Gdk.Pixbuf LoadIcon(string iconname)
		{
			return Gtk.IconTheme.Default.LoadIcon(iconname, icon_size, Gtk.IconLookupFlags.NoSvg);
		}
	}
}