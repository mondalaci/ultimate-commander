"""This module implements Sub."""

import gtk

from Widget import *

class Sub(Widget, gtk.VBox):

    """
    Sub widget provides a nice way to hierarchially organize widgets.
    """

    def __init__(self, name, sub_widget):
        """Construct UI."""
        Widget.__init__(self)
        gtk.VBox.__init__(self)

        label = gtk.Label(name)
        label.set_alignment(0,0.5)

        empty_label = gtk.Label()
        empty_label.set_size_request(20,-1)

        hbox = gtk.HBox()
        hbox.pack_start(empty_label)
        hbox.pack_start(sub_widget)
        
        self.pack_start(label)
        self.pack_start(hbox)
