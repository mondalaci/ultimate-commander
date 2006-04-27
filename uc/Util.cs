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

        public static void PaintWidgetBackgroundGray(Widget widget)
        {
            widget.ModifyBase(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Insensitive]);
        }

        public static void PaintWidgetBackgroundWhite(Widget widget)
        {
            widget.ModifyBase(StateType.Normal,
                Widget.DefaultStyle.BaseColors[(int)StateType.Normal]);
        }
	}
}
