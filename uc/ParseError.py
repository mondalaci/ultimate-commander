"""This module implements ParseError."""

class ParseError(Exception):
    """ParseError should be raised whenever any errors occured relating
     parsing.
    """

    def __init__(self, type, pos=-1):
        self.type = type
        self.pos = pos

    def __str__(self):
        if self.pos == -1:
            return self.type
        else:
            return "%s at position %i" % (self.type, self.pos)
