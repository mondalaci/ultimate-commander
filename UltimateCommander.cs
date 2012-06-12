using System;
using Gnome.Vfs;

namespace UltimateCommander {

    public class UltimateCommander {

        public static void Main(string[] args)
        {              
            Config.Initialize();
            Gtk.Application.Init();
            new MainWindow();
            Gtk.Application.Run();
        }
    }
}
