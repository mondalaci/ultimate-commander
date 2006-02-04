SOURCE_FILES = depot-video-thumbnailer.c bacon-video-widget-xine.c video-utils.c bacon-resize.c baconvideowidget-marshal.c
OUTPUT_FILE = depot-video-thumbnailer
DATADIR = /usr/share
GCONF_PREFIX = /apps/totem

$(OUTPUT_FILE): $(SOURCE_FILES)
	gcc $(SOURCE_FILES) \
	-DGCONF_PREFIX=\"$(GCONF_PREFIX)\" -DDATADIR=\"$(DATADIR)\" \
	`pkg-config --cflags libxine --libs libxine` \
	`pkg-config --cflags gtk+-2.0 --libs gtk+-2.0` \
	`pkg-config --cflags libgnomeui-2.0 --libs libgnomeui-2.0` \
	-o $(OUTPUT_FILE)

clean:
	rm -f $(OUTPUT_FILE)
	#-DGCONF_PREFIX=\"/apps/totem\" -DDATADIR=\"/usr/local/share\" \
    