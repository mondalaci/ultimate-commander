#ifndef __PANEL_H
#define __PANEL_H

#include <linux/limits.h>
#include "filelist.h"
#include "widget.h"

typedef enum panel_mode_t {
    PANEL_MODE_NORMAL,
    PANEL_MODE_EDIT,
    PANEL_MODE_SORT
} panel_mode_t;

typedef struct panel_t {
    widget_t widget;
    panel_column_t *columns[2]; /* the column format spewed by reformat_panel and swallowed by new_file_entry */
    file_list_t *file_list;
    struct panel_t *target; /* the target of various file operations */
    char* format; /* the actual format string serves as default value in case of reformatting */
    char path[PATH_MAX]; /* also known as current working directory */
    int panes; /* the number of panel slices, negative value defines the minimum width per pane */
    int index; /* the first visible entry */
    int row; /* the actual row */
    int col; /* the current row, meaningful only in edit mode */
    panel_mode_t mode;
} panel_t;

/* construction and destruction */
panel_t *new_panel(char* path, char* format);
void destroy_panel(panel_t *panel);

/* data manipulation */
int reformat_panel(panel_t *panel, char *format); /* you need to call this function in order to display the panel */
void chdir_panel(panel_t *panel, char* dir);
int set_panel_panes(panel_t *panel, int panes);

/* common widget interfaces */
void draw_panel(panel_t *panel);
void input_panel(panel_t *panel);

/* ui functions */
void prev_panel_row(panel_t *panel);
void next_panel_row(panel_t *panel);
void prev_panel_pane(panel_t *panel);
void next_panel_pane(panel_t *panel);
void prev_panel_page(panel_t *panel);
void next_panel_page(panel_t *panel);
void move_panel_start(panel_t *panel);
void move_panel_end(panel_t *panel);
void select_panel_entry(panel_t *panel);

#endif /* __PANEL_H */
