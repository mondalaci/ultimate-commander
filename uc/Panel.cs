using System;
using System.Collections;
using Mono.Unix;
using Mono.Unix.Native;
using Gdk;
using Gtk;
using Gnome.Vfs;
using Gnome;

namespace UltimateCommander {

	public class Panel: View {
                
		static Gdk.Color selected_row_bgcolor = new Gdk.Color(224, 224, 0);
		static Gdk.Color invalid_encoding_color = new Gdk.Color(255, 0, 0);

		public static Gdk.Color SelectedRowBgColor {
			get { return selected_row_bgcolor; }
		}

		public static Gdk.Color InvalidEncodingColor {
			get { return invalid_encoding_color; }
		}

		[Glade.Widget] TreeView view;
		[Glade.Widget] ToolButton up_one_directory_button;
		[Glade.Widget] ToolButton go_to_home_directory_button;
		[Glade.Widget] Label statusbar;
		[Glade.Widget] EventBox unreadable_directory_notifier_slot;
		[Glade.Widget] EventBox invalid_encoding_notifier_slot;
		bool unreadable_directory_notifier_added;
		bool invalid_encoding_notifier_added;
		
		ListStore store = new ListStore(typeof(File));

   		string current_directory = null;
		int number_of_files = 0;
		bool active;
		bool button3_pressed = false;
		int prev_row_num;

		PanelListingConfigurator listing_configurator;
		PanelSortingConfigurator sorting_configurator;
		FileComparer comparer = new FileComparer();
		InvalidEncodingNotifier invalid_encoding_notifier;
		UnreadableDirectoryNotifier unreadable_directory_notifier;

		public Panel(string path): base("panel_window")
		{			
			view.Model = store;
			listing_configurator = new PanelListingConfigurator(this);
			sorting_configurator = new PanelSortingConfigurator(this);
			invalid_encoding_notifier = new InvalidEncodingNotifier();
			unreadable_directory_notifier = new UnreadableDirectoryNotifier();

			ChangeDirectory(path);
			RefreshButtonStates();
		}

		public ListStore Store {
			get { return store; }
		}

		public TreeView View {
			get { return view; }
		}

		public PanelListingConfigurator ListingConfigurator {
			get { return listing_configurator; }
		}

		public PanelSortingConfigurator SortingConfigurator {
			get { return sorting_configurator; }
		}

		public FileComparer Comparer {
			get { return comparer; }
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

		public void ChangeDirectory(string path)
		{
			string tla_path = GetTopLevelAccessiblePath(path);

			File directory = new File(tla_path);

			if (!directory.IsSearchable) {
				InfoBar.Error("You are not permitted to enter to this directory.  " +
					"It is not searchable.");
				return;
			}

			store.Clear();
			string prev_dir = CurrentDirectory;
			int invalid_encodings_counter = 0;
			current_directory = tla_path;
			slot.Title = File.StringifyInvalidFileNameEncoding(CurrentDirectory);
			bool readable = directory.IsReadable;
			RefreshUnreadableDirectoryNotifier(readable);
			File[] files;

			if (!readable) {
				File updir = new File(UnixPath.Combine(CurrentDirectory, ".."));
				files = new File[]{updir};
			} else {
				files = File.ListDirectory(CurrentDirectory);
			}
			
			Array.Sort(files, 1, files.Length-1, comparer);

			foreach (File file in files) {
				if (!file.HasValidEncoding) {
					invalid_encodings_counter += 1;
				}
				store.AppendValues(file);
			}

			view.ColumnsAutosize();
			RefreshInvalidEncodingNotifier(invalid_encodings_counter);
			SetCursor(prev_dir, CurrentDirectory);
			number_of_files = files.Length;
			RefreshButtonStates();
     	}

		void RefreshButtonStates()
		{
			up_one_directory_button.Sensitive = CurrentDirectory != "/";
			go_to_home_directory_button.Sensitive = CurrentDirectory != Util.HomeDirectoryPath;
		}

		void RefreshUnreadableDirectoryNotifier(bool readable)
		{
			if (readable && unreadable_directory_notifier_added) {
				unreadable_directory_notifier_slot.Remove(unreadable_directory_notifier);
				unreadable_directory_notifier_added = false;
			} else if (!readable && !unreadable_directory_notifier_added) {
				unreadable_directory_notifier_slot.Add(unreadable_directory_notifier);
				unreadable_directory_notifier_added = true;
			}

			unreadable_directory_notifier_slot.ShowAll();
		}

		void RefreshInvalidEncodingNotifier(int count)
		{
			if (count == 0 && invalid_encoding_notifier_added) {
				invalid_encoding_notifier_slot.Remove(invalid_encoding_notifier);
				invalid_encoding_notifier_added = false;
			} else if (count > 0 && !invalid_encoding_notifier_added) {
				invalid_encoding_notifier_slot.Add(invalid_encoding_notifier);
				invalid_encoding_notifier_added = true;
			}

			if (invalid_encoding_notifier_added) {
				invalid_encoding_notifier.SetText(count);
			}

			invalid_encoding_notifier_slot.ShowAll();
		}

		void SetCursor(string old_directory, string current_directory)
		{
			bool upper_level = false;
			string old_updir = null;
				
			if (old_directory != null) {
				if (!(old_directory.Equals("") || old_directory.Equals("/"))) {
					old_updir = UnixPath.Combine(old_directory, "..");
					old_updir = UnixPath.GetFullPath(old_updir);
					upper_level = old_updir.Equals(current_directory);
				}
			}

			if (upper_level) {  // Stepping up a level.
				string filename = UnixPath.GetFileName(old_directory);
				bool has_next = false;
				TreeIter iter = new TreeIter();
				has_next = store.GetIterFirst(out iter);
				
				while (has_next) {
					File file2 = (File)store.GetValue(iter, 0);
					if (file2.Name == filename) {
						TreePath path = store.GetPath(iter);
						view.SetCursor(path, view.GetColumn(1), false);
						view.ScrollToCell(path, null, true, 0.5f, 0.5f);
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
           		ChangeDirectory(CurrentFile.FullPath);
           	}
     	}

		void SelectCurrentRow()
		{
			CurrentFile.InvertSelection();

			TreeIter iter = CurrentIter;
			TreePath path = null;
			TreeViewColumn column = null;
			view.GetCursor(out path, out column);

			if (store.IterNext(ref iter)) {
				view.SetCursor(store.GetPath(iter), column, false);			
			} else {
				view.SetCursor(store.GetPath(CurrentIter), column, false);  // Refresh row.
			}
		}

		int GetRowNumFromCoords(double x, double y)
		{
			TreePath path;
			view.GetPathAtPos((int)x, (int)y, out path);

			if (path == null) {
				if (y < 0) {
					return 0;
				} else {
					return number_of_files - 1;
				}
			} else {
				return path.Indices[0];
			}
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

		string GetTopLevelAccessiblePath(string path)
		{
			string full_path = UnixPath.GetFullPath(path);
			while (!new UnixDirectoryInfo(full_path).Exists) {
				full_path = UnixPath.Combine(full_path, "..");
				full_path = UnixPath.GetFullPath(full_path);
			}
			
			return full_path;
		}

     	public string CurrentDirectory {
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
	           	File file = (File)store.GetValue(iter, 0);
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

			if (key == Gdk.Key.Insert) {
				SelectCurrentRow();
			} else if (key == Gdk.Key.Tab) {
			}
		}

     	void OnPanelCursorChanged(object o, EventArgs e)
     	{
         	statusbar.Text = CurrentFile.NameString;
		}		

		/*void OnFrameFocusInEvent(object o, FocusInEventArgs args)
		{
			other_panel.Active = false;
			Active = true;
		}*/

/*		void OnToggled(object o, ToggledArgs args)
		{
			TreeIter iter;
			if (store.GetIter(out iter, new TreePath(args.Path))) {
	           	File file = GetFile(iter);
				file.Selected = !file.Selected;
			}
		}
*/
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
			if (args.Event.Button == 3) {
				button3_pressed = false;
			}
		}

		[GLib.ConnectBefore]
		void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
		{
			int row_num = GetRowNumFromCoords(args.Event.X, args.Event.Y);

			if (row_num >= number_of_files - 1) {
				row_num = number_of_files -1;
			}

			if (!(button3_pressed && row_num != prev_row_num)) {
				return;
			}

			int start = prev_row_num;
			int end = row_num;

			if (prev_row_num < row_num) {
				start++;
			} else {
				start--;
			}
			
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

		void OnThumbnailViewButtonClicked(object sender, EventArgs args)
		{
		}

		void OnZoomOutButtonClicked(object sender, EventArgs args)
		{
		}

		void OnZoomInButtonClicked(object sender, EventArgs args)
		{
		}

		void OnSetListingButtonToggled(object toggletoolbutton, EventArgs args)
		{
			ShowAndSelectConfigurator((ToggleToolButton)toggletoolbutton, ListingConfigurator);
		}

		void OnSetSortingButtonToggled(object toggletoolbutton, EventArgs args)
		{
			ShowAndSelectConfigurator((ToggleToolButton)toggletoolbutton, SortingConfigurator);
		}

		void OnSetFilteringButtonToggled(object toggletoolbutton, EventArgs args)
		{
		}

		void OnQuickCdButtonClicked(object sender, EventArgs args)
		{
		}

		void OnUpOneDirectoryButtonClicked(object sender, EventArgs args)
		{
			ChangeDirectory(UnixPath.Combine(CurrentDirectory, ".."));
		}

		void OnBackwardButtonClicked(object sender, EventArgs args)
		{
		}

		void OnForwardButtonClicked(object sender, EventArgs args)
		{
		}

		void OnGoToHomeButtonClicked(object sender, EventArgs args)
		{
			ChangeDirectory(Util.HomeDirectoryPath);
		}

		void ShowAndSelectConfigurator(ToggleToolButton toggletoolbutton,
									   PanelConfigurator configurator)
		{
			PanelFrame current_frame = (PanelFrame)slot.Frame;
			PanelFrame other_frame = current_frame.OtherFrame;

			other_frame.ShowConfigurator(configurator, toggletoolbutton.Active);

			if (!active) {
				Select();
			}
		}

		protected override bool OnButtonPressEvent(EventButton eventbutton)
		{
			Active = true;
			return true;
		}
	}
}