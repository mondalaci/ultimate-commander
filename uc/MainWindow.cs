using System;
using Gtk;
using Glade;

namespace UltimateCommander {

    public class MainWindow {

        [Glade.Widget] Gtk.Window main_window;
        [Glade.Widget] MenuBar menubar;
        [Glade.Widget] HandleBox handlebox;
        [Glade.Widget] EventBox dialog_frame_slot;
        [Glade.Widget] HPaned hpaned;
        [Glade.Widget] HBox command_hbox;
        [Glade.Widget] EventBox infobar_slot;
        
        // static Glade widgets
        static Gtk.Window main_window_static;
        static MenuBar menubar_static;
        static HandleBox handlebox_static;
        static HPaned hpaned_static;
        static HBox command_hbox_static;
        static EventBox infobar_slot_static;

        static Frame dialog_frame;
        static PanelFrame left_panel_frame;
        static PanelFrame right_panel_frame;
        static InfoBar infobar = new InfoBar();
        static Frame active_frame;
        static Panel active_panel;

        float panel_ratio = 0.5f;
        int width = 0;
        int old_width = 0;

        public MainWindow()
        {
            Glade.XML glade_xml = new Glade.XML(Config.GladeFileName, "main_window", null);
            glade_xml.Autoconnect(this);
            
            // these sigleton variables are meant to be static,
            // but Glade can only bind to non-static variables
            main_window_static = main_window;
            menubar_static = menubar;
            handlebox_static = handlebox;
            hpaned_static = hpaned;
            command_hbox_static = command_hbox;

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
            infobar_slot.Add(infobar);
            InfoBar.Notice("Ultimate Commander started.");

            main_window.ShowAll();
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
            Widget[] widgets = {
                menubar_static,
                handlebox_static,
                hpaned_static,
                command_hbox_static,
            };
            foreach (Widget widget in widgets) {
                widget.Sensitive = sensitive;
            }
        }

        // Public accessors
        
        public static Gtk.Window Window {
            get { return main_window_static; }
        }
        
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
        
        void OnCreateDirectoryButtonClicked(object sender, EventArgs args)
        {
            CreateDirectory();
        }

        void OnCreateDirectoryMenuItemActivate(object sender, EventArgs args)
        {
            CreateDirectory();
        }

        void OnRenameButtonClicked(object sender, EventArgs args)
        {
            Rename();
        }

        void OnRenameMenuItemActivate(object sender, EventArgs args)
        {
            Rename();
        }

        void OnWindowDeleteEvent(object sender, DeleteEventArgs args)
        {
            Gtk.Application.Quit();
        }

        void OnQuitMenuItemActivate(object sender, EventArgs args)
        {
            Gtk.Application.Quit();
        }

        void OnWindowCheckResize(object sender, EventArgs args)
        {
            ResizePanes();
        }

        void OnHPanedCycleChildFocus(object sender, CycleChildFocusArgs args)
        {
            args.RetVal = true;
            ((PanelFrame)ActiveFrame).Panel.Select();
            //ActivePanel.Select();  // TODO
        }
    }
}
