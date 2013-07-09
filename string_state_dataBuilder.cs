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

// Just threw some code together to generate test data for the string_state.c
// proof of concept.

using System;

namespace DataBuilder
{
    class Program
    {
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        static void Main(string[] args)
        {
            const int TOTAL_LINES = 2000000;
            const string FILE_NAME = "data.txt";

            string[] HOST_NAMES = { "apple", "apricot", "avocado", "banana", "breadfruit", "bilberry", "blackberry", 
                                     "blackcurrant", "blueberry", "boysenberry", "currant", "cherry", "cherimoya", 
                                     "chili", "cloudberry", "coconut", "damson", "date", "dragonfruit", "durian", 
                                     "elderberry", "feijoa", "fig", "gooseberry", "grape", "grapefruit", "guava", 
                                     "huckleberry", "honeydew", "jackfruit", "jettamelon", "jambul", "jujube", "kiwi", 
                                     "kumquat", "legume", "lemon", "lime", "loquat", "lychee", "mango", "melon", 
                                     "cantaloupe", "honeydew", "watermelon", "nectarine", "nut", "orange", "clementine", 
                                     "mandarine", "tangerine", "papaya", "peach", "pepper", "pear", "persimmon", "physalis", 
                                     "pineapple", "pomegranate", "pomelo", "quince", "raspberry", "rambutan", "redcurrant", 
                                     "satsuma", "strawberry", "tamarillo", "tomato" };

            string[] JOB_STATES = { "S", "F", "A" };

            const int epochStateTime = 1370044800; // june 1, 2013
            const int epochEndTime = 1372550400; // june 30, 2013

            DateTime timeToWrite;

            const int startingJobId = 50;
            int jobId = startingJobId;
            int maxJobId = 100;
            int jobIncrement = 1;

            Random r = new Random();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FILE_NAME))
            {
                for (int i = 0; i < TOTAL_LINES; i++)
                {
                    int randomTime = r.Next(epochStateTime, epochEndTime);
                    timeToWrite = FromUnixTime(randomTime);

                    int jobStateIndex = r.Next(0, JOB_STATES.Length);
                    int hostNameInedx = r.Next(0, HOST_NAMES.Length);
                    int subDomainIndex = r.Next(0, 4);

                    string jobState = JOB_STATES[jobStateIndex];
                    string hostName = HOST_NAMES[hostNameInedx];
                    string subDomain = String.Format("{0}{1}", hostName.Substring(0, 3), subDomainIndex.ToString());

                    string url = String.Format(".edu.{0}.{1}", hostName, subDomain);

                    string toWrite = String.Format("{0};{1};{2}{3}",
                        timeToWrite.ToString("yyyy/MM/dd:HH:mm:ss.fff"),
                        jobState,
                        jobId.ToString("D6"),
                        url
                        );

                    file.WriteLine(toWrite);

                    jobId = jobId + jobIncrement;
                    if (jobId > maxJobId)
                    {
                        jobId = startingJobId;
                        jobIncrement++;
                        maxJobId = (int)(((double)maxJobId) * 1.1);
                    }
                }
            }
        }
    }
}
