using System;
using Gtk;
using Glade;

namespace UltimateCommander {

	public class MainWindow {

		[Glade.Widget] HPaned hpaned;
		[Glade.Widget] Gtk.Window main_window;
		[Glade.Widget] EventBox infobar_slot;

		float panel_ratio = 0.5f;
		int width = 0;
		int old_width = 0;

        static PanelFrame left_panel_frame;
        static PanelFrame right_panel_frame;
		static InfoBar infobar = new InfoBar();
        static Frame active_frame;

		public MainWindow()
		{
			Glade.XML glade_xml = new Glade.XML(Config.GladeFileName, "main_window", null);
			glade_xml.Autoconnect(this);

			left_panel_frame = new PanelFrame(Config.InitialPath);
			hpaned.Add1(left_panel_frame);

			right_panel_frame = new PanelFrame(Config.InitialPath);
			hpaned.Add2(right_panel_frame);

			ResizePanes();
			infobar_slot.Add(infobar);
			InfoBar.Notice("Ultimate Commander started.");

			main_window.ShowAll();
		}

        public static PanelFrame LeftPanelFrame {
            get { return left_panel_frame; }
        }

        public static PanelFrame RightPanelFrame {
            get { return right_panel_frame; }
        }

        public static Frame ActiveFrame {
            get { return active_frame; }
            set { active_frame = value; }
        }

        public static Panel LeftPanel {
            get { return left_panel_frame.Panel; }
        }

        public static Panel RightPanel {
            get { return right_panel_frame.Panel; }
        }

		public static InfoBar InfoBar {
			get { return infobar; }
		}

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

		// Rename signal handlers
		
		void OnRenameButtonClicked(object sender, EventArgs args)
		{
            ((PanelFrame)ActiveFrame).Panel.StartRename();
		}

		void OnRenameMenuItemActivate(object o, EventArgs args)
		{
            ((PanelFrame)ActiveFrame).Panel.StartRename();
		}

     	// Quit signal handers
     	
     	void OnWindowDeleteEvent(object o, DeleteEventArgs args)
     	{
          	Gtk.Application.Quit();
     	}

		void OnQuitMenuItemActivate(object o, EventArgs args)
		{
			Gtk.Application.Quit();
		}

		// Other signal handlers
		
		void OnWindowCheckResize(object o, EventArgs args)
		{
			ResizePanes();
		}
	}
}
