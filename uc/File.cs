using System;
using System.Collections;
using MUN = Mono.Unix.Native;
using Gnome.Vfs;
using Gdk;

namespace UltimateCommander {

	public class File {

		static Hashtable mime_to_icon_hash = new Hashtable();
		static AttributeIcons attribute_icons = new AttributeIcons();
		static Gdk.Pixbuf updir_icon = LoadIcon(Gtk.Stock.GoUp);
		static Gdk.Pixbuf directory_icon = LoadIcon("gnome-fs-directory");
		static Gdk.Pixbuf fifo_icon = LoadIcon("gnome-fs-fifo");
		static Gdk.Pixbuf socket_icon = LoadIcon("gnome-fs-socket");
		static Gdk.Pixbuf chardev_icon = LoadIcon("gnome-fs-chardev");
		static Gdk.Pixbuf blockdev_icon = LoadIcon("gnome-fs-blockdev");

		string full_path;
		string filename;
		MUN.Stat stat;
		MUN.Stat lstat;
		bool selected;
		string mimetype;
		Gdk.Pixbuf icon;
		
		public File(string full_path_arg) {
			full_path = full_path_arg;
			filename = System.IO.Path.GetFileName(full_path);
			MUN.Syscall.stat(full_path, out stat);
			MUN.Syscall.lstat(full_path, out lstat);
			mimetype = Mime.TypeFromName(filename);
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

		public string FileName {
			get { return filename; }
		}

		public string FullPath {
			get { return full_path; }
		}

		public long Size {
			get { return lstat.st_size; }
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
					} else if (IsCharDevice) {
						return chardev_icon;
					} else /* IsBlockDevice */ {
						return blockdev_icon;
					}
				} else if (!mime_to_icon_hash.ContainsKey(MimeType)) {
					// This is a regular file.  The mime icon is not cached yet, so it needs to be cached.
					Gnome.IconLookupResultFlags result_flags;
					string iconname = Gnome.Icon.Lookup(new Gnome.IconTheme(), null, "", null,
 												 		new Gnome.Vfs.FileInfo(), MimeType,
 												 		Gnome.IconLookupFlags.None,
 												 		out result_flags);
					Gdk.Pixbuf icon = LoadIcon(iconname);
					mime_to_icon_hash.Add(MimeType, icon);
					return icon;
				} else {
					// This is a regular file and its mime icon is already cached.
					return mime_to_icon_hash[MimeType] as Gdk.Pixbuf;
				}
			}
		}
		
		public Gdk.Pixbuf AttributeIcon {
			get {
				return attribute_icons.GetIcon(IsExecutable, IsSymLink, !IsWritable, !IsReadable);
			}
		}

		public bool IsFile {
			get { return (lstat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFREG; }
		}

		public bool IsDirectory {
			get { return (stat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFDIR; }
		}

		public bool IsFifo {
			get { return (lstat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFIFO; }
		}

		public bool IsSocket {
			get { return (lstat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFSOCK; }
		}

		public bool IsCharDevice {
			get { return (lstat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFCHR; }
		}

		public bool IsBlockDevice {
			get { return (lstat.st_mode & MUN.FilePermissions.S_IFMT) == MUN.FilePermissions.S_IFBLK; }
		}

		public bool IsExecutable {
			get { return IsFile && MUN.Syscall.access(FullPath, MUN.AccessModes.X_OK) == 0; }
		}

		public bool IsSymLink {
			get { return (lstat.st_mode & MUN.FilePermissions.S_IFLNK) == MUN.FilePermissions.S_IFLNK; }
		}

		public bool IsWritable {
			get { return MUN.Syscall.access(FullPath, MUN.AccessModes.W_OK) == 0; }
		}

		public bool IsReadable {
			get { return MUN.Syscall.access(FullPath, MUN.AccessModes.R_OK) == 0; }
		}

		public bool IsUpDirectory {
			get { return FileName == ".."; }
		}

		static Gdk.Pixbuf LoadIcon(string iconname)
		{
			return Gtk.IconTheme.Default.LoadIcon(iconname, 16, Gtk.IconLookupFlags.NoSvg);
		}
	}
}
