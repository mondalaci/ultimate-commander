#include <curses.h>
#include <stdlib.h>
#include "mem.h"

void *xmalloc(size_t size)
{
    void *data;
    if (!(data=malloc(size))) {
	endwin();
	fputs("Virtual memory exhausted!\n", stderr);
	exit(1);
    }
    return data;
}

