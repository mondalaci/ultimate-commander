#include <curses.h>
#include "color.h"

void init_color_pairs()
{
    int fg, bg;
    static short colors[8] = {
	COLOR_BLACK, COLOR_RED, COLOR_GREEN, COLOR_YELLOW,
	COLOR_BLUE, COLOR_MAGENTA, COLOR_CYAN, COLOR_WHITE
    };

    for (fg=0; fg<8; fg++)
	for (bg=0; bg<8; bg++)
	    init_pair(bg*8+fg, colors[fg], colors[bg]);
}
