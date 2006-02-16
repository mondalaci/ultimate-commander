using System;
using Mono.Posix;

namespace UltimateCommander {

	class ColumnType {

		public enum Type {
			Icon,
			Name,
			Size,
			CTime,
			MTime,
			ATime,
			Permission,
			Uid,
			Gid,
			Description,
			MimeType,
			FileType,
			INode,
			ExtendedAttribute,
			MetaTag
		}

		public enum FileTypeArg {
			LetterType,
			SymbolicType
		}

		public enum NameArg {
			ShortName,
			FullPath
		}

		public enum SizeArg {
			Byte,
			Kib,
			Kb,
			Mib,
			Mb
		}

		// for both UIDs and GIDs
		public enum IdArg {
			Name,
			Num
		}

		Type type;
		int arg;

		public ColumnType(Type t, int a) {
			type = t;
			arg = a;
		}

		public string GetContent(File file) {
/*			switch (type) {
			case Type.FileType:
				return "x";
			case Type.Name:
				if (arg == (int)NameArg.ShortName)
					return file.Name;
				else if (arg == (int)NameArg.FullPath)
					return file.FullName;
				else {}
				break;
			}*/
			return "unknown";
		}
	}
}
