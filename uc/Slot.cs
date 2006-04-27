using System;
using System.Collections;
using Gdk;
using Gtk;

namespace UltimateCommander {

	public class Slot: GladeWidget {

		[Glade.Widget] Label header;
		[Glade.Widget] EventBox view_slot;

		Frame frame;
        View view;
        
		public Slot(View view_arg): base("slot_widget")
		{
            view = view_arg;
			view_slot.Add(view);
			view.Slot = this;
		}

		public void Select()
		{
            view.Select();
		}

		public string Title {
			get { return header.Text; }
			set {
				if (this != null) {
					header.Text = value;
					Redraw();
				}
			}
		}

		public Frame Frame {
			get { return frame; }
			set { frame = value; }
		}

		public View View {
			get { return view; }
		}

		public void Redraw()
		{
			string header_colorstring;
			Color header_bgcolor;

			if (frame != null && frame.Selected) {
				header_colorstring = Config.ActiveSlotHeaderColorString;
				header_bgcolor = Widget.DefaultStyle.BaseColors[(int)StateType.Selected];
			} else {
				header_colorstring = Config.InactiveSlotHeaderColorString;
				header_bgcolor = Widget.DefaultStyle.BaseColors[(int)StateType.Insensitive];
			}

			header.Markup = GetFgPangoMarkup(header_colorstring, header.Text);
			topwidget.ModifyBg(StateType.Normal, header_bgcolor);
		}

		string GetFgPangoMarkup(string color, string unescaped_text)
		{
			string escaped_text = EscapeText(unescaped_text, "<>");
			return "<span foreground=\"" + color + "\">" + escaped_text + "</span>";
		}

		string EscapeText(string unescaped_text, string chars_to_escape)
		{
			string escaped_text = "";

			foreach (char c in unescaped_text)
				if (chars_to_escape.IndexOf(c) != -1)
					escaped_text += "\\" + c;
				else
					escaped_text += c;

			return escaped_text;
		}

		void OnButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			Select();
		}
	}
}
