"""This module implements Widget."""

import gtk

class Widget:

    """Widget extends gtk.Widget by making it able to handle additional
    signals over the standard ones.
    """

    CALLBACK_FUNC = 0
    CALLBACK_DATA = 1

    def __init__(self):
        """Initialize class variables."""
        self.handlers = {}

    def connect(self, signal, handler, data=None):
        """Extend gtk.Widget.connect."""
        if self.handlers.has_key(signal):
            self.handlers[signal][self.CALLBACK_FUNC] = handler
            self.handlers[signal][self.CALLBACK_DATA] = data
        else:
            return gtk.Widget.connect(self, signal, handler)

    def emit(self, signal_name, signal_data=None):
        """Extend gtk.Widget.emit."""
        if self.handlers.has_key(signal_name):
            callback_func = self.handlers[signal_name][self.CALLBACK_FUNC]

            if callback_func:
                if signal_data != None:
                    callback_func(self, signal_data,
                                  self.handlers[signal_name][self.CALLBACK_DATA])
                else:
                    callback_func(self,
                                  self.handlers[signal_name][self.CALLBACK_DATA])
        else:
            return gtk.Widget.emit(signal_name)

    def add_signal(self, signal):
        """Register a new signal name."""
        self.handlers[signal] = [None, None]
