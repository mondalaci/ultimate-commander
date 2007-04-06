using System;
using Mono.Unix;
using Gdk;
using Gtk;

namespace UltimateCommander {

    public static class Util {

        public static Gdk.Pixbuf LoadGtkIcon(string iconname)
        {
            Gdk.Pixbuf icon;
            try {
            icon = Gtk.IconTheme.Default.LoadIcon(iconname,
                Config.IconSize, Gtk.IconLookupFlags.NoSvg);
            } catch (Exception) {
                return LoadGtkIcon("gtk-missing-image");
            }
            return icon;
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
