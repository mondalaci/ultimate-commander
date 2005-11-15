using System;

using Gtk;
using Gdk;

namespace UltimateCommander {

	public class UltimateCommander {

		static Panel panel1;
		static Panel panel2;
		static HPaned hpaned;

     	public static void Main (string [] args)
     	{              
          	Application.Init();
                        
			string default_directory = ".";

			panel1 = new Panel(default_directory);
			panel1.ActivatedEvent += new ActivatedHandler(OnPanelActivated);

			panel2 = new Panel(default_directory);
			panel2.ActivatedEvent += new ActivatedHandler(OnPanelActivated);

			hpaned = new HPaned();
			hpaned.Add1(panel1);
			hpaned.Add2(panel2);
			hpaned.Position = 500;
          	Gtk.Window window = new Gtk.Window("Ultimate Commander");
          	window.SetDefaultSize(1000, 700);
          	window.DeleteEvent += new DeleteEventHandler(OnDeleteEvent);
			window.Add(hpaned);
			window.ResizeChecked += new EventHandler(OnResizeChecked);
          	window.ShowAll();

          	Application.Run ();
		}

		private static void OnPanelActivated(Panel panel)
		{
			Panel passive_panel;
			
			if (panel == panel1)
				passive_panel = panel2;
			else
				passive_panel = panel1;
				
			passive_panel.SetActivated(false);
		}

		[GLib.ConnectBefore]
		private static void OnResizeChecked(object o, EventArgs args)
		{
			float ratio =  (float)hpaned.Position / (float)hpaned.Allocation.Width;
			Console.WriteLine("resized {0}", ratio);
		}

     	private static void OnDeleteEvent (System.Object o, DeleteEventArgs args)
     	{
          	Application.Quit();
     	}
	}
}
