from gtk import *

def get_color(color):
    'Returns a color object of the given color.'
    widget = Window()
    map = widget.get_colormap()
    color = map.alloc_color(color)
    return color
