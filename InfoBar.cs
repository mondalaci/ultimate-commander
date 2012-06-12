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

    public class InfoBar {

        public static void Notice(string text, params object[] args)
        {
            MainWindow.PrintInfo(InfoType.Notice, text, args);
        }

        public static void Warning(string text, params object[] args)
        {
            MainWindow.PrintInfo(InfoType.Warning, text, args);
        }

        public static void Error(string text, params object[] args)
        {
            MainWindow.PrintInfo(InfoType.Error, text, args);
        }
    }
}
