"""This module implements Panel."""

import gtk
import os
import time

import Colors
import config

from Panel import *
from File import *

class Panel(gtk.VBox):

    """The Panel which is one of the central widgets of UC.
    """

    def __init__(self):
        gtk.VBox.__init__(self)
        # initialize input flags
        self.buttons_pressed = [0, 0, 0]
        self.prev_active_row = 0

        # build widgets
        self.scrollwin = gtk.ScrolledWindow()
        self.scrollwin.set_policy(gtk.POLICY_AUTOMATIC, gtk.POLICY_AUTOMATIC)

        self.title = gtk.Label()
        self.title.set_line_wrap(gtk.TRUE)

        self.list = gtk.CList(1) # `set_column_hook' will replace this CList

        self.statusbar = gtk.Label()
        self.statusbar.set_line_wrap(gtk.TRUE)
        self.statusbar.set_alignment(xalign=0, yalign=0.5)

        # connect widgets
        self.scrollwin.add(self.list)
        self.pack_start(self.title, gtk.FALSE)
        self.pack_start(self.scrollwin)
        self.pack_start(self.statusbar, gtk.FALSE)

        #Panel.__init__(self)
        self.set_columns(config.panel_column_format)
        #self.set_statusbar(statusbar_format)
        #self.set_sort_order(sort_format)
        #self.set_filter(filter_format)
        self.chdir('~')


        self.set_colors()

    def __len__(self):
        return len(self.files) + self.have_updir()

# general methods

    def chdir(self, path='~'):
        'Change the current directory of the panel.'
        self.path = os.path.abspath(os.path.expanduser(path))

        if self.have_updir():
            path = self.path + '/'
        else:
            path = self.path

        self.files = [path + file for file in os.listdir(self.path)]

        self.chdir_hook()

    def set_columns(self, format=config.panel_column_format):
        'Sets the columns of the panel.'
        paren_stack = []
        format_pos_list = []
        prev_char = ' '

        # examine parenthesis structure, prepare tokenization
        for pos in range(len(format)):
            current_char = format[pos]
            if current_char == '(':
                paren_stack.append(pos)
            elif current_char == ')':
                try:
                    paren_stack.pop()
                except:
                    raise ParseError, ('parenthesis depth is below zero', pos)

            if len(paren_stack) == 0 and prev_char in string.whitespace\
            and current_char not in string.whitespace:
                format_pos_list.append(pos)

            prev_char = current_char

        if len(paren_stack) != 0:
            raise ParseError, ('parenthesis has no closing pair',
                               paren_stack.pop())

        format_pos_list.append(pos+1)
        prev_pos = format_pos_list[0]
        column_list = []
        column_names = []

        # tokenize and check the validity of column formats
        test_entry = File('/etc/fstab')

        for pos in format_pos_list[1:]:
            column = string.strip(format[prev_pos:pos])
            try:
                paren_pos = column.index('(')
                column_name = column[:paren_pos]
            except:
                column_name = column
                column = column + '()'

            printer = 'File.print_' + column_name
            try:
                eval(printer)
            except:
                raise ParseError, ("invalid column type: '" +
                                   column_name + "'", prev_pos)

            validator = 'test_entry.validate_' + column
            try:
                eval(validator)
            except AttributeError:
                pass
            except ParseError, error:
                raise ParseError, (error.type + " in '" + column_name + "' type", prev_pos + paren_pos)

            printer_with_args = 'test_entry.print_' + column
            try:
                eval(printer_with_args)
            except:
                raise ParseError, ("invalid argument(s) passed to '" +
                                   column_name + "'", prev_pos + paren_pos)

            column_list.append(column)
            column_names.append(column_name)
            prev_pos = pos

        self.column_format = format
        self.column_list = ['self.print_' + column for
                            column in column_list]
        self.column_names = column_names
        self.set_columns_hook()

    def have_updir(self):
        'Asks whether there should be a ".." entry in the panel.'
        return self.path != '/'

# hooks

    def chdir_hook(self):
        pass

    def set_columns_hook(self):
        pass
    def move_tagged_entries(self, diff, extrude=1):
        'Moves tagged entries without with the given difference.'
        if diff < 0:
            start = self.have_updir()
            end = len(self)
            step = 1
        elif diff > 0:
            start = len(self) - 1
            end = self.have_updir() - 1
            step = -1
        else: # diff == 0
            return

        rows = [row for row in range(start, end, step)
                if self.list.get_row_data(row).tagged]

        if not rows:
            return

        gap = abs(rows[0] - start)

        if gap == 0:
            return
        if gap < abs(diff):
            if extrude:
                diff = -step * gap
            else:
                return

        self.list.freeze()

        for row in rows:
            self.swap_rows(row, row + diff)

        self.list.thaw()

    def crowd_tagged_entries(self, diff):
        'Crowds tagged entries without with the given difference.'
        squash = 0

        if diff < 0:
            start = self.have_updir()
            end = len(self)
            step = 1
        elif diff > 0:
            start = len(self) -1
            end = self.have_updir() - 1
            step = -1
        else: # diff == 0
            return

        self.list.freeze()

        for row in [row for row in range(start, end, step)]:
            tagged = self.list.get_row_data(row).tagged

            if not tagged:
                squash += 1
            elif tagged and squash > 0:
                squash = min(abs(diff), squash)
                self.swap_rows(row, row - step * squash)

        self.list.thaw()

    def tag_row(self, row_num):
        'Inverts the tag of the given row.'
        if self.have_updir() and row_num == 0: # '..' is not taggable
            return

        row = self.list.get_row_data(row_num)
        row.tag()

        if row.tagged:
            self.list.set_background(row_num, Colors.panel_tagged)
        else:
            self.list.set_background(row_num, Colors.panel_background)

    def open_row(self, row):
        file = self.list.get_row_data(row)

        if S_ISDIR(file.stat[ST_MODE]):
            self.chdir(file.filename)
        else:
            file.open()

    def swap_rows(self, row1, row2):
        self.list.swap_rows(row1, row2)

        if self.have_updir():
            row1, row2 = row1-1, row2-1
            temp = self.files[row1]
            self.files[row1] = self.files[row2]
            self.files[row2] = temp

    def set_colors(self):
        '(Re)Sets the colors of the panel widget.'
        style = self.list.get_style()
        style.base[gtk.STATE_NORMAL] = Colors.panel_background
        style.bg[gtk.STATE_NORMAL] = Colors.panel_background
        style.fg[gtk.STATE_NORMAL] = Colors.panel_foreground
        self.list.set_style(style)

    def chdir_hook(self):
        'Fills the list according to the new files and sets the title.'
        self.files = [File(file) for file in self.files]

        self.list.freeze()
        self.list.clear()
        row_num=0

        if self.have_updir():
            up_dir = File(self.path + '/..')
            self.list.append(up_dir.print_column_list(self.column_list))
            self.list.set_row_data(0, up_dir)
            row_num += 1

        for row in [[entry, entry.print_column_list(self.column_list)]
                    for entry in self.files]:
            self.list.append(row[1])
            self.list.set_row_data(row_num,row[0])
            self.list.set_foreground(row_num, row[0].get_color())
            row_num += 1

        self.list.thaw()

        self.title.set_text(unicode(self.path, config.encoding))

    def set_columns_hook(self):
        'Sets the column titles of the list widget.'
        self.scrollwin.remove(self.list)
        del self.list

        list = gtk.CList(len(self.column_list))

        list.set_selection_mode(gtk.SELECTION_BROWSE)
        list.set_shadow_type(gtk.SHADOW_NONE)
        list.set_events(gtk.gdk.KEY_PRESS_MASK | gtk.gdk.KEY_RELEASE_MASK |
                        gtk.gdk.BUTTON_PRESS_MASK | gtk.gdk.BUTTON_RELEASE_MASK |
                        gtk.gdk.BUTTON_MOTION_MASK)
        list.connect('event', self.main_event_handler)
        list.connect('select_row', lambda widget, row, col, event:
                     self.select_row_handler(row))

        for col_num in range(len(self.column_list)):
            list.set_column_title(col_num, self.column_names[col_num])

        list.column_titles_show()
        self.scrollwin.add(list)
        self.list = list

# event handlers

# this part below, the event handling mechanism will be rewritten next
# a far more general method should be used

    def select_row_handler(self, row):
        "Handles the `select_row' signal."
        row_data = self.list.get_row_data(row)

        if row_data:
            text = unicode(row_data.filename, config.encoding)
            self.statusbar.set_text(text)

    def main_event_handler(self, widget, event):
        'Dispatches the events of the list to the appropriate handlers.'
        key_press_events = [gtk.gdk.KEY_PRESS]
        mouse_button_events = [gtk.gdk.BUTTON_PRESS, gtk.gdk._2BUTTON_PRESS,
                               gtk.gdk._3BUTTON_PRESS, gtk.gdk.BUTTON_RELEASE]
        mouse_motion_events = [gtk.gdk.MOTION_NOTIFY]
        mouse_events = mouse_button_events + mouse_motion_events
        type = event.type

        if type in key_press_events:
            self.key_press_handler(event)
        elif type in mouse_events:
            try:
                row_num, col_num = self.list.get_selection_info(event.x, event.y)
            except TypeError: # we are out of the area of the list
                return

            entry = self.list.get_row_data(row_num)

            if type in mouse_button_events:
                button = event.button
                type = event.type
                self.mouse_button_handler(event)
            elif type in mouse_motion_events:
                self.mouse_motion_handler(event)

    def mouse_tag_handler(self, row):
        'Handles mouse tagging.'
        if self.prev_active_row == row or row < 0:
            return

        start = min(self.prev_active_row, row)
        end = max(self.prev_active_row, row)
        diff = row - self.prev_active_row

        if diff > 0:
            start += 1
            end += 1

        row_range = range(start, end)

        for row_iter in row_range:
            self.tag_row(row_iter)

        self.prev_active_row = row

    def key_press_handler(self, event):
        'Handles key press events of the list.'
        keyval = event.keyval
        file = self.list.get_row_data(self.list.focus_row)

        if keyval == gtk.keysyms.Return: # chdir to the actual dir
            self.open_row(self.list.focus_row)
        elif keyval == gtk.keysyms.Insert:
            focus_row = self.list.focus_row
            if focus_row < self.list.rows:
                self.list.select_row(focus_row+1, 1)
                #self.list.select_row(focus_row+1,1)
        elif keyval == gtk.keysyms.a:
            self.move_tagged_entries(-1)
        elif keyval == gtk.keysyms.z:
            self.move_tagged_entries(1)
        elif keyval == gtk.keysyms.s:
            self.crowd_tagged_entries(-1)
        elif keyval == gtk.keysyms.x:
            self.crowd_tagged_entries(1)

    def mouse_button_handler(self, event):
        'Handles mouse button events of the list.'
        row_num, col_num = self.list.get_selection_info(event.x, event.y)
        entry = self.list.get_row_data(row_num)
        button = event.button
        type = event.type

        # set input flags
        self.prev_active_row = row_num

        if type == gtk.gdk.BUTTON_PRESS:
            self.buttons_pressed[button-1] = 1
        elif type == gtk.gdk.BUTTON_RELEASE:
            self.buttons_pressed[button-1] = 0

        # change directory
        if event.button == 1 and event.type == gtk.gdk._2BUTTON_PRESS:
            self.chdir(entry.filename)

        # mouse tagging
        elif event.button == 3 and event.type == gtk.gdk.BUTTON_PRESS:
            self.tag_row(row_num)

    def mouse_motion_handler(self, event):
        'Handles mouse motion events of the list.'
        row_num, col_num = self.list.get_selection_info(event.x, event.y)

        if self.buttons_pressed[2]:
            self.mouse_tag_handler(row_num)
