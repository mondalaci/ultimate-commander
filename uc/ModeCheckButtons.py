"""This module implements ModeCheckButtons."""

import gtk

from Widget import *

class ModeCheckButtons(Widget, gtk.VBox):

    """ModeCheckButtons widget is designed to set Unix file premission
    bits with checkboxes.

    Provided signals:
    mode_changed
    """

    def __init__(self, mode):
        """Construct UI."""
        checkbutton_labels = [
            'Set User ID on execution',
            'Set Group ID on execution',
            'Sticky bit',
            'Read by owner',
            'Write by owner',
            'Execute/search by owner',
            'Read by group',
            'Write by group',
            'Execute/search by group',
            'Read by others',
            'Write by others',
            'Execute/search by others'
            ]

        Widget.__init__(self)
        self.add_signal('mode_changed')
        gtk.VBox.__init__(self)
        self.checkbuttons = []

        for checkbutton_label in checkbutton_labels:
            checkbutton = gtk.CheckButton(checkbutton_label)
            checkbutton.connect('toggled', self._on_toggled)
            self.checkbuttons.insert(0, checkbutton)
            self.pack_start(checkbutton)

        self.set_mode(mode)

    def _on_toggled(self, widget, data=None):
        self.emit('mode_changed', self.get_mode())

    def set_mode(self, mode):
        """Set mode."""
        for i in range(0, len(self.checkbuttons)):
            checkbutton = self.checkbuttons[i]
            active = mode & (2**i)

            if active:
                checkbutton.set_active(gtk.TRUE)
            else:
                checkbutton.set_active(gtk.FALSE)

    def get_mode(self):
        """Return the mode."""
        mode = 0

        for i in range(0, len(self.checkbuttons)):
            if self.checkbuttons[i].get_active():
                mode = mode + 2**i

        return mode
