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

        public static void SetWidgetBaseColorInsensitive(Widget widget)
        {
            widget.ModifyBase(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Insensitive]);
        }

        public static void SetWidgetBaseColorNormal(Widget widget)
        {
            widget.ModifyBase(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Normal]);
        }

        public static void SetWidgetBaseColorSelected(Widget widget)
        {
            widget.ModifyBase(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Selected]);
        }

        public static void SetWidgetBgColorInsensitive(Widget widget)
        {
            widget.ModifyBg(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Insensitive]);
        }

        public static void SetWidgetBgColorNormal(Widget widget)
        {
            widget.ModifyBg(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Normal]);
        }

        public static void SetWidgetBgColorSelected(Widget widget)
        {
            widget.ModifyBg(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Selected]);
        }
    }
}
