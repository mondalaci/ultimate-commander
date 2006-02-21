using System;
using System.Collections;
using System.Text;
using Mono.Unix;
using MUN = Mono.Unix.Native;
using Gdk;
using Gnome.Vfs;

namespace UltimateCommander {

	public enum SymbolicLinkType {
		NotLink,
		ValidLink,
		DanglingLink
	};

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
		MUN.Stat stat;
		MUN.Stat lstat;
		SymbolicLinkType linktype;
		string linkpath;
		
		public File(string fullpath_arg) {
			Selected = false;
			fullpath = fullpath_arg;
			MUN.Syscall.lstat(fullpath, out lstat);

			// Set linktype and linkpath.
			if (IsSymbolicLink) {
				MUN.Syscall.stat(fullpath, out stat);

				StringBuilder dest_strbuilder = new StringBuilder(max_path_length);
				MUN.Syscall.readlink(fullpath, dest_strbuilder);
				string dest = dest_strbuilder.ToString();

				if (!System.IO.Path.IsPathRooted(dest)) {
					string directoryname = System.IO.Path.GetDirectoryName(fullpath);
					dest = System.IO.Path.Combine(directoryname, dest);
				}

				linkpath = dest;

				if (MUN.Syscall.access(dest, MUN.AccessModes.F_OK) == 0) {
					linktype = SymbolicLinkType.ValidLink;
				} else {
					linktype = SymbolicLinkType.DanglingLink;
				}
			} else {
				stat = lstat;
				linktype = SymbolicLinkType.NotLink;
			}
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

		// Icon properties

		public Gdk.Pixbuf MimeIcon {
			get {
				if (!IsFile && !IsDanglingLink) {
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

		public Gdk.Pixbuf AttributeIcon {
			get {
				 return attribute_icons.GetIcon(IsExecutable, !IsWritable,
												!IsReadable, LinkType);
			}
		}

		// Often used VFS properties

		public string FullPath {
			get { return fullpath; }
		}

		public string Name {
			get { return System.IO.Path.GetFileName(fullpath); }
		}

		public long Size {
			get {
				if (IsDanglingLink) {
					return 0;
				} else {
					return stat.st_size;
				}
			}
		}

		public string SizeString {
			get { return CheckDanglingLink(stat.st_size); }
		}

		// Protection properties

		public MUN.FilePermissions Permissions {
			get { return stat.st_mode; }
		}

		public string SymbolicPermissions {
			get {
				if (IsDanglingLink) {
					return "l---------";
				}

				string perm = "";

				if (IsDirectory) {
					perm += "d";
				} else if (IsSymbolicLink) {
					perm += "l";
				} else if (IsFifo) {
					perm += "p";
				} else if (IsSocket) {
					perm += "s";
				} else if (IsCharacterDevice) {
					perm += "c";
				} else if (IsBlockDevice) {
					perm += "b";
				} else /* if IsFile */ {
					perm += "-";
				}

				perm += IsOwnerReadable ? "r" : "-";
				perm += IsOwnerWritable ? "w" : "-";
				perm += IsOwnerExecutable ? (IsSetGid ? "s" : "x") : (IsSetGid ? "S" : "-");
				perm += IsGroupReadable ? "r" : "-";
				perm += IsGroupWritable ? "w" : "-";
				perm += IsGroupExecutable ? (IsSetUid ? "s" : "x") : (IsSetUid ? "S" : "-");
				perm += IsOthersReadable ? "r" : "-";
				perm += IsOthersWritable ? "w" : "-";
				perm += IsOthersExecutable ? (IsSticky ? "t" : "x") : (IsSticky ? "T" : "-");

				return perm;
			}
		}

		// Owner / Group properties

		public uint OwnerUserId {
			get { return stat.st_uid; }
		}

		public string OwnerUser {
			get {
				if (linktype == SymbolicLinkType.DanglingLink) {
					return "N/A";
				}
	 			try {
					return new Mono.Unix.UnixUserInfo(OwnerUserId).UserName;
				} catch (ArgumentException e) {
					return "N/A (UID: " + OwnerUserId.ToString() + ")";
				}
			}
		}

		public string OwnerUserIdString {
			get { return CheckDanglingLink(stat.st_uid); }
		}

		public uint OwnerGroupId {
			get { return stat.st_gid; }
		}

		public string OwnerGroup {
			get {
				if (linktype == SymbolicLinkType.DanglingLink) {
					return "N/A";
				}
				try {
					return new Mono.Unix.UnixGroupInfo(OwnerGroupId).GroupName;
				} catch (ArgumentException e) {
					return "N/A (GID:" + OwnerGroupId.ToString() + ")";
				}
			}
		}

		public string OwnerGroupIdString {
			get { return CheckDanglingLink(stat.st_gid); }
		}

		// Date properties

		public long LastAccessTime {
			get { return stat.st_atime; }
		}

		public string LastAccessTimeString {
			get { return GetDateTimeString(LastAccessTime); }
		}

		public long LastStatusChangeTime {
			get { return stat.st_mtime; }
		}

		public string LastStatusChangeTimeString {
			get { return GetDateTimeString(LastStatusChangeTime); }
		}

		public long LastWriteTime {
			get { return stat.st_ctime; }
		}

		public string LastWriteTimeString {
			get { return GetDateTimeString(LastWriteTime); }
		}

		// Rarely used VFS properties

		public ulong LinkCount {
			get { return stat.st_nlink; }
		}

		public ulong Inode {
			get { return stat.st_ino; }
		}
		
		public string InodeString {
			get { return CheckDanglingLink(stat.st_ino); }
		}
		
		// Symbolic link related properties

		public SymbolicLinkType LinkType {
			get { return linktype; }
		}

		public bool IsDanglingLink {
			get { return linktype == SymbolicLinkType.DanglingLink; }
		}

		public string LinkPath {
			get {
				if (linkpath == null) {
					return "";
				} else {
					return linkpath;
				}
			}
		}

		// Mime properties

		public string MimeType {
			get { return Mime.TypeFromName(Name); }
		}
		
		public string Description {
			get { return Mime.GetDescription(MimeType); }
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

		// Unix properties
		
		public bool IsSetUid {
			get { return (stat.st_mode & MUN.FilePermissions.S_ISUID) != 0; }
		}
		
		public bool IsSetGid {
			get { return (stat.st_mode & MUN.FilePermissions.S_ISGID) != 0; }
		}
		
		public bool IsSticky {
			get { return (stat.st_mode & MUN.FilePermissions.S_ISVTX) != 0; }
		}

		public bool IsOwnerReadable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IRUSR) != 0; }
		}		

		public bool IsOwnerWritable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IWUSR) != 0; }
		}		

		public bool IsOwnerExecutable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IXUSR) != 0; }
		}		

		public bool IsGroupReadable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IRGRP) != 0; }
		}		

		public bool IsGroupWritable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IWGRP) != 0; }
		}		

		public bool IsGroupExecutable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IXGRP) != 0; }
		}		

		public bool IsOthersReadable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IROTH) != 0; }
		}		

		public bool IsOthersWritable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IWOTH) != 0; }
		}		

		public bool IsOthersExecutable {
			get { return (stat.st_mode & MUN.FilePermissions.S_IXOTH) != 0; }
		}		

		// Private methods

		string CheckDanglingLink(object o)
		{
			if (IsDanglingLink) {
				return "N/A";
			} else {
				return o.ToString();
			}
		}

		string GetDateTimeString(long time)
		{
			if (IsDanglingLink) {
				return "N/A";
			} else {
				return MUN.NativeConvert.FromTimeT(time).ToString("yyyy-MM-dd HH:mm:ss");
			}
		}

		static Gdk.Pixbuf LoadIcon(string iconname)
		{
			return Gtk.IconTheme.Default.LoadIcon(iconname, icon_size, Gtk.IconLookupFlags.NoSvg);
		}

		static bool GetFlag(MUN.Stat st, MUN.FilePermissions perm)
		{
			return (st.st_mode & MUN.FilePermissions.S_IFMT) == perm;
		}
	}
}
