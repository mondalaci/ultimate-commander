using System;
using System.Collections;

namespace UltimateCommander {

	public enum FileComparerType {
		DirectoriesFirst,
		Filename,
		Size,
		LastAccessTime,
		LastStatusChangeTime,
		LastWriteTime,
		OwnerUser,
		OwnerUserId,
		OwnerGroup,
		OwnerGroupId,
		LinkCount,
		Inode,
		LinkPath,
		MimeType,
		Description
	}

	public class FileComparer: IComparer
	{
		FileComparerInfo[] infos;

		public int Compare(object object1, object object2)
		{
			File file1 = (File)object1;
			File file2 = (File)object2;
			
			foreach (FileComparerInfo info in infos) {
				int value = info.Method(file1, file2);
				if (value != 0) {
					return value;
				}
			}

			return 0;
		}

		public void SetTypes(FileComparerType[] types)
		{
			ArrayList info_list = new ArrayList();

			foreach (FileComparerType type in types) {
				FileComparerInfo info = FileComparerInfo.GetInfo(type);
				info_list.Add(info);
			}

			infos = (FileComparerInfo[])info_list.ToArray(typeof(FileComparerInfo));
		}
	}
}
