//   Copyright 2013 Benjamin Burns
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
