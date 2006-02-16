using System;
using Gtk;

namespace UltimateCommander {

	abstract public class PanelConfigurator: View {
		
		[Glade.Widget] protected Label available_label;
		[Glade.Widget] protected Label used_label;
		[Glade.Widget] protected TreeView avail_types_view;
		[Glade.Widget] protected TreeView used_types_view;

		public PanelConfigurator(): base("panel_configurator_window")
		{
		}
	}
}
