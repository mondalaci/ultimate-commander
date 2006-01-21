using System;
using Gtk;

namespace UltimateCommander {

	public class View: GladeWidget {

		protected Slot slot;

		public View(string window_name): base(window_name)
		{
			Slot = new Slot(this);
			topwidget.ButtonPressEvent += new ButtonPressEventHandler(OnViewButtonPressEvent);
		}

		public Slot Slot {
			get { return slot; }
			set { slot = value; }
		}

		protected void Select()
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
			//Select();
		}
		
	}
}