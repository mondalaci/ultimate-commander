using System;

namespace UltimateCommander {

    public delegate int FileComparerMethod(File file1, File file2);

    public class FileComparerInfo: PanelConfiguratorInfo {

        FileComparerType type;
        FileComparerMethod method;
        string name;

        FileComparerInfo(FileComparerType type_arg, FileComparerMethod method_arg, string name_arg)
        {
            type = type_arg;
            method = method_arg;
            name = name_arg;
        }

        public FileComparerType Type {
            get { return type; }
        }

        public FileComparerMethod Method {
            get { return method; }
        }

        public string Name {
            get { return name; }
        }

        public static FileComparerInfo GetInfo(FileComparerType comparertype)
        {
            foreach (FileComparerInfo comparerinfo in AllInfos) {
                if (comparertype == comparerinfo.Type) {
                    return comparerinfo;
                }
            }
            return null;
        }

        public static FileComparerInfo[] AllInfos {
            get { return all_infos; }
        }

        // The FileComparerInfo instances

        static FileComparerInfo[] all_infos = {
            new FileComparerInfo(FileComparerType.DirectoriesFirst,
                DirectoriesFirstComparerMethod, "Directories First"),
            new FileComparerInfo(FileComparerType.Filename,
                FilenameComparerMethod, "Filename"),
            new FileComparerInfo(FileComparerType.Size,
                SizeComparerMethod, "Size"),
            new FileComparerInfo(FileComparerType.LastAccessTime,
                LastAccessTimeComparerMethod, "Last Access Time"),
            new FileComparerInfo(FileComparerType.LastStatusChangeTime,
                LastStatusChangeTimeComparerMethod, "Last Status Change Time"),
            new FileComparerInfo(FileComparerType.LastWriteTime,
                LastWriteTimeComparerMethod, "Last Write Time"),
            new FileComparerInfo(FileComparerType.OwnerUser,
                OwnerUserComparerMethod, "Owner Name"),
            new FileComparerInfo(FileComparerType.OwnerUserId,
                OwnerUserIdComparerMethod, "Owner ID"),
            new FileComparerInfo(FileComparerType.OwnerGroup,
                OwnerGroupComparerMethod, "Group Name"),
            new FileComparerInfo(FileComparerType.OwnerGroupId,
                OwnerGroupIdComparerMethod, "Group ID"),
            new FileComparerInfo(FileComparerType.LinkCount,
                LinkCountComparerMethod, "Link Count"),
            new FileComparerInfo(FileComparerType.Inode,
                InodeComparerMethod, "Inode"),
            new FileComparerInfo(FileComparerType.LinkPath,
                LinkPathComparerMethod, "Link Path"),
            new FileComparerInfo(FileComparerType.MimeType,
                MimeTypeComparerMethod, "Mime Type"),
            new FileComparerInfo(FileComparerType.Description,
                DescriptionComparerMethod, "Description")
        };

        // The FileComparerMethod delegates

        static int DirectoriesFirstComparerMethod(File file1, File file2)
        {
            if (file1.IsDirectory && !file2.IsDirectory) {
                return -1;
            } else if (!file1.IsDirectory && file2.IsDirectory) {
                return 1;
            }
            return 0;			
        }

        static int FilenameComparerMethod(File file1, File file2)
        {
            return file1.NameString.CompareTo(file2.NameString);
        }

        static int SizeComparerMethod(File file1, File file2)
        {
            return file1.Size < file2.Size ? -1 : 1;
        }

        static int LastAccessTimeComparerMethod(File file1, File file2)
        {
            return file1.LastAccessTime < file2.LastAccessTime ? -1 : 1;
        }

        static int LastStatusChangeTimeComparerMethod(File file1, File file2)
        {
            return file1.LastStatusChangeTime < file2.LastStatusChangeTime ? -1 : 1;
        }

        static int LastWriteTimeComparerMethod(File file1, File file2)
        {
            return file1.LastWriteTime < file2.LastWriteTime ? -1 : 1;
        }

        static int OwnerUserComparerMethod(File file1, File file2)
        {
            return file1.OwnerUser.CompareTo(file2.OwnerUser);
        }

        static int OwnerUserIdComparerMethod(File file1, File file2)
        {
            return file1.OwnerUserId < file2.OwnerUserId ? -1 : 1;
        }

        static int OwnerGroupComparerMethod(File file1, File file2)
        {
            return file1.OwnerGroup.CompareTo(file2.OwnerGroup);
        }

        static int OwnerGroupIdComparerMethod(File file1, File file2)
        {
            return file1.OwnerGroupId < file2.OwnerGroupId ? -1 : 1;
        }

        static int LinkCountComparerMethod(File file1, File file2)
        {
            return file1.LinkCount < file2.LinkCount ? -1 : 1;
        }

        static int InodeComparerMethod(File file1, File file2)
        {
            return file1.Inode < file2.Inode ? -1 : 1;
        }

        static int LinkPathComparerMethod(File file1, File file2)
        {
            if (file1.LinkPath == "" && file2.LinkPath == "") {
                return 0;
            } else if (file1.LinkPath == "" && file2.LinkPath != "") {
                return 1;
            } else if (file1.LinkPath != "" && file2.LinkPath == "") {
                return -1;
            } else {
                return file1.LinkPath.CompareTo(file2.LinkPath);
            }
        }

        static int MimeTypeComparerMethod(File file1, File file2)
        {
            return file1.MimeType.CompareTo(file2.MimeType);
        }

        static int DescriptionComparerMethod(File file1, File file2)
        {
            return file1.Description.CompareTo(file2.Description);
        }
    }
}
