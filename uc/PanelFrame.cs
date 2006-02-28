using System;
using Gtk;

namespace UltimateCommander {

	public enum PanelFramePosition {
		Left,
		Right
	};

	public class PanelFrame: Frame {

		Panel panel;
		PanelFramePosition position;
		PanelFrame other_frame;

		public PanelFrame(string path, PanelFramePosition position_arg): base()
		{
			position = position_arg;
			panel = new Panel(path);
			AppendView(panel, FrameName + " Panel");
		}

		public void ShowConfigurator(PanelConfigurator configurator, bool show)
		{
			if (show) {
				string title = "Set " + OtherFrameName + " Panel " + configurator.Name;
				AppendView(configurator, title);
			} else {
				RemoveView(configurator);
			}

			ShowAll();
			SelectLastPage();
		}

		public PanelFrame OtherFrame {
			set { other_frame = value; }
			get { return other_frame; }
		}

		public Panel Panel {
			get { return panel; }
		}

		public Panel OtherPanel {
			get { return OtherFrame.Panel; }
		}

		void SelectLastPage()
		{
			Page = NPages -1;
			Slot slot = (Slot)GetNthPage(NPages-1);
			slot.Select();
		}

		string FrameName {
			get {
				if (position == PanelFramePosition.Left) {
					return "Left";
				} else /* position == PanelFramePosition.Right */ {
					return "Right";
				}
			}
		}

		string OtherFrameName {
			get {
				if (position == PanelFramePosition.Left) {
					return "Right";
				} else /* position == PanelFramePosition.Right */ {
					return "Left";
				}
			}
		}
	}
}
