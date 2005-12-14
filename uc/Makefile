#!/bin/sh

SOURCES = ColumnType.cs File.cs Panel.cs UltimateCommander.cs

UltimateCommander.exe: $(SOURCES)
	mcs -debug -pkg:gtk-sharp-2.0 -pkg:gnome-sharp-2.0 -pkg:gmime-sharp -r:/usr/lib/mono/gtk-sharp-2.0/gnome-vfs-sharp.dll -r:Mono.Posix -out:UltimateCommander.exe $(SOURCES)
	