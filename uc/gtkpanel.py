from gtk import *
import os

from basepanel import *
from gtkbasic import *

class Panel(BasePanel):
    def __init__(self):
        # initializing input flags
        self.buttons_pressed = [0, 0, 0]
        self.prev_active_row = 0

        # building widgets
        self.scrollwin = ScrolledWindow()
        self.scrollwin.set_policy(POLICY_AUTOMATIC, POLICY_AUTOMATIC)

        self.title = Label()
        self.title.set_line_wrap(TRUE)

        self.statusbar = Label()
        self.statusbar.set_line_wrap(TRUE)
        self.statusbar.set_justify(JUSTIFY_LEFT)
        
        self.widget = VBox()

        BasePanel.__init__(self) # builds self.list

        # connecting widgets
        self.scrollwin.add(self.list)
        self.widget.pack_start(self.title, FALSE)
        self.widget.pack_start(self.scrollwin)
        self.widget.pack_start(self.statusbar, FALSE)

        self.set_colors()

    def __len__(self):
        return len(self.files) + self.have_updir()

# general methods

    def move_tagged_entries_squashlessly(self, diff):
        'Moves the tagged entries without squash with the given difference.'
        self.list.freeze()

        if diff < 0:
            squash = self.have_updir()
            start = self.have_updir() - diff
            end = len(self)
            step = 1
        elif diff > 0:
            squash = len(self)
            start = len(self) - diff
            end = self.have_updir() - 1
            step = -1
            
        for row_num in [row_num for row_num in range(start, end, step)
                        if self.list.get_row_data(row_num).tagged]:
            self.list.swap_rows(row_num, row_num + diff)

        self.list.thaw()
            
    def tag_row(self, row_num):
        'Inverts the tag of a given row.'
        if self.have_updir() and row_num == 0: # '..' is not taggable
            return

        row = self.list.get_row_data(row_num)
        row.tag()

        if row.tagged:
            self.list.set_background(row_num, self.color_tagged)
        else:
            self.list.set_background(row_num, self.color_background)
            
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
            
        for row in [[entry, entry.print_column_list(self.column_list)] for entry in self.files]:
            self.list.append(row[1])
            self.list.set_row_data(row_num,row[0])
            row_num += 1

        self.list.thaw()

        self.title.set_text(unicode(self.path, 'iso-8859-2'))

    def set_columns_hook(self):
        'Sets the column titles of the list widget.'
        self.list = self.new_list(len(self.column_list))
        for col_num in range(len(self.column_list)):
            self.list.set_column_title(col_num, self.column_list[col_num])
        self.list.column_titles_show()

# event handlers

    def main_event_handler(self, widget, event):
        'Dispatches the events of the list to the appropriate handlers.'
        self.event = event
        key_press_events = [gdk.KEY_PRESS]
        mouse_button_events = [gdk.BUTTON_PRESS, gdk._2BUTTON_PRESS,
                               gdk._3BUTTON_PRESS, gdk.BUTTON_RELEASE]
        mouse_motion_events = [gdk.MOTION_NOTIFY]
        mouse_events = mouse_button_events + mouse_motion_events
        type = event.type
        
        if type in key_press_events:
            self.key_press_handler(event.keyval)
        elif type in mouse_events:
            try:
                row_num, col_num = self.list.get_selection_info(event.x, event.y)
            except TypeError: # we are out of the area of the list
                return

            entry = self.list.get_row_data(row_num)

            if type in mouse_button_events:
                button = event.button
                type = event.type
                self.mouse_button_handler(event, row_num, col_num, entry, button, type)
            elif type in mouse_motion_events:
                self.mouse_motion_handler(event, row_num, col_num, entry)
                    
    def key_press_handler(self, keyval):
        'Handles key press events of the list.'
        #print row:
        if keyval == keysyms.Return: # chdir to the actual dir
            self.chdir(self.list.get_row_data(self.list.focus_row).filename)
        elif keyval == keysyms.Insert:
            focus_row = self.list.focus_row
            #self.tag_row(focus_row)
            if focus_row < self.list.rows:
                self.list.select_row(focus_row+1,1)
                #self.list.select_row(focus_row+2,1)
                #self.list.emit('select_row', focus_row+1, 1, self.event)
        #self.move_tagged_entries_squashlessly(1)

    def mouse_button_handler(self, event, row_num, col_num, entry, button, type):
        'Handles mouse button events of the list.'
        self.prev_active_row = row_num
        if type == gdk.BUTTON_PRESS:
            self.buttons_pressed[button-1] = 1
        elif type == gdk.BUTTON_RELEASE:
            self.buttons_pressed[button-1] = 0

        # changing directory
        if event.button == 1 and event.type == gdk._2BUTTON_PRESS:
            self.chdir(entry.filename)
            
        # mouse tagging
        elif event.button == 3 and event.type == gdk.BUTTON_PRESS:
            self.tag_row(row_num)

    def mouse_motion_handler(self, event, row_num, col_num, entry):
        'Handles mouse motion events of the list.'
        if self.buttons_pressed[2]:
            self.mouse_tag_handler(row_num)
                
    def mouse_tag_handler(self, row_num):
        'Mouse tagging handler.'
        if self.prev_active_row == row_num or row_num < 0:
            return

        start = min(self.prev_active_row, row_num)
        end = max(self.prev_active_row, row_num)
        diff = row_num - self.prev_active_row

        if diff > 0:
            start += 1
            end += 1

        row_range = range(start, end)

        for row in row_range:
            self.tag_row(row)

        self.prev_active_row = row_num

    def select_row_handler(self, widget, row, column, event):
        'This handler caring with the "select_row" signal of the list.'
        # this is only a temporary solution without using statusbar_format
        try:
            self.statusbar.set_text(unicode(self.list.get_row_data(row).filename, 'iso-8859-2'))
        except AttributeError: # in case of refreshing the list
            pass

# user interface helpers

    def new_list(self, columns):
        'Returns a new CList widget containing "columns" columns.'
        list = CList(columns)

        list.set_selection_mode(SELECTION_BROWSE)
        list.set_shadow_type(SHADOW_NONE)
        list.set_events(gdk.KEY_PRESS_MASK | gdk.KEY_RELEASE_MASK |
                        gdk.BUTTON_PRESS_MASK | gdk.BUTTON_RELEASE_MASK |
                        gdk.BUTTON_MOTION_MASK)
        list.connect('event', self.main_event_handler)
        list.connect('select_row', self.select_row_handler)

        return list

    def resize_list(self, columns):
        'Replaces the current CList widget with a new one containing "columns" columns.'
        self.scrollwin.remove(self.list)
        del self.list
        self.list = self.new_list(columns)
        self.scrollwin.add(self.list)

    def set_colors(self):
        'Sets the colors of the panel widget. Call if the color changed!'
        self.color_background = get_color(config.gtk_panel_background)
        self.color_foreground = get_color(config.gtk_panel_foreground)
        self.color_tagged = get_color(config.gtk_panel_tagged)

        style = self.list.get_style()
        style.base[STATE_NORMAL] = self.color_background
        style.bg[STATE_NORMAL] = self.color_background
        style.fg[STATE_NORMAL] = self.color_foreground
        self.list.set_style(style)
