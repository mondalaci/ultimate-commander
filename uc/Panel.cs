using System;
using System.IO;
using Mono.Unix;

using Gtk;
using Gdk;
using Gnome.Vfs;

namespace UltimateCommander {

	public delegate void ActivatedHandler(Panel panel);

/* TODO

	class ClickableTreeViewColumn: TreeViewColumn {
		protected override void OnClicked()
		{
			Console.WriteLine("TreeViewColumn.OnClicked()");
		}
	}
*/

/* TODO

	class MyListStore: ListStore {
		protected override void OnRowsReordered(TreePath path, TreeIter iter, int new_order) {
			Console.WriteLine("ListStore.OnRowsReordered()");
		}
	}
*/

	public class Panel: VBox {
                
		static int STOREROW_FILE = 0;

		const int border_width = 3;
		const int label_space_width = 3;

		const string active_header_colorstring = "#ffffff";
		const string inactive_header_colorstring = "#000000";

		static Gdk.Color active_header_bgcolor = new Gdk.Color(66, 105, 123);
		static Gdk.Color inactive_header_bgcolor = new Gdk.Color(231, 227, 222);

		static Gdk.Color selected_row_bgcolor = new Gdk.Color(224, 224, 0);
		static Gdk.Color unselected_row_bgcolor = new Gdk.Color(255, 255, 255);

		bool activated;
    	string current_directory = null;
		ListStore store = null;

		Label header = new Label();
     	TreeView view = new TreeView();
		EventBox frame = new EventBox();
		Label statusbar = new Label();

		public ActivatedHandler ActivatedEvent;

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

		// Primary methods.

		public File GetFile(TreeIter iter)
		{
			return (File)store.GetValue(iter, STOREROW_FILE);
		}

		public void SetCellBackground(TreeIter iter, CellRendererText cellrenderertext)
		{
           	File file = GetFile(iter);

           	if (file.Selected)
           		cellrenderertext.BackgroundGdk = selected_row_bgcolor;
           	else
           		cellrenderertext.BackgroundGdk = unselected_row_bgcolor;
		}

		public void CellDataToggleFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererToggle cellrenderertoggle = (CellRendererToggle)renderer;
           	File file = GetFile(iter);
			cellrenderertoggle.Active = file.Selected;
			cellrenderertoggle.Activatable = !(file.FileName == "..");
		}

		public void CellDataFilenameFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	File file = GetFile(iter);
			SetCellBackground(iter, cellrenderertext);
			cellrenderertext.Text = file.FileName;
		}

		public void CellDataSizeFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	File file = GetFile(iter);
			SetCellBackground(iter, cellrenderertext);
			cellrenderertext.Text = ((long)file.stat.st_size).ToString();
		}

		public void InitWidget ()
		{
			Label header_label = GetSpacedLabel(header);

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
			view.EnableSearch = false;
			// view.HeadersClickable = true;  // TODO
			// view.Reorderable = true;  // TODO

			view.KeyPressEvent += new KeyPressEventHandler(OnKeyPressEvent);
          	view.CursorChanged += new EventHandler(OnCursorChanged);
			view.RowActivated += new RowActivatedHandler(OnRowActivated);
			view.FocusInEvent += new FocusInEventHandler(OnFocusInEvent);

			ScrolledWindow scrolled_window = new ScrolledWindow();
			scrolled_window.Add(view);

			Label statusbar_label = GetSpacedLabel(statusbar);

			VBox vbox1 = new VBox();
          	vbox1.PackStart(scrolled_window);
			vbox1.PackStart(statusbar_label, false, true, 0);

			EventBox content_area = new EventBox();
			content_area.Add(vbox1);

			Label bottom_space = new Label();
			bottom_space.HeightRequest = border_width;

			VBox vbox2 = new VBox();
			vbox2.PackStart(header_label, false, true, 0);
			vbox2.PackStart(content_area);
			vbox2.PackStart(bottom_space, false, true, 0);

			Label left_space = new Label();
			left_space.WidthRequest = border_width;
			Label right_space = new Label();
			right_space.WidthRequest = border_width;

			HBox hbox = new HBox();
			hbox.PackStart(left_space, false, true, 0);
			hbox.PackStart(vbox2);
			hbox.PackStart(right_space, false, true, 0);

			frame.Add(hbox);
			PackStart(frame);
		}
		
		public void SetCurrentDirectory(string path)
		{
       		store = new ListStore(typeof(File));

			string old_directory = current_directory;
			path = UnixPath.GetFullPath(path);
			current_directory = GetTopLevelAccessiblePath(path);
			RefreshHeaderString();
			
			string full_path = UnixPath.Combine(current_directory, "..");
			File file = new File(full_path);
       		store.AppendValues(file);

			// FIXME: When $MONO_EXTERNAL_ENCODINGS is not or inappropriately
			//        set, GetFileSystemEntries() skips accentuated filenames.
			string[] filenames = System.IO.Directory.GetFileSystemEntries(current_directory);

			foreach (string filename in filenames) {
				file = new File(filename);
           	   	store.AppendValues(file);
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

			//TreePath treepath = null;
			//TreeViewColumn treeviewcolumn = null;
			//view.GetCursor(out treepath, out treeviewcolumn);

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

     	public void ActivateRow()
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

		public void SelectRow()
		{
			TreeIter iter = CurrentIter;
			File file = CurrentFile;

			if (file.FileName != "..")
				file.Selected = !file.Selected;

			TreePath treepath = null;
			TreeViewColumn treeviewcolumn = null;
			view.GetCursor(out treepath, out treeviewcolumn);

			if (store.IterNext(ref iter))
				view.SetCursor(store.GetPath(iter), treeviewcolumn, false);			
			else
				view.SetCursor(store.GetPath(CurrentIter), treeviewcolumn, false);  // Refresh row.
		}

		public void SetActivated(bool activated)
		{
			Color header_bgcolor;

			this.activated = activated;

			if (activated) {
				ActivatedEvent(this);
				header_bgcolor = active_header_bgcolor;
				view.HasFocus = true;
			} else
				header_bgcolor = inactive_header_bgcolor;
			
			RefreshHeaderString();
			frame.ModifyBg(StateType.Normal, header_bgcolor);
		}

		private void RefreshHeaderString()
		{
			string header_colorstring;

			if (activated)
				header_colorstring = active_header_colorstring;
			else
				header_colorstring = inactive_header_colorstring;

			header.Markup = GetFgPangoMarkup(header_colorstring, CurrentDirectory);
		}

		// Utility methods.

		static public string EscapeText(string unescaped_text, string chars_to_escape)
		{
			string escaped_text = "";

			foreach (char c in unescaped_text)
				if (chars_to_escape.IndexOf(c) != -1)
					escaped_text += "\\" + c;
				else
					escaped_text += c;

			return escaped_text;
		}

		static public string GetFgPangoMarkup(string color, string unescaped_text)
		{
			string escaped_text = EscapeText(unescaped_text, "<>");
			return "<span foreground=\"" + color + "\">" + escaped_text + "</span>";
		}

		static private Label GetSpacedLabel(Label label)
		{
			label.Xalign = 0;
			label.LineWrap = true;
			label.Xpad = 2;
			label.Ypad = 2;
			return label;
		}

		static private string GetTopLevelAccessiblePath(string full_path)
		{
			while (!new UnixDirectoryInfo(full_path).Exists) {
				full_path = UnixPath.Combine(full_path, "..");
				full_path = UnixPath.GetFullPath(full_path);
			}
			
			return full_path;
		}

		// Accessors.

     	public string CurrentDirectory {
			get { return current_directory; }
		}

		public TreeIter CurrentIter {
			get {
        		TreePath path;
	           	TreeViewColumn column;
    	       	TreeIter iter;

        	   	view.GetCursor(out path, out column);
           		store.GetIter(out iter, path);
           		return iter;
			}
		}

		public File CurrentFile {
			get {
				TreeIter iter = CurrentIter;
	           	File file = (File)store.GetValue(iter, STOREROW_FILE);
    	       	return file;
			}
        }
        
		// Event handlers.

		public void OnRowActivated(object o, RowActivatedArgs args)
		{
			ActivateRow();
     	}

		public void OnKeyPressEvent(object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Insert)
				SelectRow();
		}

     	public void OnCursorChanged(object o, EventArgs e)
     	{
         	statusbar.Text = CurrentFile.FileName;
		}		

		public void OnFocusInEvent(object o, FocusInEventArgs args)
		{
			SetActivated(true);
		}

		public void OnToggled(object o, ToggledArgs args)
		{
			TreeIter iter;
			if (store.GetIter(out iter, new TreePath(args.Path))) {
	           	File file = GetFile(iter);
				file.Selected = !file.Selected;
			}
		}

		protected override bool OnButtonPressEvent(EventButton eventbutton)
		{
			SetActivated(true);
			return true;
		}
	}
}