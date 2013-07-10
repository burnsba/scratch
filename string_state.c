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
// A file contains 20,000,000 lines of text (293 MB or 307,405,822 bytes) of 
//log output, such as the following:
//
//    06/06/2013 02:21:12.000;pbs_server:19705;A;000050.gua1.guava.edu;Resource.List=dmc61/1,dmc61/1,dmc50/1,ncpus=2,nodes=1,mem=1gb,lqueue=small-serial;Torque 14097
//    03/06/2013 11:38:00.000;pbs_server:19705;A;Job;Resource.List=dmc57/1,dmc57/2,dmc50/1,ncpus=1,nodes=2,mem=2gb,lqueue=small-serial;Torque 14373
//    18/06/2013 04:59:53.000;pbs_server:19705;F;000052.hon0.honeydew.edu;Resource.List=dmc57/2,dmc57/1,dmc52/2,ncpus=1,nodes=1,mem=1gb,lqueue=small-serial;Torque 14659
//    19/06/2013 22:31:22.000;pbs_server:19705;F;Job;Resource.List=dmc65/1,dmc65/2,dmc50/1,ncpus=2,nodes=2,mem=2gb,lqueue=small-serial;Torque 15330
//    28/06/2013 03:53:43.000;pbs_server:19705;F;000054.clo3.cloudberry.edu;Resource.List=dmc56/1,dmc56/1,dmc51/1,ncpus=2,nodes=1,mem=1gb,lqueue=small-serial;Torque 15412
//    16/06/2013 02:46:52.000;pbs_server:19705;S;Job;Resource.List=dmc60/1,dmc60/1,dmc50/1,ncpus=1,nodes=1,mem=1gb,lqueue=small-serial;Torque 14129
//    25/06/2013 18:24:13.000;pbs_server:19705;S;000056.bil1.bilberry.edu;Resource.List=dmc59/2,dmc59/2,dmc52/2,ncpus=1,nodes=2,mem=2gb,lqueue=small-serial;Torque 15741
//    11/06/2013 14:48:28.000;pbs_server:19705;F;Job;Resource.List=dmc57/2,dmc57/1,dmc53/2,ncpus=2,nodes=1,mem=1gb,lqueue=small-serial;Torque 15975
//    27/06/2013 04:41:47.000;pbs_server:19705;F;000058.coc2.coconut.edu;Resource.List=dmc63/1,dmc63/1,dmc52/1,ncpus=2,nodes=1,mem=1gb,lqueue=small-serial;Torque 15879
//
// If the string ";S;" appears on the line, the six digits immediately
// following must be tracked. The number of times these six digits
// appears must be counted. If there are no digits, ignore.
// 
// The code below will evaluate the entire file in about 15s.
// Sample output:
//    
//    
//        C:\code>main.exe data_large.txt
//        (before) there are 0 items in the hash
//        (after) there are 322909 items in the hash
//        hash key='000056', value(count)='1'
//        hash key='000060', value(count)='2'
//        hash key='000068', value(count)='3'
//        hash key='000080', value(count)='2'
//        hash key='000084', value(count)='1'
//        hash key='000086', value(count)='1'
//        hash key='000090', value(count)='2'
//        hash key='000092', value(count)='2'
//        hash key='000096', value(count)='2'
//        hash key='000098', value(count)='4'
//        hash key='000100', value(count)='1'
//        hash key='000052', value(count)='1'
//        hash key='000104', value(count)='3'
//        hash key='000108', value(count)='1'
//        hash key='000110', value(count)='3'
//        hash key='000074', value(count)='1'
//        hash key='000106', value(count)='2'
//        hash key='000114', value(count)='1'
//        hash key='000065', value(count)='1'
//        hash key='000075', value(count)='1'
//        hash key='000085', value(count)='1'
//        hash key='000105', value(count)='1'
//        hash key='000115', value(count)='2'
//        hash key='000125', value(count)='1'
//        hash key='000145', value(count)='2'
//        hash key='000152', value(count)='2'
//        hash key='000050', value(count)='21'
//        hash key='000120', value(count)='1'
//        hash key='000134', value(count)='2'
//        hash key='000162', value(count)='2'
//        found 333897 matches, 10988 unique items
//
// Ben Burns
// July 10, 2013

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

// well, this will work for ascii. Hopefully there's no unicode or anything ...
int isnumeric(unsigned char c)
{
    if (c < '0') 
        return 0;
    return !(c > '9');
}

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
    
    memset(current_key, 0, 10);
    printf("(before) there are %d items in the hash\n", HASH_COUNT(head));
    
    do
    {
        // next character
        c = fgetc(handle);


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
                }
                else
                    state = 0;
            break;
            
            case 'S':
                if (state == 1) /* after semicolon is an 'S' */
                {
                    state++;
                }
                else
                    state = 0;
            break;
            
            default:
            
                // found ";S;" start saving the name field, one character at a time
                if (state == 3)
                {
                    if (isnumeric(c))
                    {
                        // safety check ...
                        if (job_id_name_index < MAX_KEY_NAME_LEN)
                        {
                            current_key[job_id_name_index] = c;
                            job_id_name_index++;
                        }
                    }
                    else
                        state = 0;
                }
            
                // otherwise, reset state machine.
                // state can change above.
                if (state != 3)
                {
                    // check to see if the latest key has been inserted
                    // or it's still reset since last time.
                    if (current_key[0] != 0)
                    {
                        matches_found++;
                        increment_count(current_key);
                    }
                
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
