// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Stetic.SteticGenerated {
    
    
    internal class UltimateCommanderMainWindow {
        
        public static void Build(Gtk.Window cobj) {
            System.Collections.Hashtable bindings = new System.Collections.Hashtable();
            // Widget UltimateCommander.MainWindow
            Gtk.UIManager w1 = new Gtk.UIManager();
            Gtk.ActionGroup w2 = new Gtk.ActionGroup("Default");
            Gtk.Action w3 = new Gtk.Action("File", Mono.Unix.Catalog.GetString("File"), null, null);
            bindings["File"] = w3;
            w3.ShortLabel = Mono.Unix.Catalog.GetString("File");
            w2.Add(w3, null);
            Gtk.Action w4 = new Gtk.Action("CreateDirectory", Mono.Unix.Catalog.GetString("Create Directory"), null, null);
            bindings["CreateDirectory"] = w4;
            w4.ShortLabel = Mono.Unix.Catalog.GetString("Create Directory");
            w2.Add(w4, null);
            Gtk.Action w5 = new Gtk.Action("Rename", Mono.Unix.Catalog.GetString("Rename"), null, "gtk-copy");
            bindings["Rename"] = w5;
            w5.ShortLabel = Mono.Unix.Catalog.GetString("Rename");
            w2.Add(w5, null);
            Gtk.Action w6 = new Gtk.Action("Quit", Mono.Unix.Catalog.GetString("Quit"), null, "gtk-quit");
            bindings["Quit"] = w6;
            w6.ShortLabel = Mono.Unix.Catalog.GetString("Quit");
            w2.Add(w6, null);
            Gtk.Action w7 = new Gtk.Action("CreateDirectory3", Mono.Unix.Catalog.GetString("Create Directory"), null, "stock_new-dir");
            bindings["CreateDirectory3"] = w7;
            w7.ShortLabel = Mono.Unix.Catalog.GetString("Create Directory");
            w2.Add(w7, null);
            Gtk.Action w8 = new Gtk.Action("Copy", Mono.Unix.Catalog.GetString("Copy"), null, "gtk-copy");
            bindings["Copy"] = w8;
            w8.ShortLabel = Mono.Unix.Catalog.GetString("Copy");
            w2.Add(w8, null);
            Gtk.Action w9 = new Gtk.Action("Move", Mono.Unix.Catalog.GetString("Move"), null, "gtk-indent");
            bindings["Move"] = w9;
            w9.ShortLabel = Mono.Unix.Catalog.GetString("Move");
            w2.Add(w9, null);
            Gtk.Action w10 = new Gtk.Action("CreateDirectory2", Mono.Unix.Catalog.GetString("Create Directory"), null, "stock_new-dir");
            bindings["CreateDirectory2"] = w10;
            w10.ShortLabel = Mono.Unix.Catalog.GetString("Create Directory");
            w2.Add(w10, null);
            Gtk.Action w11 = new Gtk.Action("Rename1", Mono.Unix.Catalog.GetString("Rename"), null, "gtk-edit");
            bindings["Rename1"] = w11;
            w11.ShortLabel = Mono.Unix.Catalog.GetString("Rename");
            w2.Add(w11, null);
            Gtk.Action w12 = new Gtk.Action("Delete", Mono.Unix.Catalog.GetString("Delete"), null, "gtk-delete");
            bindings["Delete"] = w12;
            w12.ShortLabel = Mono.Unix.Catalog.GetString("Delete");
            w2.Add(w12, null);
            Gtk.Action w13 = new Gtk.Action("CreateDirectory4", Mono.Unix.Catalog.GetString("Create Directory"), null, "stock_new-dir");
            bindings["CreateDirectory4"] = w13;
            w13.ShortLabel = Mono.Unix.Catalog.GetString("Create Directory");
            w2.Add(w13, null);
            w1.InsertActionGroup(w2, 0);
            cobj.AddAccelGroup(w1.AccelGroup);
            cobj.Name = "UltimateCommander.MainWindow";
            cobj.Title = Mono.Unix.Catalog.GetString("Ultimate Commander");
            cobj.Icon = new Gdk.Pixbuf(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "./gui/logo.png"));
            cobj.WindowPosition = ((Gtk.WindowPosition)(4));
            // Container child UltimateCommander.MainWindow.Gtk.Container+ContainerChild
            Gtk.VBox w14 = new Gtk.VBox();
            bindings["vbox1"] = w14;
            w14.Name = "vbox1";
            // Container child vbox1.Gtk.Box+BoxChild
            w1.AddUiFromString("<ui><menubar name='menubar'><menu action='File'><menuitem action='Rename'/><menuitem action='CreateDirectory3'/><separator/><menuitem action='Quit'/></menu></menubar></ui>");
            Gtk.MenuBar w15 = ((Gtk.MenuBar)(w1.GetWidget("/menubar")));
            bindings["menubar"] = w15;
            w15.Name = "menubar";
            w14.Add(w15);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(w14[w15]));
            w16.Position = 0;
            w16.Expand = false;
            w16.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            w1.AddUiFromString("<ui><toolbar name='toolbar'><toolitem action='Copy'/><toolitem action='Move'/><toolitem action='Rename1'/><toolitem action='CreateDirectory4'/><toolitem action='Delete'/></toolbar></ui>");
            Gtk.Toolbar w17 = ((Gtk.Toolbar)(w1.GetWidget("/toolbar")));
            bindings["toolbar"] = w17;
            w17.Name = "toolbar";
            w17.ShowArrow = false;
            w17.ToolbarStyle = ((Gtk.ToolbarStyle)(2));
            w17.IconSize = ((Gtk.IconSize)(2));
            w14.Add(w17);
            Gtk.Box.BoxChild w18 = ((Gtk.Box.BoxChild)(w14[w17]));
            w18.Position = 1;
            w18.Expand = false;
            w18.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            Gtk.EventBox w19 = new Gtk.EventBox();
            bindings["dialog_frame_slot"] = w19;
            w19.Name = "dialog_frame_slot";
            w14.Add(w19);
            Gtk.Box.BoxChild w20 = ((Gtk.Box.BoxChild)(w14[w19]));
            w20.Position = 2;
            w20.Expand = false;
            // Container child vbox1.Gtk.Box+BoxChild
            Gtk.HPaned w21 = new Gtk.HPaned();
            bindings["hpaned"] = w21;
            w21.CanFocus = true;
            w21.Name = "hpaned";
            w21.Position = 401;
            w14.Add(w21);
            Gtk.Box.BoxChild w22 = ((Gtk.Box.BoxChild)(w14[w21]));
            w22.Position = 3;
            // Container child vbox1.Gtk.Box+BoxChild
            Gtk.HBox w23 = new Gtk.HBox();
            bindings["command_hbox"] = w23;
            w23.Name = "command_hbox";
            w23.Spacing = 6;
            w23.BorderWidth = ((uint)(2));
            // Container child command_hbox.Gtk.Box+BoxChild
            Gtk.Label w24 = new Gtk.Label();
            bindings["label1"] = w24;
            w24.Name = "label1";
            w24.LabelProp = Mono.Unix.Catalog.GetString("/currently/static/path");
            w23.Add(w24);
            Gtk.Box.BoxChild w25 = ((Gtk.Box.BoxChild)(w23[w24]));
            w25.Position = 0;
            w25.Expand = false;
            w25.Fill = false;
            // Container child command_hbox.Gtk.Box+BoxChild
            Gtk.ComboBoxEntry w26 = new Gtk.ComboBoxEntry();
            bindings["comboboxentry1"] = w26;
            w26.Name = "comboboxentry1";
            w23.Add(w26);
            Gtk.Box.BoxChild w27 = ((Gtk.Box.BoxChild)(w23[w26]));
            w27.Position = 1;
            w14.Add(w23);
            Gtk.Box.BoxChild w28 = ((Gtk.Box.BoxChild)(w14[w23]));
            w28.Position = 4;
            w28.Expand = false;
            w28.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            Gtk.Alignment w29 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            bindings["alignment1"] = w29;
            w29.Name = "alignment1";
            w29.BottomPadding = ((uint)(3));
            // Container child alignment1.Gtk.Container+ContainerChild
            Gtk.HSeparator w30 = new Gtk.HSeparator();
            bindings["hseparator1"] = w30;
            w30.Name = "hseparator1";
            w29.Add(w30);
            w14.Add(w29);
            Gtk.Box.BoxChild w32 = ((Gtk.Box.BoxChild)(w14[w29]));
            w32.Position = 5;
            w32.Expand = false;
            w32.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            Gtk.EventBox w33 = new Gtk.EventBox();
            bindings["infobar_slot"] = w33;
            w33.Name = "infobar_slot";
            // Container child infobar_slot.Gtk.Container+ContainerChild
            Gtk.HBox w34 = new Gtk.HBox();
            bindings["hbox1"] = w34;
            w34.Name = "hbox1";
            w34.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            Gtk.Image w35 = new Gtk.Image();
            bindings["infobar_icon"] = w35;
            w35.Name = "infobar_icon";
            w34.Add(w35);
            Gtk.Box.BoxChild w36 = ((Gtk.Box.BoxChild)(w34[w35]));
            w36.Position = 0;
            w36.Expand = false;
            w36.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            Gtk.TextView w37 = new Gtk.TextView();
            bindings["infobar_message"] = w37;
            w37.CanFocus = true;
            w37.Name = "infobar_message";
            w34.Add(w37);
            Gtk.Box.BoxChild w38 = ((Gtk.Box.BoxChild)(w34[w37]));
            w38.Position = 1;
            w33.Add(w34);
            w14.Add(w33);
            Gtk.Box.BoxChild w40 = ((Gtk.Box.BoxChild)(w14[w33]));
            w40.Position = 6;
            w40.Expand = false;
            cobj.Add(w14);
            if ((cobj.Child != null)) {
                cobj.Child.ShowAll();
            }
            cobj.DefaultWidth = 904;
            cobj.DefaultHeight = 630;
            cobj.Show();
            cobj.DeleteEvent += ((Gtk.DeleteEventHandler)(System.Delegate.CreateDelegate(typeof(Gtk.DeleteEventHandler), cobj, "OnWindowDeleteEvent")));
            cobj.ResizeChecked += ((System.EventHandler)(System.Delegate.CreateDelegate(typeof(System.EventHandler), cobj, "OnWindowResizeChecked")));
            w5.Activated += ((System.EventHandler)(System.Delegate.CreateDelegate(typeof(System.EventHandler), cobj, "OnRenameMenuItemActivated")));
            w6.Activated += ((System.EventHandler)(System.Delegate.CreateDelegate(typeof(System.EventHandler), cobj, "OnQuitMenuItemActivated")));
            w7.Activated += ((System.EventHandler)(System.Delegate.CreateDelegate(typeof(System.EventHandler), cobj, "OnCreateDirectoryMenItemActivated")));
            w10.Activated += ((System.EventHandler)(System.Delegate.CreateDelegate(typeof(System.EventHandler), cobj, "OnCreateDirectoryButtonActivated")));
            w11.Activated += ((System.EventHandler)(System.Delegate.CreateDelegate(typeof(System.EventHandler), cobj, "OnRenameButtonActivated")));
            w13.Activated += ((System.EventHandler)(System.Delegate.CreateDelegate(typeof(System.EventHandler), cobj, "OnCreateDirectoryButtonActivated")));
            w21.CycleChildFocus += ((Gtk.CycleChildFocusHandler)(System.Delegate.CreateDelegate(typeof(Gtk.CycleChildFocusHandler), cobj, "OnHPanedCycleChildFocus")));
            System.Reflection.FieldInfo[] fields = cobj.GetType().GetFields(((System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic) | System.Reflection.BindingFlags.Instance));
            for (int n = 0; (n < fields.Length); n = (n + 1)) {
                System.Reflection.FieldInfo field = fields[n];
                object widget = bindings[field.Name];
                if (((widget != null) && field.FieldType.IsInstanceOfType(widget))) {
                    field.SetValue(cobj, widget);
                }
            }
        }
    }
}
