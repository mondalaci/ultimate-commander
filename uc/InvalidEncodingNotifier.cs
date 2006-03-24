using System;
using Gtk;
using Glade;

namespace UltimateCommander {

	public class InvalidEncodingNotifier: GladeWidget {
		
		[Glade.Widget] TextView textview;
		TextTag bold_tag;
		TextTag red_tag;
		
		public InvalidEncodingNotifier(): base("invalid_encoding_notifier_window")
		{
			textview.ModifyBase(StateType.Normal,
				Widget.DefaultStyle.BaseColors[(int)StateType.Insensitive]);
			red_tag = new TextTag("red");
			red_tag.ForegroundGdk = new Gdk.Color(255, 0, 0);
			textview.Buffer.TagTable.Add(red_tag);
			bold_tag = new TextTag("bold");
			bold_tag.Weight = Pango.Weight.Bold;
			textview.Buffer.TagTable.Add(bold_tag);
		}

		public void SetText(int count)
		{
			bool plural = count > 1 ? true : false;
			string s = plural ? "s" : "";
			string are = plural ? "are" : "is";
			string these = plural ? "these" : "this";
			textview.Buffer.Clear();

			AppendText("Invalid filename encoding" + s + " detected\n", bold_tag);
			AppendText(count.ToString() + " file" + s + " in this directory " +	are  +
				" encoded with invalid character encoding" + s + ". Some applications are " +
				"having problems handling " + these + " file" + s + " properly. The related " +
				"filename" + s + " " + are + " displayed with ");
			AppendText("red color", red_tag);
			AppendText(". You should correct this problem by renaming " + these + " file" + s +
				" or setting the encoding of your file system properly.");
		}

		void AppendText(string text, params TextTag[] tags)
		{
			TextIter iter = textview.Buffer.EndIter;
			textview.Buffer.InsertWithTags(ref iter, text, tags);
		}
	}
}