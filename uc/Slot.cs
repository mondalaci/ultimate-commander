using System;
using System.Collections;
using Gdk;
using Gtk;

namespace UltimateCommander {

    public class Slot: GladeWidget {

        [Glade.Widget] TextView header;
        [Glade.Widget] EventBox view_slot;

        Frame frame;
        View view;
        TextTag white_tag;
        
        public Slot(View view_arg): base("slot_widget")
        {
            view = view_arg;
            view_slot.Add(view);
            view.Slot = this;
        
            white_tag = new TextTag("white");
            white_tag.ForegroundGdk = Widget.DefaultStyle.White;
            header.Buffer.TagTable.Add(white_tag);
        }

        public void Select()
        {
            view.Select();
        }

        public string Title {
            get { return header.Buffer.Text; }
            set {
                if (this != null) {
                    header.Buffer.Text = value;
                    Redraw();
                }
            }
        }

        public Frame Frame {
            get { return frame; }
            set { frame = value; }
        }

        public View View {
            get { return view; }
        }

        public void Redraw()
        {
            TextBuffer buffer = header.Buffer;
            buffer.RemoveAllTags(buffer.StartIter, buffer.EndIter);
            
            if (frame != null && frame.Selected) {
                buffer.ApplyTag(white_tag, buffer.StartIter, buffer.EndIter);
                Util.ModifyWidgetBase(header, StateType.Selected);
                Util.ModifyWidgetBg(topwidget, StateType.Selected);
            } else {
                Util.ModifyWidgetBase(header, StateType.Insensitive);
                Util.ModifyWidgetBg(topwidget, StateType.Insensitive);
            }
        }

        [GLib.ConnectBefore]
        void OnButtonPressEvent(object sender, ButtonPressEventArgs args)
        {
            Select();
            args.RetVal = true;
        }
    }
}
