using System;
using Gtk;

namespace UltimateCommander {

	public enum PanelColumnType {
		Toggle,
		Filename,
		Size,
		SymbolicPermissions,
		OwnerUser,
		OwnerUserId,
		OwnerGroup,
		OwnerGroupId,
		LastAccessTime,
		LastStatusChangeTime,
		LastWriteTime,
		LinkCount,
		Inode,
		LinkPath,
		MimeType,
		Description,
		MimeIcon,
		AttributeIcon
	}

	public class PanelColumn: TreeViewColumn {

		static Gdk.Color selected_row_bgcolor = new Gdk.Color(224, 224, 0);

		PanelColumnInfo columninfo;
		CellRendererManipulator cellrenderermanipulator;
		Panel panel;

		public PanelColumn(PanelColumnType type, Panel panel_arg): base()
		{
			panel = panel_arg;

			foreach (PanelColumnInfo columninfo_i in PanelColumnInfo.AllColumnInfos) {
				if (columninfo_i.ColumnType == type) {
					columninfo = columninfo_i;
					break;
				}
			}

			CellRenderer cellrenderer;
			switch (columninfo.CellRendererType) {
			case CellRendererType.Toggle:
				cellrenderer = new CellRendererToggle();
				((CellRendererToggle)cellrenderer).Toggled += new ToggledHandler(OnToggled);
				break;
			case CellRendererType.Pixbuf:
				cellrenderer = new CellRendererPixbuf();
				break;
			default:  // CellRendererType.Text:
				cellrenderer = new CellRendererText();
				break;
			}
			
			cellrenderermanipulator = columninfo.CellRendererManipulator;
			PackStart(cellrenderer, true);
			SetCellDataFunc(cellrenderer, CellDataFunc);
			Resizable = true;
			Title = (string)columninfo.ShortName;
		}

		public PanelColumnInfo ColumnInfo {
			get { return columninfo; }
		}

		void CellDataFunc(TreeViewColumn column, CellRenderer cellrenderer,
						  TreeModel model, TreeIter iter)
		{
           	File file = (File)model.GetValue(iter, 0);

           	if (file.Selected) {
           		cellrenderer.CellBackgroundGdk = selected_row_bgcolor;
           	} else {
           		cellrenderer.CellBackgroundGdk = 
           			Widget.DefaultStyle.BaseColors[(int)StateType.Normal];
			}

			cellrenderermanipulator(cellrenderer, file);
		}

		void OnToggled(object o, ToggledArgs args)
		{
			TreeIter iter;
			if (panel.Store.GetIter(out iter, new TreePath(args.Path))) {
	           	File file = (File)panel.Store.GetValue(iter, 0);
				file.Selected = !file.Selected;
			}
		}
	}
}