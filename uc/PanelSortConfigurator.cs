using System;
using Gtk;

namespace UltimateCommander {

	public class PanelSortConfigurator: PanelConfigurator {
		
		public PanelSortConfigurator(): base()
		{
			available_label.Markup = "<b>Available Sort Orders</b>";
			used_label.Markup = "<b>Used Sort Orders</b>";
		}
	}
}
