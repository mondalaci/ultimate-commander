using System;
using System.Collections;
using Gtk;

namespace UltimateCommander {

	public class PanelSortingConfigurator: PanelConfigurator {
		
		public PanelSortingConfigurator(Panel other_panel): base(other_panel, "Sorting")
		{
			foreach (FileComparerInfo info in FileComparerInfo.AllInfos) {
				avail_store.AppendValues(info);
			}

			FileComparerType[] types = {
				FileComparerType.DirectoriesFirst,
				FileComparerType.Filename,
			};

			SetTypes(types);
			
			AppendColumn(avail_view, CellRendererType.Toggle, OnSetAvailableCellToggle);
			AppendColumn(avail_view, CellRendererType.Text, OnSetAvailableCellColumnName);
			AppendColumn(used_view, CellRendererType.Text, OnSetUsedCellColumnName);
		}

		public void SetTypes(FileComparerType[] types)
		{
			used_store.Clear();

			foreach (FileComparerType type in types) {
				FileComparerInfo info = FileComparerInfo.GetInfo(type);
				used_store.AppendValues(info);
			}

			panel.Comparer.SetTypes(types);
		}

		protected override void Synchronize()
		{
			TreeIter iter;
			bool has_element = used_store.GetIterFirst(out iter);

			RefreshButtonsSensitivity();

			ArrayList type_list = new ArrayList();
			while (has_element) {
				FileComparerInfo info = (FileComparerInfo)used_store.GetValue(iter, 0);
				type_list.Add(info.Type);
				has_element = used_store.IterNext(ref iter);
			}

			FileComparerType[] types =
				(FileComparerType[])type_list.ToArray(typeof(FileComparerType));
			panel.Comparer.SetTypes(types);
			panel.ChangeDirectory(panel.CurrentDirectory);
		}

		// Signal handlers

		void OnSetAvailableCellToggle(TreeViewColumn column, CellRenderer cellrenderer,
									  TreeModel model, TreeIter iter)
		{
           	FileComparerInfo comparerinfo = (FileComparerInfo)avail_store.GetValue(iter, 0);
			((CellRendererToggle)cellrenderer).Active = HasInfo(comparerinfo);
		}

		void OnSetAvailableCellColumnName(TreeViewColumn column, CellRenderer cellrenderer,
										  TreeModel model, TreeIter iter)
		{
           	FileComparerInfo comparerinfo = (FileComparerInfo)avail_store.GetValue(iter, 0);
			((CellRendererText)cellrenderer).Text = comparerinfo.Name;
		}
		
		void OnSetUsedCellColumnName(TreeViewColumn column, CellRenderer cellrenderer,
								     TreeModel model, TreeIter iter)
		{
           	FileComparerInfo comparerinfo = (FileComparerInfo)used_store.GetValue(iter, 0);
			((CellRendererText)cellrenderer).Text = comparerinfo.Name;
		}

		protected override void OnOkButtonClicked(object sender, EventArgs args)
		{
		}
	}
}
