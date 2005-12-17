using System;

using Gtk;
using Gdk;

namespace UltimateCommander {

	public class UltimateCommander {

		static Panel panel1;
		static Panel panel2;
		static HPaned hpaned;

		static float panel_ratio = 0.5f;
		static int width = 1000;
		static int height = 700;
		static int old_width = width;

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

			Pixbuf icon = new Pixbuf("logo.png");
          	Gtk.Window window = new Gtk.Window("Ultimate Commander");
			window.Icon = icon;
          	window.SetDefaultSize(width, height);
          	window.DeleteEvent += new DeleteEventHandler(OnDeleteEvent);
			window.Add(hpaned);
			Resize();
			window.ResizeChecked += new EventHandler(OnResizeChecked);
          	window.ShowAll();

          	Application.Run ();
		}

		// FIXME: Use Paned.OnMoveHandle later with Gtk# 2.6+ to correctly resize.
		static void Resize()
		{
			width = hpaned.Allocation.Width;
			if (width != old_width) {
				PanelRatio = panel_ratio;
				old_width = width;
			} else {
				panel_ratio = PanelRatio;
			}
		}

		private static void OnPanelActivated(Panel panel)
		{
			Panel passive_panel;
			
			if (panel == panel1)
				passive_panel = panel2;
			else
				passive_panel = panel1;
				
			passive_panel.SetActivatedState(false);
		}

		[GLib.ConnectBefore]
		private static void OnResizeChecked(object o, EventArgs args)
		{
			Resize();
		}

     	private static void OnDeleteEvent (System.Object o, DeleteEventArgs args)
     	{
          	Application.Quit();
     	}

		private static float PanelRatio {
			get {
				return (float)hpaned.Position / (float)hpaned.Allocation.Width;
			}
			set {
				int pos = (int)(value * hpaned.Allocation.Width);
				hpaned.Position = pos;
			}
		}
	}
}
