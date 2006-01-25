using System;
using Gtk;

namespace UltimateCommander {

	public class PanelFrame: Frame {

		public enum PanelFramePosition {Left, Right};

		Panel panel;
		PanelListingConfigurator listing_configurator;
		PanelSortConfigurator sort_configurator;
		PanelFramePosition position;

		PanelFrame other_frame;

		public PanelFrame(PanelFramePosition position): base()
		{
			this.position = position;
			panel = new Panel(".");
			listing_configurator = new PanelListingConfigurator();
			sort_configurator = new PanelSortConfigurator();
					
			AppendView(panel, "Panel");
		}

		public void ShowListing(bool show)
		{
			if (show) {
				AppendView(listing_configurator, "Set " + OtherPanelName + " Panel Listing");
			} else {
				RemoveView(listing_configurator);
			}

			ShowAll();
			SelectLastPage();
		}

		public void ShowSorting(bool show)
		{
			if (show) {
				AppendView(sort_configurator, "Set " + OtherPanelName + " Panel Sorting");
			} else {
				RemoveView(sort_configurator);
			}

			ShowAll();
			SelectLastPage();
		}

		void SelectLastPage()
		{
			Page = NPages -1;
			Slot slot = (Slot)GetNthPage(NPages-1);
			slot.Select();
		}

		string OtherPanelName {
			get {
			if (position == PanelFramePosition.Left)
				return "Right";
			else
				return "Left";
			}
		}

		public PanelFrame OtherFrame {
			get { return other_frame; }
			set { other_frame = value; }
		}

		public Panel Panel {
			get { return panel; }
		}

	}
}
