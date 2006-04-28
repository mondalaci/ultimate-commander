using System;
using Gtk;

namespace UltimateCommander {

    public class InvalidEncodingNotifier: Notifier {

        public InvalidEncodingNotifier(): base()
        {
            SetIcon("gtk-dialog-error");
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
    }
}
