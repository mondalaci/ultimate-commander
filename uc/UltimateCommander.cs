using System;
using Mono.Unix;
using Gtk;
using Gdk;
using Gnome.Vfs;

namespace UltimateCommander {

	public class UltimateCommander {

		public static Pixbuf LoadPixbuf(string filename)
		{
			string filepath = UnixPath.Combine(GuiPath, filename);
			return new Pixbuf(filepath);
		}

		// Config variables
		public static string GuiPath;
		public static string GladeFileName;

		public static MainWindow MainWindow;

     	public static void Main(string[] args)
     	{              
			GuiPath = Environment.GetEnvironmentVariable("UC_GUI_PATH");

			if (GuiPath == null) {
				GuiPath = "gui";
			}

			GladeFileName = UnixPath.Combine(GuiPath, "uc.glade");

			Gnome.Vfs.Vfs.Initialize();
          	Gtk.Application.Init();
			MainWindow = new MainWindow();
          	Gtk.Application.Run ();
		}
	}
}
