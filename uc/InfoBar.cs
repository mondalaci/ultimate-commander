using System;
using Gtk;
using Glade;

namespace UltimateCommander {

	enum InfoType {
		Notice,
		Warning,
		Error
	}

	public class InfoBar: GladeWidget {

		public static void Notice(string notice)
		{
			MainWindow.InfoBar.PrintInfo(InfoType.Notice, notice);
		}

		public static void Warning(string warning)
		{
			MainWindow.InfoBar.PrintInfo(InfoType.Warning, warning);
		}

		public static void Error(string error)
		{
			MainWindow.InfoBar.PrintInfo(InfoType.Error, error);
		}

		[Glade.Widget] Image icon;
		[Glade.Widget] Label label;

		public InfoBar(): base("infobar_window")
		{
		}

		void PrintInfo(InfoType type, string info)
		{
			string icon_name = "gtk-info";

			switch (type) {
			case InfoType.Warning:
				icon_name = "gtk-dialog-warning";
				break;
			case InfoType.Error:
				icon_name = "gtk-dialog-error";
				break;
			}

			icon.Pixbuf = Util.LoadIcon(icon_name);
			label.Text = info;
		}
	
		void OnForwardHistoryButtonClicked(object sender, EventArgs args)
		{
		}

		void OnBackwardHistoryButtonClicked(object sender, EventArgs args)
		{
		}

		void OnExpanderButtonClicked(object sender, EventArgs args)
		{
		}
	}
}