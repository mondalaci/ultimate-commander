from gtk import *

import config

def get_color(color):
    'Returns a color object of the given color.'
    widget = Window()
    map = widget.get_colormap()
    color = map.alloc_color(color)
    return color

def init_colors():
    'Initializes the colors of all widgets.'
    global color_panel_background
    global color_panel_foreground
    global color_panel_tagged

    color_panel_background = get_color(config.gtk_panel_background)
    color_panel_foreground = get_color(config.gtk_panel_foreground)
    color_panel_tagged = get_color(config.gtk_panel_tagged)
