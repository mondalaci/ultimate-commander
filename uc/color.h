#ifndef __COLOR_H
#define __COLOR_H

#define getpair(fg, bg) COLOR_PAIR(bg*8+fg)

void init_color_pairs();

#endif /* __COLOR_H */
