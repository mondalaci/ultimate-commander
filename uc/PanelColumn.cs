using System;
using Gtk;

namespace UltimateCommander {

	public enum PanelColumnType {
		Toggle,
		FileName,
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

		CellRendererManipulator manipulator;
		Panel panel;
        //PanelColumnType type;

		public PanelColumn(PanelColumnType type, Panel panel_arg): base()
		{
            //type = type_arg;
			panel = panel_arg;
			PanelColumnInfo info = PanelColumnInfo.GetInfo(type);
			CellRenderer cellrenderer;

			switch (info.CellRendererType) {
			case CellRendererType.Toggle:
				cellrenderer = new CellRendererToggle();
				((CellRendererToggle)cellrenderer).Toggled += new ToggledHandler(OnToggled);
				break;
			case CellRendererType.Pixbuf:
				cellrenderer = new CellRendererPixbuf();
				break;
			default:  // CellRendererType.Text:
				cellrenderer = new CellRendererText();

                /*if (info.ColumnType == PanelColumnType.FileName) {
                    cellrenderer.EditingCanceled += new EventHandler(panel.OnCellEditingCanceled);
                    ((CellRendererText)cellrenderer).Edited += new EditedHandler(panel.OnCellEdited);
                }*/
				break;
			}
			
			manipulator = info.CellRendererManipulator;
            Expand = info.ColumnType == PanelColumnType.FileName;
            cellrenderer.Xalign = info.Alignment;
			PackStart(cellrenderer, true);
			SetCellDataFunc(cellrenderer, CellDataFunc);
			Resizable = true;
			Title = (string)info.ShortName;
		}

/*        public PanelColumnType ColumnType {
            get { return type; }
        }
*/
		void CellDataFunc(TreeViewColumn column, CellRenderer renderer,
						  TreeModel model, TreeIter iter)
		{
           	File file = (File)model.GetValue(iter, 0);
			renderer.CellBackgroundGdk =
				file.Selected ? Config.SelectedFileBgColor : 
				Widget.DefaultStyle.BaseColors[(int)StateType.Normal];
			manipulator(renderer, file);
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
