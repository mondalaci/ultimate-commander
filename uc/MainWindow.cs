using System;
using Gtk;
using Glade;
using Gnome.Vfs;

namespace UltimateCommander {

	public class MainWindow {

		static InfoBar infobar = new InfoBar();

		public static InfoBar InfoBar {
			get { return infobar; }
		}

		[Glade.Widget] HPaned hpaned;
		[Glade.Widget] Gtk.Window main_window;
		[Glade.Widget] EventBox infobar_slot;

		float panel_ratio = 0.5f;
		int width = 0;
		int old_width = 0;

		public MainWindow()
		{
			Glade.XML glade_xml =
				new Glade.XML(UltimateCommander.GladeFileName, "main_window", null);
			glade_xml.Autoconnect(this);

			string initial_path = ".";

			PanelFrame left_panel_frame = new PanelFrame(initial_path, PanelFramePosition.Left);
			PanelFrame right_panel_frame = new PanelFrame(initial_path, PanelFramePosition.Right);
			left_panel_frame.OtherFrame = right_panel_frame;
			right_panel_frame.OtherFrame = left_panel_frame;

			hpaned.Add1(left_panel_frame);
			hpaned.Add2(right_panel_frame);

			ResizePanes();

			infobar_slot.Add(infobar);
			InfoBar.Notice("Ultimate Commander started.");

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
			} else {
				panel_ratio = PanelRatio;
			}
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
          	Gtk.Application.Quit();
     	}

		void OnQuitMenuItemActivated(object o, EventArgs args)
		{
			Gtk.Application.Quit();
		}

		void OnCopyButtonClicked(object sender, EventArgs args)
		{
		}

		void OnMoveButtonClicked(object sender, EventArgs args)
		{
		}

		void OnCreateDirectoryButtonClicked(object sender, EventArgs args)
		{
		}

		void OnRenameButtonClicked(object sender, EventArgs args)
		{
		}

		void OnDeleteButtonClicked(object sender, EventArgs args)
		{
		}
	}
}
