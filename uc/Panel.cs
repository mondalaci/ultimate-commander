using System;
using System.IO;
using Mono.Unix;
using Gdk;
using Gtk;
using Gnome.Vfs;
using Gnome;

namespace UltimateCommander {

	public class Panel: View {
                
		const int STOREROW_FILE = 0;
		static Gdk.Color selected_row_bgcolor = new Gdk.Color(224, 224, 0);

		[Glade.Widget] TreeView view;
		[Glade.Widget] Label statusbar;

		ListStore store = null;

    		string current_directory = null;
		int number_of_files = 0;
		bool active;
		bool button3_pressed = false;
		int prev_row_num;

		public Panel(string path): base("panel_window")
		{
			InitWidget();
			SetCurrentDirectory(path);
		}

		public bool Active {
			get {
				return active;
			}
			set {
				/*active = value;
				slot.Active = active;

				if (active) {
					view.GrabFocus();
				}*/
			}
		}        

		File GetFile(TreeIter iter)
		{
			return (File)store.GetValue(iter, STOREROW_FILE);
		}

		void SetCellBackground(TreeIter iter, CellRenderer cellrenderertext)
		{
           	File file = GetFile(iter);

           	if (file.Selected)
           		cellrenderertext.CellBackgroundGdk = selected_row_bgcolor;
           	else
           		cellrenderertext.CellBackgroundGdk = Widget.DefaultStyle.BaseColors[(int)StateType.Normal];
		}

		void CellDataToggleFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererToggle cellrenderertoggle = (CellRendererToggle)renderer;
           	File file = GetFile(iter);
			cellrenderertoggle.Active = file.Selected;
			cellrenderertoggle.Activatable = !file.IsUpDirectory;
			SetCellBackground(iter, cellrenderertoggle);
		}

		void CellDataIconFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererPixbuf cellrendererpixbuf = (CellRendererPixbuf)renderer;
           	File file = GetFile(iter);
			cellrendererpixbuf.Pixbuf = file.MimeIcon;
			SetCellBackground(iter, cellrendererpixbuf);
		}

		void CellDataIconFunc2(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererPixbuf cellrendererpixbuf = (CellRendererPixbuf)renderer;
           	File file = GetFile(iter);
			cellrendererpixbuf.Pixbuf = file.AttributeIcon;
			SetCellBackground(iter, cellrendererpixbuf);
		}

		void CellDataFilenameFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	File file = GetFile(iter);
			SetCellBackground(iter, cellrenderertext);
			cellrenderertext.Text = file.Name;
		}

		void CellDataSizeFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
		{
			CellRendererText cellrenderertext = (CellRendererText)renderer;
           	File file = GetFile(iter);
			SetCellBackground(iter, cellrenderertext);
			cellrenderertext.Text = file.Size.ToString();
		}

		void InitWidget ()
		{
			CellRendererToggle cellrenderertoggle = new CellRendererToggle();
			cellrenderertoggle.Toggled += new ToggledHandler(OnToggled);
			TreeViewColumn column1 = new TreeViewColumn();
			column1.PackStart(cellrenderertoggle, false);
			column1.SetCellDataFunc(cellrenderertoggle, CellDataToggleFunc);
			column1.Resizable = true;
			column1.Title = "S";
			column1.Alignment = 0.5f;
			
			CellRendererPixbuf cellrendererpixbuf = new CellRendererPixbuf();
			TreeViewColumn columnp = new TreeViewColumn();
			columnp.PackStart(cellrendererpixbuf, true);
			columnp.SetCellDataFunc(cellrendererpixbuf, CellDataIconFunc);
			columnp.Title = "I";
			columnp.Alignment = 0.5f;

			CellRendererPixbuf cellrendererpixbuf2 = new CellRendererPixbuf();
			TreeViewColumn columnp2 = new TreeViewColumn();
			columnp2.PackStart(cellrendererpixbuf2, true);
			columnp2.SetCellDataFunc(cellrendererpixbuf2, CellDataIconFunc2);
			columnp2.Title = "A";
			columnp2.Alignment = 0.5f;

			CellRendererText mycellrenderertext = new CellRendererText();
			TreeViewColumn column2 = new TreeViewColumn();
			column2.PackStart(mycellrenderertext, true);
			column2.SetCellDataFunc(mycellrenderertext, CellDataFilenameFunc);
			column2.Resizable = true;
			column2.Title = "Filename";

			CellRendererText cellrenderertext = new CellRendererText();
			TreeViewColumn column3 = new TreeViewColumn();
			column3.PackStart(cellrenderertext, true);
			column3.SetCellDataFunc(cellrenderertext, CellDataSizeFunc);
			column3.Resizable = true;
			column3.Title = "Size";

			view.AppendColumn(column1);
			view.AppendColumn(columnp2);
			view.AppendColumn(columnp);
			view.AppendColumn(column2);
			view.AppendColumn(column3);
		}
		
		void SetCurrentDirectory(string path)
		{
       		store = new ListStore(typeof(File));

			string old_directory = current_directory;
			path = UnixPath.GetFullPath(path);
			current_directory = GetTopLevelAccessiblePath(path);
			slot.Title = current_directory;
			
			string full_path = UnixPath.Combine(current_directory, "..");
			File file = new File(full_path);
       		store.AppendValues(file);
			number_of_files = 1;

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
					if (file2.Name == filename) {
						view.SetCursor(store.GetPath(iter), view.GetColumn(1), false);
						break;
					}
					has_next = store.IterNext(ref iter);
				}
			} else {  // Not stepping up a level, but somewhere else.
   	      		view.SetCursor(new TreePath("0"), view.GetColumn(1), false);
			}
     	}

		// Secondary methods.

     	void ActivateRow()
     	{
           	if (CurrentFile.IsDirectory) {
           		SetCurrentDirectory(CurrentFile.FullPath);
           	}
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
			}
		}

     	void OnPanelCursorChanged(object o, EventArgs e)
     	{
         	statusbar.Text = CurrentFile.Name;
		}		

		/*void OnFrameFocusInEvent(object o, FocusInEventArgs args)
		{
			other_panel.Active = false;
			Active = true;
		}*/

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

			if (row_num >= number_of_files - 1)
				row_num = number_of_files -1;

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
			//UltimateCommander.MainWindow.ActivePanel.view.GrabFocus();
		}

		void OnSetListingButtonToggled(object o, EventArgs args)
		{
			ToggleToolButton button = (ToggleToolButton)o;
			bool active = button.Active;
			PanelFrame current_frame = (PanelFrame)slot.Frame;
			PanelFrame other_frame = current_frame.OtherFrame;

			other_frame.ShowListing(active);

			if (!active)
				Select();
		}

		void OnSetSortingButtonToggled(object o, EventArgs args)
		{
			ToggleToolButton button = (ToggleToolButton)o;
			bool active = button.Active;
			PanelFrame current_frame = (PanelFrame)slot.Frame;
			PanelFrame other_frame = current_frame.OtherFrame;

			other_frame.ShowSorting(active);

			if (!active)
				Select();
		}

		protected override bool OnButtonPressEvent(EventButton eventbutton)
		{
			Active = true;
			return true;
		}
	}
}