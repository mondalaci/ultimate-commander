import gtk

import config

def init():
    '(Re)Initializes the colors of all widgets.'
    global panel_background
    global panel_foreground
    global panel_tagged

    global directory
    global executable
    global stalled_link
    global regular_file
    
    panel_background = get_color(config.color_panel_background)
    panel_foreground = get_color(config.color_panel_foreground)
    panel_tagged = get_color(config.color_panel_tagged)

    directory = get_color(config.color_directory)
    executable = get_color(config.color_executable)
    stalled_link = get_color(config.color_stalled_link)
    regular_file = get_color(config.color_regular_file)

def get_color(color):
    'Returns a color object of the given color.'
    widget = gtk.Window()
    map = widget.get_colormap()
    color = map.alloc_color(color)
    return color
