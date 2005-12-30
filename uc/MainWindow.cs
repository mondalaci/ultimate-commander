using System;
using Gtk;
using Glade;

namespace UltimateCommander {

	public class MainWindow {

		[Glade.Widget] HPaned hpaned;
		[Glade.Widget] Gtk.Window main_window;

		Panel panel1;
		Panel panel2;
		Panel active_panel;
		Panel passive_panel;

		float panel_ratio = 0.5f;
		int width = 0;
		int old_width = 0;

		public MainWindow()
		{
			string default_directory = ".";

			Glade.XML glade_xml = new Glade.XML(UltimateCommander.GladeFileName, "main_window", null);
			glade_xml.Autoconnect(this);

			panel1 = new Panel(default_directory);
			panel2 = new Panel(default_directory);
			panel1.other_panel = panel2;
			panel2.other_panel = panel1;

			hpaned.Add1(panel1);
			hpaned.Add2(panel2);

			ResizePanes();
			panel1.SetActive(true);
			main_window.ShowAll();
		}

		public Panel ActivePanel
		{
			get {
				if (panel1.Activated)
					return panel1;
				else
					return panel2;
			}
		}

		void ResizePanes()
		{
			width = hpaned.Allocation.Width;

			if (width != old_width) {
				PanelRatio = panel_ratio;
				old_width = width;
			} else
				panel_ratio = PanelRatio;
		}

		float PanelRatio {
			get {
				return (float)hpaned.Position / (float)hpaned.Allocation.Width;
			}
			set {
				int pos = (int)(value * hpaned.Allocation.Width);
				hpaned.Position = pos;
			}
		}

		void OnWindowCheckResize(object o, EventArgs args)
		{
			ResizePanes();
		}

		void OnToolBarButtonEvent(object o, EventArgs args)
		{
			ActivePanel.SetActive(true);
		}

     	void OnWindowDeleteEvent(object o, DeleteEventArgs args)
     	{
          	Application.Quit();
     	}

		void OnQuitMenuItemActivated(object o, EventArgs args)
		{
			Application.Quit();
		}
	}
}
