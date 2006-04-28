using System;
using Gtk;

namespace UltimateCommander {

    public abstract class PanelConfigurator: View {
        
        [Glade.Widget] protected TreeView avail_view;
        [Glade.Widget] protected TreeView used_view;
        [Glade.Widget] Label avail_label;
        [Glade.Widget] Label used_label;
        [Glade.Widget] Button remove_button;
        [Glade.Widget] Button up_button;
        [Glade.Widget] Button down_button;

        protected ListStore avail_store;
        protected ListStore used_store;
        protected Panel panel;
        string name;
        
        public PanelConfigurator(Panel panel_arg, string name_arg):
            base("panel_configurator_widget")
        {
            panel = panel_arg;
            name = name_arg;
            avail_label.Markup = "<b>Available " + Name + " Types</b>";
            used_label.Markup = "<b>Used " + Name + " Types</b>";
            avail_view.Model = avail_store = new ListStore(typeof(PanelConfiguratorInfo));
            used_view.Model = used_store = new ListStore(typeof(PanelConfiguratorInfo));
        }

        public new string Name {
            get { return name; }
        }

        protected abstract void Synchronize();

        protected void AppendColumn(TreeView view, CellRendererType cellrenderertype,
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

        protected bool HasInfo(PanelConfiguratorInfo info_arg)
        {
            TreeIter iter;
            bool has_element = used_store.GetIterFirst(out iter);

            while (has_element) {
                PanelConfiguratorInfo info = (PanelConfiguratorInfo)used_store.GetValue(iter, 0);

                if (info == info_arg) {
                    return true;
                }

                has_element = used_store.IterNext(ref iter);
            }

            return false;
        }

        protected void RefreshButtonsSensitivity()
        {
            TreePath path;
            TreeViewColumn column;
            used_view.GetCursor(out path, out column);

            if (path == null) {
                remove_button.Sensitive = false;
                up_button.Sensitive = false;
                down_button.Sensitive = false;
            } else {
                TreeIter iter;
                TreePath next_path = path.Copy();
                next_path.Next();
                remove_button.Sensitive = true;
                up_button.Sensitive = path.Prev();
                down_button.Sensitive = used_store.GetIter(out iter, next_path);
            }
        }

        void InvertInfo(TreeIter iter)
        {
            PanelConfiguratorInfo info = (PanelConfiguratorInfo)avail_store.GetValue(iter, 0);

            if (HasInfo(info)) {
                RemoveInfo(info);
            } else {
                AppendInfo(info);
            }
        }

        void RemoveInfo(PanelConfiguratorInfo info_arg)
        {
            TreeIter iter;
            bool has_element = used_store.GetIterFirst(out iter);

            while (has_element) {
                PanelConfiguratorInfo info = (PanelConfiguratorInfo)used_store.GetValue(iter, 0);

                if (info == info_arg) {
                    used_store.Remove(ref iter);
                    Synchronize();
                    return;
                }
                
                has_element = used_store.IterNext(ref iter);
            }
        }

        void AppendInfo(PanelConfiguratorInfo info)
        {
            used_store.AppendValues(info);
            Synchronize();
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

        PanelConfiguratorInfo GetCurrentInfo(ListStore store, TreeView view)
        {
            TreeIter iter = GetCurrentIter(store, view);
            return (PanelConfiguratorInfo)store.GetValue(iter, 0);
        }

        TreeIter GetCurrentIter(ListStore store, TreeView view)
        {
            TreePath path;
            TreeViewColumn column;
            TreeIter iter;

            view.GetCursor(out path, out column);
            store.GetIter(out iter, path);
            return iter;
        }

        // Signal handlers

        void OnToggled(object o, ToggledArgs args)
        {
            TreeIter iter;
            avail_store.GetIter(out iter, new TreePath(args.Path));
            InvertInfo(iter);
        }

        void OnAvailTypesViewRowActivated(object o, RowActivatedArgs args)
        {
            TreeIter iter = GetCurrentIter(avail_store, avail_view);
            InvertInfo(iter);
        }

        void OnUsedTypesViewRowActivated(object sender, RowActivatedArgs args)
        {
            PanelConfiguratorInfo columninfo = GetCurrentInfo(used_store, used_view);
            RemoveInfo(columninfo);
            avail_view.QueueDraw();
        }

        void OnRemoveButtonClicked(object sender, EventArgs args)
        {
            PanelConfiguratorInfo comparerinfo = GetCurrentInfo(used_store, used_view);
            RemoveInfo(comparerinfo);
            avail_view.QueueDraw();
        }

        void OnUpButtonClicked(object sender, EventArgs args)
        {
            TreeIter iter = GetCurrentIter(used_store, used_view);
            TreeIter iter2 = iter;
            IterPrev(used_store, ref iter2);
            used_store.MoveBefore(iter, iter2);
            Synchronize();
        }

        void OnDownButtonClicked(object sender, EventArgs args)
        {
            TreeIter iter = GetCurrentIter(used_store, used_view);
            TreeIter iter2 = iter;
            used_store.IterNext(ref iter2);
            used_store.MoveAfter(iter, iter2);
            Synchronize();
        }

        protected abstract void OnOkButtonClicked(object sender, EventArgs args);

        void OnCursorChanged(object sender, EventArgs args)
        {
            RefreshButtonsSensitivity();
        }
    }
}
