#ifndef __FRAME_H
#define __FRAME_H

#include "widget.h"

typedef enum frame_dir_t {
    FRAME_DIR_HOR,
    FRAME_DIR_VERT
} frame_dir_t;

typedef struct frame_t {
    widget_t widget;
    frame_dir_t dir;
    struct widget_t *childs;
    int size;
} frame_t;

widget_t *new_frame(widget_t *parent, frame_dir_t dir);
void destroy_frame(widget_t *frame);

/* user interface functions */
int delete_window();
void other_window();
int split_window_horizontally(int size);
int split_window_vertically(int size);

void add_child_frame(frame_t* parent, frame_t* child, frame_dir_t dir, int size);

extern widget_t *root_frame;
extern widget_t *current_frame;
extern widget_t *minibuffer;

#endif /* __FRAME_H */
