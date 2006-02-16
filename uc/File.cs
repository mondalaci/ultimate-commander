using System;
using System.Collections;
using System.Text;
using Mono.Unix;
using MUN = Mono.Unix.Native;
using Gnome.Vfs;
using Gdk;

namespace UltimateCommander {

	public enum SymbolicLinkType {NotLink, ValidLink, StalledLink};

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

		string fullpath;
		string filename;
		MUN.Stat stat;
		MUN.Stat lstat;
		string mimetype;
		SymbolicLinkType linktype;
		string linkpath = "";
		bool selected;
		Gdk.Pixbuf icon;
		
		public File(string fullpath_arg) {
			fullpath = fullpath_arg;
			filename = System.IO.Path.GetFileName(fullpath);
			MUN.Syscall.stat(fullpath, out stat);
			MUN.Syscall.lstat(fullpath, out lstat);
			mimetype = Mime.TypeFromName(filename);

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
					linktype = SymbolicLinkType.StalledLink;
				}
			} else {
				linktype = SymbolicLinkType.NotLink;
			}

			Selected = false;
		}

		public bool Selected {
			get { return selected; }
			set { selected = value; }
		}

		public void InvertSelection()
		{
			if (!IsUpDirectory)
				Selected = !Selected;
		}

		public string Name {
			get { return filename; }
		}

		public string FullPath {
			get { return fullpath; }
		}

		public long Size {
			get { return lstat.st_size; }
		}

		public SymbolicLinkType LinkType {
			get { return linktype; }
		}

		public string MimeType {
			get { return mimetype; }
		}
		
		public string Description {
			get { return Mime.GetDescription(MimeType); }
		}

		public Gdk.Pixbuf Icon {
			get {
				if (!IsFile) {
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
 												 		Gnome.IconLookupFlags.None,
 												 		out result_flags);
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
				return attribute_icons.GetIcon(IsExecutable, !IsWritable, !IsReadable, LinkType);
			}
		}

		public bool IsReadable {
			get { return MUN.Syscall.access(FullPath, MUN.AccessModes.R_OK) == 0; }
		}

		public bool IsWritable {
			get { return MUN.Syscall.access(FullPath, MUN.AccessModes.W_OK) == 0; }
		}

		public bool IsExecutable {
			get { return IsFile && MUN.Syscall.access(FullPath, MUN.AccessModes.X_OK) == 0; }
		}

		public bool IsFile {
			get { return (stat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFREG; }
		}

		public bool IsDirectory {
			get { return (stat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFDIR; }
		}

		public bool IsFifo {
			get { return (stat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFIFO; }
		}

		public bool IsSocket {
			get { return (stat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFSOCK; }
		}

		public bool IsCharacterDevice {
			get { return (stat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFCHR; }
		}

		public bool IsBlockDevice {
			get { return (stat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFBLK; }
		}
		
		public bool IsSymbolicLink {
			get { return (lstat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFLNK; }
		}

		public bool IsUpDirectory {
			get { return Name == ".."; }
		}

		static Gdk.Pixbuf LoadIcon(string iconname)
		{
			return Gtk.IconTheme.Default.LoadIcon(iconname, icon_size, Gtk.IconLookupFlags.NoSvg);
		}
	}
}
