from stat import *
import grp
import os
import pwd
import string
import time

from parseerror import *
import config

unit_table = { # used by 'print_size' and 'validate_size'
    'b': 2**0,
    'K': 2**10,
    'M': 2**20,
    'G': 2**30,
    'T': 2**40,
    'P': 2**50,
    'E': 2**60,
    'Z': 2**70,
    'Y': 2**80,
    'KiB': 10**3,
    'MiB': 10**6,
    'GiB': 10**9,
    'TiB': 10**12,
    'PiB': 10**15,
    'EiB': 10**18,
    'ZiB': 10**21,
    'YiB': 10**24
}
        
class FileEntry:
    '''Describes a single file.
       This file can be either on VFS or it can be any regular file.
    '''
    def __init__(self, filename, stat=0, link=''):
        '''stat(2)ing is not achievable if the file resides on a VFS so higher
           layers should pass the stat tuple.
        '''
        self.filename = filename
        self.tagged = 0
        self.cached = ''

        if self.is_direct():
            try:
                self.stat = os.stat(filename)
            except: # stalled link
                self.stat = (-1, -1, -1, -1, -1, -1, -1, -1, -1, -1)
        else:
            self.stat = stat

    def __del__(self):
        'Decaching should happen when the object is destroyed.'
        self.decache()

    def __repr__(self):
        return self.__str__()

    def __str__(self):
        s = "<FileEntry '" + self.filename + "'"
        if self.cached:
            s += " cached='" + self.cached + "'"
        if self.tagged:
            s += ' tagged'
        s += '>'
        return s
        
    def is_direct(self):
        '''Checks whether the file is directly accessible through the Unix VFS.
           The counterpart of "is_virtual".
        '''
        if string.find(self.filename, '//') == -1: # '//' is the VFS path separator
            return 1
        else:
            return 0

    def is_virtual(self):
        '''Checks whether the file is virtually accessible through the UC VFS.
           The counterpart of "is_direct".
        '''
        return not self.is_direct()
    
    def is_local(self):
        '''Checks whether the file is on a local storage.
           The counterpart of "is_remote".
        '''
        return 0

    def is_remote(self):
        '''Checks whether the file is on a remote storage.
           The counterpart of "is_local".
        '''
        return not self.is_local();

    def is_cached(self):
        '''Checks whether the file is on a fast storage and directly accessible
           for time critical operations.
        '''
        return is_local(self) and is_direct(self) or self.cached

    def tag(self):
        self.tagged = not self.tagged
    
    def cache(self, done=0):
        '''Caching a file copies it to a fast local storage for time critical operations.
           This method is not related to files on UC VFSes since that would be extremely
           time consuming in many cases. UC VFS caching is implemented at "BasePanel" level.
        '''
        pass

    def decache(self):
        'Decaching the file removes it from local storage.'
        pass

    # print methods

    def print_column_list(self, format_list):
        'Returns the fields of the file described by "format".'
        return [eval(format) for format in format_list]

    def print_type_sym(self):
        'Returns the type symbol of the file.'
        mode = self.stat[ST_MODE]

        if S_ISREG(mode):
            return ' '
        elif S_ISDIR(mode):
            return '/'
        elif S_ISLNK(mode):
            return '@'
        elif S_ISFIFO(mode):
            return '|'
        elif S_ISSOCK(mode):
            return '='
        elif S_ISCHR(mode):
            return '-'
        elif S_ISBLK(mode):
            return '+'

    def print_type_letter(self):
        'Returns the type letter of the file.'
        mode = self.stat[ST_MODE]

        if S_ISREG(mode):
            return '-'
        elif S_ISDIR(mode):
            return 'd'
        elif S_ISLNK(mode):
            return 'l'
        elif S_ISFIFO(mode):
            return 'p'
        elif S_ISSOCK(mode):
            return 's'
        elif S_ISCHR(mode):
            return 'c'
        elif S_ISBLK(mode):
            return 'b'
        
    def print_filename(self, prefixed=0, absolute=0):
        '''Returns the filename of the file.
           If "prefixed" is true, filename will be prefixed with symbolic type prefix.
           If "absolute" is true, the full path of the filename will be returned.
        '''
        if not absolute:
            filename = os.path.basename(self.filename)
        else:
            filename = self.filename

        if prefixed:
            filename = self.print_type_sym() + filename

        return unicode(filename, 'iso-8859-2')
    
    def print_mode(self, full=0):
        '''Returns the octal mode of the file.
           If "full" is true, the type part of the mode will be included too.
        '''
        mode = self.stat[ST_MODE]

        if not full:
            mode = S_IMODE(mode)

        return '%o' % mode

    def print_perm(self, prefixed=0):
        '''Returns the permission string of the file.
           If "prefixed" is true, the returned string will be prefixed with symbolic type prefix.
        '''
        mode = self.stat[ST_MODE]

        if prefixed:
            s = self.print_type_letter()
        else:
            s = ''

        if mode & 2**8: # read by owner
            s += 'r'
        else:
            s += '-'

        if mode & 2**7: # write by owner
            s += 'w'
        else:
            s += '-'

        if mode & 2**11 and mode & 2**6: # SUID and execute/search by owner
            s += 's'
        elif mode & 2**11: # SUID bit present
            s += 'S'
        elif mode & 2**6: # execute/search by owner
            s += 'x'
        else:
            s += '-'

        if mode & 2**5: # read by group
            s += 'r'
        else:
            s += '-'

        if mode & 2**4: # write by group
            s += 'w'
        else:
            s += '-'

        if mode & 2**10 and mode & 2**3: # SGID and execute/search by group
            s += 's'
        elif mode & 2**10: # SGID bit present
            s += 'S'
        elif mode & 2**3: # execute/search by group
            s += 'x'
        else:
            s += '-'

        if mode & 2**2: # read by others
            s += 'r'
        else:
            s += '-'

        if mode & 2**1: # write by others
            s += 'w'
        else:
            s += '-'

        if mode & 2**9 and mode & 2**0: # sticky bit and execute/search by others
            s += 's'
        elif mode & 2**9: # sticky bit present
            s += 'S'
        elif mode & 2**0: # execute/search by others
            s += 'x'
        else:
            s += '-'

        return s

    def print_inode(self):
        'Returns the inode of the file.'
        return '%i' % self.stat[ST_INO]

    def print_dev(self):
        'Returns the major and minor number of the device which the file resides on.'
        dev = self.stat[ST_DEV]
        return '%i, %i' % (dev/256, dev%256)

    def print_nlink(self):
        'Returns the number of links pointed to the file.'
        return '%i' % self.stat[ST_NLINK]

    def print_uid(self):
        'Returns the UID of the file.'
        return '%i' % self.stat[ST_UID]

    def print_owner(self):
        "Returns the owner of the file (owner's name)."
        return pwd.getpwuid(self.stat[ST_UID])[0]
        
    def print_gid(self):
        'Returns the GID of the file.'
        return '%i' % self.stat[ST_GID]

    def print_group(self):
        "Returns the GID of the file (group's name)."
        return grp.getgrgid(self.stat[ST_GID])[0]
        
    def print_size(self, unit='b'):
        '''Returns the size of the file.
           "unit" can be one of the followings:
               Powers of 1024: [bKMGTPEZY]
               Powers of 1000: [KMGTPEZY]iB
        '''
        size = self.stat[ST_SIZE]
        unit_size = unit_table[unit]
        return '%i' % (size/unit_size) + unit

    def print_atime(self, format=config.panel_time_format):
        'Returns the time of the last access time of the file.'
        return time.strftime(format, time.gmtime(self.stat[ST_ATIME]))

    def print_mtime(self, format=config.panel_time_format):
        'Returns the time of the last modification time of the file.'
        return time.strftime(format, time.gmtime(self.stat[ST_MTIME]))

    def print_ctime(self, format=config.panel_time_format):
        'Returns the time of the last change time of the file.'
        return time.strftime(format, time.gmtime(self.stat[ST_CTIME]))

    def print_link(self):
        'Returns the filename referred by "self" if it is a link.'
        return self.link
    
    def print_last_link(self):
        'Recursively searches the filename referred by a chain of links and returns it.'
        filename = self.filename
        return '' #TODO

    # validation methods
    
    def validate_size(self, unit='KiB'):
        'Throws a ParseError exception if "unit" has illegal value.'
        if not unit_table.has_key(unit):
            raise ParseError, "invalid unit: '%s'" % unit
    
    # comparsion methods for sorting
