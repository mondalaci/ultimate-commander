import os
import string

from fileentry import *
from parseerror import *
import config

class BasePanel:
    '''This is the base panel class.
       User interface implementations are left to children.
    '''
    def __init__(self, path='~', column_format=config.panel_column_format,
                 statusbar_format=config.panel_statusbar_format,
                 sort_format=config.panel_sort_format,
                 filter_format=config.panel_filter_format):
        self.set_columns(column_format)
        #self.set_statusbar(statusbar_format)
        #self.set_sort_order(sort_format)
        #self.set_filter(filter_format)
        self.chdir(path)
        
# general methods

    def chdir(self, path='~'):
        'Change the current directory of the panel.'
        self.path = os.path.abspath(os.path.expanduser(path))

        if self.path == '/':
            path = self.path
        else:
            path = self.path + '/'

        self.files = [FileEntry(path + file) for
                      file in os.listdir(self.path)]

        self.chdir_hook()

    def set_columns(self, format=config.panel_column_format):
        'Sets the columns of the panel.'
        paren_stack = []
        token_pos_list = []
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
                token_pos_list.append(pos)

            prev_char = current_char

        if len(paren_stack) != 0:
            raise ParseError, ('parenthesis has no closing pair',
                               paren_stack.pop())
        
        token_pos_list.append(pos+1)
        prev_pos = token_pos_list[0]
        column_list = []

        # tokenize and check the validity of column formats
        entry = FileEntry('/etc/fstab') # for validity check
        for current_pos in token_pos_list[1:]:
            column = string.strip(format[prev_pos:current_pos])
            try:
                paren_pos = column.index('(')
                column_name = column[:paren_pos]
            except:
                column_name = column
                column = column + '()'

            printer = 'FileEntry.print_' + column_name
            try:
                eval(printer)
            except:
                raise ParseError, ("invalid column type: '" +
                                   column_name + "'", prev_pos)

            validator = 'entry.validate_' + column
            try:
                eval(validator)
            except AttributeError:
                pass
            except ParseError, error:
                raise ParseError, (error.type + " in '" + column_name + "' type", prev_pos + paren_pos)
            
            printer_with_args = 'entry.print_' + column
            try:
                eval(printer_with_args)
            except:
                raise ParseError, ("invalid argument(s) passed to '" +
                                   column_name + "'", prev_pos + paren_pos)
            
            column_list.append(column)
            prev_pos = current_pos

        self.column_format = format
        self.column_list = ['self.print_' + column for column in column_list]

        self.set_columns_hook()

    def have_updir(self):
        'Asks whether there suould be a ".." entry in the panel.'
        return self.path != '/'

# hooks

        def chdir_hook(self):
            pass
        
        def set_columns_hook(self):
            pass
        
