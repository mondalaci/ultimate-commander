using System;
using Gdk;
using Gtk;
using Glade;

namespace UltimateCommander {

    public class CreateDirectoryWidget: GladeWidget {

        [Glade.Widget] TextView textview;
        Panel panel;
        
        public CreateDirectoryWidget(Panel panel_arg): base("create_directory_widget")
        {
            panel = panel_arg;
            Util.ModifyWidgetBg(topwidget, StateType.Active);
            textview.KeyPressEvent += new KeyPressEventHandler
                                      (panel.OnCreateDirectoryWidgetKeyPressEvent);
        }

        public TextView TextView {
            get { return textview; }
        }
        
        void OnOkButtonClicked(object sender, EventArgs args)
        {
            panel.DoCreateDirectory();
        }

        void OnCancelButtonClicked(object sender, EventArgs args)
        {
            panel.CancelCreateDirectory();
        }

        [GLib.ConnectBefore]
        void OnButtonPressEvent(object sender, ButtonPressEventArgs args)
        {
            panel.Select();
        }
    }
}
