using System;
using Gdk;
using Gtk;

namespace UltimateCommander {

	public class AttributeIcons {

		static int icon_size = 16;
		static int square_size = 7;

		Gdk.Pixbuf[] icons = new Gdk.Pixbuf[16];

		public AttributeIcons()
		{
			Gdk.Pixbuf executable_icon = GetSquare(0, 192, 0);
			Gdk.Pixbuf symlink_icon = GetSquare(0, 128, 192);
			Gdk.Pixbuf noread_icon = GetSquare(255, 128, 0);
			Gdk.Pixbuf nowrite_icon = GetSquare(255, 0, 0);
			
			for (int i=0; i<16; i++) {
				bool executable = (i & 1) > 0;
				bool symlink = (i & 2) > 0;
				bool nowrite = (i & 4) > 0;
				bool noread = (i & 8) > 0;
				
				Gdk.Pixbuf icon = GetBaseIcon();

				if (executable) executable_icon.CopyArea(0, 0, 7, 7, icon, 0, 0);
				if (symlink) symlink_icon.CopyArea(0, 0, 7, 7, icon, 8, 0);
				if (noread) noread_icon.CopyArea(0, 0, 7, 7, icon, 0, 8);
				if (nowrite) nowrite_icon.CopyArea(0, 0, 7, 7, icon, 8, 8);
				
				icons[i] = icon;
			}
		}

		public Gdk.Pixbuf GetIcon(bool executable, bool symlink, bool nowrite, bool noread)
		{
			int i = 0;
			if (executable) i += 1;
			if (symlink) i += 2;
			if (nowrite) i += 4;
			if (noread) i += 8;
			return icons[i];
		}

		static void GetDrawableAndGC(int width, int height, out Gdk.GC gc, out Gdk.Pixmap pixmap)
		{
			Gtk.Window gtkwin = new Gtk.Window(Gtk.WindowType.Toplevel);
			gtkwin.Realize();
			Gdk.Window gdkwin = gtkwin.GdkWindow;
			gc = new Gdk.GC(gdkwin);
			pixmap = new Gdk.Pixmap(gdkwin, width, height);
		}

		static Gdk.Pixbuf GetBaseIcon()
		{

			Gdk.GC gc;
			Gdk.Pixmap pixmap;
			GetDrawableAndGC(icon_size, icon_size, out gc, out pixmap);

			gc.RgbFgColor = new Gdk.Color(255, 255, 255);
			pixmap.DrawRectangle(gc, true, 0, 0, icon_size, icon_size);
			gc.RgbFgColor = new Gdk.Color(192, 192, 192);
			pixmap.DrawRectangle(gc, false, 0, 0, square_size-1, square_size-1);
			pixmap.DrawRectangle(gc, false, 0, square_size+1, square_size-1, square_size-1);
			pixmap.DrawRectangle(gc, false, square_size+1, 0, square_size-1, square_size-1);
			pixmap.DrawRectangle(gc, false, square_size+1, square_size+1, square_size-1, square_size-1);

			return Gdk.Pixbuf.FromDrawable(pixmap, pixmap.Colormap, 0, 0, 0, 0, icon_size, icon_size);
		}

		static Gdk.Pixbuf GetSquare(byte red, byte green, byte blue)
		{
			Gdk.GC gc;
			Gdk.Pixmap pixmap;
			GetDrawableAndGC(square_size, square_size, out gc, out pixmap);

			gc.RgbFgColor = new Gdk.Color(0, 0, 0);
			pixmap.DrawRectangle(gc, true, 0, 0, square_size, square_size);
			gc.RgbFgColor = new Gdk.Color(red, green, blue);
			pixmap.DrawRectangle(gc, true, 1, 1, square_size-2, square_size-2);

			return Gdk.Pixbuf.FromDrawable(pixmap, pixmap.Colormap, 0, 0, 0, 0, square_size, square_size);
		}
	}
}