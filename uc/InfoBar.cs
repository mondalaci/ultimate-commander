using System;
using Gtk;
using Glade;

namespace UltimateCommander {

    enum InfoType {
        Notice,
        Warning,
        Error
    }

    public class InfoBar: GladeWidget {

        [Glade.Widget] Image icon;
        [Glade.Widget] Label label;

        public InfoBar(): base("infobar_widget")
        {
        }

        public static void Notice(string notice)
        {
            MainWindow.InfoBar.PrintInfo(InfoType.Notice, notice);
        }

        public static void Warning(string warning)
        {
            MainWindow.InfoBar.PrintInfo(InfoType.Warning, warning);
        }

        public static void Error(string error)
        {
            MainWindow.InfoBar.PrintInfo(InfoType.Error, error);
        }

        void PrintInfo(InfoType type, string info)
        {
            switch (type) {
            case InfoType.Warning:
                icon.Pixbuf = Util.LoadGtkIcon("gtk-dialog-warning");
                break;
            case InfoType.Error:
                icon.Pixbuf = Util.LoadGtkIcon("gtk-dialog-error");
                break;
            default:  // InfoType.Notice
                icon.Pixbuf = Util.LoadGtkIcon("gtk-info");
                break;
            }

            label.Text = info;
        }
    }
}
