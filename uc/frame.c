#include "frame.h"
#include "mem.h"

frame_t *new_frame(frame_t *parent, frame_dir_t dir)
{
    frame_t *frame = (frame_t*)xmalloc(sizeof(frame_t));
    frame->parent = parent;
    frame->dir = dir;
    frame->childs = NULL;
    return frame;
}

void add_child_frame(frame_t* parent, void* child, frame_child_t type, int size)
{
    
}

void init_frames()
{
//    root_frame=new_frame(NULL, FRAME_DIR_HOR);
//    add_child_frame(root_frame, NULL, FRAME_TYPE_PANEL, 1000)
}
