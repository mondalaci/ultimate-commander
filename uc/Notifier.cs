using System;
using Gtk;
using Glade;

namespace UltimateCommander {

    public abstract class Notifier: GladeWidget {
        
        [Glade.Widget] protected TextView textview;
        [Glade.Widget] protected Image icon;

        protected TextTag bold_tag;
        protected TextTag red_tag;
        
        Panel panel;
        
        public Notifier(Panel panel_arg): base("notifier_widget")
        {
            panel = panel_arg;
            red_tag = new TextTag("red");
            red_tag.ForegroundGdk = new Gdk.Color(255, 0, 0);
            textview.Buffer.TagTable.Add(red_tag);
            bold_tag = new TextTag("bold");
            bold_tag.Weight = Pango.Weight.Bold;
            textview.Buffer.TagTable.Add(bold_tag);
            Util.ModifyWidgetBase(textview, StateType.Insensitive);
        }

        protected void AppendText(string text, params TextTag[] tags)
        {
            TextIter iter = textview.Buffer.EndIter;
            textview.Buffer.InsertWithTags(ref iter, text, tags);
        }

        protected void SetIcon(string icon_name)
        {
            icon.Pixbuf = icon.RenderIcon(icon_name, IconSize.SmallToolbar, "");
        }
        
        [GLib.ConnectBefore]
        void OnButtonPressEvent(object sender, ButtonPressEventArgs args)
        {
            panel.Select();
        }
    }
}
