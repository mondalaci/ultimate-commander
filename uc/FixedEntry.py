"""This module implements FixedEntry."""

import gtk

from Widget import *

class FixedEntry(Widget, gtk.Entry):

    """FixedEntry is a widget designed to overwrite the characters of a
    fixed size string.

    This class is provided to be subclassed.

    Provided signals:
    overwrite_character

    Methods to extend:
    on_input_char

    """

    def __init__(self, string):
        """Initialize and extend ancestors."""
        Widget.__init__(self)
        gtk.Entry.__init__(self)

        self.text = string
        self.position = 0
        self.step_char = 0
        self.focus_changed = 0

        self.add_signal('overwrite_character')
        self.connect('focus', self._on_focus)
        self.connect('key_press_event', self._on_key_press_event)
        self.connect('expose_event', self._on_expose_event)

    def _on_focus(self, widget, event):
        """Handle focus signal."""
        self.focus_changed = 1

    def _on_key_press_event(self, widget, event):
        """Handle key_press_event signal."""
        text = self.text
        length = len(text)

        pos = self.get_position()
        if self.position == 0:  # position value correction
            self.position = pos
        else:
            pos = self.position

        if event.string:  # printable input received
            char = self.on_input_char(event.string, pos)
            if char:
                self.step_char = 1
                if pos <= length:
                    self.text = text[0:pos-1] + char + text[pos:length]
                self.emit('overwrite_character')

        if event.keyval == gtk.keysyms.Right:
            self.set_position(pos)

    def _on_expose_event(self, widget, event):
        """Handle expose_event signal."""
        pos = self.get_position()
        self.position = 0

        if self.get_text() != self.text:
            self.set_text(self.text)
        if self.focus_changed:
            pos = 1
            self.focus_changed = 0
        if self.step_char:
            pos = pos + 1
            self.step_char = 0
        if pos == 0:
            self.set_position(1)
            return

        self.select_region(pos-1,pos)

    def on_input_char(self, char, pos):
        """Input handler provided to be overriden.

        Keyword arguments:
        char -- the pressed character
        pos -- the current cursor position

        Return value:
        The desired character in the current position or empty string if
        unchanged.

        Raised exceptions:
        InputError explaining why the given input is incorrect in the
        current position.

        """

        return char
