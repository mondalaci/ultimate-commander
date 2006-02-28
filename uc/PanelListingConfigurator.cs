using System;
using Gtk;

namespace UltimateCommander {

	public class PanelListingConfigurator: PanelConfigurator {
		
		ListStore avail_store = new ListStore(typeof(PanelColumnInfo));
		ListStore used_store = new ListStore(typeof(PanelColumnInfo));

		public PanelListingConfigurator(Panel panel): base(panel, "Listing")
		{
			avail_view.Model = avail_store;
			used_view.Model = used_store;

			foreach (PanelColumnInfo columninfo in PanelColumnInfo.AllColumnInfos) {
				avail_store.AppendValues(columninfo);
			}

			AppendColumn(avail_view, CellRendererType.Toggle, OnSetAvailableCellToggle);
			AppendColumn(avail_view, CellRendererType.Text, OnSetAvailableCellColumnName);
			AppendColumn(used_view, CellRendererType.Text, OnSetUsedCellColumnName);
		}

		public void SetListing(PanelColumnType[] columntypes)
		{
			used_store.Clear();

			foreach (PanelColumnType columntype in columntypes) {
				PanelColumnInfo columninfo = PanelColumnInfo.GetColumnInfo(columntype);
				used_store.AppendValues(columninfo);
			}

			SynchronizePanelColumns();
		}

		void AppendColumn(TreeView view, CellRendererType cellrenderertype,
		                  TreeCellDataFunc celldatafunc)
		{
			TreeViewColumn column = new TreeViewColumn();
			
			CellRenderer cellrenderer;
			if (cellrenderertype == CellRendererType.Text) {
				cellrenderer = new CellRendererText();
			} else /* cellrenderertype == CellRendererType.Toggle */ {
				cellrenderer = new CellRendererToggle();
				((CellRendererToggle)cellrenderer).Toggled += new ToggledHandler(OnToggled);
			}

			column.PackStart(cellrenderer, true);
			column.SetCellDataFunc(cellrenderer, celldatafunc);
			view.AppendColumn(column);
		}

		void AppendColumnInfo(PanelColumnInfo columninfo)
		{
			used_store.AppendValues(columninfo);
			SynchronizePanelColumns();
		}

		void RemoveColumnInfo(PanelColumnInfo columninfo_arg)
		{
			TreeIter iter;
			bool has_element = used_store.GetIterFirst(out iter);

			while (has_element) {
				PanelColumnInfo columninfo = (PanelColumnInfo)used_store.GetValue(iter, 0);

				if (columninfo == columninfo_arg) {
					used_store.Remove(ref iter);
					CheckRemoveButtonSensitivity();
					SynchronizePanelColumns();
					return;
				}
				
				has_element = used_store.IterNext(ref iter);
			}
		}

		void RemoveColumns()
		{
			foreach (TreeViewColumn column in panel.View.Columns) {
				panel.View.RemoveColumn(column);
			}
		}

		bool HasColumnInfo(PanelColumnInfo columninfo_arg)
		{
			TreeIter iter;
			bool has_element = used_store.GetIterFirst(out iter);

			while (has_element) {
				PanelColumnInfo columninfo = (PanelColumnInfo)used_store.GetValue(iter, 0);

				if (columninfo == columninfo_arg) {
					return true;
				}

				has_element = used_store.IterNext(ref iter);
			}

			return false;
		}

		void SynchronizePanelColumns()
		{
			RemoveColumns();
			TreeIter iter;
			bool has_element = used_store.GetIterFirst(out iter);

			while (has_element) {
				PanelColumnInfo columninfo = (PanelColumnInfo)used_store.GetValue(iter, 0);
				PanelColumn column = new PanelColumn(columninfo.ColumnType, panel);
				panel.View.AppendColumn(column);
				has_element = used_store.IterNext(ref iter);
			}
		}

		void CheckRemoveButtonSensitivity()
		{
    			TreePath path;
	        TreeViewColumn column;
        	   	used_view.GetCursor(out path, out column);
			remove_button.Sensitive = (path != null);
		}

		// Utility methods and properties

		TreeIter GetCurrentIter(ListStore store, TreeView view)
		{
    			TreePath path;
	        TreeViewColumn column;
    	       	TreeIter iter;

        	   	view.GetCursor(out path, out column);
           	store.GetIter(out iter, path);
           	return iter;
		}

		PanelColumnInfo GetCurrentColumnInfo(ListStore store, TreeView view)
		{
			TreeIter iter = GetCurrentIter(store, view);
           	return (PanelColumnInfo)store.GetValue(iter, 0);
		}

		bool IterPrev(ListStore store, ref TreeIter iter)
		{
			TreePath path = store.GetPath(iter);

			if (!path.Prev()) {
				return false;
			}

			store.GetIter(out iter, path);
			return true;
		}

		// Signal handlers

		void OnSetAvailableCellToggle(TreeViewColumn column, CellRenderer cellrenderer,
									  TreeModel model, TreeIter iter)
		{
           	PanelColumnInfo columninfo = (PanelColumnInfo)avail_store.GetValue(iter, 0);
			((CellRendererToggle)cellrenderer).Active = HasColumnInfo(columninfo);
		}

		void OnSetAvailableCellColumnName(TreeViewColumn column, CellRenderer cellrenderer,
										  TreeModel model, TreeIter iter)
		{
           	PanelColumnInfo columninfo = (PanelColumnInfo)avail_store.GetValue(iter, 0);
			((CellRendererText)cellrenderer).Text = columninfo.LongName;
		}

		void OnSetUsedCellColumnName(TreeViewColumn column, CellRenderer cellrenderer,
								     TreeModel model, TreeIter iter)
		{
           	PanelColumnInfo columninfo = (PanelColumnInfo)used_store.GetValue(iter, 0);
			((CellRendererText)cellrenderer).Text = columninfo.LongName;
		}

		void OnToggled(object o, ToggledArgs args)
		{
			TreeIter iter;
			avail_store.GetIter(out iter, new TreePath(args.Path));
           	PanelColumnInfo columninfo = (PanelColumnInfo)avail_store.GetValue(iter, 0);

			if (HasColumnInfo(columninfo)) {
				RemoveColumnInfo(columninfo);
			} else {
				AppendColumnInfo(columninfo);
			}
		}

		void OnAvailTypesViewRowActivated(object o, RowActivatedArgs args)
		{
			TreeIter iter = GetCurrentIter(avail_store, avail_view);
           	PanelColumnInfo columninfo = (PanelColumnInfo)avail_store.GetValue(iter, 0);

			if (HasColumnInfo(columninfo)) {
				RemoveColumnInfo(columninfo);
			} else {
				AppendColumnInfo(columninfo);
			}
		}

		void OnRemoveButtonClicked(object o, EventArgs args)
		{
			PanelColumnInfo columninfo = GetCurrentColumnInfo(used_store, used_view);
			RemoveColumnInfo(columninfo);
			avail_view.QueueDraw();
		}
		
		void OnUsedTypesViewRowActivated(object o, RowActivatedArgs args)
		{
			PanelColumnInfo columninfo = GetCurrentColumnInfo(used_store, used_view);
			RemoveColumnInfo(columninfo);
			avail_view.QueueDraw();
		}

		void OnUpButtonClicked(object o, EventArgs args)
		{
			TreeIter iter = GetCurrentIter(used_store, used_view);
			TreeIter iter2 = iter;

			if (IterPrev(used_store, ref iter2)) {
				used_store.MoveBefore(iter, iter2);
				SynchronizePanelColumns();
			}
		}

		void OnDownButtonClicked(object o, EventArgs args)
		{
			TreeIter iter = GetCurrentIter(used_store, used_view);
			TreeIter iter2 = iter;

			if (used_store.IterNext(ref iter2)) {
				used_store.MoveAfter(iter, iter2);
				SynchronizePanelColumns();
			}
		}

		void OnCursorChanged(object o, EventArgs args)
		{
			CheckRemoveButtonSensitivity();
		}
	}
}
