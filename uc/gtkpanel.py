from gtk import *
import os

from basepanel import *
import gtkbasic

class Panel(BasePanel):
    'This class implements the GTK2 Panel widget.'
    def __init__(self):
        # initialize input flags
        self.buttons_pressed = [0, 0, 0]
        self.prev_active_row = 0

        # build widgets
        self.scrollwin = ScrolledWindow()
        self.scrollwin.set_policy(POLICY_AUTOMATIC, POLICY_AUTOMATIC)

        self.title = Label()
        self.title.set_line_wrap(TRUE)

        self.list = CList(1) # `set_column_hook' will replace this CList

        self.statusbar = Label()
        self.statusbar.set_line_wrap(TRUE)
        self.statusbar.set_alignment(xalign=0, yalign=0.5)

        self.widget = VBox()

        # connect widgets
        self.scrollwin.add(self.list)
        self.widget.pack_start(self.title, FALSE)
        self.widget.pack_start(self.scrollwin)
        self.widget.pack_start(self.statusbar, FALSE)

        BasePanel.__init__(self)

        self.set_colors()

    def __len__(self):
        return len(self.files) + self.have_updir()

# general methods

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
            self.list.set_background(row_num, gtkbasic.color_panel_tagged)
        else:
            self.list.set_background(row_num, gtkbasic.color_panel_background)

# helper methods

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
        style.base[STATE_NORMAL] = gtkbasic.color_panel_background
        style.bg[STATE_NORMAL] = gtkbasic.color_panel_background
        style.fg[STATE_NORMAL] = gtkbasic.color_panel_foreground
        self.list.set_style(style)

# hooks

    def chdir_hook(self):
        'Fills the list according to the new files and sets the title.'
        self.list.freeze()
        self.list.clear()
        row_num=0

        if self.have_updir():
            up_dir = FileEntry(self.path + '/..')
            self.list.append(up_dir.print_column_list(self.column_list))
            self.list.set_row_data(0, up_dir)
            row_num += 1

        for row in [[entry, entry.print_column_list(self.column_list)]
                    for entry in self.files]:
            self.list.append(row[1])
            self.list.set_row_data(row_num,row[0])
            row_num += 1

        self.list.thaw()

        self.title.set_text(unicode(self.path, config.encoding))

    def set_columns_hook(self):
        'Sets the column titles of the list widget.'
        self.scrollwin.remove(self.list)
        del self.list

        list = CList(len(self.column_list))

        list.set_selection_mode(SELECTION_BROWSE)
        list.set_shadow_type(SHADOW_NONE)
        list.set_events(gdk.KEY_PRESS_MASK | gdk.KEY_RELEASE_MASK |
                        gdk.BUTTON_PRESS_MASK | gdk.BUTTON_RELEASE_MASK |
                        gdk.BUTTON_MOTION_MASK)
        list.connect('event', self.main_event_handler)
        list.connect('select_row', lambda widget, row, col, event:
                     self.select_row_handler(row))

        for col_num in range(len(self.column_list)):
            list.set_column_title(col_num, self.column_list[col_num])

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
        key_press_events = [gdk.KEY_PRESS]
        mouse_button_events = [gdk.BUTTON_PRESS, gdk._2BUTTON_PRESS,
                               gdk._3BUTTON_PRESS, gdk.BUTTON_RELEASE]
        mouse_motion_events = [gdk.MOTION_NOTIFY]
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

        if keyval == keysyms.Return: # chdir to the actual dir
            self.chdir(self.list.get_row_data(self.list.focus_row).filename)
        elif keyval == keysyms.Insert:
            focus_row = self.list.focus_row
            if focus_row < self.list.rows:
                self.list.select_row(focus_row+1,1)
                self.list.select_row(focus_row+2,1)
        elif keyval == keysyms.a:
            self.move_tagged_entries(-1)
        elif keyval == keysyms.z:
            self.move_tagged_entries(1)
        elif keyval == keysyms.s:
            self.crowd_tagged_entries(-1)
        elif keyval == keysyms.x:
            self.crowd_tagged_entries(1)

    def mouse_button_handler(self, event):
        'Handles mouse button events of the list.'
        row_num, col_num = self.list.get_selection_info(event.x, event.y)
        entry = self.list.get_row_data(row_num)
        button = event.button
        type = event.type

        # set input flags
        self.prev_active_row = row_num

        if type == gdk.BUTTON_PRESS:
            self.buttons_pressed[button-1] = 1
        elif type == gdk.BUTTON_RELEASE:
            self.buttons_pressed[button-1] = 0

        # change directory
        if event.button == 1 and event.type == gdk._2BUTTON_PRESS:
            self.chdir(entry.filename)

        # mouse tagging
        elif event.button == 3 and event.type == gdk.BUTTON_PRESS:
            self.tag_row(row_num)

    def mouse_motion_handler(self, event):
        'Handles mouse motion events of the list.'
        row_num, col_num = self.list.get_selection_info(event.x, event.y)

        if self.buttons_pressed[2]:
            self.mouse_tag_handler(row_num)
