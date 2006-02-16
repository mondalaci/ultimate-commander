using System;

namespace UltimateCommander {

	class ListingType {
		string name;
		string[] args;

		public ListingType(string name, string[] args)
		{
			this.name = name;
			this.args = args;
		}

		public string Name {
			get { return name; }
		}
	}
}