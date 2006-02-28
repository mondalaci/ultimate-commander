#!/bin/sh

SOURCES = File.cs Panel.cs UltimateCommander.cs MainWindow.cs Slot.cs GladeWidget.cs GladeContainer.cs PanelConfigurator.cs PanelListingConfigurator.cs PanelSortingConfigurator.cs Frame.cs View.cs PanelFrame.cs AttributeIcons.cs PanelColumn.cs PanelColumnInfo.cs

UltimateCommander.exe: $(SOURCES)
	mcs -debug -nowarn:0169 -pkg:gtk-sharp-2.0 -pkg:gnome-sharp-2.0 -pkg:glade-sharp-2.0 -pkg:gmime-sharp -r:Mono.Posix -out:UltimateCommander.exe $(SOURCES)

clean:
	rm -f UltimateCommander.exe UltimateCommander.exe.mdb
	