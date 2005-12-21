using System;

using Gtk;
using Gdk;
using Glade;

namespace UltimateCommander {

	public class MainWindow {

		[Glade.Widget] HPaned hpaned;
		[Glade.Widget] Gtk.Window main_window;

		Panel panel1;
		Panel panel2;

		float panel_ratio = 0.5f;
		int width = 0;
		int old_width = 0;

		public MainWindow() {
			string default_directory = ".";

			Glade.XML glade_xml = new Glade.XML("gui/uc.glade", "main_window", null);
			glade_xml.Autoconnect(this);

			panel1 = new Panel(default_directory);
			panel1.ActivatedEvent += new ActivatedHandler(OnPanelActivated);

			panel2 = new Panel(default_directory);
			panel2.ActivatedEvent += new ActivatedHandler(OnPanelActivated);

			hpaned.Add1(panel1);
			hpaned.Add2(panel2);

			ResizePanes();

			main_window.ShowAll();
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

		void OnPanelActivated(Panel panel)
		{
			Panel passive_panel;
			
			if (panel == panel1)
				passive_panel = panel2;
			else
				passive_panel = panel1;
				
			passive_panel.SetActivatedState(false);
		}

		void OnPanesResizeChecked(object o, EventArgs args)
		{
			ResizePanes();
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
