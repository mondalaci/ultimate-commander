using System;
using Gtk;

namespace UltimateCommander {

	public class PanelSortConfigurator: PanelConfigurator {
		
		public PanelSortConfigurator(): base()
		{
			available_label.Markup = "<b>Available Sort Orders</b>";
			used_label.Markup = "<b>Used Sort Orders</b>";
		}

		void OnAddButtonClicked(object o, EventArgs args)
		{
		}

		void OnAvailTypesViewRowActivated(object o, RowActivatedArgs args)
		{
		}

		void OnRemoveButtonClicked(object o, EventArgs args)
		{
		}

		void OnUsedTypesViewRowActivated(object o, RowActivatedArgs args)
		{
		}

		void OnUpButtonClicked(object o, EventArgs args)
		{
		}

		void OnDownButtonClicked(object o, EventArgs args)
		{
		}
	}
}
