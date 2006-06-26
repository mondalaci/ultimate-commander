using System;
using Gdk;
using Gtk;

namespace UltimateCommander {

    [Flags]
    public enum FileAttribute {
        NormalExecutable = 1,
        SetUidExecutable = 2,
        SetGidExecutable = 4,
        ValidSymlink = 8,
        Readable = 16,
        OnlySearchableDirectory = 32,
        Writable = 64,
        DanglingSymlink = 128,
    }
    
    public class AttributeIcons {

        static int icons_num = (int)FileAttribute.DanglingSymlink + 1;
        static int icon_size = 16;
        static int square_size = 7;

        static Gdk.Color black_color = new Gdk.Color(0, 0, 0);
        static Gdk.Color lightgray_color = new Gdk.Color(192, 192, 192);
        static Gdk.Color white_color = new Gdk.Color(255, 255, 255);

        Pixbuf[] icons = new Gdk.Pixbuf[icons_num];

        Pixbuf green_square;
        Pixbuf light_red_square;
        Pixbuf light_orange_square;
        Pixbuf yellow_square;
        Pixbuf blue_square;
        Pixbuf orange_square;
        Pixbuf red_square;
        Pixbuf black_square;
        
        public AttributeIcons()
        {
            green_square = CreateSquare(0, 192, 0);
            light_red_square = CreateSquare(255, 128, 128);
            light_orange_square = CreateSquare(255, 192, 0);
            yellow_square = CreateSquare(255, 255, 0);
            blue_square = CreateSquare(0, 128, 192);
            orange_square = CreateSquare(255, 128, 0);
            red_square = CreateSquare(255, 0, 0);
            black_square = CreateSquare(0, 0, 0);
        }

        public Gdk.Pixbuf GetIcon(FileAttribute attributes) {
            int index = (int)attributes;
            if (icons[index] == null) {
                icons[index] = CreateIcon(attributes);
            }
            return icons[index];
        }
        
        public Gdk.Pixbuf CreateIcon(FileAttribute attributes)
        {
            if (attributes == FileAttribute.DanglingSymlink) {
                return ConstructIcon(black_square, blue_square, black_square, black_square);
            }
            
            Pixbuf executable_square;
            if (((attributes & FileAttribute.SetUidExecutable) != 0) &&
                ((attributes & FileAttribute.SetGidExecutable) != 0)) {
                executable_square = yellow_square;
            } else if ((attributes & FileAttribute.SetUidExecutable) != 0) {
                executable_square = light_red_square;
            } else if ((attributes & FileAttribute.SetGidExecutable) != 0) {
                executable_square = light_orange_square;
            } else if ((attributes & FileAttribute.NormalExecutable) != 0) {
                executable_square = green_square;
            } else {
                executable_square = null;
            }
            
            Pixbuf symlink_square = (attributes & FileAttribute.ValidSymlink) != 0 ?
                                    blue_square : null;
            
            
            Pixbuf readable_square;
            if ((attributes & FileAttribute.Readable) != 0) {
                readable_square = null;
            } else if ((attributes & FileAttribute.OnlySearchableDirectory) != 0) {
                readable_square = orange_square;
            } else {
                readable_square = red_square;
            }
            
            Pixbuf writable_square = (attributes & FileAttribute.Writable) != 0 ?
                                     null : red_square;
            
            return ConstructIcon(executable_square, symlink_square,
                                 readable_square, writable_square);
        }

        static Pixbuf ConstructIcon(Pixbuf upper_left_square, Pixbuf upper_right_square,
                                    Pixbuf bottom_left_square, Pixbuf bottom_right_square)
        {
            Pixbuf icon = CreateBaseIcon();
            icon = icon.AddAlpha(true, 255, 255, 255);
            
            if (upper_left_square != null) {
                upper_left_square.CopyArea(0, 0, square_size, square_size, icon, 0, 0);
            }
            
            if (upper_right_square != null) {
                upper_right_square.CopyArea(0, 0, square_size, square_size, icon, square_size+1, 0);
            }
            
            if (bottom_left_square != null) {
                bottom_left_square.CopyArea(0, 0, square_size, square_size, icon, 0, square_size+1);
            }
            
            if (bottom_right_square != null) {
                bottom_right_square.CopyArea(0, 0, square_size, square_size,
                                         icon, square_size+1, square_size+1);
            }
            
            return icon;
        }

        static Pixbuf CreateBaseIcon()
        {

            Gdk.GC gc;
            Pixmap pixmap;
            GetDrawableAndGC(icon_size, icon_size, out gc, out pixmap);

            gc.RgbFgColor = white_color;
            pixmap.DrawRectangle(gc, true, 0, 0, icon_size, icon_size);
            gc.RgbFgColor = lightgray_color;
            pixmap.DrawRectangle(gc, false, 0, 0, square_size-1, square_size-1);
            pixmap.DrawRectangle(gc, false, 0, square_size+1, square_size-1, square_size-1);
            pixmap.DrawRectangle(gc, false, square_size+1, 0, square_size-1, square_size-1);
            pixmap.DrawRectangle(gc, false, square_size+1,
                                 square_size+1, square_size-1, square_size-1);

            return Pixbuf.FromDrawable(pixmap, pixmap.Colormap,
                                       0, 0, 0, 0, icon_size, icon_size);
        }

        static Pixbuf CreateSquare(byte red, byte green, byte blue)
        {
            Gdk.GC gc;
            Gdk.Pixmap pixmap;
            GetDrawableAndGC(square_size, square_size, out gc, out pixmap);

            gc.RgbFgColor = black_color;
            pixmap.DrawRectangle(gc, true, 0, 0, square_size, square_size);
            gc.RgbFgColor = new Gdk.Color(red, green, blue);
            pixmap.DrawRectangle(gc, true, 1, 1, square_size-2, square_size-2);

            return Pixbuf.FromDrawable(pixmap, pixmap.Colormap,
                                           0, 0, 0, 0, square_size, square_size);
        }

        static void GetDrawableAndGC(int width, int height, out Gdk.GC gc, out Gdk.Pixmap pixmap)
        {
            Gtk.Window gtkwin = new Gtk.Window(Gtk.WindowType.Toplevel);
            gtkwin.Realize();
            Gdk.Window gdkwin = gtkwin.GdkWindow;

            gc = new Gdk.GC(gdkwin);
            pixmap = new Gdk.Pixmap(gdkwin, width, height);
        }
    }
}
