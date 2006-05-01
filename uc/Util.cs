using System;
using Mono.Unix;
using Gdk;
using Gtk;

namespace UltimateCommander {

    public static class Util {

        public static Gdk.Pixbuf LoadGtkIcon(string iconname)
        {
            return Gtk.IconTheme.Default.LoadIcon(iconname,
                Config.IconSize, Gtk.IconLookupFlags.NoSvg);
        }

        public static Pixbuf LoadIcon(string filename)
        {
            string filepath = UnixPath.Combine(Config.GuiPath, filename);
            return new Pixbuf(filepath);
        }

        public static void ModifyWidgetBase(Widget widget, StateType statetype)
        {
            widget.ModifyBase(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)statetype]);
        }

        public static void ModifyWidgetBg(Widget widget, StateType statetype)
        {
            widget.ModifyBg(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)statetype]);
        }
    }
}
