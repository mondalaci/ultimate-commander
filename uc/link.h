#ifndef __LINK_H
#define __LINK_H

typedef enum type_t {
    FRAME_TYPE_FRAME,
    FRAME_TYPE_PANEL,
    FRAME_TYPE_JOB_LIST,
    FRAME_TYPE_OUTPUT,
    FRAME_TYPE_MINIBUFFER
} type_t;

typedef struct rect_t {
    int y, x, h, w;
} rect_t;

typedef struct link_t {
    type_t type;
    struct link_t *parent;
    rect_t rect;
} link_t;

void link_refresh(link_t *link);
void link_redraw(link_t *link);
//link_input(link_t *link, );

#endif /* __LINK_H */
