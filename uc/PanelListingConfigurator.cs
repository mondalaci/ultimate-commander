using System;
using Gtk;

namespace UltimateCommander {

	public class PanelListingConfigurator: PanelConfigurator {
		
		public PanelListingConfigurator(): base()
		{
			available_label.Markup = "<b>Available Listing Modes</b>";
			used_label.Markup = "<b>Used Listing Modes</b>";
		}
	}
}
