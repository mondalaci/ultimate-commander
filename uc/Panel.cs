using System;
using System.IO;
using Mono.Unix;
using Gdk;
using Gtk;
using Gnome.Vfs;

namespace UltimateCommander {

	public class Panel: EventBox {
                
		const int STOREROW_FILE = 0;
		static Gdk.Color selected_row_bgcolor = new Gdk.Color(224, 224, 0);

		[Glade.Widget] Gtk.Window panel_window;
		[Glade.Widget] Gtk.VBox child;
		[Glade.Widget] TreeView view;
		[Glade.Widget] Label statusbar;

		Frame frame;
		ListStore store = null;

    	string current_directory = null;
		int number_of_files = 0;
		Panel other_panel;
		bool active;
		bool button3_pressed = false;
		int prev_row_num;

     	public Panel(): base()
     	{              
			InitWidget ();
			SetCurrentDirectory(UnixDirectoryInfo.GetCurrentDirectory());
     	}

		public Panel (string path) : base()
		{
			InitWidget();
			SetCurrentDirectory(path);
		}

		public bool Active {
			get {
				return active;
			}
			set {
				active = value;
				frame.Active = active;

				if (active) {
					view.GrabFocus();
				}
			}
		}        

		public Panel OtherPanel {
			set {
				other_panel = value;
			}
		}

		File GetFile(TreeIter iter)
		{
			return (File)store.GetValue(iter, STOREROW_FILE);
		}

		void SetCellBackground(TreeIter iter, CellRendererText cellrenderertext)
		{
           	File file = GetFile(iter);

           	if (file.Selected)
           		cellrenderertext.BackgroundGdk = selected_row_bgcolor;
           	else
           		cellrenderertext.BackgroundGdk = Widget.DefaultStyle.BaseColors[(int)StateType.Normal];
		}

		void CellDataToggleFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererToggle cellrenderertoggle = (CellRendererToggle)renderer;
           	File file = GetFile(iter);
			cellrenderertoggle.Active = file.Selected;
			cellrenderertoggle.Activatable = !file.IsUpDirectory;
		}

		void CellDataFilenameFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	File file = GetFile(iter);
			SetCellBackground(iter, cellrenderertext);
			cellrenderertext.Text = file.FileName;
		}

		void CellDataSizeFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	File file = GetFile(iter);
			SetCellBackground(iter, cellrenderertext);
			cellrenderertext.Text = ((long)file.stat.st_size).ToString();
		}

		void InitWidget ()
		{
			Glade.XML glade_xml = new Glade.XML(UltimateCommander.GladeFileName, "panel_window", null);
			glade_xml.Autoconnect(this);
			panel_window.Remove(child);
			
			CellRendererToggle cellrenderertoggle = new CellRendererToggle();
			cellrenderertoggle.Toggled += new ToggledHandler(OnToggled);
			TreeViewColumn column1 = new TreeViewColumn();
			column1.PackStart(cellrenderertoggle, false);
			column1.SetCellDataFunc(cellrenderertoggle, CellDataToggleFunc);
			column1.Resizable = true;

			CellRendererText cellrenderertext = new CellRendererText();
			TreeViewColumn column2 = new TreeViewColumn();
			column2.PackStart(cellrenderertext, true);
			column2.SetCellDataFunc(cellrenderertext, CellDataFilenameFunc);
			column2.Resizable = true;
			column2.Title = "filename";

			cellrenderertext = new CellRendererText();
			TreeViewColumn column3 = new TreeViewColumn();
			column3.PackStart(cellrenderertext, true);
			column3.SetCellDataFunc(cellrenderertext, CellDataSizeFunc);
			column3.Resizable = true;
			column3.Title = "size";

			view.AppendColumn(column1);
			view.AppendColumn(column2);
			view.AppendColumn(column3);

			frame = new Frame(child);
			Add(frame);
		}
		
		void SetCurrentDirectory(string path)
		{
       		store = new ListStore(typeof(File));

			string old_directory = current_directory;
			path = UnixPath.GetFullPath(path);
			current_directory = GetTopLevelAccessiblePath(path);
			frame.Title = current_directory;
			
			string full_path = UnixPath.Combine(current_directory, "..");
			File file = new File(full_path);
       		store.AppendValues(file);
			number_of_files++;

			// FIXME: When $MONO_EXTERNAL_ENCODINGS is not or inappropriately
			//        set, GetFileSystemEntries() skips accentuated filenames.
			string[] filenames = System.IO.Directory.GetFileSystemEntries(current_directory);

			foreach (string filename in filenames) {
				file = new File(filename);
           	   	store.AppendValues(file);
				number_of_files++;
       		}

          	view.Model = store;

			// Set the cursor.
			bool upper_level = false;
			string old_updir = null;
				
			if (old_directory != null)
				if (!(old_directory.Equals("") || old_directory.Equals("/"))) {
					old_updir = UnixPath.Combine(old_directory, "..");
					old_updir = UnixPath.GetFullPath(old_updir);
					upper_level = old_updir.Equals(current_directory);
				}

			if (upper_level) {  // Stepping up a level.
				string filename = UnixPath.GetFileName(old_directory);
				bool has_next = false;
				TreeIter iter = new TreeIter();
				has_next = store.GetIterFirst(out iter);
				
				while (has_next) {
					File file2 = (File)store.GetValue(iter, STOREROW_FILE);
					if (file2.FileName == filename) {
						view.SetCursor(store.GetPath(iter), view.GetColumn(1), false);
						break;
					}
					has_next = store.IterNext(ref iter);
				}
			} else  // Not stepping up a level, but somewhere else.
   	      		view.SetCursor(new TreePath("0"), view.GetColumn(1), false);
     	}

		// Secondary methods.

     	void ActivateRow()
     	{
			File file = CurrentFile;
			string filename = file.FileName;

           	if (file.IsDirectory)
           		SetCurrentDirectory(file.FullPath);
           	else if (file.IsFile) {
           		Console.WriteLine("{0} is a regular file.", filename);
				string type = Mime.TypeFromName(filename);
				string i = Mime.GetDescription(type);
				Console.WriteLine("type: {0}, icon: {1}", type, i);
           	} else
           		Console.WriteLine("{0} is not a regular file nor a directory.", filename);
     	}

		void SelectCurrentRow()
		{
			CurrentFile.InvertSelection();

			TreeIter iter = CurrentIter;
			TreePath path = null;
			TreeViewColumn column = null;
			view.GetCursor(out path, out column);

			if (store.IterNext(ref iter))
				view.SetCursor(store.GetPath(iter), column, false);			
			else
				view.SetCursor(store.GetPath(CurrentIter), column, false);  // Refresh row.
		}

		int GetRowNumFromCoords(double x, double y)
		{
			TreePath path;
			view.GetPathAtPos((int)x, (int)y, out path);

			if (path == null) {
				if (y<0)
					return 0;
				else
					return number_of_files - 1;
			} else
				return path.Indices[0];
		}

		void InvertRow(int row_num)
		{
			TreeIter iter;
			int[] path_array = {row_num};
			TreePath path = new TreePath(path_array);
			store.GetIter(out iter, path);
			File file = (File)store.GetValue(iter, 0);
			file.InvertSelection();
		}

		string GetTopLevelAccessiblePath(string full_path)
		{
			while (!new UnixDirectoryInfo(full_path).Exists) {
				full_path = UnixPath.Combine(full_path, "..");
				full_path = UnixPath.GetFullPath(full_path);
			}
			
			return full_path;
		}

     	string CurrentDirectory {
			get { return current_directory; }
		}

		TreeIter CurrentIter {
			get {
        		TreePath path;
	           	TreeViewColumn column;
    	       	TreeIter iter;

        	   	view.GetCursor(out path, out column);
           		store.GetIter(out iter, path);
           		return iter;
			}
		}

		File CurrentFile {
			get {
				TreeIter iter = CurrentIter;
	           	File file = (File)store.GetValue(iter, STOREROW_FILE);
    	       	return file;
			}
        }

		void OnPanelRowActivated(object o, RowActivatedArgs args)
		{
			ActivateRow();
     	}

		[GLib.ConnectBefore]
		void OnPanelKeyPressEvent(object o, KeyPressEventArgs args)
		{
			Gdk.Key key = args.Event.Key;

			if (key == Gdk.Key.Insert)
				SelectCurrentRow();
			else if (key == Gdk.Key.Tab) {
				Active = false;
				other_panel.Active = true;
			}
		}

     	void OnPanelCursorChanged(object o, EventArgs e)
     	{
         	statusbar.Text = CurrentFile.FileName;
		}		

		void OnFrameFocusInEvent(object o, FocusInEventArgs args)
		{
			other_panel.Active = false;
			Active = true;
		}

		void OnToggled(object o, ToggledArgs args)
		{
			TreeIter iter;
			if (store.GetIter(out iter, new TreePath(args.Path))) {
	           	File file = GetFile(iter);
				file.Selected = !file.Selected;
			}
		}

		[GLib.ConnectBefore]
		void OnButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3) {
				button3_pressed = true;
				prev_row_num = GetRowNumFromCoords(args.Event.X, args.Event.Y);
				InvertRow(prev_row_num);
			}
		}

		void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 3)
				button3_pressed = false;
		}

		[GLib.ConnectBefore]
		void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
		{
			int row_num = GetRowNumFromCoords(args.Event.X, args.Event.Y);

			if (!(button3_pressed && row_num != prev_row_num))
				return;

			int start = prev_row_num;
			int end = row_num;

			if (prev_row_num < row_num)
				start++;
			else
				start--;
			
			if (start > end) {
				int tmp = start;
				start = end;
				end = tmp;
			}
				
			for (int row=start; row<=end; row++) {
				InvertRow(row);
				int[] path_array = {row};
				TreeIter iter;
				TreePath path = new TreePath(path_array);
				store.GetIter(out iter, path);
				store.EmitRowChanged(path, iter);
			}
			
			prev_row_num = row_num;

			int[] path_array2 = {end};
      		view.SetCursor(new TreePath(path_array2), view.GetColumn(1), false);
			
		}

		void OnToolBarButtonEvent(object o, EventArgs args)
		{
			UltimateCommander.MainWindow.ActivePanel.view.GrabFocus();
		}

		protected override bool OnButtonPressEvent(EventButton eventbutton)
		{
			Active = true;
			return true;
		}
	}
}