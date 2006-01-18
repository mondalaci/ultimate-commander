using System;
using System.Collections;
using Gdk;
using Gtk;

namespace UltimateCommander {

	public class Slot: GladeContainer {

		const string active_header_colorstring = "#ffffff";
		const string inactive_header_colorstring = "#000000";
		
		static ArrayList slots = new ArrayList();
		
		[Glade.Widget] Label header_label;

		bool active = false;
		string title = "";

		public Slot(): base("slot_window")
		{
			slots.Add(this);
		}

		~Slot()
		{
			slots.Remove(this);
		}

		public void SetView(View view)
		{
			SetChild(view);
			view.Slot = this;
		}

		public string Title {
			get {
				return title;
			}
			set {
				if (this != null) {
					title = value;
					RefreshHeader();
				}
			}
		}
					
		public bool Active {
			get {
				return active;
			}
			set {
				if (value == true)
					foreach (Slot slot in slots)
						slot.Active = false;
				
				active = value;
				RefreshHeader();
			}
		}

		void RefreshHeader()
		{
			string header_colorstring;
			Color header_bgcolor;

			if (active) {
				header_colorstring = active_header_colorstring;
				header_bgcolor = Widget.DefaultStyle.BaseColors[(int)StateType.Selected];
			} else {
				header_colorstring = inactive_header_colorstring;
				header_bgcolor = Widget.DefaultStyle.BaseColors[(int)StateType.Insensitive];
			}

			header_label.Markup = GetFgPangoMarkup(header_colorstring, title);
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
			Active = true;
		}
	}
}
