using System;
using Gtk;
using Gnome.Vfs;

namespace UltimateCommander {

	public class UltimateCommander {

		static string program_gui_path;
		public static string GladeFileName;
		public static MainWindow MainWindow;

     	public static void Main(string[] args)
     	{              
			program_gui_path = Environment.GetEnvironmentVariable("UC_GUI_PATH");

			if (program_gui_path == null) {
				program_gui_path = "gui";
			}

			GladeFileName = program_gui_path + "/uc.glade";

			Gnome.Vfs.Vfs.Initialize();
          	Gtk.Application.Init();
			MainWindow = new MainWindow();
          	Gtk.Application.Run ();
		}
	}
}
