using System;
using Gdk;

namespace UltimateCommander {

	public static class Util {

		static string home_directory_path;
		static int icon_size = 16;

		public static void Initialize()
		{
			home_directory_path = Environment.GetEnvironmentVariable("HOME");
		}

		public static Gdk.Pixbuf LoadIcon(string iconname)
		{
			return Gtk.IconTheme.Default.LoadIcon(iconname, icon_size, Gtk.IconLookupFlags.NoSvg);
		}

		public static string HomeDirectoryPath {
			get { return home_directory_path; }
		}
	}
}