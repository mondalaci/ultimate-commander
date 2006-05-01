using System;
using Gdk;
using Gtk;

namespace UltimateCommander {

    public class AttributeIcons {

        static int icons_num = 49;
        static int icon_size = 16;
        static int square_size = 7;

        static Gdk.Color black_color = new Gdk.Color(0, 0, 0);
        static Gdk.Color lightgray_color = new Gdk.Color(192, 192, 192);
        static Gdk.Color white_color = new Gdk.Color(255, 255, 255);

        Gdk.Pixbuf[] icons = new Gdk.Pixbuf[icons_num];

        public AttributeIcons()
        {
            Gdk.Pixbuf green_square = GetSquare(0, 192, 0);
            Gdk.Pixbuf blue_square = GetSquare(0, 128, 192);
            Gdk.Pixbuf orange_square = GetSquare(255, 128, 0);
            Gdk.Pixbuf red_square = GetSquare(255, 0, 0);
            Gdk.Pixbuf black_square = GetSquare(0, 0, 0);
            
            // Construct regular icons.

            for (int i=0; i<icons_num-1; i++) {
                bool executable = (i & 1) > 0;
                bool validsymlink = (i & 2) > 0;
                bool nowrite = (i & 4) > 0;
                FileReadability readability = ValueToFileReadability((i & 24) >> 3); 
                
                Gdk.Pixbuf icon = GetBaseIcon();
                icon = icon.AddAlpha(true, 255, 255, 255);

                if (executable) {
                    green_square.CopyArea(0, 0, square_size, square_size, icon, 0, 0);
                }
                if (validsymlink) {
                    blue_square.CopyArea(0, 0, square_size, square_size, icon, square_size+1, 0);
                }
                if (nowrite) {
                    red_square.CopyArea(0, 0, square_size, square_size,
                                        icon, square_size+1, square_size+1);
                }
                switch (readability) {
                case FileReadability.NotReadable:
                    red_square.CopyArea(0, 0, square_size, square_size, icon, 0, square_size+1);
                    break;
                case FileReadability.OnlySearchableDirectory:
                    orange_square.CopyArea(0, 0, square_size, square_size, icon, 0, square_size+1);
                    break;                              
                }
                
                icons[i] = icon;
            }

            // Construct dangling link icon.

            Gdk.Pixbuf dangling_icon = GetBaseIcon();
            dangling_icon = dangling_icon.AddAlpha(true, 255, 255, 255);
            black_square.CopyArea(0, 0, square_size, square_size, dangling_icon, 0, 0);
            blue_square.CopyArea(0, 0, square_size, square_size, dangling_icon, square_size+1, 0);
            black_square.CopyArea(0, 0, square_size, square_size, dangling_icon, 0, square_size+1);
            black_square.CopyArea(0, 0, square_size, square_size,
                                  dangling_icon, square_size+1, square_size+1);
            icons[icons_num-1] = dangling_icon;
        }

        public Gdk.Pixbuf GetIcon(bool executable, bool nowrite,
                                  FileReadability readability, SymbolicLinkType linktype)
        {
            if (linktype == SymbolicLinkType.DanglingLink) {
                return icons[icons_num-1];
            }
            
            int idx = (executable ? 1 : 0) +
                      (linktype == SymbolicLinkType.ValidLink ? 2 : 0) +
                      (nowrite ? 4 : 0) +
                      (FileReadabilityToValue(readability) << 3);

            return icons[idx];
        }

        int FileReadabilityToValue(FileReadability readability)
        {
            switch (readability) {
            case FileReadability.NotReadable:
                return 0;
            case FileReadability.OnlySearchableDirectory:
                return 1;
            default:  // FileReadability.Readable
                return 2;
            }
        }

        FileReadability ValueToFileReadability(int value)
        {
            switch (value) {
            case 0:
                return FileReadability.NotReadable;
            case 1:
                return FileReadability.OnlySearchableDirectory;
            default:  // 2
                return FileReadability.Readable;
            }
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

            gc.RgbFgColor = white_color;
            pixmap.DrawRectangle(gc, true, 0, 0, icon_size, icon_size);
            gc.RgbFgColor = lightgray_color;
            pixmap.DrawRectangle(gc, false, 0, 0, square_size-1, square_size-1);
            pixmap.DrawRectangle(gc, false, 0, square_size+1, square_size-1, square_size-1);
            pixmap.DrawRectangle(gc, false, square_size+1, 0, square_size-1, square_size-1);
            pixmap.DrawRectangle(gc, false, square_size+1,
                                 square_size+1, square_size-1, square_size-1);

            return Gdk.Pixbuf.FromDrawable(pixmap, pixmap.Colormap,
                                           0, 0, 0, 0, icon_size, icon_size);
        }

        static Gdk.Pixbuf GetSquare(byte red, byte green, byte blue)
        {
            Gdk.GC gc;
            Gdk.Pixmap pixmap;
            GetDrawableAndGC(square_size, square_size, out gc, out pixmap);

            gc.RgbFgColor = black_color;
            pixmap.DrawRectangle(gc, true, 0, 0, square_size, square_size);
            gc.RgbFgColor = new Gdk.Color(red, green, blue);
            pixmap.DrawRectangle(gc, true, 1, 1, square_size-2, square_size-2);

            return Gdk.Pixbuf.FromDrawable(pixmap, pixmap.Colormap,
                                           0, 0, 0, 0, square_size, square_size);
        }
    }
}
