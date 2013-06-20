#include "stdio.h"

// quick demo file to see the differences between an array
// and a pointer to a string.

// amessage is an array of characters in the local stack. Changing 
// the contents of the array is acceptable, but amessage can't point
// to anything else.
char amessage[] = "text";

// pmessage is a pointer in the local stack. The string is implicitly
// constant, and probably assigned in a section of read-only memory.
// Attempting to change the contents by dereferencing as an array may,
// but is not guaranteed, to cause a segmentation fault. However, 
// pmessage can point to another object.
char *pmessage = "1234";

int main()
{
    // dereferencing amessage as an array and changing it is fine.
    amessage[0] = 'a';
    // dereferencing pmessage as an array and changing it is not fine
    //pmessage[0] = 'b'; /* segmentation fault */

    // changing the pointer is acceptable though
    pmessage = amessage;

    // brackets are notation for +
    // arr[i] is something like *(arr + i)
    // following is output showing bracket notation and pmessage
    // pointer change
    printf("amessage[0]: %c\n", amessage[0]); // output: a
    printf("0[amessage]: %c\n", 0[amessage]); // output: a
    printf("pmessage[0]: %c\n", pmessage[0]); // output: a
    printf("1[\"asdf\"]: %c\n", 1["asdf"]); // output: s
    printf("\"asdf\"[1]: %c\n", "asdf"[1]); // output: s


    // bonus: on some compilers this is acceptable:
    // int b=3; int c=5;
    // int a=b+++-++++c; // a now holds -4

    return 0;
}
