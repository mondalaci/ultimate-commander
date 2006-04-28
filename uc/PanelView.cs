using System;
using Gdk;
using Gtk;

namespace UltimateCommander {

    public class PanelView: TreeView {

        bool button3_pressed = false;
        int prev_row_num;
        int number_of_files = 0;
        Panel panel;

        public PanelView(Panel panel_arg): base()
        {
            panel = panel_arg;
            Model = new ListStore(typeof(File)); 
            KeyPressEvent += new KeyPressEventHandler(OnKeyPressEvent);
            ButtonPressEvent += new ButtonPressEventHandler(OnButtonPressEvent);
            ButtonReleaseEvent += new ButtonReleaseEventHandler(OnButtonReleaseEvent);
            MotionNotifyEvent += new MotionNotifyEventHandler(OnMotionNotifyEvent);
        }

        public int NumberOfFiles {
            set { number_of_files = value; }
        }

        int GetRowNumFromCoords(double x, double y)
        {
            TreePath path;
            GetPathAtPos((int)x, (int)y, out path);

            if (path == null) {
                return y < 0 ? 0 : number_of_files - 1;
            } else {
                return path.Indices[0];
            }
        }

        void InvertRow(int row_num)
        {
            TreeIter iter;
            int[] path_array = {row_num};
            TreePath path = new TreePath(path_array);
            Model.GetIter(out iter, path);
            File file = (File)Model.GetValue(iter, 0);
            file.InvertSelection();
        }

        void OnKeyPressEvent(object sender, KeyPressEventArgs args)
        {
            Gdk.Key key = args.Event.Key; 
            if (key == Gdk.Key.Tab) {
                panel.OtherPanel.Select();
                args.RetVal = true;
            }
        }

        [GLib.ConnectBefore]
        void OnButtonPressEvent(object sender, ButtonPressEventArgs args)
        {
            if (args.Event.Button == 3) {
                button3_pressed = true;
                prev_row_num = GetRowNumFromCoords(args.Event.X, args.Event.Y);
                InvertRow(prev_row_num);
            }
        }

        void OnButtonReleaseEvent(object sender, ButtonReleaseEventArgs args)
        {
            if (args.Event.Button == 3) {
                button3_pressed = false;
            }
        }

        [GLib.ConnectBefore]
        void OnMotionNotifyEvent(object sender, MotionNotifyEventArgs args)
        {
            int row_num = GetRowNumFromCoords(args.Event.X, args.Event.Y);

            if (row_num >= number_of_files - 1) {
                row_num = number_of_files -1;
            }

            if (!(button3_pressed && row_num != prev_row_num)) {
                return;
            }

            int start = prev_row_num;
            int end = row_num;

            if (prev_row_num < row_num) {
                start++;
            } else {
                start--;
            }
            
            if (start > end) {
                int tmp = start;
                start = end;
                end = tmp;
            }
                
            for (int row=start; row<=end; row++) {
                InvertRow(row);
                int[] path_array = {row};
                TreeIter iter;
                TreePath path = new TreePath(path_array);
                Model.GetIter(out iter, path);
                Model.EmitRowChanged(path, iter);
            }
            
            prev_row_num = row_num;

            int[] path_array2 = {end};
            SetCursor(new TreePath(path_array2), GetColumn(1), false);
        }
    }
}
