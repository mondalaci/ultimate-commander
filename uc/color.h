#ifndef __COLOR_H
#define __COLOR_H

#define getpair(fg, bg) COLOR_PAIR(bg*8+fg)

void init_color_pairs();
/* maybe `draw_hline' is not closely belongs here but I think it's a good place */
void draw_hline(int y, int x, int h);

#endif /* __COLOR_H */
