#ifndef __FRAME_H
#define __FRAME_H

#include <link.h>

typedef enum frame_dir_t {
    FRAME_DIR_VERT,
    FRAME_DIR_HOR
} frame_dir_t;

typedef struct frame_child_t {
    void *child;
    int (*size_callback)(); /* if it's NULL then `size' is correct */
    int size; /* negative is fixed size, 0 fills remaining and from 1 to 1000 is `percentage' */
} frame_child_t;

typedef struct frame_t {
    struct frame_t *parent;
    frame_dir_t dir;
    frame_child_t *childs;
} frame_t;

/* start & end */
frame_t *new_frame(frame_t *parent, frame_dir_t dir);
void destroy_frame(frame_t *frame);

/* manipulating frames */
/*void split_frame_horizontally(buffer_t *buffer);
void split_frame_vertically(buffer_t *buffer);
*/
void add_child_frame(frame_t* parent, void* child, frame_child_t type, int size);

extern frame_t *root_frame;
extern frame_t *current_frame;

#endif /* __FRAME_H */
