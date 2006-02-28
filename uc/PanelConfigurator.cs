using System;
using Gtk;

namespace UltimateCommander {

	abstract public class PanelConfigurator: View {
		
		[Glade.Widget] protected Label available_label;
		[Glade.Widget] protected Label used_label;
		[Glade.Widget] protected TreeView avail_view;
		[Glade.Widget] protected TreeView used_view;
		[Glade.Widget] protected Button remove_button;

		protected Panel panel;
		private string name;
		
		public PanelConfigurator(Panel panel_arg, string name_arg): base("panel_configurator_window")
		{
			panel = panel_arg;
			name = name_arg;

			available_label.Markup = "<b>Available " + Name + " Types</b>";
			used_label.Markup = "<b>Used " + Name + " Types</b>";
		}

		new public string Name {
			get { return name; }
		}
	}
}
