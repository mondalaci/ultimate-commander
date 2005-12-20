#!/bin/sh

SOURCES = ColumnType.cs File.cs Panel.cs UltimateCommander.cs

UltimateCommander.exe: $(SOURCES)
	mcs -debug -pkg:gtk-sharp-2.0 -pkg:gnome-sharp-2.0 -pkg:gmime-sharp -r:Mono.Posix -out:UltimateCommander.exe $(SOURCES)

clean:
	rm -f UltimateCommander.exe UltimateCommander.exe.mdb
	