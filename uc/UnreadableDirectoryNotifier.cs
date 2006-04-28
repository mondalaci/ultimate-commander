using System;
using Gtk;

namespace UltimateCommander {

    public class UnreadableDirectoryNotifier: Notifier {

        public UnreadableDirectoryNotifier(): base()
        {
            SetIcon("gtk-dialog-warning");

            AppendText("Unreadable directory detected\n", bold_tag);
            AppendText("You are not permitted to list the contents of this directory, " +
                "since it is only searchable, not readable.  You can enter to subdirectories " +
                "using the ");
            AppendText("cd", bold_tag);
            AppendText(" command");
        }
    }
}
