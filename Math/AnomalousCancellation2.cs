using System.Text.RegularExpressions;

/// <summary>
/// Treats a string as a collection and enumerates combinations of elements.
/// Adjacent elements are grouped together and returned as one.
/// </summary>
/// <example>
/// FlatPrint(AdjacentCombinations("123"))
///     {3} {2} {23} {1} {1,3} {12} {123}
/// </example>
public static IEnumerable<List<string>> AdjacentCombinations(string source)
{
    bool canIterate = !(string.IsNullOrEmpty(source) || source.Length < 1);
    int len = canIterate ? source.Length : 0;
    var state = new int[len];
    
    while(canIterate)
    {
        bool inc = false;
        var ret = new List<string>();
        
        for (int i=len - 1; i>=0; i--)
        {
            if (state[i] == 0)
            {
                state[i]++;
                inc = true;
                break;
            }
            else
            {
                state[i] = 0;
            }
        }
        
        if (inc == false)
        {
            canIterate = false;
            break;
        }
        
        int startRun = -1;
        int runLen = 0;
        
        for (int i=0; i<len; i++)
        {
            if (state[i] == 1)
            {
                if (startRun < 0)
                {
                    startRun = i;
                }
                
                runLen++;
            }
            else
            {
                if (startRun >= 0)
                {
                    ret.Add(source.Substring(startRun, runLen));
                    startRun = -1;
                    runLen = 0;
                }
            }
        }
        
        if (startRun >= 0)
        {
            ret.Add(source.Substring(startRun, runLen));
            startRun = -1;
            runLen = 0;
        }
        
        yield return ret;
    }
}

public static string ReplaceFirst(this string text, string search, string replace)
{
    int pos = text.IndexOf(search);
    if (pos < 0)
    {
        return text;
    }
    
    return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
}


// An update
// Allows two digit factors, and multiple disjoint factors.
public static void Cancel(int start, int max, bool allowIntersectAfter = true, bool allowAmbiguous = false, bool allowFactors = true)
{
    int totalCount = 0;
    int duplicateCount = 0;
    
    var cancelled = new HashSet<long>();
    
    for (int num = start; num < max; num++)
    {
        for (int den = num + 1; den < max; den++)
        {
            var numString = num.ToString();
            var denString = den.ToString();
            
            var combinations = AdjacentCombinations(numString);
            var seen = new HashSet<string>();
            
            bool zeroEnding = numString.EndsWith("0") && denString.EndsWith("0");
            
            foreach (var set in combinations)
            {
                var hash = String.Join(",", set);
                if (seen.Contains(hash))
                {
                    continue;
                }

                seen.Add(hash);

                var workingNum = numString;
                var workingDen = denString;
                
                bool good = true;
                foreach (var s in set)
                {
                    if (!workingDen.Contains(s) || (zeroEnding && s.EndsWith("0")) || (set.Count == 1 && s.EndsWith("0")))
                    {
                        good = false;
                        break;
                    }
                    
                    int sn = int.Parse(s);
                    if (!allowFactors && sn > 0)
                    {
                        if (num % sn == 0 || den % sn == 0)
                        {
                            good = false;
                            break;
                        }
                    }
                    
                    if (!allowAmbiguous && Regex.Matches(workingNum, s).Count != Regex.Matches(workingDen, s).Count)
                    {
                        good = false;
                        break;
                    }
                    
                    workingDen = workingDen.ReplaceFirst(s, String.Empty);
                    workingNum = workingNum.ReplaceFirst(s, String.Empty);
                }
                
                if (good == false || String.IsNullOrEmpty(workingDen) || String.IsNullOrEmpty(workingNum))
                {
                    continue;
                }
                
                if (!allowIntersectAfter && workingNum.Intersect(workingDen).Any())
                {
                    continue;
                }
                
                var newNum = int.Parse(workingNum);
                var newDen = int.Parse(workingDen);
                
                // If the result got factored to nothing.
                if (newNum == 0 || newDen == 0)
                {
                    continue;
                }
                
                var lhs = num * newDen;
                var rhs = newNum * den;
                
                if (lhs == rhs)
                {
                    long numDenHash = (long)num * (long)max + (long)den;
                    if (!cancelled.Add(numDenHash))
                    {
                        duplicateCount++;
                    }
                    
                    Console.WriteLine($"Cancel {{{String.Join(",", set)}}} : {num} / {den} == {newNum} / {newDen}");
                    totalCount++;
                }
            }
        }
    }
    
    Console.WriteLine($"TotalCount: {totalCount}, unique: {totalCount - duplicateCount}");
}
