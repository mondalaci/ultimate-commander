using System;
using System.Collections;
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

		enum CellRendererType {
			Toggle,
			Pixbuf,
			Text
		}

		delegate void CellRendererManipulator(CellRenderer cellrenderer, File file);

		class ColumnInfo {

			PanelColumnType panelcolumntype;
			CellRendererType cellrenderertype;
			CellRendererManipulator cellrenderermanipulator;
			string name;

			public ColumnInfo(PanelColumnType panelcolumntype_arg,
							  CellRendererType cellrenderertype_arg,
						      CellRendererManipulator cellrenderermanipulator_arg,
						      string name_arg)
			{
				panelcolumntype = panelcolumntype_arg;
				cellrenderertype = cellrenderertype_arg;
				cellrenderermanipulator = cellrenderermanipulator_arg;
				name = name_arg;
			}

			public PanelColumnType PanelColumnType {
				get { return panelcolumntype; }
			}

			public CellRendererType CellRendererType {
				get { return cellrenderertype; }
			}

			public CellRendererManipulator CellRendererManipulator {
				get { return cellrenderermanipulator; }
			}

			public string Name {
				get { return name; }
			}
		}

		static Gdk.Color selected_row_bgcolor = new Gdk.Color(224, 224, 0);

		static ColumnInfo[] column_infos = {
			// Toggle
			new ColumnInfo(PanelColumnType.Toggle, CellRendererType.Toggle,
				new CellRendererManipulator(SetCellRendererToggle), "S"),
			// Filename
			new ColumnInfo(PanelColumnType.Filename, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererFilename), "Filename"),
			// Size
			new ColumnInfo(PanelColumnType.Size, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererSize), "Size"),
			// Permissions
			new ColumnInfo(PanelColumnType.SymbolicPermissions, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererSymbolicPermissions), "Permissions"),
			// Owner User
			new ColumnInfo(PanelColumnType.OwnerUser, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerUser), "Owner"),
			// Owner User ID
			new ColumnInfo(PanelColumnType.OwnerUserId, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerUserId), "UID"),
			// Owner Group
			new ColumnInfo(PanelColumnType.OwnerGroup, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerGroup), "Group"),
			// Owner Group ID
			new ColumnInfo(PanelColumnType.OwnerGroupId, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerGroupId), "GID"),
			// Last Access Time
			new ColumnInfo(PanelColumnType.LastAccessTime, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLastAccessTime), "Last Access Time"),
			// Last Status Change Time
			new ColumnInfo(PanelColumnType.LastStatusChangeTime, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLastStatusChangeTime), "Last Status Change Time"),
			// Last Write Time
			new ColumnInfo(PanelColumnType.LastWriteTime, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLastWriteTime), "Last Write Time"),
			// Link Count
			new ColumnInfo(PanelColumnType.LinkCount, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLinkCount), "Link Count"),
			// Inode
			new ColumnInfo(PanelColumnType.Inode, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererInode), "Inode"),
			// Link Path
			new ColumnInfo(PanelColumnType.LinkPath, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLinkPath), "Link Path"),
			// Mime Type
			new ColumnInfo(PanelColumnType.MimeType, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererMimeType), "Mime Type"),
			// Description
			new ColumnInfo(PanelColumnType.Description, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererDescription), "Description"),
			// Mime Icon
			new ColumnInfo(PanelColumnType.MimeIcon, CellRendererType.Pixbuf,
				new CellRendererManipulator(SetCellRendererMimeIcon), "M"),
			// Attribute Icon
			new ColumnInfo(PanelColumnType.AttributeIcon, CellRendererType.Pixbuf,
				new CellRendererManipulator(SetCellRendererAttributeIcon), "A")
		};

		CellRendererManipulator cellrenderermanipulator;
		Panel panel;

		public PanelColumn(PanelColumnType type, Panel panel_arg): base()
		{
			panel = panel_arg;
			ColumnInfo column_info = null;

			foreach (ColumnInfo column_info_i in column_infos) {
				if (column_info_i.PanelColumnType == type) {
					column_info = column_info_i;
					break;
				}
			}

			CellRenderer cellrenderer;
			switch (column_info.CellRendererType) {
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
			
			cellrenderermanipulator = column_info.CellRendererManipulator;
			PackStart(cellrenderer, true);
			SetCellDataFunc(cellrenderer, CellDataFunc);
			Resizable = true;
			Title = (string)column_info.Name;
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

		// CellRendererManipulator delegates

		static void SetCellRendererToggle(CellRenderer cellrenderer, File file)
		{
			((CellRendererToggle)cellrenderer).Active = file.Selected;
			((CellRendererToggle)cellrenderer).Activatable = !file.IsUpDirectory;
		}

		static void SetCellRendererFilename(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.Name;
		}

		static void SetCellRendererSize(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.SizeString;
		}

		static void SetCellRendererSymbolicPermissions(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.SymbolicPermissions;
		}

		static void SetCellRendererOwnerUser(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerUser;
		}

		static void SetCellRendererOwnerUserId(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerUserIdString;
		}

		static void SetCellRendererOwnerGroup(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerGroup;
		}

		static void SetCellRendererOwnerGroupId(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerGroupIdString;
		}

		static void SetCellRendererLastAccessTime(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LastAccessTimeString;
		}

		static void SetCellRendererLastStatusChangeTime(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LastStatusChangeTimeString;
		}

		static void SetCellRendererLastWriteTime(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LastWriteTimeString;
		}

		static void SetCellRendererLinkCount(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LinkCount.ToString();
		}

		static void SetCellRendererInode(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.InodeString;
		}

		static void SetCellRendererLinkPath(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LinkPath;
		}

		static void SetCellRendererMimeType(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.MimeType;
		}

		static void SetCellRendererDescription(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.Description;
		}

		static void SetCellRendererMimeIcon(CellRenderer cellrenderer, File file)
		{
			((CellRendererPixbuf)cellrenderer).Pixbuf = file.MimeIcon;
		}

		static void SetCellRendererAttributeIcon(CellRenderer cellrenderer, File file)
		{
			((CellRendererPixbuf)cellrenderer).Pixbuf = file.AttributeIcon;
		}
	}
}