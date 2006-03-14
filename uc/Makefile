#!/bin/sh

SOURCES = *.cs

UltimateCommander.exe: $(SOURCES)
	mcs -debug -nowarn:0169 -pkg:gtk-sharp-2.0 -pkg:gnome-sharp-2.0 -pkg:glade-sharp-2.0 \
-r:Mono.Posix -out:UltimateCommander.exe $(SOURCES)

clean:
	rm -f UltimateCommander.exe UltimateCommander.exe.mdb
	