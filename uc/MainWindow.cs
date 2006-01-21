using System;
using Gtk;
using Glade;

namespace UltimateCommander {

	public class MainWindow {

		[Glade.Widget] HPaned hpaned;
		[Glade.Widget] Gtk.Window main_window;

		//Panel panel1;
		//Panel panel2;

		float panel_ratio = 0.5f;
		int width = 0;
		int old_width = 0;

		public MainWindow()
		{
			Glade.XML glade_xml = new Glade.XML(UltimateCommander.GladeFileName, "main_window", null);
			glade_xml.Autoconnect(this);

			PanelFrame left_panel_frame = new PanelFrame(PanelFrame.PanelFramePosition.Left);
			PanelFrame right_panel_frame = new PanelFrame(PanelFrame.PanelFramePosition.Right);
			left_panel_frame.OtherFrame = right_panel_frame;
			right_panel_frame.OtherFrame = left_panel_frame;

			hpaned.Add1(left_panel_frame);
			hpaned.Add2(right_panel_frame);

			ResizePanes();
			main_window.ShowAll();
		}

		/*public Panel ActivePanel {
			get {
				if (panel1.Active)
					return panel1;
				else
					return panel2;
			}
		}*/

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
			//ActivePanel.Active = true;
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
