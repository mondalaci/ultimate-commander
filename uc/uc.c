#include <curses.h>
#include <signal.h>
#include "input.h"
#include "panel.h"

void init();
void finish(int sig);

int main(int argc, char *argv[])
{
    panel_t *panel=new_panel();
    int c;
    char s[512];
    init();

//    main_input_loop();

    getmaxyx(stdscr, panel->link.rect.h, panel->link.rect.w);
    if (reformat_panel(panel, "fn(pa)|size:4|uid.sym|mode.sym")) {
	addstr("bad format string\n");
	getch();
	finish(0);
    }
    chdir_panel(panel, "/");
    draw_panel(panel);
    while (c=getch()) {
	switch (c) {
    	    case KEY_UP:
		prev_panel_row(panel);
		break;
    	    case KEY_DOWN:
		next_panel_row(panel);
		break;
    	    case KEY_LEFT:
		prev_panel_page(panel);
		break;
    	    case KEY_RIGHT:
		next_panel_page(panel);
		break;
    	    case KEY_PPAGE:
		prev_panel_pane(panel);
		break;
    	    case KEY_NPAGE:
		next_panel_pane(panel);
		break;
    	    case KEY_HOME:
		move_panel_start(panel);
		break;
    	    case KEY_END:
		move_panel_end(panel);
		break;
	    case KEY_RETURN:
		select_panel_entry(panel);
		break;
	}
	draw_panel(panel);
    }
    getch();
    chdir_panel(panel, "/home/lee");
    draw_panel(panel);
    getch();

    free_file_list(panel->file_list);
//    getch();
//    main_key_loop();
    finish(0);
}

void init() {
    init_panel_cols();
    signal(SIGINT, finish);
    initscr();
    keypad(stdscr, TRUE);
    nonl();
    cbreak();
    noecho();
    start_color();
    init_color_pairs();
}

void finish(int sig)
{
    endwin();
    exit(0);
}
