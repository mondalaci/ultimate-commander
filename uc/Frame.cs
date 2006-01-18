using System;
using Gtk;

namespace UltimateCommander {

	class Frame: Notebook {

		Frame(): base()
		{
			this.SwitchPage += new SwitchPageHandler(OnSwitchPage);
		}

		public void AppendView(Widget widget)
		{
			AppendPage(widget, null);
		}

		[GLib.ConnectBefore]
		void OnSwitchPage(object o, SwitchPageArgs args)
		{
			Console.WriteLine("OnSwitchPage");
		}
	}
}