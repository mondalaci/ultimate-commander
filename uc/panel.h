#ifndef __PANEL_H
#define __PANEL_H

#include <linux/limits.h>
#include "filelist.h"
#include "link.h"

typedef enum panel_mode_t {
    PANEL_MODE_NORMAL,
    PANEL_MODE_EDIT,
    PANEL_MODE_SORT
} panel_mode_t;

typedef struct panel_t {
    link_t link; /* mainly the glue between the panel and the frames */
    panel_column_t *columns[2]; /* the column format spewed by reformat_panel and swallowed by new_file_entry */
    file_list_t *file_list; /* guess what ;) */
    struct panel_t *target; /* target of various file operations */
    char* format; /* actual format string serves as default value in case of reformatting */
    char path[PATH_MAX]; /* also known as current working directory */
    int panes; /* the number of panel slices, negative value defines (int)(panel_width/-panes) panes */
    int index; /* the first visible entry */
    int row; /* the actual row */
    int col; /* the current row, meaningful only in edit mode */
    panel_mode_t mode; /* see above */
} panel_t;

//file_entry_t *draw_panel_pane(panel_t *panel, file_entry_t *entry, panel_column_t *column, int row, rect_t *rect);
void draw_panel_entry(char* entry, int len);
//void draw_panel_row(file_entry_t *entry, panel_column_t *column, int row, int panel_row);

/* initialization and destroy */
void init_panel_cols();
panel_t *new_panel();
void destroy_panel(panel_t *panel);

/* member manipulation */
int reformat_panel(panel_t *panel, char *format);
panel_column_t **set_panel_format(char *format);
int get_panel_panes(panel_t *panel);
void chdir_panel(panel_t *panel, char* dir);
file_entry_t *get_panel_entry(panel_t *panel);

/* input and output */
void draw_panel(panel_t *panel);
void input_panel(panel_t *panel);

/* high-level ui */
void next_panel_row(panel_t *panel);
void prev_panel_row(panel_t *panel);
void prev_panel_page(panel_t *panel);
void prev_panel_page(panel_t *panel);
void prev_panel_pane(panel_t *panel);
void prev_panel_pane(panel_t *panel);
void move_panel_start(panel_t *panel);
void move_panel_end(panel_t *panel);
void select_panel_entry(panel_t *panel);

/* low-level ui */
int set_panel_row(panel_t* panel, int index);

#endif /* __PANEL_H */
