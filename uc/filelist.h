#ifndef __FILELIST_H
#define __FILELIST_H

#include "fileentry.h"

typedef struct file_list_t {
    int len; /* the number of items (the length) of the list */
    file_entry_t *head; /* as you may except these are the head */
    file_entry_t *tail; /* and the tail of our file list */
} file_list_t;

/* creation and destiny */
file_list_t *new_file_list();
void free_file_list(file_list_t *file_list);

/* file list manipulation */
void add_file_to_list(file_list_t *list, char* fn);
file_list_t *chdir_file_list(char *sdir);
void cache_file_list(file_list_t *list, panel_column_t *columns);

#endif /* __FILELIST_H */
