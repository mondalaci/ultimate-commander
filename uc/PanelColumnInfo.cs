using System;
using Gtk;

namespace UltimateCommander {

	public enum CellRendererType {
		Toggle,
		Pixbuf,
		Text
	}

	public delegate void CellRendererManipulator(CellRenderer cellrenderer, File file);

	public class PanelColumnInfo {

		static public PanelColumnInfo GetColumnInfo(PanelColumnType columntype)
		{
			foreach (PanelColumnInfo columninfo in AllColumnInfos) {
				if (columntype == columninfo.ColumnType) {
					return columninfo;
				}
			}
			return null;
		}

		PanelColumnType columntype;
		CellRendererType cellrenderertype;
		CellRendererManipulator cellrenderermanipulator;
		string longname;
		string shortname;

		public PanelColumnInfo(PanelColumnType columntype_arg,
						  CellRendererType cellrenderertype_arg,
					      CellRendererManipulator cellrenderermanipulator_arg,
					      string longname_arg, string shortname_arg)
		{
			columntype = columntype_arg;
			cellrenderertype = cellrenderertype_arg;
			cellrenderermanipulator = cellrenderermanipulator_arg;
			longname = longname_arg;
			shortname = shortname_arg;
		}

		public PanelColumnType ColumnType {
			get { return columntype; }
		}

		public CellRendererType CellRendererType {
			get { return cellrenderertype; }
		}

		public CellRendererManipulator CellRendererManipulator {
			get { return cellrenderermanipulator; }
		}

		public string LongName {
			get { return longname; }
		}

		public string ShortName {
			get { return shortname; }
		}

		static public PanelColumnInfo[] AllColumnInfos {
			get { return column_infos; }
		}

		// The list of all ColumnInfos, the most used one first

		static PanelColumnInfo[] column_infos = {
			// Toggle
			new PanelColumnInfo(PanelColumnType.Toggle, CellRendererType.Toggle,
				new CellRendererManipulator(OnSetCellRendererToggle),
				"Selected Toggle", "S"),
			// Mime Icon
			new PanelColumnInfo(PanelColumnType.MimeIcon, CellRendererType.Pixbuf,
				new CellRendererManipulator(OnSetCellRendererMimeIcon),
				"Mime Icon", "M"),
			// Attribute Icon
			new PanelColumnInfo(PanelColumnType.AttributeIcon, CellRendererType.Pixbuf,
				new CellRendererManipulator(OnSetCellRendererAttributeIcon),
				"Attribute Icon", "A"),
			// Filename
			new PanelColumnInfo(PanelColumnType.Filename, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererFilename),
				"Filename", "Filename"),
			// Size
			new PanelColumnInfo(PanelColumnType.Size, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererSize),
				"Size", "Size"),
			// Permissions
			new PanelColumnInfo(PanelColumnType.SymbolicPermissions, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererSymbolicPermissions),
				"Permissions", "Permissions"),
			// Owner User
			new PanelColumnInfo(PanelColumnType.OwnerUser, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererOwnerUser),
				"Owner Name", "Owner"),
			// Owner User ID
			new PanelColumnInfo(PanelColumnType.OwnerUserId, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererOwnerUserId),
				"Owner ID", "UID"),
			// Owner Group
			new PanelColumnInfo(PanelColumnType.OwnerGroup, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererOwnerGroup),
				"Group Name", "Group"),
			// Owner Group ID
			new PanelColumnInfo(PanelColumnType.OwnerGroupId, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererOwnerGroupId),
				"Group ID", "GID"),
			// Last Access Time
			new PanelColumnInfo(PanelColumnType.LastAccessTime, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererLastAccessTime),
				"Last Access Time", "Last Access Time"),
			// Last Status Change Time
			new PanelColumnInfo(PanelColumnType.LastStatusChangeTime, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererLastStatusChangeTime),
				"Last Status Change Time", "Last Change Time"),
			// Last Write Time
			new PanelColumnInfo(PanelColumnType.LastWriteTime, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererLastWriteTime),
				"Last Write Time", "Last Write Time"),
			// Mime Type
			new PanelColumnInfo(PanelColumnType.MimeType, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererMimeType),
				"Mime Type", "Mime Type"),
			// Description
			new PanelColumnInfo(PanelColumnType.Description, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererDescription),
				"Description", "Description"),
			// Link Path
			new PanelColumnInfo(PanelColumnType.LinkPath, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererLinkPath),
				"Link Path", "Link Path"),
			// Link Count
			new PanelColumnInfo(PanelColumnType.LinkCount, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererLinkCount),
				"Link Count", "Link Count"),
			// Inode
			new PanelColumnInfo(PanelColumnType.Inode, CellRendererType.Text,
				new CellRendererManipulator(OnSetCellRendererInode),
				"Inode", "Inode")
		};

		// CellRendererManipulator delegates

		static void OnSetCellRendererToggle(CellRenderer cellrenderer, File file)
		{
			((CellRendererToggle)cellrenderer).Active = file.Selected;
			((CellRendererToggle)cellrenderer).Activatable = !file.IsUpDirectory;
		}

		static void OnSetCellRendererFilename(CellRenderer cellrenderer, File file)
		{
			CellRendererText cellrenderertext = (CellRendererText)cellrenderer;
			cellrenderertext.Text = file.NameString;
			cellrenderertext.ForegroundGdk =
				file.HasValidEncoding ? Widget.DefaultStyle.Black : Panel.InvalidEncodingColor;
		}

		static void OnSetCellRendererSize(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.SizeString;
		}

		static void OnSetCellRendererSymbolicPermissions(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.SymbolicPermissions;
		}

		static void OnSetCellRendererOwnerUser(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerUser;
		}

		static void OnSetCellRendererOwnerUserId(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerUserIdString;
		}

		static void OnSetCellRendererOwnerGroup(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerGroup;
		}

		static void OnSetCellRendererOwnerGroupId(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.OwnerGroupIdString;
		}

		static void OnSetCellRendererLastAccessTime(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LastAccessTimeString;
		}

		static void OnSetCellRendererLastStatusChangeTime(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LastStatusChangeTimeString;
		}

		static void OnSetCellRendererLastWriteTime(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LastWriteTimeString;
		}

		static void OnSetCellRendererLinkCount(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LinkCountString;
		}

		static void OnSetCellRendererInode(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.InodeString;
		}

		static void OnSetCellRendererLinkPath(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.LinkPath;
		}

		static void OnSetCellRendererMimeType(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.MimeType;
		}

		static void OnSetCellRendererDescription(CellRenderer cellrenderer, File file)
		{
			((CellRendererText)cellrenderer).Text = file.Description;
		}

		static void OnSetCellRendererMimeIcon(CellRenderer cellrenderer, File file)
		{
			((CellRendererPixbuf)cellrenderer).Pixbuf = file.MimeIcon;
		}

		static void OnSetCellRendererAttributeIcon(CellRenderer cellrenderer, File file)
		{
			((CellRendererPixbuf)cellrenderer).Pixbuf = file.AttributeIcon;
		}
	}
}