"""This module implements MainWindow."""

import gtk

from Panel import *

class MainWindow(gtk.Window):

    """MainWindow Implements the main window.
    """

    def __init__(self):
        """Construct UI."""
        gtk.Window.__init__(self)

        self.set_title('The Ultimate Commander')
        self.set_default_size(400, 700)
        self.connect('destroy', lambda widget: gtk.main_quit())

        panel = Panel()
        self.add(panel)
