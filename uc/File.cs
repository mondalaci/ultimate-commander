using System;
using System.Collections;
using System.Text;
using Mono.Unix;
using MUN = Mono.Unix.Native;
using Gdk;
using Gnome.Vfs;

namespace UltimateCommander {

	public enum SymbolicLinkType {NotLink, ValidLink, DanglingLink};

	public class File {

		static int icon_size = 16;

		// If the actual maximum path length is greater
		// than this value, we're in some serious shit.
		static int max_path_length = 512;  

		static Hashtable mime_to_icon_hash = new Hashtable();
		static AttributeIcons attribute_icons = new AttributeIcons();

		static Gdk.Pixbuf updir_icon = LoadIcon(Gtk.Stock.GoUp);
		static Gdk.Pixbuf directory_icon = LoadIcon("gnome-fs-directory");
		static Gdk.Pixbuf fifo_icon = LoadIcon("gnome-fs-fifo");
		static Gdk.Pixbuf socket_icon = LoadIcon("gnome-fs-socket");
		static Gdk.Pixbuf chardev_icon = LoadIcon("gnome-fs-chardev");
		static Gdk.Pixbuf blockdev_icon = LoadIcon("gnome-fs-blockdev");

		bool selected;
		string fullpath;
		string filename;
		MUN.Stat stat;
		MUN.Stat lstat;
		SymbolicLinkType linktype;
		string linkpath = "";
		string mimetype;
		string description;
		Gdk.Pixbuf mime_icon;
		Gdk.Pixbuf attribute_icon;
		
		public File(string fullpath_arg) {
			Selected = false;
			fullpath = fullpath_arg;
			filename = System.IO.Path.GetFileName(fullpath);
			MUN.Syscall.stat(fullpath, out stat);
			MUN.Syscall.lstat(fullpath, out lstat);
			
			if (IsSymbolicLink) {
				StringBuilder dest_strbuilder = new StringBuilder(max_path_length);
				MUN.Syscall.readlink(fullpath, dest_strbuilder);
				string dest = dest_strbuilder.ToString();

				if (!System.IO.Path.IsPathRooted(dest)) {
					string directoryname = System.IO.Path.GetDirectoryName(fullpath);
					dest = System.IO.Path.Combine(directoryname, dest);
				}

				if (MUN.Syscall.access(dest, MUN.AccessModes.F_OK) == 0) {
					linktype = SymbolicLinkType.ValidLink;
					linkpath = dest;
				} else {
					linktype = SymbolicLinkType.DanglingLink;
				}
			} else {
				linktype = SymbolicLinkType.NotLink;
			}

			mimetype = Mime.TypeFromName(filename);
			description = Mime.GetDescription(MimeType);
			mime_icon = GetMimeIcon();
			attribute_icon =  attribute_icons.GetIcon(IsExecutable, !IsWritable,
													  !IsReadable, LinkType);
		}

		// Selection handling

		public bool Selected {
			get { return selected; }
			set { selected = value; }
		}

		public void InvertSelection()
		{
			if (!IsUpDirectory) {
				Selected = !Selected;
			}
		}

		// Often used VFS properties

		public string FullPath {
			get { return fullpath; }
		}

		public string Name {
			get { return filename; }
		}

		public long Size {
			get {
				if (LinkType == SymbolicLinkType.DanglingLink) {
					return 0;
				} else {
					return stat.st_size;
				}
			}
		}

		// Protection properties

		public MUN.FilePermissions Permissions {
			get { return stat.st_mode; }
		}

		//OctalPermissions

		//SymbolicPermissions

		// Owner / Group properties

		//OwnerUser

		//OwnerUserId

		//OwnerGroup

		//OwnerGroupId

		public long LastAccessTime {
			get { return stat.st_atime; }
		}

		public long LastStatusChangeTime {
			get { return stat.st_mtime; }
		}

		public long LastWriteTimeTime {
			get { return stat.st_ctime; }
		}

		// Rarely used VFS properties

		public ulong LinkCount {
			get { return stat.st_nlink; }
		}

		public ulong Inode {
			get { return stat.st_ino; }
		}
		
		// Symbolic link related properties

		public SymbolicLinkType LinkType {
			get { return linktype; }
		}

		public string LinkPath {
			get { return linkpath; }
		}

		// Mime properties

		public string MimeType {
			get { return mimetype; }
		}
		
		public string Description {
			get { return description; }
		}

		// Icon properties

		public Gdk.Pixbuf MimeIcon {
			get { return mime_icon; }
		}

		public Gdk.Pixbuf AttributeIcon {
			get { return attribute_icon; }
		}

		// Type properties

		public bool IsFile {
			get { return GetFlag(stat, MUN.FilePermissions.S_IFREG); }
		}

		public bool IsDirectory {
			get { return GetFlag(stat, MUN.FilePermissions.S_IFDIR); }
		}

		public bool IsFifo {
			get { return GetFlag(stat, MUN.FilePermissions.S_IFIFO); }
		}

		public bool IsSocket {
			get { return GetFlag(stat, MUN.FilePermissions.S_IFSOCK); }
		}

		public bool IsCharacterDevice {
			get { return GetFlag(stat, MUN.FilePermissions.S_IFCHR); }
		}

		public bool IsBlockDevice {
			get { return GetFlag(stat, MUN.FilePermissions.S_IFBLK); }
		}
		
		public bool IsSymbolicLink {
			get { return GetFlag(lstat, MUN.FilePermissions.S_IFLNK); }
		}

		public bool IsUpDirectory {
			get { return Name == ".."; }
		}

		// Access properties

		public bool IsReadable {
			get { return MUN.Syscall.access(FullPath, MUN.AccessModes.R_OK) == 0; }
		}

		public bool IsWritable {
			get { return MUN.Syscall.access(FullPath, MUN.AccessModes.W_OK) == 0; }
		}

		public bool IsExecutable {
			get { return IsFile && MUN.Syscall.access(FullPath, MUN.AccessModes.X_OK) == 0; }
		}

		// Private methods

		static Gdk.Pixbuf LoadIcon(string iconname)
		{
			return Gtk.IconTheme.Default.LoadIcon(iconname, icon_size, Gtk.IconLookupFlags.NoSvg);
		}

		static bool GetFlag(MUN.Stat st, MUN.FilePermissions perm)
		{
			return (st.st_mode & MUN.FilePermissions.S_IFMT) == perm;
		}

		Gdk.Pixbuf GetMimeIcon()
		{
			if (!IsFile && linktype != SymbolicLinkType.DanglingLink) {
				// This is not a regular file, so a related filesystem icon needs to be returned.
				if (IsDirectory) {
					if (IsUpDirectory) {
						return updir_icon;
					}
					return directory_icon;
				} else if (IsFifo) {
					return fifo_icon;
				} else if (IsSocket) {
					return socket_icon;
				} else if (IsCharacterDevice) {
					return chardev_icon;
				} else if (IsBlockDevice) {
					return blockdev_icon;
				}
			}
			
			// This is a regular file or a stalled link.
			if (!mime_to_icon_hash.ContainsKey(MimeType)) {
				// The mime icon is not cached yet, so it needs to be cached.
				Gnome.IconLookupResultFlags result_flags;
				string iconname = Gnome.Icon.Lookup(new Gnome.IconTheme(), null, "", null,
											 		new Gnome.Vfs.FileInfo(), MimeType,
											 		Gnome.IconLookupFlags.None,	out result_flags);
				Gdk.Pixbuf icon = LoadIcon(iconname);
				mime_to_icon_hash.Add(MimeType, icon);
				return icon;
			} else {
				// The mime icon is already cached.
				return mime_to_icon_hash[MimeType] as Gdk.Pixbuf;
			}
		}
	}
}
