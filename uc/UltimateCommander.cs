using System;
using Gtk;

namespace UltimateCommander {

	public class UltimateCommander {

     	public static void Main(string[] args)
     	{              
          	Application.Init();
			new MainWindow();
          	Application.Run ();
		}
	}
}
