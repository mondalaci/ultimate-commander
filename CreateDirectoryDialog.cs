using System;
using Mono.Unix;
using MUN = Mono.Unix.Native;
using Gdk;
using Gtk;
using Glade;

namespace UltimateCommander {

    public class CreateDirectoryDialog: Dialog {

        [Glade.Widget] Entry entry;
        
        public CreateDirectoryDialog(): base("create_directory_dialog", "Create Directory")
        {
        }

        private void DoCreateDirectory()
        {
            string dest_filename = entry.Text;
            string current_directory = MainWindow.ActivePanel.CurrentDirectory;
            string dest_filepath = UnixPath.Combine(current_directory, dest_filename);

            if (File.IsFilePathExists(dest_filepath)) {
                InfoBar.Warning("Destination filename already exists. " +
                                "You should choose a different directory name.");
                return;
            }

            if (MUN.Syscall.mkdir(dest_filepath, MUN.FilePermissions.S_IRWXU | 
                MUN.FilePermissions.S_IRGRP | MUN.FilePermissions.S_IROTH |
                MUN.FilePermissions.S_IXGRP | MUN.FilePermissions.S_IXOTH) == 0) {
                MainWindow.ActivePanel.ChangeDirectory(current_directory);
                MainWindow.ActivePanel.SelectFileName(dest_filename);
                InfoBar.Notice("Directory successfully created.");
            } else {
                InfoBar.Error("Directory creation failed: {0}",
                    MUN.Stdlib.strerror(MUN.Stdlib.GetLastError()));
            }

            DestroyDialog();
        }

        override protected bool OnOkButtonClicked()
        {
            DoCreateDirectory();
            return true;
        }

        [GLib.ConnectBefore]
        private void OnEntryActivate(object sender, EventArgs args)
        {
            OnOkButtonClicked();
        }
        
        override protected void OnExpose()
        {
            entry.GrabFocus();
        }
    }
}
