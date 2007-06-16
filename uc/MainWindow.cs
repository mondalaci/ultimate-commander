using System;
using Gdk;
using Gtk;

namespace UltimateCommander {
	
	public class MainWindow : Gtk.Window {

        // Stetic widgets
        protected Gtk.EventBox infobar_slot;
        protected Gtk.HPaned hpaned;
        protected Gtk.EventBox dialog_frame_slot;
        protected Gtk.MenuBar menubar;
        protected Gtk.Toolbar toolbar;
        protected Gtk.HBox command_hbox;
        protected Gtk.Image infobar_icon;
        protected Gtk.TextView infobar_message;

        // static Stetic widgets
        static Gtk.MenuBar menubar_static;
        static Gtk.Toolbar toolbar_static;
        static Gtk.HPaned hpaned_static;
        static Gtk.HBox command_hbox_static;
        static Gtk.Image infobar_icon_static;
        static Gtk.TextView infobar_message_static;

        // InfoBar related variables
        static Gdk.Pixbuf error;
        static Gdk.Pixbuf warning;
        static Gdk.Pixbuf notice;
        static InfoType type;
        static uint timer;
        static uint timeout_handler;
        static bool timeout_running = false;

        static Frame dialog_frame;
        static PanelFrame left_panel_frame;
        static PanelFrame right_panel_frame;
        static Frame active_frame;
        static Panel active_panel;
        static InfoBar infobar;
		
        float panel_ratio = 0.5f;
        int width = 0;
        int old_width = 0;

        public MainWindow(): base(Gtk.WindowType.Toplevel)
        {
            Stetic.Gui.Build(this, typeof(MainWindow));

            // these sigleton variables are meant to be static,
            // but Stetic can only bind to non-static variables
            menubar_static = menubar;
            toolbar_static = toolbar;
            hpaned_static = hpaned;
            command_hbox_static = command_hbox;
            infobar_icon_static = infobar_icon;
            infobar_message_static = infobar_message;

            // set up the dialog frame
            dialog_frame = new Frame();
            dialog_frame_slot.Add(dialog_frame);
            
            // set up the panels
            left_panel_frame = new PanelFrame(Config.InitialPath);
            hpaned.Add1(left_panel_frame);
            right_panel_frame = new PanelFrame(Config.InitialPath);
            hpaned.Add2(right_panel_frame);
            ResizePanes();
            SetActivePanel();

            // set up the infobar
            error = Util.LoadGtkIcon("gtk-dialog-error");
            warning = Util.LoadGtkIcon("gtk-dialog-warning");
            notice = Util.LoadGtkIcon("gtk-info");
            Util.ModifyWidgetBase(infobar_message, StateType.Insensitive);
            InfoBar.Notice("Ultimate Commander started.");

            this.ShowAll();
        }

        // File operations
        
        void CreateDirectory()
        {
            SetDialog(new CreateDirectoryDialog(), "Create Directory");
        }

        void Rename()
        {
            ActivePanel.StartRename();
        }
        
        // Dialog handling

        static void SetDialog(Dialog dialog, string title)
        {
            SetWindowSensitive(dialog == null);
        
            if (dialog != null) {
                DialogFrame.AppendView(dialog, title);
                DialogFrame.ShowAll();
            } else {
                RemoveDialog();
            }
        }

        public static void RemoveDialog()
        {
            DialogFrame.ClearViews();
            SetWindowSensitive(true);
            MainWindow.ActivePanel.Select();
        }

        static void SetWindowSensitive(bool sensitive)
        {
            Gtk.Widget[] widgets = {
                (Gtk.Widget)menubar_static,
                (Gtk.Widget)toolbar_static,
                (Gtk.Widget)hpaned_static,
                (Gtk.Widget)command_hbox_static,
            };
            foreach (Gtk.Widget widget in widgets) {
                widget.Sensitive = sensitive;
            }
        }

        // InfoBar handling
        
        public static void PrintInfo(InfoType type_arg, string text, params object[] args)
        {
            type = type_arg;
            infobar_message_static.Buffer.Text = String.Format(text, args);

            switch (type) {
            case InfoType.Error:
                infobar_icon_static.Pixbuf = error;
                break;
            case InfoType.Warning:
                infobar_icon_static.Pixbuf = warning;
                break;
            default:  // InfoType.Notice
                infobar_icon_static.Pixbuf = notice;
                break;
            }

            if (timeout_running) {
                timeout_running = false;
                GLib.Source.Remove(timeout_handler);
            }
            
            ShowBell(true);
            timeout_running = true;
            timer = 0;
            timeout_handler = GLib.Timeout.Add(Config.FlashInterval, ProgressHandler);
        }

        static bool ProgressHandler()
        {
            if (type == InfoType.Error || type == InfoType.Warning) {
                bool show = timer / Config.FlashInterval % 2 != 0;
                ShowBell(show);
            }

            timer += Config.FlashInterval;
            if (timer <= Config.WaitInterval) {
                return true;
            }

            // Terminate
            timeout_running = false;
            ShowBell(false);
            return false;
        }
        
        static void ShowBell(bool show)
        {
            if (show) {
                if (type == InfoType.Error) {
                    infobar_message_static.ModifyBase(StateType.Normal, Config.ErrorColor);
                } else if (type == InfoType.Warning) {
                    infobar_message_static.ModifyBase(StateType.Normal, Config.WarningColor);
                }
            } else {
                Util.ModifyWidgetBase(infobar_message_static, StateType.Insensitive);
            }
        }

        // Public accessors

        public static Frame DialogFrame {
            get { return dialog_frame; }
        }
        
        public static PanelFrame LeftPanelFrame {
            get { return left_panel_frame; }
        }

        public static PanelFrame RightPanelFrame {
            get { return right_panel_frame; }
        }

        public static Frame ActiveFrame {
            get { return active_frame; }
            set { active_frame = value; }
        }

        public static Panel LeftPanel {
            get { return left_panel_frame.Panel; }
        }

        public static Panel RightPanel {
            get { return right_panel_frame.Panel; }
        }

        public static Panel ActivePanel {
            get { return active_panel; }
            set { active_panel = value; }
        }
        
        public static Panel PassivePanel {
            get { return ActivePanel == LeftPanel ? RightPanel : LeftPanel; }
        }
        
        public static InfoBar InfoBar {
            get { return infobar; }
        }

        // Private utility members
        
        void ResizePanes()
        {
            width = hpaned.Allocation.Width;

            if (width != old_width) {
                PanelRatio = panel_ratio;
                old_width = width;
            } else {
                panel_ratio = PanelRatio;
            }
        }

        float PanelRatio {
            get {
                return (float)hpaned.Position / (float)hpaned.Allocation.Width;
            }
            set {
                int pos = (int)(value * hpaned.Allocation.Width);
                hpaned.Position = pos;
            }
        }
        
        void SetActivePanel()
        {
            if (Config.ActivePanel == WhichPanel.LeftPanel) {
                ActivePanel = LeftPanel;
            } else {
                ActivePanel = RightPanel;
            }
        }

        // Signal handlers

        protected virtual void OnCreateDirectoryButtonActivated (object sender, System.EventArgs e)
        {
            CreateDirectory();
        }

        protected virtual void OnCreateDirectoryMenItemActivated (object sender, System.EventArgs e)
        {
            CreateDirectory();
        }

        protected virtual void OnRenameButtonActivated (object sender, System.EventArgs e)
        {
            Rename();
        }

        protected virtual void OnRenameMenuItemActivated (object sender, System.EventArgs e)
        {
            Rename();
        }

        protected virtual void OnWindowResizeChecked (object sender, System.EventArgs e)
        {
            ResizePanes();
        }

        // Override the F6 key which focuses the HPaned by default.
        protected virtual void OnHPanedCycleChildFocus (object o, Gtk.CycleChildFocusArgs args)
        {
            args.RetVal = true;
            ActivePanel.Select();
        }

        protected virtual void OnWindowDeleteEvent (object o, Gtk.DeleteEventArgs args)
        {
            Gtk.Application.Quit();
        }

        protected virtual void OnQuitMenuItemActivated (object sender, System.EventArgs e)
        {
            Gtk.Application.Quit();
        }
    }
}
