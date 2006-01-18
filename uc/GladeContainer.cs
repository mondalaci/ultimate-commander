using System;
using Gtk;

namespace UltimateCommander {

	public class GladeContainer: GladeWidget {

		[Glade.Widget] protected EventBox containerwidget;
		Widget child;
		
		public GladeContainer(string window_name): base(window_name)
		{
		}

		public GladeContainer(string window_name, Widget child): base(window_name)
		{
			containerwidget.Add(child);
		}

		public void SetChild(Widget child)
		{
			this.child = child;
			containerwidget.Add(child);
		}
	}
}
