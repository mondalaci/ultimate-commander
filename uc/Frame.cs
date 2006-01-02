using Gdk;
using Gtk;

namespace UltimateCommander {

	public class Frame: HBox {

		const string active_header_colorstring = "#ffffff";
		const string inactive_header_colorstring = "#000000";
		
		[Glade.Widget] Label header_label;
		[Glade.Widget] Gtk.Window frame_window;
		[Glade.Widget] EventBox eventbox;
		[Glade.Widget] EventBox eventbox2;

		bool active = false;
		string title = "";

		public Frame(): base()
		{
			PackGladeWidget();
		}

		public Frame(Widget child): base()
		{
			PackGladeWidget();
			eventbox2.Add(child);
		}

		public string Title {
			get {
				return title;
			}
			set {
				title = value;
				RefreshHeader();
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

		void PackGladeWidget()
		{
			Glade.XML glade_xml = new Glade.XML(UltimateCommander.GladeFileName, "frame_window", null);
			glade_xml.Autoconnect(this);
			frame_window.Remove(eventbox);
			PackStart(eventbox, true, true, 0);
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
			eventbox.ModifyBg(StateType.Normal, header_bgcolor);
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
