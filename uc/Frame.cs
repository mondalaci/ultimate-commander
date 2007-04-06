using System;
using System.Collections;
using Gtk;

namespace UltimateCommander {

    public class Frame: Notebook {

        static ArrayList frames = new ArrayList();

        public Frame(): base()
        {
            frames.Add(this);
            SwitchPage += new SwitchPageHandler(OnSwitchPage);
            ButtonPressEvent += new ButtonPressEventHandler(OnButtonPressEvent);
        }

        ~Frame()
        {
            frames.Remove(this);
        }

        // Public members
        
        public bool Selected {
            get { return MainWindow.ActiveFrame == this; }
        }

        public void Select()
        {
            MainWindow.ActiveFrame = this;
            CurrentView.OnSelect();
            
            foreach (Frame frame in frames) {
                frame.Redraw();
            }
        }

        public void Redraw()
        {
            if (NPages > 0) {
                CurrentView.Slot.Redraw();
            }
        }

        public void AppendView(View view, string text)
        {
            view.Slot.Frame = this;
            Label label = new Label(text);
            AppendPage(view.Slot, label);
            RefreshTabs();
        }

        public void SelectView(View view)
        {
            Slot slot = view.Slot;
            Page = PageNum(slot);
            slot.Select();
        }

        public void RemoveView(View view)
        {
            RemovePage(PageNum(view.Slot));
            RefreshTabs();
        }

        public void ClearViews()
        {
            for (int i=0; i<NPages; i++) {
                RemovePage(0);
            }
        }
        
        public View CurrentView {
            get { return ((Slot)GetNthPage(CurrentPage)).View; }
        }

        // Private members
        
        void RefreshTabs()
        {
            ShowTabs = NPages > 1 ? true : false;
        }

        // Signal handlers
        
        [GLib.ConnectBefore]
        void OnSwitchPage(object sender, SwitchPageArgs args)
        {
            int current_page = (int)args.PageNum;
            Slot slot = (Slot)GetNthPage(current_page);
            slot.Select();
        }

        [GLib.ConnectBefore]
        void OnButtonPressEvent(object sender, ButtonPressEventArgs args)
        {
            CurrentView.Select();
        }
    }
}
