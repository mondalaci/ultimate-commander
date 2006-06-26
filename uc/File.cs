using System;
using System.Collections;
using System.Text;
using Mono.Unix;
using MUN = Mono.Unix.Native;
using Gnome.Vfs;

namespace UltimateCommander {

    public enum SymbolicLinkType {
        NotLink,
        ValidLink,
        DanglingLink
    };

    public enum FileReadability {
        Readable,
        OnlySearchableDirectory,
        NotReadable
    };

    public class File {

        public static File[] ListDirectory(string path)
        {
            ArrayList files = new ArrayList();

            // FIXME: When $MONO_EXTERNAL_ENCODINGS is not or inappropriately
            //        set, GetFileSystemEntries() skips accentuated filenames.
            IntPtr dir = MUN.Syscall.opendir(path);
            MUN.Dirent nextentry;
            
            while ((nextentry = MUN.Syscall.readdir(dir)) != null) {
                string filename = nextentry.d_name;
                
                if (filename == ".") {
                    continue;
                }

                string filepath = UnixPath.Combine(path, filename);
                files.Add(new File(filepath));
            }
            
            return (File[])(files.ToArray(typeof(File)));
        }

        public static bool IsFileNameEncodingValid(string filename)
        {
            foreach (char c in filename) {
                if (c == UnixEncoding.EscapeByte) {
                    return false;
                }
            }
            return true;
        }

        public static string StringifyInvalidFileNameEncoding(string filename)
        {
            StringBuilder stringbuilder = new StringBuilder();
            foreach (char c in filename) {
                if (c != UnixEncoding.EscapeByte) {
                    stringbuilder.Append(c);
                }
            }
            return stringbuilder.ToString();
        }

        public static bool IsFilePathExists(string path)
        {
            if (MUN.Syscall.access(path, MUN.AccessModes.F_OK) == 0) {
                return true;
            }
            
            MUN.Stat stat;
            return MUN.Syscall.lstat(path, out stat) == 0;
        }

        // If the actual maximum path length is greater
        // than this value, we're in some serious shit.
        static int max_path_length = 512;  

        static string not_available = "N/A";
        static string datetime_format = "yy-MM-dd HH:mm:ss";

        static Hashtable mime_to_icon_hash = new Hashtable();
        static AttributeIcons attribute_icons = new AttributeIcons();

        static Gdk.Pixbuf updir_icon = Util.LoadIcon("panel-navigate-up-one-directory.png");
        static Gdk.Pixbuf directory_icon = Util.LoadIcon("folder.png");
        static Gdk.Pixbuf fifo_icon = Util.LoadGtkIcon("gnome-fs-fifo");
        static Gdk.Pixbuf socket_icon = Util.LoadGtkIcon("gnome-fs-socket");
        static Gdk.Pixbuf chardev_icon = Util.LoadGtkIcon("gnome-fs-chardev");
        static Gdk.Pixbuf blockdev_icon = Util.LoadGtkIcon("gnome-fs-blockdev");

        bool selected;
        string fullpath;
        MUN.Stat stat;
        MUN.Stat lstat;
        SymbolicLinkType linktype;
        string linkpath = null;
        
        public File(string fullpath_arg)
        {
            Selected = false;
            fullpath = fullpath_arg;
            MUN.Syscall.lstat(fullpath, out lstat);

            // Set linktype and linkpath.
            if (IsSymbolicLink) {
                MUN.Syscall.stat(fullpath, out stat);

                StringBuilder dest_strbuilder = new StringBuilder(max_path_length);
                MUN.Syscall.readlink(fullpath, dest_strbuilder);
                linkpath = dest_strbuilder.ToString();

                if (!UnixPath.IsPathRooted(linkpath)) {
                    string directoryname = UnixPath.GetDirectoryName(fullpath);
                    // EXTERNALBUG
                    if (directoryname == "") {
                        directoryname = "/";
                    }
                    linkpath = UnixPath.Combine(directoryname, linkpath);
                }

                if (MUN.Syscall.access(linkpath, MUN.AccessModes.F_OK) == 0) {
                    linktype = SymbolicLinkType.ValidLink;
                } else {
                    linktype = SymbolicLinkType.DanglingLink;
                    stat.st_size = 0;
                    stat.st_uid = 0;
                    stat.st_gid = 0;
                    stat.st_nlink = 0;
                    stat.st_ino = 0;
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
                    // This is not a regular file, so a related
                    // filesystem icon needs to be returned.
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
                    Gdk.Pixbuf icon = Util.LoadGtkIcon(iconname);
                    mime_to_icon_hash.Add(MimeType, icon);
                    return icon;
                } else {
                    // The mime icon is already cached.
                    return (Gdk.Pixbuf)mime_to_icon_hash[MimeType];
                }
            }
        }

        public Gdk.Pixbuf AttributeIcon {
            get {
                FileAttribute attr = 0;
                 
                if (IsExecutable) {
                    if (IsSetUid && IsSetGid) {
                        attr |= FileAttribute.SetUidExecutable | FileAttribute.SetGidExecutable;
                    } else if (IsSetUid) {
                        attr |= FileAttribute.SetUidExecutable;
                    } else if (IsSetGid) {
                        attr |= FileAttribute.SetGidExecutable;
                    } else {
                        attr |= FileAttribute.NormalExecutable;
                    }
                }

                if (IsValidLink) {
                    attr |= FileAttribute.ValidSymlink;
                } else if (IsDanglingLink) {
                    attr |= FileAttribute.DanglingSymlink;
                }

                FileReadability readability = Readability;
                if (readability == FileReadability.OnlySearchableDirectory) {
                    attr |= FileAttribute.OnlySearchableDirectory;
                } else if (readability == FileReadability.Readable) {
                    attr |= FileAttribute.Readable;
                }
                 
                attr |= IsWritable ? FileAttribute.Writable : 0;

                return attribute_icons.GetIcon(attr);
            }
        }

        // Often used properties

        public string FullPath {
            get { return fullpath; }
        }

        public string DirectoryName {
            get { return UnixPath.GetDirectoryName(fullpath); }
        }

        public string Name {
            get { return UnixPath.GetFileName(FullPath); }
        }

        public string NameString {
            get { return StringifyInvalidFileNameEncoding(Name); }
        }

        public bool HasValidEncoding {
            get { return IsFileNameEncodingValid(Name); }
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
            get {
                long size = stat.st_size;
                string postfix = "";

                if (size > 999999999) {
                    postfix = "M";
                    size /= 1024 * 1024;
                } else if (size > 9999999) {
                    postfix = "K";
                    size /= 1024;
                }
                
                string size_str = size.ToString();
                string triplet_str = "";
                int length = size_str.Length;
                for (int i=length; i>0; i-=3) {
                    triplet_str = size_str.Substring(i-3 < 0 ? 0 : i-3, i>3 ? 3 : i) +
                        (i == length ? "" : " ") + triplet_str;
                }

                return CheckDanglingLink(triplet_str + postfix);
            }
        }

        // Permission properties

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

        // Owner and Group properties

        public uint OwnerUserId {
            get { return stat.st_uid; }
        }

        public string OwnerUser {
            get {
                if (IsDanglingLink) {
                    return not_available;
                }
                try {
                    return new Mono.Unix.UnixUserInfo(OwnerUserId).UserName;
                } catch (ArgumentException e) {
                    return not_available + " (UID: " + OwnerUserId.ToString() + ")";
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
                if (IsDanglingLink) {
                    return not_available;
                }
                try {
                    return new Mono.Unix.UnixGroupInfo(OwnerGroupId).GroupName;
                } catch (ArgumentException e) {
                    return not_available + " (GID:" + OwnerGroupId.ToString() + ")";
                }
            }
        }

        public string OwnerGroupIdString {
            get { return CheckDanglingLink(stat.st_gid); }
        }

        // Time properties

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

        // Rarely used properties

        public ulong LinkCount {
            get { return stat.st_nlink; }
        }

        public string LinkCountString {
            get { return CheckDanglingLink(stat.st_nlink); }
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

        public bool IsValidLink {
            get { return linktype == SymbolicLinkType.ValidLink; }
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

        public string TypeName {
            get {
                if (IsFile) {
                    return "regular file";
                } else if (IsDirectory) {
                    return "directory";
                } else if (IsFifo) {
                    return "fifo";
                } else if (IsSocket) {
                    return "socket";
                } else if (IsCharacterDevice) {
                    return "character device";
                } else if (IsBlockDevice) {
                    return "block device";
                } else if (IsDanglingLink) {
                    return "dangling link";
                } else {
                    return "unknown type";
                }
            }
        }

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

        public bool IsSearchable {
            get { return IsDirectory && MUN.Syscall.access(FullPath, MUN.AccessModes.X_OK) == 0; }
        }

        public bool IsDirectoryReadable {
            get { return MUN.Syscall.access(DirectoryName, MUN.AccessModes.R_OK) == 0; }
        }

        public bool IsDirectoryWritable {
            get { return MUN.Syscall.access(DirectoryName, MUN.AccessModes.W_OK) == 0; }
        }

        public bool IsDirectorySearchable {
            get { return MUN.Syscall.access(DirectoryName, MUN.AccessModes.X_OK) == 0; }
        }

        public FileReadability Readability {
            get {
                if (IsDirectory) {
                    if (IsSearchable) {
                        return IsReadable ? FileReadability.Readable :
                            FileReadability.OnlySearchableDirectory;
                    } else {
                        return FileReadability.NotReadable;
                    }
                } else {
                    return IsReadable ? FileReadability.Readable : FileReadability.NotReadable;
                }
            }
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
                return not_available;
            } else {
                return o.ToString();
            }
        }

        string GetDateTimeString(long time)
        {
            if (IsDanglingLink) {
                return not_available;
            } else {
                return MUN.NativeConvert.FromTimeT(time).ToString(datetime_format);
            }
        }

        static bool GetFlag(MUN.Stat st, MUN.FilePermissions perm)
        {
            return (st.st_mode & MUN.FilePermissions.S_IFMT) == perm;
        }
    }
}
