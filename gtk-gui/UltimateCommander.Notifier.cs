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
    
    
    internal class UltimateCommanderNotifier {
        
        public static void Build(Gtk.Bin cobj) {
            System.Collections.Hashtable bindings = new System.Collections.Hashtable();
            // Widget UltimateCommander.Notifier
            Stetic.BinContainer.Attach(cobj);
            cobj.Name = "UltimateCommander.Notifier";
            // Container child UltimateCommander.Notifier.Gtk.Container+ContainerChild
            Gtk.VBox w1 = new Gtk.VBox();
            bindings["vbox1"] = w1;
            w1.Name = "vbox1";
            w1.Spacing = 6;
            w1.BorderWidth = ((uint)(2));
            // Container child vbox1.Gtk.Box+BoxChild
            Gtk.HSeparator w2 = new Gtk.HSeparator();
            bindings["hseparator1"] = w2;
            w2.Name = "hseparator1";
            w1.Add(w2);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(w1[w2]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            Gtk.HBox w4 = new Gtk.HBox();
            bindings["hbox1"] = w4;
            w4.Name = "hbox1";
            w4.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            Gtk.Image w5 = new Gtk.Image();
            bindings["icon"] = w5;
            w5.Name = "icon";
            w5.Yalign = 0.01F;
            w4.Add(w5);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(w4[w5]));
            w6.Position = 0;
            w6.Expand = false;
            w6.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            Gtk.TextView w7 = new Gtk.TextView();
            bindings["textview"] = w7;
            w7.CanFocus = true;
            w7.Name = "textview";
            w7.Editable = false;
            w7.CursorVisible = false;
            w7.WrapMode = ((Gtk.WrapMode)(2));
            w4.Add(w7);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(w4[w7]));
            w8.Position = 1;
            w1.Add(w4);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(w1[w4]));
            w9.Position = 1;
            w9.Expand = false;
            w9.Fill = false;
            cobj.Add(w1);
            if ((cobj.Child != null)) {
                cobj.Child.ShowAll();
            }
            cobj.Show();
            cobj.ButtonPressEvent += ((Gtk.ButtonPressEventHandler)(System.Delegate.CreateDelegate(typeof(Gtk.ButtonPressEventHandler), cobj, "OnButtonPressEvent")));
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
