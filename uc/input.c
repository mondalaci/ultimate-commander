#include <curses.h>
#include "input.h"

#define BUFFSIZE 128

static char buffer[BUFFSIZE];

char *print_key(int key)
{
    char ch[2]="0";
    *buffer='\0';
    if (GET_KEY_CTRL(key))
	strcat(buffer, "C-");
    if (GET_KEY_MOD(key))
	strcat(buffer, "M-");
    *ch=GET_KEY_CHAR(key);
    strcat(buffer, ch);
    return buffer;
}

void main_input_loop()
{
    int key, ctrl=0, mod=0;
    while (1) {
	key=getch();
	if (key==ASCII_ESC) {
	    if (mod) {
		key=KEY_ESC;
		mod=0;
	    } else {
		mod=1;
		continue;
	    }
	}
	ctrl=GET_KEY_CTRL(key) ;

	addstr(print_key(MAKE_KEY(ctrl, mod, key)));
	addstr("\n");

	if (key!=ASCII_ESC)
	    mod=0;
    }
}
