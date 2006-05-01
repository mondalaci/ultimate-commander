using System;
using Mono.Unix;
using MUN = Mono.Unix.Native;
using Gdk;
using Gtk;

namespace UltimateCommander {

    public enum WhichPanel {
        LeftPanel,
        RightPanel
    }

    public class Panel: View {
                
        [Glade.Widget] ToggleToolButton listing_toggletoolbutton;
        [Glade.Widget] ToggleToolButton sorting_toggletoolbutton;
        [Glade.Widget] ToolButton up_directory_button;
        [Glade.Widget] ToolButton home_directory_button;
        [Glade.Widget] EventBox view_slot;
        [Glade.Widget] EventBox unreadable_directory_notifier_slot;
        [Glade.Widget] EventBox invalid_encoding_notifier_slot;
        [Glade.Widget] TextView statusbar;
        
        string current_directory = null;

        PanelView view;
        FileComparer comparer = new FileComparer();
        PanelListingConfigurator listing_configurator;
        PanelSortingConfigurator sorting_configurator;
        InvalidEncodingNotifier invalid_encoding_notifier;
        UnreadableDirectoryNotifier unreadable_directory_notifier;

        public Panel(string path): base("panel_widget")
        {			
            view = new PanelView(this);
            ScrolledWindow sw = new ScrolledWindow();
            sw.VScrollbar.ButtonPressEvent += new ButtonPressEventHandler(OnButtonPressEvent);
            sw.HScrollbar.ButtonPressEvent += new ButtonPressEventHandler(OnButtonPressEvent);
            sw.Add(view);
            view_slot.Add(sw);

            listing_configurator = new PanelListingConfigurator(this);
            sorting_configurator = new PanelSortingConfigurator(this);
            invalid_encoding_notifier = new InvalidEncodingNotifier();
            unreadable_directory_notifier = new UnreadableDirectoryNotifier();

            view.KeyPressEvent += new KeyPressEventHandler(OnPanelViewKeyPressEvent);
            view.RowActivated += new RowActivatedHandler(OnRowActivated);
            view.CursorChanged += new EventHandler(OnCursorChanged);
            view.MapEvent += new MapEventHandler(OnViewMapEvent);

            ChangeDirectory(path);
            RefreshButtonStates();
            RenameActive = false;
        }

        public string CurrentDirectory {
            get { return current_directory; }
        }

        public void ChangeDirectory(string path)
        {
            path = GetTopLevelAccessiblePath(path);
            File pathdir = new File(path);

            if (!pathdir.IsSearchable) {
                InfoBar.Error("You are not permitted to enter to this directory. " +
                              "It is not searchable.");
                return;
            }

            Store.Clear();
            string prev_dir = CurrentDirectory;
            current_directory = path;
            Title = File.StringifyInvalidFileNameEncoding(CurrentDirectory);
            bool readable = pathdir.IsReadable;
            File[] files;

            if (!readable) {
                File updir = new File(UnixPath.Combine(CurrentDirectory, ".."));
                files = new File[]{updir};
            } else {
                files = File.ListDirectory(CurrentDirectory);
            }
            
            Array.Sort(files, 1, files.Length-1, comparer);

            int invalid_encodings_counter = 0;
            foreach (File file in files) {
                if (!file.HasValidEncoding) {
                    invalid_encodings_counter += 1;
                }
                Store.AppendValues(file);
            }
            RefreshInvalidEncodingNotifier(invalid_encodings_counter);

            view.ColumnsAutosize();
            SetCursor(prev_dir);
            RefreshButtonStates();
            RefreshUnreadableDirectoryNotifier(readable);
            view.NumberOfFiles = files.Length;
        }

        public PanelFrame PanelFrame {
            get { return (PanelFrame)Frame; }
        }

        public PanelFrame OtherPanelFrame {
            get { return PanelFrame.OtherPanelFrame; }
        }

        public Panel OtherPanel {
            get { return OtherPanelFrame.Panel; }
        }

        public ListStore Store {
            get { return (ListStore)view.Model; }
        }

        public TreeView View {
            get { return view; }
        }

        public FileComparer Comparer {
            get { return comparer; }
        }

        public void DisableListing()
        {
            listing_toggletoolbutton.Active = false;
        }

        public void DisableSorting()
        {
            sorting_toggletoolbutton.Active = false;
        }

        public override void Select()
        {
            if (!view.IsMapped) {
                return;
            }
            base.Select();
            view.GrabFocus();
        }

        // Cursor positioning methods

        void SetCursor(string old_dirpath)
        {
            bool upper_level = false;
                
            if (old_dirpath != null) {
                if (!(old_dirpath.Equals("") || old_dirpath.Equals("/"))) {
                    string old_rel_updirpath = UnixPath.Combine(old_dirpath, "..");
                    string old_abs_updirpath = UnixPath.GetFullPath(old_rel_updirpath);
                    upper_level = old_abs_updirpath.Equals(current_directory);
                }
            }

            if (upper_level) {  // Stepping up a level.
                string old_dirname = UnixPath.GetFileName(old_dirpath);
                SelectFileName(old_dirname);
            } else {  // Not stepping up a level, but somewhere else.
                view.SetCursor(new TreePath("0"), view.GetColumn(0), false);
            }
        }

        void SelectFileName(string filename)
        {
            TreeIter iter = new TreeIter();
            bool has_next = Store.GetIterFirst(out iter);
                
            while (has_next) {
                File file = (File)Store.GetValue(iter, 0);
                if (file.Name == filename) {
                    TreePath path = Store.GetPath(iter);
                    view.SetCursor(path, view.GetColumn(0), false);
                    view.ScrollToCell(path, null, true, 0.5f, 0.5f);
                    break;
                }
                has_next = Store.IterNext(ref iter);
            }
        }

        // Notification related methods

        void RefreshButtonStates()
        {
            up_directory_button.Sensitive = CurrentDirectory != "/";
            home_directory_button.Sensitive = CurrentDirectory != Config.HomePath;
        }

        void RefreshUnreadableDirectoryNotifier(bool readable)
        {
            if (readable && unreadable_directory_notifier.Parent != null) {
                unreadable_directory_notifier_slot.Remove(unreadable_directory_notifier);
            } else if (!readable && unreadable_directory_notifier.Parent == null) {
                unreadable_directory_notifier_slot.Add(unreadable_directory_notifier);
            }

            unreadable_directory_notifier_slot.ShowAll();
        }

        void RefreshInvalidEncodingNotifier(int count)
        {
            if (count == 0 && invalid_encoding_notifier.Parent != null) {
                invalid_encoding_notifier_slot.Remove(invalid_encoding_notifier);
            } else if (count > 0 && invalid_encoding_notifier.Parent == null) {
                invalid_encoding_notifier_slot.Add(invalid_encoding_notifier);
            }

            if (invalid_encoding_notifier.Parent != null) {
                invalid_encoding_notifier.SetText(count);
            }

            invalid_encoding_notifier_slot.ShowAll();
        }

        // Various utility methods

        void ActivateRow()
        {
            if (CurrentFile.IsDirectory) {
                ChangeDirectory(CurrentFile.FullPath);
            }
        }

        void InvertCurrentRow()
        {
            CurrentFile.InvertSelection();

            TreeIter iter = CurrentIter;
            TreePath path = null;
            TreeViewColumn column = null;
            view.GetCursor(out path, out column);

            if (Store.IterNext(ref iter)) {
                view.SetCursor(Store.GetPath(iter), column, false);			
            } else {
                view.SetCursor(Store.GetPath(CurrentIter), column, false);  // Refresh row.
            }
        }

        void RefreshStatusBar()
        {
            statusbar.Buffer.Text = CurrentFile.NameString;
        }

        string GetTopLevelAccessiblePath(string path)
        {
            string full_path = UnixPath.GetFullPath(path);
            while (!new UnixDirectoryInfo(full_path).Exists) {
                full_path = UnixPath.Combine(full_path, "..");
                full_path = UnixPath.GetFullPath(full_path);
            }
            
            return full_path;
        }

        TreePath CurrentPath {
            get {
                TreePath path;
                TreeViewColumn column;
                view.GetCursor(out path, out column);
                return path;
            }
        }

        TreeIter CurrentIter {
            get {
                TreeIter iter;
                Store.GetIter(out iter, CurrentPath);
                return iter;
            }
        }

        File CurrentFile {
            get {
                File file = (File)Store.GetValue(CurrentIter, 0);
                return file;
            }
        }

        // Renaming related methods

        [GLib.ConnectBefore]
        void OnStatusBarKeyPressEvent(object sender, KeyPressEventArgs args)
        {
            Gdk.Key key = args.Event.Key;

            switch (key) {
            case Gdk.Key.Tab:    // Disable tab and slash keys when renaming,
            case Gdk.Key.slash:  // because they are not appropriate in filenames.
                args.RetVal = true;
                break;
            case Gdk.Key.Return:
            case Gdk.Key.KP_Enter:
                args.RetVal = true;
                DoRename();
                break;
            case Gdk.Key.Escape:
                CancelRename();
                break;
            }
        }

        bool RenameActive {
            get { return statusbar.Editable; }
            set {
                statusbar.Editable = value;
                statusbar.CursorVisible = value;
                
                if (value) {
                    Util.SetWidgetBaseColorNormal(statusbar);
                } else {
                    Util.SetWidgetBaseColorInsensitive(statusbar);
                }
            }
        }
        
        public void StartRename()
        {
            if (CurrentFile.IsUpDirectory) {
                InfoBar.Error("Rename failed: up directory entries cannot be renamed.");
                return;
            }

            if (!CurrentFile.IsDirectoryWritable) {
                InfoBar.Error("Rename failed: the current directory is not writable.");
                return;
            }

            if (!CurrentFile.IsWritable) {
                InfoBar.Error("Rename failed: this file is not writable.");
                return;
            }

            TextBuffer buffer = statusbar.Buffer;
            RenameActive = true;
            buffer.SelectRange(buffer.StartIter, buffer.EndIter);
            statusbar.GrabFocus();
        }

        void CancelRename()
        {
            FinishRename();
            InfoBar.Notice("Rename cancelled by user.");
        }

        void DoRename()
        {
            string source_filepath = UnixPath.Combine(CurrentDirectory, CurrentFile.Name);
            string dest_filename = statusbar.Buffer.Text;
            string dest_filepath = UnixPath.Combine(CurrentDirectory, dest_filename); 

            if (File.IsFilePathExists(dest_filepath)) {
                InfoBar.Warning("Destination filename already exists. " +
                                "You should choose a different filename.");
                return;
            }

            FinishRename();

            if (MUN.Syscall.rename(source_filepath, dest_filepath) == 0) {
                ChangeDirectory(CurrentDirectory);
                SelectFileName(dest_filename);
                InfoBar.Notice("File successfully renamed.");
            } else {
                InfoBar.Error("Rename failed: " + MUN.Stdlib.strerror(MUN.Stdlib.GetLastError()));
            }
        }
        
        void FinishRename()
        {
            RenameActive = false;
            RefreshStatusBar();
            Select();
        }

        // Other signal handlers

        [GLib.ConnectBefore]
        void OnPanelViewKeyPressEvent(object sender, KeyPressEventArgs args)
        {
            Gdk.Key key = args.Event.Key;

            if (key == Gdk.Key.Insert) {
                InvertCurrentRow();
            } else if (key == Gdk.Key.Return) {
                ActivateRow();
                args.RetVal = true;
            } else if (key == Gdk.Key.space) {
                args.RetVal = true;
            }
        }

        void OnCursorChanged(object sender, EventArgs args)
        {
            Select();
            if (RenameActive) {
                CancelRename();
            } else {
                RefreshStatusBar();
            }
        }

        void OnRowActivated(object sender, RowActivatedArgs args)
        {
            ActivateRow();
        }

        [GLib.ConnectBefore]
        void OnButtonPressEvent(object sender, ButtonPressEventArgs args)
        {
            Select();
        }

        void OnSetListingButtonToggled(object toggletoolbutton, EventArgs args)
        {
            OtherPanelFrame.ShowConfigurator(listing_configurator,
                ((ToggleToolButton)toggletoolbutton).Active);
        }

        void OnSetSortingButtonToggled(object toggletoolbutton, EventArgs args)
        {
            OtherPanelFrame.ShowConfigurator(sorting_configurator,
                ((ToggleToolButton)toggletoolbutton).Active);
        }

        void OnUpDirectoryButtonClicked(object sender, EventArgs args)
        {
            Select();
            ChangeDirectory(UnixPath.Combine(CurrentDirectory, ".."));
        }

        void OnHomeButtonClicked(object sender, EventArgs args)
        {
            Select();
            ChangeDirectory(Config.HomePath);
        }

        void OnViewMapEvent(object sender, MapEventArgs args)
        {
            if ((Config.ActivePanel == WhichPanel.LeftPanel && MainWindow.LeftPanel == this) ||
                (Config.ActivePanel == WhichPanel.RightPanel && MainWindow.RightPanel == this)) {
                Select();
            }
        }
    }
}
