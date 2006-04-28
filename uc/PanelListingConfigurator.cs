using System;
using Gtk;

namespace UltimateCommander {

    public class PanelListingConfigurator: PanelConfigurator {
        
        public PanelListingConfigurator(Panel panel): base(panel, "Listing")
        {
            foreach (PanelColumnInfo columninfo in PanelColumnInfo.AllColumnInfos) {
                avail_store.AppendValues(columninfo);
            }

            PanelColumnType[] types = {
                PanelColumnType.Toggle,
                PanelColumnType.AttributeIcon,
                PanelColumnType.MimeIcon,
                PanelColumnType.FileName,
                PanelColumnType.Size
            };

            SetTypes(types);

            AppendColumn(avail_view, CellRendererType.Toggle, OnSetAvailableCellToggle);
            AppendColumn(avail_view, CellRendererType.Text, OnSetAvailableCellColumnName);
            AppendColumn(used_view, CellRendererType.Text, OnSetUsedCellColumnName);
        
            Title = "Current listing preset: Default";
        }

        public void SetTypes(PanelColumnType[] types)
        {
            used_store.Clear();

            foreach (PanelColumnType type in types) {
                PanelColumnInfo info = PanelColumnInfo.GetInfo(type);
                used_store.AppendValues(info);
            }

            Synchronize();
        }

        protected override void Synchronize()
        {
            TreeIter iter;
            bool has_element = used_store.GetIterFirst(out iter);

            RefreshButtonsSensitivity();

            foreach (TreeViewColumn column in panel.View.Columns) {
                panel.View.RemoveColumn(column);
            }

            while (has_element) {
                PanelColumnInfo columninfo = (PanelColumnInfo)used_store.GetValue(iter, 0);
                PanelColumn column = new PanelColumn(columninfo.ColumnType, panel);
                panel.View.AppendColumn(column);
                has_element = used_store.IterNext(ref iter);
            }
        }

        // Signal handlers

        void OnSetAvailableCellToggle(TreeViewColumn column, CellRenderer cellrenderer,
                                      TreeModel model, TreeIter iter)
        {
            PanelColumnInfo columninfo = (PanelColumnInfo)avail_store.GetValue(iter, 0);
            ((CellRendererToggle)cellrenderer).Active = HasInfo(columninfo);
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

        protected override void OnOkButtonClicked(object sender, EventArgs args)
        {
            PanelFrame.OtherPanel.DisableListing();
        }
    }
}
