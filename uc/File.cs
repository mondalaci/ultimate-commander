using System;
using System.IO;
using Mono.Unix.Native;

namespace UltimateCommander {

	public class File {

		string full_path;
		string filename;
		public Stat stat;
		bool selected;
		
		public File(string full_path) {
			//full_path = System.IO.Path.GetFullPath(path);
			this.full_path = full_path;
			filename = System.IO.Path.GetFileName(full_path);
			Mono.Unix.Native.Syscall.lstat(full_path, out stat);
			Selected = false;
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

		public bool IsFile {
			get {return (int)(stat.st_mode & Mono.Unix.Native.FilePermissions.S_IFREG) != 0; }
		}

		public bool IsDirectory {
			get { return (int)(stat.st_mode & Mono.Unix.Native.FilePermissions.S_IFDIR) != 0; }
		}

		public bool IsUpDirectory {
			get { return FileName == ".."; }
		}

		public bool Selected {
			get { return selected; }
			set { selected = value; }
		}
	}
}
