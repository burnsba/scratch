/*
The MIT License (MIT)

Copyright (c) <year> <copyright holders>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/



// This is a quick proof of concept showing pattern matching in a file
// using a state machine. And a hash table.
//
//
// example:
// 
// A file contains 20,000,000 lines of text (99.9 MB or 104,812,748 bytes) of 
//log output, such as the following:
//
//    2013-06-17:04:49:10.000;A;000050.edu.watermelon.wat0
//    2013-06-14:01:17:27.000;A;000051.edu.pear.pea1
//    2013-06-09:22:31:48.000;S;000052.edu.avocado.avo3
//    2013-06-12:09:32:09.000;A;000053.edu.durian.dur2
//    2013-06-22:14:06:59.000;F;000054.edu.boysenberry.boy0
//    2013-06-27:05:27:16.000;F;000055.edu.melon.mel1
//    2013-06-17:09:16:16.000;S;000056.edu.huckleberry.huc2
//    2013-06-22:06:14:17.000;F;000057.edu.dragonfruit.dra0
//    2013-06-12:00:12:46.000;S;000058.edu.lychee.lyc1
//    2013-06-19:15:29:04.000;A;000059.edu.pomegranate.pom3
//
// If the string ";S;" appears on the line, the six digits immediately
// following must be tracked. The number of times these six digits
// appears must be counted.
// 
// The code below will evaluate the entire file in about 10s.
// Sample output:
//    
//    
//    C:\code>main.exe data_large.txt
//    (before) there are 0 items in the hash
//    (after) there are 624768 items in the hash
//    hash key='000052', value(count)='1'
//    hash key='000056', value(count)='2'
//    hash key='000058', value(count)='3'
//    hash key='000060', value(count)='2'
//    hash key='000063', value(count)='2'
//    hash key='000064', value(count)='3'
//    hash key='000065', value(count)='3'
//    hash key='000069', value(count)='1'
//    hash key='000070', value(count)='2'
//    hash key='000071', value(count)='1'
//    hash key='000074', value(count)='3'
//    hash key='000076', value(count)='2'
//    hash key='000077', value(count)='2'
//    hash key='000080', value(count)='3'
//    hash key='000089', value(count)='2'
//    hash key='000090', value(count)='3'
//    hash key='000094', value(count)='3'
//    hash key='000100', value(count)='5'
//    hash key='000050', value(count)='45'
//    hash key='000072', value(count)='1'
//    hash key='000078', value(count)='2'
//    hash key='000084', value(count)='2'
//    hash key='000102', value(count)='2'
//    hash key='000106', value(count)='3'
//    hash key='000059', value(count)='1'
//    hash key='000098', value(count)='2'
//    hash key='000107', value(count)='3'
//    hash key='000110', value(count)='8'
//    hash key='000116', value(count)='4'
//    found 666343 matches, 41575 unique items
//
// Ben Burns
// July 09, 2013

#include <stdio.h>
#include "uthash.h"

/*
  get uthash at either
  https://github.com/troydhanson/uthash/blob/master/src/uthash.h
  http://troydhanson.github.io/uthash/
*/

#define MAX_KEY_NAME_LEN 10

// object to be hashed
typedef struct HASH_OBJECT
{
    char name[MAX_KEY_NAME_LEN];   /* key (string) */
    int count;                        /* additional data; used to track number of times item has appeard */
    UT_hash_handle hh;
} HASH_OBJECT;

// declare hash table
HASH_OBJECT *head = NULL;    /* important! initialize to NULL */

// Will search the hash table for a given key.
// If the hash object is found, the counter of the associated object
// is incremented. Otherwise, the key is inserted into the hash table.
void increment_count(char *key) {
    struct HASH_OBJECT *s;
    // search the hash table (head) for the given id (key), set pointer (s)
    // if it exists
    HASH_FIND_STR(head, key, s);
    if (s==NULL) 
    {
        // key not found in hash table
        s = (struct HASH_OBJECT*)malloc(sizeof(struct HASH_OBJECT));
        // set the id
        strcpy(s->name, key);
        // set counter (first time)
        s->count = 1;
        // insert into hash table
        HASH_ADD_STR(head, name, s );  /* id: name of key field */
    }
    else
    {
        // object exists in hash table, so increment
        s->count = s->count + 1;
    }
}

int main(int argc, char **argv)
{
    FILE* handle;
    int state = 0;
    char c = 0;
    int job_id_name_index = 0;
    
    char current_key[MAX_KEY_NAME_LEN];
    long matches_found = 0;
    
    long i = 0;         // loop var
    HASH_OBJECT *s;    // loop var

    // argument check
    if (argc != 2)
    {
        printf("no arguments.\n");
        return 0;
    }
    // open the file or die
    handle = fopen(argv[1], "r");
    if (!handle)
    {
        printf("Could not open file \"%s\"\n", argv[1]);
        return 0;
    }
    
    printf("(before) there are %d items in the hash\n", HASH_COUNT(head));
    
    do
    {
        // next character
        c = fgetc(handle);
        
        /* 2013-06-12:10:22:07.000;S;000050.edu.legume.leg3 */
        switch (c)
        {
            // looking for the string ";S;"
            case ';':
                if (state == 0) /* first semicolon */
                {
                    state++;
                }
                else if (state == 2) /* second semicolon */
                {
                    state++;
                    matches_found++;
                }
            break;
            case 'S':
                if (state == 1) /* after semicolon is an 'S' */
                {
                    state++;
                }
            break;
            case '.':
                
  			if (state == 3)
				{
					// insert key into hash table/increment
					increment_count(current_key);
				}
                
                // reset state machine
                state = 0;
                job_id_name_index = 0;
                memset(current_key, 0, 10);
                
                break;
            
            default:
                // found ";S;" start saving the name field, one character at a time
                if (state == 3)
                {
                    // safety check ...
                    if (job_id_name_index < MAX_KEY_NAME_LEN)
                    {
                        current_key[job_id_name_index] = c;
                        job_id_name_index++;
                    }
                }
                // otherwise, reset state machine
                else if (state != 3)
                {
                    state = 0;
                    job_id_name_index = 0;
                    memset(current_key, 0, 10);
                }
        }
    }
    while (c != EOF);
    fclose(handle);
    
    printf("(after) there are %d items in the hash\n", HASH_COUNT(head));

    i = 0;
    s = head;
    while (s != NULL && i++ < 30)
    {
        printf("hash key='%s', value(count)='%d'\n", s->name, s->count);
        s=s->hh.next;
    }
    
    printf("found %d matches, %d unique items\n", matches_found, matches_found - HASH_COUNT(head));
    
    return 0;
}
