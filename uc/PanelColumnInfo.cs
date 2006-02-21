using Gtk;

namespace UltimateCommander {

	enum CellRendererType {
		Toggle,
		Pixbuf,
		Text
	}

	delegate void CellRendererManipulator(CellRenderer cellrenderer, File file);

	class PanelColumnInfo {

		PanelColumnType panelcolumntype;
		CellRendererType cellrenderertype;
		CellRendererManipulator cellrenderermanipulator;
		string name;

		public PanelColumnInfo(PanelColumnType panelcolumntype_arg,
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

		static public PanelColumnInfo[] AllColumnInfos {
			get { return column_infos; }
		}

		static PanelColumnInfo[] column_infos = {
			// Toggle
			new PanelColumnInfo(PanelColumnType.Toggle, CellRendererType.Toggle,
				new CellRendererManipulator(SetCellRendererToggle), "S"),
			// Filename
			new PanelColumnInfo(PanelColumnType.Filename, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererFilename), "Filename"),
			// Size
			new PanelColumnInfo(PanelColumnType.Size, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererSize), "Size"),
			// Permissions
			new PanelColumnInfo(PanelColumnType.SymbolicPermissions, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererSymbolicPermissions), "Permissions"),
			// Owner User
			new PanelColumnInfo(PanelColumnType.OwnerUser, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerUser), "Owner"),
			// Owner User ID
			new PanelColumnInfo(PanelColumnType.OwnerUserId, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerUserId), "UID"),
			// Owner Group
			new PanelColumnInfo(PanelColumnType.OwnerGroup, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerGroup), "Group"),
			// Owner Group ID
			new PanelColumnInfo(PanelColumnType.OwnerGroupId, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererOwnerGroupId), "GID"),
			// Last Access Time
			new PanelColumnInfo(PanelColumnType.LastAccessTime, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLastAccessTime), "Last Access Time"),
			// Last Status Change Time
			new PanelColumnInfo(PanelColumnType.LastStatusChangeTime, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLastStatusChangeTime), "Last Status Change Time"),
			// Last Write Time
			new PanelColumnInfo(PanelColumnType.LastWriteTime, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLastWriteTime), "Last Write Time"),
			// Link Count
			new PanelColumnInfo(PanelColumnType.LinkCount, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLinkCount), "Link Count"),
			// Inode
			new PanelColumnInfo(PanelColumnType.Inode, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererInode), "Inode"),
			// Link Path
			new PanelColumnInfo(PanelColumnType.LinkPath, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererLinkPath), "Link Path"),
			// Mime Type
			new PanelColumnInfo(PanelColumnType.MimeType, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererMimeType), "Mime Type"),
			// Description
			new PanelColumnInfo(PanelColumnType.Description, CellRendererType.Text,
				new CellRendererManipulator(SetCellRendererDescription), "Description"),
			// Mime Icon
			new PanelColumnInfo(PanelColumnType.MimeIcon, CellRendererType.Pixbuf,
				new CellRendererManipulator(SetCellRendererMimeIcon), "M"),
			// Attribute Icon
			new PanelColumnInfo(PanelColumnType.AttributeIcon, CellRendererType.Pixbuf,
				new CellRendererManipulator(SetCellRendererAttributeIcon), "A")
		};

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
			((CellRendererText)cellrenderer).Text = file.LinkCountString;
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