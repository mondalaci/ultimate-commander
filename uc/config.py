from gtkconfig import *

## General settings

# list of directories where files will be cached
cache_dir_list = ['/tmp']

## Panel settings

# default panel column format
#panel_column_format = 'filename mode perm inode dev nlink uid owner gid group size("MiB") atime mtime ctime'
panel_column_format = 'filename size'
# default panel statusbar format
panel_statusbar_format = 'filename size perm selected'

# default panel sort order format
panel_sort_format = 'dir hidden filename'

# default panel filter format
panel_filter_format = ''

# default panel time format in strftime(3) syntax
panel_time_format = '%Y-%m-%d %H:%M:%S'
