#ifndef __INPUT_H
#define __INPUT_H

#include <link.h>

#define KEY_RETURN 13
#define KEY_ESC 0x1000
#define ASCII_ESC 0x1b

#define MOD_MASK 0x8000
#define CTRL_MASK 0xff9f
#define KEY_MASK 0x00ff

#define MAKE_KEY(c, m, k) ((m?MOD_MASK:0)|(c?CTRL_MASK:0xffff)&k)

#define GET_KEY_CTRL(k) ((KEY_MASK&k)<27) /* ctrl */
#define GET_KEY_MOD(k) (k&MOD_MASK) /* the modifier (generally alt) */
#define GET_KEY_CHAR(k) ((GET_KEY_CTRL(k)?k+96:k))

typedef int input_t;

typedef struct input_entry_t {
    input_t input;
    int (*handler)(int numarg);
    struct input_entry_t *next;
} input_entry_t;

typedef struct input_list_t {
    input_entry_t entry;
    
} input_list_t;

/* born and death */
input_entry_t *new_key(int mod, int ctrl, int key);
void destroy_key(input_entry_t *entry);

void *lookup_key_in_table(int key, input_entry_t *table);

void main_input_loop();
void add_keymap(input_entry_t *target, input_entry_t *source);
void add_input_entry();

extern input_entry_t *root_input_map;
extern struct link_t *minibuffer;

#endif __INPUT_H
