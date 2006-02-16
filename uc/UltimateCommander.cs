using System;
using Gtk;
using Gnome.Vfs;

namespace UltimateCommander {

	public class UltimateCommander {

		public static string GladeFileName = "gui/uc.glade";
		public static MainWindow MainWindow;

     	public static void Main(string[] args)
     	{              
			Gnome.Vfs.Vfs.Initialize();
          	Gtk.Application.Init();
			MainWindow = new MainWindow();
          	Gtk.Application.Run ();
		}
	}
}
