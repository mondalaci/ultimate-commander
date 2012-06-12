using System;
using Gtk;
using Glade;

namespace UltimateCommander {

    public class GladeWidget: HBox {

        [Glade.Widget] protected EventBox topwidget;

        public GladeWidget(string window_name): base()
        {
            Glade.XML glade_xml = new Glade.XML(Config.GladeFileName, window_name, null);
            glade_xml.Autoconnect(this);
            Gtk.Window window = (Gtk.Window)glade_xml.GetWidget(window_name);
            window.Remove(topwidget);
            PackStart(topwidget, true, true, 0);
        }
    }
}
