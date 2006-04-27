using System;
using Mono.Unix;

namespace UltimateCommander {

	public static class Config {
        
        public static WhichPanel ActivePanel = WhichPanel.LeftPanel;

        // Path related attributes

		public static string HomePath;
        public static string InitialPath = ".";
		public static string GuiPath;
		public static string GladeFileName;

        // Visual attributes

		public static int IconSize = 16;
		public static Gdk.Color SelectedFileBgColor = new Gdk.Color(224, 224, 0);
		public static Gdk.Color InvalidFileNameEncodingColor = new Gdk.Color(255, 0, 0);
		public static string ActiveSlotHeaderColorString = "#ffffff";
		public static string InactiveSlotHeaderColorString = "#000000";
		

        public static void Initialize()
        {
			HomePath = Environment.GetEnvironmentVariable("HOME");
			GuiPath = Environment.GetEnvironmentVariable("UC_GUI_PATH");

			if (GuiPath == null) {
				GuiPath = "gui";
			}

			GladeFileName = UnixPath.Combine(GuiPath, "uc.glade");
        }
	}
}
