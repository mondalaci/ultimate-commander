"""This module implements OctalModeEntry."""

#import string

from FixedEntry import *
#from InputError import *

class OctalModeEntry(FixedEntry):

    """OctalModeEntry widget is designed to edit the Unix octal file
    mode string.
    """

    def __init__(self, mode):
        """Fill up the mode field."""
        FixedEntry.__init__(self, '0000')
        self.set_mode(mode)
        self.add_signal('mode_changed')
        self.connect('overwrite_character', self._on_mode_changed)

    def _on_mode_changed(self, widget, data=None):
        self.emit('mode_changed', self.get_mode())

    def on_input_char(self, char, pos):
        """Override FixedEntry.on_input_char."""
        try:
            num = int(char)
            if num > 7:
                raise ValueError
        except:
            print "'" + char + "' is not an octal numeral."
            return ''

        return char

    def set_mode(self, mode):
        """Set mode."""
        string = '%o' % mode
        string = '0' * (4 - len(string)) + string
        self.text = string
        gtk.Widget.emit(self, 'state_changed', gtk.TRUE)

    def get_mode(self):
        """Return the mode."""
        sum = 0
        mul = 1

        for i in range(3, -1, -1):
            sum = sum + int(self.text[i]) * mul
            mul = mul * 8
        return sum
