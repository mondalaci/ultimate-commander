#ifndef __FILEENTRY_H
#define __FILEENTRY_H

#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>

typedef struct file_entry_t {
    char *fn; /* the absolute file name */
    struct stat st; /* see stat(2) */
    int marked:1;
    int field_num; /* the number of fields */
    char **fields; /* the fields themselves. NULL is for `ord' */
    struct file_entry_t *prev; /* the next and ... */
    struct file_entry_t *next; /* previous entries of the double linked file list */
} file_entry_t;

typedef struct panel_column_t {
    char *name;
    char *(*printer)(file_entry_t *entry, struct panel_column_t *column);
    size_t offset;
    char *data;
    int len;
} panel_column_t;

/* initialization and destroy */
file_entry_t *new_file_entry(char *fn);
void free_file_entry(file_entry_t *entry);

/* data manipulation */
void cache_file_entry(file_entry_t *entry, panel_column_t *columns);


extern panel_column_t column_types[];

#endif /* __FILEENTRY_H */
