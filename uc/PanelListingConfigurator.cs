using System;
using Gtk;

namespace UltimateCommander {

	public class PanelListingConfigurator: PanelConfigurator {
		
		enum ListingSource { Available, Used };

		static ListingType[] available_types = {
			new ListingType("Filename", null),
			new ListingType("Size", new string[] {"Tarditional units (MB)", "SI units (MiB)"}),
			new ListingType("Permission", null),
			new ListingType("Owner", new string[] {"Owner name", "Owner ID", "Owner name and ID"}),
			new ListingType("Group", new string[] {"Group name", "Group ID", "Group name and ID"}),
			new ListingType("Time of last access", null),
			new ListingType("Time of last modification", null),
			new ListingType("Time of last status change", null)
		};

		ListStore avail_types_store = new ListStore(new Type[] {typeof(ListingType)});
		ListStore used_types_store = new ListStore(new Type[] {typeof(ListingType)});
		TreeViewColumn avail_types_first_col;
		TreeViewColumn used_types_first_col;

		void OnListAvailableCellData(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	ListingType type = (ListingType)avail_types_store.GetValue(iter, 0);
			cellrenderertext.Text = type.Name;
		}

		void OnListUsedCellTextData1(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	ListingType type = (ListingType)used_types_store.GetValue(iter, 0);
			cellrenderertext.Text = type.Name;
		}

		void OnListUsedCellTextData2(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	//ListingType type = (ListingType)used_types_store.GetValue(iter, 0);
			cellrenderertext.Markup = "<b>[</b> <u>Tarditional units (MB)</u> <b>|</b> SI units (MiB) <b>]</b>";
		}
		
		public PanelListingConfigurator(): base()
		{
			available_label.Markup = "<b>Available Listing Modes</b>";
			used_label.Markup = "<b>Used Listing Modes</b>";

			foreach (ListingType type in available_types) {
				avail_types_store.AppendValues(type);
			}

			// Name column.

			CellRendererText cellrenderertext = new CellRendererText();
			avail_types_first_col = new TreeViewColumn();
			avail_types_first_col.PackStart(cellrenderertext, true);
			avail_types_first_col.SetCellDataFunc(cellrenderertext, OnListAvailableCellData);

			avail_types_view.AppendColumn(avail_types_first_col);
			avail_types_view.Model = avail_types_store;
			avail_types_view.SetCursor(new TreePath("0"), avail_types_first_col, false);

			// ComboBox column.

			CellRendererText cellrenderertext2 = new CellRendererText();
			used_types_first_col = new TreeViewColumn("Type", cellrenderertext2, (""));
			//used_types_first_col.PackStart(cellrenderertext2, true);
			used_types_first_col.SetCellDataFunc(cellrenderertext2, OnListUsedCellTextData1);
			used_types_view.AppendColumn(used_types_first_col);

			CellRendererText cellrenderercombo = new CellRendererText();
			TreeViewColumn used_types_second_col = new TreeViewColumn("Options", cellrenderercombo, (""));
			//used_types_second_col.PackStart(cellrenderercombo, true);
			used_types_second_col.SetCellDataFunc(cellrenderercombo, OnListUsedCellTextData2);
			used_types_view.AppendColumn(used_types_second_col);

			used_types_view.Model = used_types_store;
			used_types_view.SetCursor(new TreePath("0"), used_types_first_col, false);
		}

		ListingType RemoveListingType(ListingSource src)
		{
			ListStore src_store;
			TreeView view;
			TreeViewColumn col;

			if (src == ListingSource.Available) {
				src_store = avail_types_store;
				view = avail_types_view;
				col = avail_types_first_col;
			} else {
				src_store = used_types_store;
				view = used_types_view;
				col = used_types_first_col;
			}
			
			TreePath path;
			TreeViewColumn column;
			TreeIter iter;

			view.GetCursor(out path, out column);
			src_store.GetIter(out iter, path);
			ListingType type = (ListingType)src_store.GetValue(iter, 0);
			TreeIter orig_iter = iter;

			if (src_store.IterNext(ref iter)) {  // not last entry
				path = src_store.GetPath(iter);
				view.SetCursor(path, col, false);
			} else {  // last entry
				TreePath prev_path = src_store.GetPath(orig_iter);
				prev_path.Prev();
				view.SetCursor(prev_path, col, false);
			}
			
			src_store.Remove(ref orig_iter);
			return type;
		}

		void OnAddButtonClicked(object o, EventArgs args)
		{
			ListingType type = RemoveListingType(ListingSource.Available);
			used_types_store.AppendValues(type);
		}

		void OnAvailTypesViewRowActivated(object o, RowActivatedArgs args)
		{
			ListingType type = RemoveListingType(ListingSource.Available);
			used_types_store.AppendValues(type);
		}

		void OnRemoveButtonClicked(object o, EventArgs args)
		{
			ListingType type = RemoveListingType(ListingSource.Used);
			avail_types_store.AppendValues(type);
		}
		
		void OnUsedTypesViewRowActivated(object o, RowActivatedArgs args)
		{
			ListingType type = RemoveListingType(ListingSource.Used);
			avail_types_store.AppendValues(type);
		}

		void OnUpButtonClicked(object o, EventArgs args)
		{
/*			TreePath path;
			TreeViewColumn column;
			TreeIter iter;

			view.GetCursor(out path, out column);
			src_store.GetIter(out iter, path);
			ListingType type = (ListingType)src_store.GetValue(iter, 0);
			TreeIter orig_iter = iter;

			used_types_store.MoveBefore();*/
		}

		void OnDownButtonClicked(object o, EventArgs args)
		{
		}
	}
}
