#include <dirent.h>
//#include <string.h>
#include <libgen.h>
#include "filelist.h"

file_list_t *new_file_list()
{
    file_list_t *list=(file_list_t*)xmalloc(sizeof(file_list_t));
    list->len=0;
    list->head=NULL;
    list->tail=NULL;
    return list;
}

void free_file_list(file_list_t *list)
{
    file_entry_t *next, *current;
    if (!list)
	return;
    if (!list->head)
	return;
    current=list->head;
    do {
	next=current->next;
	free(current);
    } while (current=next);
}

void add_file_to_list(file_list_t *list, char *fn)
{
    file_entry_t *entry=new_file_entry(fn);
    if (list->tail)
	list->tail->next=entry;
    entry->prev=list->tail;
    entry->next=NULL;
    if (!list->head)
	list->head=entry;
    list->tail=entry;    
    list->len++;
}

void cache_file_list(file_list_t *list, panel_column_t *columns)
{
    file_entry_t *entry;
    for (entry=list->head; entry; entry=entry->next)
	cache_file_entry(entry, columns);
}

file_list_t *chdir_file_list(char *sdir)
{
    DIR *dir;
    struct dirent *ent;
    file_list_t* file_list=new_file_list();
    file_list->len=0;
    file_list->head=NULL;
    if (chdir(sdir))
	return NULL;
    if (!(dir = opendir(sdir)))
	return NULL;
    while (ent = readdir(dir))
	add_file_to_list(file_list, ent->d_name);
    closedir(dir);
    return file_list;
}

void remove_entry(file_list_t *list, file_entry_t *entry)
{
    if (!strcmp(basename(entry->fn), ".."))
	return;
    if (entry->prev)
	entry->prev->next=entry->next;
    else
	list->head=entry->next;
    if (entry->next)
	entry->next->prev=entry->prev;
    else
	list->tail=entry->prev;
    list->len--;
}

void move_entry(file_list_t *list, file_entry_t *dest, file_entry_t *src, int pos)
{
    /* currently broken */
    /* unlacing old link */
/*    if (entry->prev)
	entry->prev->next=entry->next;
    else
	list->head=entry->next;
    if (entry->next)
	entry->next->prev=entry->prev;
    else
    list->tail=entry->prev;*/
    /* lacing up new link*/
/*    if (!(entry->prev->next=entry->next))
	list->tail=src;
	dest->next=entry->prev;*/
}

void shift_selected_entries(file_list_t *list, int space, int congestion)
{
    file_entry_t *entry, *last_selected_entry, *next_entry_ref;
    
}
