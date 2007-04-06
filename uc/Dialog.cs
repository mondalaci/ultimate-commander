using System;
using Gtk;

namespace UltimateCommander {

    public abstract class Dialog: View {

        public Dialog(string window_name, string title): base(window_name)
        {
            slot.Header.Justification = Justification.Center;
            Title = title;
            ExposeEvent += new ExposeEventHandler(OnExposeEvent);
            KeyPressEvent += new KeyPressEventHandler(OnKeyPressEvent);
        }

        protected void DestroyDialog()
        {
            MainWindow.RemoveDialog();
        }
        
        // abstract methods
        
        protected abstract bool OnOkButtonClicked();
        protected abstract void OnExpose();

        // signal handlers
        
        void OnOkButtonClicked(object sender, EventArgs args)
        {
            if (OnOkButtonClicked()) {
                DestroyDialog();
            }
        }

        void OnCancelButtonClicked(object sender, EventArgs args)
        {
            DestroyDialog();
        }

        void OnExposeEvent(object sender, ExposeEventArgs args) {
            OnExpose();
        }
        
        void OnKeyPressEvent(object sender, KeyPressEventArgs args)
        {
            Gdk.Key key = args.Event.Key;
            switch (key) {
            case Gdk.Key.Escape:
                DestroyDialog();
                break;
            }
        }
    }
}
