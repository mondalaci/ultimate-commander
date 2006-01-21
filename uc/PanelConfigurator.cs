using System;
using Gtk;

namespace UltimateCommander {

	abstract public class PanelConfigurator: View {
		
		[Glade.Widget] protected Label available_label;
		[Glade.Widget] protected Label used_label;

		public PanelConfigurator(): base("panel_configurator_window")
		{
		}
	}
}
