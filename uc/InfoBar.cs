using System;
using Gdk;
using Gtk;
using Glade;

namespace UltimateCommander {

    public enum InfoType {
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

            Util.ModifyWidgetBase(message, StateType.Insensitive);
        }

        public static void Notice(string text, params object[] args)
        {
            MainWindow.InfoBar.PrintInfo(InfoType.Notice, text, args);
        }

        public static void Warning(string text, params object[] args)
        {
            MainWindow.InfoBar.PrintInfo(InfoType.Warning, text, args);
        }

        public static void Error(string text, params object[] args)
        {
            MainWindow.InfoBar.PrintInfo(InfoType.Error, text, args);
        }

        public void PrintInfo(InfoType type_arg, string text, params object[] args)
        {
            type = type_arg;
            message.Buffer.Text = String.Format(text, args);

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
            
            ShowBell(true);
            timeout_running = true;
            timer = 0;
            timeout_handler = GLib.Timeout.Add(Config.FlashInterval, ProgressHandler);
        }

        bool ProgressHandler()
        {
            if (type == InfoType.Error || type == InfoType.Warning) {
                bool show = timer / Config.FlashInterval % 2 != 0;
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
                } else if (type == InfoType.Warning) {
                    message.ModifyBase(StateType.Normal, Config.WarningColor);
                }
            } else {
                Util.ModifyWidgetBase(message, StateType.Insensitive);
            }
        }
    }
}
