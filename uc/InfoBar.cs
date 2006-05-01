using System;
using Gdk;
using Gtk;
using Glade;

namespace UltimateCommander {

    enum InfoType {
        Notice,
        Warning,
        Error
    }

    public class InfoBar: GladeWidget {

        [Glade.Widget] Gtk.Image icon;
        [Glade.Widget] TextView message;

        Pixbuf error;
        Pixbuf warning;
        Pixbuf notice;

        InfoType type;
        uint timer;
        uint timeout_handler;
        bool timeout_running = false;
        
        public InfoBar(): base("infobar_widget")
        {
            error = Util.LoadGtkIcon("gtk-dialog-error");
            warning = Util.LoadGtkIcon("gtk-dialog-warning");
            notice = Util.LoadGtkIcon("gtk-info");

            Util.SetWidgetBaseColorInsensitive(message);
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

        void PrintInfo(InfoType type_arg, string info)
        {
            type = type_arg;
            message.Buffer.Text = info;

            switch (type) {
            case InfoType.Error:
                icon.Pixbuf = error;
                break;
            case InfoType.Warning:
                icon.Pixbuf = warning;
                break;
            default:  // InfoType.Notice
                icon.Pixbuf = notice;
                break;
            }

            if (timeout_running) {
                timeout_running = false;
                GLib.Source.Remove(timeout_handler);
            }
            
            timeout_running = true;
            timer = 0;
            timeout_handler = GLib.Timeout.Add(Config.FlashInterval, ProgressHandler);
        }

        bool ProgressHandler()
        {
            if (type == InfoType.Error || type == InfoType.Warning) {
                bool show = timer / Config.FlashInterval % 2 == 0;
                ShowBell(show);
            }

            timer += Config.FlashInterval;
            if (timer <= Config.WaitInterval) {
                return true;
            }

            // Terminate
            timeout_running = false;
            ShowBell(false);
            return false;
        }
        
        void ShowBell(bool show)
        {
            if (show) {
                if (type == InfoType.Error) {
                    message.ModifyBase(StateType.Normal, Config.ErrorColor);
                } else {
                    message.ModifyBase(StateType.Normal, Config.WarningColor);
                }
            } else {
                Util.SetWidgetBaseColorInsensitive(message);
            }
        }
    }
}
