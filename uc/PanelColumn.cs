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

		CellRendererManipulator manipulator;
		Panel panel;

		public PanelColumn(PanelColumnType type, Panel panel_arg): base()
		{
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
				break;
			}
			
			manipulator = info.CellRendererManipulator;
            cellrenderer.Xalign = info.Alignment;
			PackStart(cellrenderer, true);
			SetCellDataFunc(cellrenderer, CellDataFunc);
			Resizable = true;
			Title = (string)info.ShortName;
		}

		void CellDataFunc(TreeViewColumn column, CellRenderer renderer,
						  TreeModel model, TreeIter iter)
		{
           	File file = (File)model.GetValue(iter, 0);
			renderer.CellBackgroundGdk =
				file.Selected ? Panel.SelectedRowBgColor : 
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
