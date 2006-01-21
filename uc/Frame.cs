using System;
using Gtk;

namespace UltimateCommander {

	public class Frame: Notebook {

		Frame(): base()
		{
			this.SwitchPage += new SwitchPageHandler(OnSwitchPage);
			this.ButtonPressEvent += new ButtonPressEventHandler(OnButtonPressEvent);
		}

		public void AppendView(View view, string text)
		{
			view.Slot.Frame = this;
			Label label = new Label(text);
			AppendPage(view.Slot, label);
			RefreshTabs();
		}

		public void RemoveView(View view)
		{
			RemovePage(PageNum(view.Slot));
			RefreshTabs();
		}

		void RefreshTabs()
		{
			if (NPages > 1)
				ShowTabs = true;
			else
				ShowTabs = false;
		}

		[GLib.ConnectBefore]
		void OnSwitchPage(object o, SwitchPageArgs args)
		{
			int current_page = (int)args.PageNum;
			Slot slot = (Slot)GetNthPage(current_page);
			slot.Select();
		}

		[GLib.ConnectBefore]
		void OnButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			Slot slot = (Slot)GetNthPage(CurrentPage);
			slot.Select();
		}
	}
}
