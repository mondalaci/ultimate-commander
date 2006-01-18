using System;
using Gtk;

namespace UltimateCommander {

	public class View: GladeWidget {

		Slot slot;

		public View(string window_name): base(window_name)
		{
			topwidget.ButtonPressEvent += new ButtonPressEventHandler(OnViewButtonPressEvent);
		}

		public Slot Slot {
			set { slot = value; }
		}

		void Select()
		{
			slot.Active = true;
		}

		[GLib.ConnectBefore]
		void OnViewButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			Select();
		}

		[GLib.ConnectBefore]
		void OnViewClicked(object o, EventArgs args)
		{
			Select();
		}
		
	}
}