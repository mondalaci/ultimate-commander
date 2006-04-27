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

        public Frame Frame {
            get { return slot.Frame; }
        }
        
		public virtual void Select()
		{
            slot.Frame.Select();
		    slot.Redraw();
		}

		[GLib.ConnectBefore]
		void OnViewButtonPressEvent(object sender, ButtonPressEventArgs args)
		{
			Select();
		}
	}
}
