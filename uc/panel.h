#ifndef __PANEL_H
#define __PANEL_H

#include <linux/limits.h>
#include "filelist.h"
#include "window.h"

typedef enum panel_mode_t {
    PANEL_MODE_NORMAL,
    PANEL_MODE_META_EDIT,
    PANEL_MODE_GRAB,
    PANEL_MODE_SEARCH
} panel_mode_t;

typedef struct panel_t {
    window_t window;
    panel_column_t *columns[2]; /* the column format spewed out by reformat_panel and swallowed by new_file_entry */
    file_list_t *file_list;
    struct panel_t *target; /* the target of various file operations */
    char *format; /* the actual format string serves as default value in case of reformatting */
    char path[PATH_MAX+1]; /* also known as current working directory */
    int panes; /* the number of panel slices, negative value defines the minimum width per pane */
    int index; /* the first visible entry */
    int row; /* the actual row */
    int col; /* the current row, meaningful only in meta edit mode */
    panel_mode_t mode;
} panel_t;

/* construction and destruction */
panel_t *new_panel();
void free_panel(panel_t *panel);

/* member manipulation */
void chdir_panel(panel_t *panel, char *dir);
void format_panel(panel_t *panel, char *format);
void sort_panel(panel_t *panel, char *format);

/* navigation */
void prev_panel_row(panel_t *panel);
void next_panel_row(panel_t *panel);
void prev_panel_pane(panel_t *panel);
void next_panel_pane(panel_t *panel);
void prev_panel_page(panel_t *panel);
void next_panel_page(panel_t *panel);
void move_panel_start(panel_t *panel);
void move_panel_end(panel_t *panel);
void tag_panel_entry(panel_t *panel);
void activate_panel_entry(panel_t *panel);

/* visualization */
void draw_panel(panel_t *panel);

#endif /* __PANEL_H */
