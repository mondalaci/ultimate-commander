#include <dirent.h>
#include <grp.h>
#include <libgen.h>
#include <pwd.h>
#include <stdio.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <time.h>
#include "fileentry.h"
#include "mem.h"

#define BUFFSIZE 256

static char buffer[BUFFSIZE];

/* functions printing various entry infos */
char *sprint_fn(file_entry_t *entry, panel_column_t *column);
char *sprint_dev(file_entry_t *entry, panel_column_t *column);
char *sprint_num(file_entry_t *entry, panel_column_t *column);
char *sprint_mode_oct(file_entry_t *entry, panel_column_t *column);
char *sprint_mode_sym(file_entry_t *entry, panel_column_t *column);
char *sprint_uid_sym(file_entry_t *entry, panel_column_t *column);
char *sprint_gid_sym(file_entry_t *entry, panel_column_t *column);
char *sprint_size(file_entry_t *entry, panel_column_t *column);
char *sprint_time(file_entry_t *entry, panel_column_t *column);

file_entry_t *new_file_entry(char *fn)
{
    file_entry_t *entry=(file_entry_t*)xmalloc(sizeof(file_entry_t));
    entry->fn=strdup(fn);
    stat(fn, &entry->st);
    entry->marked=0;
    entry->field_num=0;
    entry->prev=NULL;
    entry->next=NULL;
    return entry;
}

void free_file_entry(file_entry_t *entry)
{
    int i;
    free(entry->fn);
    if (entry->field_num) {
	for (i=0; i<entry->field_num; i++)
	    free(entry->fields[i]);
	free(entry->fields);
    }
}

void cache_file_entry(file_entry_t *entry, panel_column_t *columns) {
    int len, i;
    for (len=0; columns[len].name; len++);
    entry->field_num=len;
    entry->fields=(char**)xmalloc(len*sizeof(char*));
    for (i=0; i<len; i++)
	entry->fields[i]=strdup(columns[i].printer(entry, &columns[i]));
}

struct stat *s;
panel_column_t column_types[] = {
    {"fn",       sprint_fn,       0,                                "",       0},
    {"dev",      sprint_dev,      (void*)&s->st_dev     - (void*)s, NULL,    -1},
    {"ino",      sprint_num,      (void*)&s->st_ino     - (void*)s, NULL,    -1},
    {"mode.oct", sprint_mode_oct, (void*)&s->st_mode    - (void*)s, NULL,     4},
    {"mode.sym", sprint_mode_sym, (void*)&s->st_mode    - (void*)s, NULL,    10},
    {"nlink",    sprint_num,      (void*)&s->st_nlink   - (void*)s, NULL,    -1},
    {"uid.num",  sprint_num,      (void*)&s->st_uid     - (void*)s, NULL,    -1},
    {"uid.sym",  sprint_uid_sym,  (void*)&s->st_uid     - (void*)s, NULL,    -1},
    {"gid.num",  sprint_num,      (void*)&s->st_gid     - (void*)s, NULL,    -1},
    {"gid.sym",  sprint_gid_sym,  (void*)&s->st_gid     - (void*)s, NULL,    -1},
    {"rdev",     sprint_dev,      (void*)&s->st_rdev    - (void*)s, NULL,    -1},
    {"size",     sprint_size,     (void*)&s->st_size    - (void*)s, NULL,    -1},
    {"blksize",  sprint_size,     (void*)&s->st_blksize - (void*)s, NULL,    -1},
    {"blocks",   sprint_num,      (void*)&s->st_blocks  - (void*)s, NULL,    -1},
    {"atime",    sprint_time,     (void*)&s->st_atime   - (void*)s, "%F %T", 19}, /* default length may be */
    {"mtime",    sprint_time,     (void*)&s->st_mtime   - (void*)s, "%F %T", 19}, /* different with other */
    {"ctime",    sprint_time,     (void*)&s->st_ctime   - (void*)s, "%F %T", 19}, /* format specifications */
    {"ord",      NULL,            0,                                NULL,     4},
    {NULL,       NULL,            0,                                NULL,     0}
};

void init_panel_cols()
{
    char s[256];
    struct passwd *passwd;
    struct group *group;
    int l, len = 0;
    static lengths[] = /* ceil(log(10, 8^i)) */
	{0, 3, 5, 8, 10, 13, 15, 17, 20};

    column_types[1].len=column_types[10].len=2*sizeof(short)+1;  /* the exceptions */
    column_types[2].len=lengths[sizeof(ino_t)];
    column_types[5].len=lengths[sizeof(nlink_t)];
    column_types[6].len=lengths[sizeof(uid_t)];
    column_types[8].len=lengths[sizeof(gid_t)];
    column_types[11].len=lengths[sizeof(off_t)];
    column_types[12].len=column_types[13].len=lengths[sizeof(unsigned long)];

    setpwent();
    while (passwd = getpwent())
	if ((l=(strlen(passwd->pw_name)))>len)
	    len = l;
    endpwent();
    column_types[7].len = len;

    len = 0;
    setgrent();
    while (group = getgrent())
	if ((l=(strlen(group->gr_name)))>len)
	    len = l;
    endgrent();
    column_types[9].len = len;
}

char *sprint_fn(file_entry_t *entry, panel_column_t *column)
{
    int buffsize=BUFFSIZE;
    char *s=buffer;
    if (strchr(column->data, 'p')) {
	switch (entry->st.st_mode&S_IFMT) {
            case S_IFIFO:
		*buffer = '|';
		break;
            case S_IFCHR:
		*buffer = '-';
		break;
            case S_IFDIR:
		*buffer = '/';
		break;
            case S_IFBLK:
		*buffer = '+';
		break;
            case S_IFREG:
		*buffer = ' ';
		break;
            case S_IFLNK:
		*buffer = '@';
		break;
            case S_IFSOCK:
		*buffer = '=';
		break;
	}
	buffsize--;
	s++;
    }
    if (strchr(column->data, 'a'))
	strncpy(s, entry->fn, buffsize);
    else
	strncpy(s, basename(entry->fn), buffsize);
    return buffer;
}

char *sprint_dev(file_entry_t *entry, panel_column_t *column)
{
    dev_t dev=*(dev_t*)((void*)&entry->st+column->offset);
    sprintf(buffer, "%lli,%lli", (dev&0xff00)>>8, dev&0xff);
    return buffer;
}

char *sprint_num(file_entry_t *entry, panel_column_t *column)
{
    sprintf(buffer, "%i", *(int*)((void*)&entry->st+column->offset));
    return buffer;
}

char *sprint_mode_oct(file_entry_t *entry, panel_column_t *column)
{
    sprintf(buffer, "%o", entry->st.st_mode&0xfff);
    return buffer;
}

char *sprint_mode_sym(file_entry_t *entry, panel_column_t *column)
{
    mode_t mode = entry->st.st_mode;

    switch (mode&S_IFMT) {
        case S_IFIFO:
	    *buffer = 'p';
	    break;
        case S_IFCHR:
	    *buffer = 'c';
	    break;
        case S_IFDIR:
	    *buffer = 'd';
	    break;
        case S_IFBLK:
	    *buffer = 'b';
	    break;
        case S_IFREG:
	    *buffer = '-';
	    break;
        case S_IFLNK:
	    *buffer = 'l';
	    break;
        case S_IFSOCK:
	    *buffer = 's';
	    break;
    }

    if (mode&S_IRUSR)
	buffer[1] = 'r';
    else
	buffer[1] = '-';

    if (mode&S_IWUSR)
	buffer[2] = 'w';
    else
	buffer[2] = '-';

    switch (mode&(S_IXUSR|S_ISUID)) {
        case S_IXUSR:
	    buffer[3] = 'x';
	    break;
        case S_ISUID:
	    buffer[3] = 'S';
	    break;
        case S_IXUSR|S_ISUID:
	    buffer[3] = 's';
	    break;
        default:
	    buffer[3] = '-';
    }

    if (mode&S_IRGRP)
	buffer[4] = 'r';
    else
	buffer[4] = '-';

    if (mode&S_IWGRP)
	buffer[5] = 'w';
    else
	buffer[5] = '-';

    switch (mode&(S_IXGRP|S_ISGID)) {
        case S_IXGRP:
	    buffer[6] = 'x';
	    break;
        case S_ISGID:
	    buffer[6] = 'S';
	    break;
        case S_IXGRP|S_ISGID:
	    buffer[6] = 's';
	    break;
        default:
	    buffer[6] = '-';
    }
    if (mode&S_IROTH)
	buffer[7] = 'r';
    else
	buffer[7] = '-';

    if (mode&S_IWOTH)
	buffer[8] = 'w';
    else
	buffer[8] = '-';

    switch (mode&(S_IXOTH|S_ISVTX)) {
        case S_IXOTH:
	    buffer[9] = 'x';
	    break;
        case S_ISVTX:
	    buffer[9] = 'T';
	    break;
        case S_IXOTH|S_ISVTX:
	    buffer[9] = 't';
	    break;
        default:
	    buffer[9] = '-';
    }

    buffer[10] = '\0';
    return buffer;
}

char *sprint_uid_sym(file_entry_t *entry, panel_column_t *column)
{
    struct passwd *passwd = getpwuid(entry->st.st_uid);
    return strncpy(buffer, passwd->pw_name, BUFFSIZE);
}

char *sprint_gid_sym(file_entry_t *entry, panel_column_t *column)
{
    struct group *group = getgrgid(entry->st.st_gid);
    return strncpy(buffer, group->gr_name, BUFFSIZE);
}

char *sprint_size(file_entry_t *entry, panel_column_t *column)
{
    char postfixes[]="BKMGTPEZY";
    int power=1024, base=1, i=0;
    int size=*(int*)((void*)&entry->st+column->offset);
    int limit1=1, limit2=1;
    for (i=0; i<column->len; i++) {
	limit1=limit2;
	limit2*=10;
    }
    if (column->data) { /* SI data measurement: power of 1000 */
	for (i=0; i<strlen(postfixes); i++)
	    postfixes[i]=tolower(postfixes[i]);
	power=1000;
    }
    i=0;
    do {
	if (i?size/base<limit1:size/base<limit2) {
	    if (i)
		sprintf(buffer, "%i%c", size/base, postfixes[i]);
	    else
		sprintf(buffer, "%i", size/base);
	    break;
	}
	base*=power;
	i++;
    } while (1);
    return buffer;
}

char *sprint_time(file_entry_t *entry, panel_column_t *column)
{
    strftime(buffer, BUFFSIZE, column->data, localtime((time_t*)((void*)&entry->st+column->offset)));
    return buffer;
}
