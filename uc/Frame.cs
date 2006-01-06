using System;
using Gdk;
using Gtk;

namespace UltimateCommander {

	public class Frame: GladeContainer {

		const string active_header_colorstring = "#ffffff";
		const string inactive_header_colorstring = "#000000";
		
		[Glade.Widget] Label header_label;

		bool active = false;
		string title = "";

		public Frame(): base("frame_window")
		{
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
				active = value;
				RefreshHeader();
			}
		}

		void RefreshHeader()
		{
			Console.WriteLine("refreshheader");

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
	}
}
