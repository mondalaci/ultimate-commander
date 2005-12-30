using System;
using Gtk;

namespace UltimateCommander {

	public class UltimateCommander {

		public static string GladeFileName = "gui/uc.glade";
		public static MainWindow MainWindow;

     	public static void Main(string[] args)
     	{              
          	Application.Init();
			MainWindow = new MainWindow();
          	Application.Run ();
		}
	}
}
