#include <sys/types.h>
#include <sys/stat.h>
#include <string.h>
#include <dirent.h>
#include <stdio.h>
#include <pwd.h>
#include <sys/types.h>
#include <unistd.h>
#include <grp.h>
#include <time.h>
#include <math.h>
#include "fileentry.h"
#include "curses.h"
#include "panel.h"
#include "color.h"
//extern char **environ;
/* draw parts of the panel */
//void draw_panel_entry(char* entry, int len);
//void draw_panel_row(file_entry_t *entry, panel_t *panel, int active);
//file_entry_t *draw_panel_pane(panel_t *panel, file_entry_t *start_entry, int row, rect_t *rect);



int get_panel_panes(panel_t *panel)
{
    if (panel->panes>0)
	return panel->panes;
    else
	return -panel->link.rect.w/panel->panes;
}


int reformat_panel(panel_t *panel, char *sformat)
{
    char *c, *token;
    int column_num=1, i;
    char *format;
    panel_column_t *columns, *column, *column_type;
    int total_percentage;
    int expanding_columns=0;
    int remaining_cells=panel->link.rect.w/get_panel_panes(panel);
    int expanding_column_len;
    int variable_column=0;
    int variable_column_len=-1;

//    printf("initial remaining cells: %i\n", remaining_cells);
    format=strdup(sformat); /* strtok doesn't like static string to be tokenized */

/* computing the number of columns */
    for (c=format; *c; c++)
	if (*c=='|')
	    column_num++;
    remaining_cells-=column_num-1;
    column=columns=(panel_column_t*)malloc((column_num+1)*sizeof(panel_column_t));
    columns[column_num].name=NULL;

    for (token=strtok(format, "|"); token; token=strtok(NULL, "|"), column++) {
	int parenthesis_depth=0;
	int defined_len=0;
	char *data=NULL;
//	printf("new token: `%s'\n", token);
	for (c=token; *c; c++) {
//	    printf("new char: %c\n", *c);
	    switch (*c) {
	        case '(':
		    if (!parenthesis_depth++)
			data=c+1;
		    break;
	        case ')':
		    if (parenthesis_depth--==1)
			*c='\0';
		    break;
	        case ':':
		    if (!parenthesis_depth) {
			*(c++)='\0';
//			printf("trying to read number: `%s'\n", c+1); //optimizable
			if (sscanf(c,"%i",&column->len)) {
			    defined_len=1;
			    if (!column->len)
				expanding_columns++;
			    else if (strchr(c,'%'))
				column->len*=-1;
			    else
				remaining_cells-=column->len;
			} else
			    goto end_modifier;
		    }
		    break;
	    }
	}
        end_modifier:

	if (data)
	    column->data=strdup(data);

	for (column_type=column_types; column_type->name; column_type++)
	    if (!strncmp(token, column_type->name, strlen(column_type->name))) {
		column->name=strdup(column_type->name);
		column->printer=column_type->printer;
		column->offset=column_type->offset;
		if (!data)
		    if (column_type->data)
			column->data=strdup(column_type->data);
		    else
			column->data=NULL;
		if (!defined_len)
		    remaining_cells-=column->len=column_type->len;
		    if (!column->len)
			expanding_columns++;
		break;
	    }
	if (!column_type->name)
	    return -1; /* invalid column type */
    }

    if ((total_percentage=remaining_cells)<0)
	return -1; /* non-fixed size columns haven't got enough space */

    /* resolving percentages */
    for (i=0; columns[i].name; i++)
	if (columns[i].len<0) {
	    if ((columns[i].len=total_percentage*-columns[i].len/100)>variable_column_len) {
		variable_column=i;
		variable_column_len=columns[i].len;
	    }	    
	    remaining_cells-=columns[i].len;
	}

    if (remaining_cells<0)
	return -1; /* not enough free space for the expanding columns */

    /* resolving expanding columns */
//    printf("listen...\n");
    if (expanding_columns) {
	expanding_column_len=remaining_cells/expanding_columns;
	printf("expanding_column_len: %i\n", expanding_column_len);
	for (i=0; columns[i].name; i++)
	    if (!columns[i].len) {
		if (!variable_column_len) {
		    variable_column=i;
		    variable_column_len=0;
		}
		columns[i].len=expanding_column_len;
	    }
    }
    
    printf("remaining cells: %i\n", remaining_cells);
    printf("expanding_columns: %i\n", expanding_columns);

    if (panel->columns[0])
	free(panel->columns[0]);
    panel->columns[0]=columns;
/*    if (panel->columns[1])
	free(panel->columns[1]);
    panel->columns[1]=columns;
    panel->columns[1][variable_column].len++;*/
    return 0;
}


void draw_panel_entry(char* entry, int len)
{
    int i, slen;
    if ((slen=strlen(entry))>len) {
	addnstr(entry, len/2);
	attron(A_UNDERLINE);
	addch('~');
	attroff(A_UNDERLINE);
	addstr(entry+slen-len/2+(!(len%2)));
    } else {
	addstr(entry);
	for (i=slen; i<len; i++)
	    addch(' ');
    }
}

void draw_panel_row(file_entry_t *entry, panel_column_t *column, int row, int panel_row)
{
    int i=0;
    int field_num;
    if (entry) {
	field_num=entry->field_num;
	for (i=0; i<field_num; i++) {
	    if (panel_row==row)
		attrset(getpair(COLOR_BLACK, COLOR_CYAN));
	    else
		attrset(getpair(COLOR_WHITE, COLOR_BLACK));
	    draw_panel_entry(entry->fields[i], column[i].len);
	    if (i!=field_num-1) {
		attrset(getpair(COLOR_WHITE, COLOR_BLACK));
		addch(ACS_VLINE);
	    }
	}
    } else {
	int len=0;
	attrset(getpair(COLOR_WHITE, COLOR_BLACK));
	for (i=0; column[i].name; i++) {
	    len+=column[i].len;
	    if (i!=field_num-1)
		len++;
	}
	    draw_panel_entry("", len);
    }
}

file_entry_t *draw_panel_pane(file_entry_t *entry, panel_column_t *column, int row, int panel_row, rect_t *rect)
{
    int y;
    for (y=0; y<rect->h; y++) {
	move(rect->y+y, rect->x);
	draw_panel_row(entry, column, row++, panel_row);
	if (entry)
	    entry=entry->next;
    }
    return entry;
}



#define min(a, b) (a>b?b:a)

void draw_hline(int y, int x, int h)
{
    int pos;
    int end=y+h;
    for (pos=y; pos<end; pos++)
	mvaddch(pos, x, ACS_VLINE);
}


void draw_panel(panel_t *panel)
{
    int pane, i, row=0;
    rect_t rect=panel->link.rect;
    file_entry_t *entry = panel->file_list->head;
    int panes=get_panel_panes(panel);
    int pane_w=rect.w/panes;
    int pane_r=rect.w%panes;
    for (i=0; i<panel->index; i++) {
	entry=entry->next;
	row++;
    }
    for (pane=0; pane<panes; pane++, row+=rect.h) {
	rect.x = pane*pane_w+min(pane, pane_r);
	rect.w = pane_w+min(pane, pane_r);
	if (pane>0) {
	    attrset(getpair(COLOR_WHITE, COLOR_BLACK)|A_BOLD);
	    draw_hline(rect.y, rect.x-1, rect.h);
	}
	entry=draw_panel_pane(entry, panel->columns[0], row, panel->row, &rect);
    }
}

panel_t *new_panel()
{
    panel_t *panel = (panel_t*)xmalloc(sizeof(panel_t));
    *panel->columns = NULL;
    panel->columns[1]=NULL;
    panel->file_list=new_file_list();
    panel->target = NULL;
    panel->format=NULL;
    *panel->path='\0';
    panel->panes = 2;
    panel->index = 0;
    panel->row = 0;
    panel->col=0;
    panel->mode = PANEL_MODE_NORMAL;
}

void chdir_panel(panel_t *panel, char* dir)
{
    file_list_t *list;
    char* path;
    panel->index=0;
    panel->row=0;
    free_file_list(panel->file_list);
    if (list=chdir_file_list(dir))
	panel->file_list=list;
    getcwd(panel->path, PATH_MAX);
    cache_file_list(panel->file_list, panel->columns[0]);
}

int set_panel_row(panel_t *panel, int row)
{
    int len=get_panel_panes(panel)*panel->link.rect.h;
    int arow=panel->row+row;
    if (arow<0)
	panel->row=0;
    else if (arow>=panel->file_list->len)
	panel->row=panel->file_list->len-1;
    else
	panel->row=arow;
    if (panel->row<panel->index||panel->row>panel->index+len-1)
	panel->index+=row;
    if (panel->index<0)
	panel->index=0;
    else if (panel->index>panel->file_list->len-get_panel_panes(panel)*panel->link.rect.h)
	panel->index=panel->file_list->len-get_panel_panes(panel)*panel->link.rect.h;
}

void prev_panel_row(panel_t *panel)
{
    set_panel_row(panel, -1);
}

void next_panel_row(panel_t *panel)
{
    set_panel_row(panel, 1);
}

void prev_panel_page(panel_t *panel)
{
    set_panel_row(panel, -panel->link.rect.h);
}

void next_panel_page(panel_t *panel)
{
    set_panel_row(panel, panel->link.rect.h);
}

void prev_panel_pane(panel_t *panel)
{
    set_panel_row(panel, -panel->link.rect.h*get_panel_panes(panel));
}

void next_panel_pane(panel_t *panel)
{
    set_panel_row(panel, panel->link.rect.h*get_panel_panes(panel));
}

void move_panel_start(panel_t *panel)
{
    set_panel_row(panel, -panel->file_list->len);
}

void move_panel_end(panel_t *panel)
{
    set_panel_row(panel, panel->file_list->len);
}

void select_panel_entry(panel_t *panel)
{
//    char *path=(char*)malloc(PATH_MAX);
    file_entry_t *entry=get_panel_entry(panel);
    if (entry->st.st_mode&S_IFDIR) {
	strcat(panel->path, "/");
	strcat(panel->path, entry->fn);
	chdir_panel(panel, panel->path);
    } else {
	if (!fork()) {
	    char path[PATH_MAX];
	    strcpy(path, panel->path);
	    strcat(path, "/");
	    strcat(path, entry->fn);
	    mvaddstr(0,0,path);
//	    getch();
/*	    s=environ;
	    while (s) {
		addstr(*s);
		s++;
	    }
	    getch();*/
	    execl("/usr/local/bin/mplayer", "mplayer",  path);
//	    free(argv[0]);
	} else {
	    wait(NULL);
	    wrefresh(curscr);
	}
    }
}
file_entry_t *get_panel_entry(panel_t *panel)
{
    file_entry_t *entry=panel->file_list->head;
    int i;
    for (i=0; i<panel->row; i++) {
	if (!entry)
	    return NULL;
	entry=entry->next;
    }
    return entry;
}

