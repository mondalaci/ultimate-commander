using System;
using Gtk;

namespace UltimateCommander {

	public class PanelFrame: Frame {

		Panel panel;

		public PanelFrame(string path): base()
		{
			panel = new Panel(path);
			string title = String.Format("{0} Panel", FrameSide);
			AppendView(panel, title);
		}

		public Panel Panel {
			get { return panel; }
		}

		public Panel OtherPanel {
			get { return OtherPanelFrame.Panel; }
		}

		public string FrameSide {
			get { return MainWindow.LeftPanelFrame == this ? "Left" : "Right"; }
		}

		public string OtherFrameSide {
			get { return MainWindow.LeftPanelFrame == this ? "Right" : "Left"; }
		}

        public PanelFrame OtherPanelFrame {
            get {
                return MainWindow.LeftPanelFrame == this ?
                    MainWindow.RightPanelFrame : MainWindow.LeftPanelFrame;
            }
        }
		public void ShowConfigurator(PanelConfigurator configurator, bool show)
		{
			if (show) {
				string title = String.Format("{0} Panel {1}", OtherFrameSide, configurator.Name);
				AppendView(configurator, title);
			    ShowAll();
			    SelectLastPage();
			} else {
				RemoveView(configurator);
                OtherPanelFrame.Select();
			}
		}

		void SelectLastPage()
		{
			Page = NPages -1;
			Slot slot = (Slot)GetNthPage(NPages-1);
			slot.Select();
		}
	}
}
