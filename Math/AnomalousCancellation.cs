// 2017-11-09
//
// This works more or less, but the defintion is ambiguous.

// Are uncancelled duplicates allowed?
// Limit this to only one digit, or more?
// This only considers a single digit that may occur more than once. That means
// - There is no way to consider multiple cancellations at the same time.
//   For instance, can't cancel the 2 and 3 in xxx2xx3xx / x23xxxxxx at the same time
// - In the same way, two digit or more numbers can't be cancelled. For instance,
//   can't cancel the 23 in x23 / 23x

// http://mathworld.wolfram.com/AnomalousCancellation.html

int min = 100;
int max = 1000;
int totalCount = 0;

for (int num = min; num <= max; num++)
{
    for (int den = min; den < num; den++)
    {
        if (num == den)
        {
            continue;
        }
        
        var numDigits = num.ToString().Select(x => x.ToString());
        var denDigits = den.ToString().Select(x => x.ToString());
        var distinct = numDigits.Distinct();
        
        foreach (var digit in distinct)
        {
            // If digit is zero
            if (digit == "0")
            {
                continue;
            }
            
            // If there's nothing to cancel.
            if (!denDigits.Contains(digit))
            {
                continue;
            }
            
            // If there are not equal number of the digit to cancel.
            var numCancelCount = numDigits.Where(x => x == digit).Count();
            var denCancelCount = denDigits.Where(x => x == digit).Count();
            if (numCancelCount != denCancelCount /* || numCancelCount != 1*/)
            {
                continue;
            }
            
            //// If this is dividing by 10.
            //if (digit == "0" && numDigits.Last() == "0" && denDigits.Last() == "0")
            //{
            //    continue;
            //}
            //
            var newNumComb = numDigits.Select((x,i) => Tuple.Create(x,i)).Where(x => x.Item1 != digit);
            var newDenComb = denDigits.Select((x,i) => Tuple.Create(x,i)).Where(x => x.Item1 != digit);
            
            var newNumDigits = newNumComb.Select(x => x.Item1);
            var newDenDigits = newDenComb.Select(x => x.Item1);
            
            var newNumIndeces = newNumComb.Select(x => x.Item2);
            var newDenIndeces = newDenComb.Select(x => x.Item2);
            
            // If the indeces for the cancelled digits are the same in the numerator and denominator.
            //if (newNumIndeces.SequenceEqual(newDenIndeces))
            //{
            //    continue;
            //}
            
            //var newNumDigits = numDigits.Where(x => x != digit);
            //var newDenDigits = denDigits.Where(x => x != digit);
            
            // If everything got cancelled in the numerator or denominator.
            if (newNumDigits.Count() == 0 || newDenDigits.Count() == 0)
            {
                continue;
            }
            
            //// If there are still overlapping digits that have not been cancelled.
            //if (newNumDigits.Intersect(newDenDigits).Any())
            //{
            //    continue;
            //}
            
            var newNum = int.Parse(String.Join(String.Empty, newNumDigits));
            var newDen = int.Parse(String.Join(String.Empty, newDenDigits));
            
            // If it got simplified to zero.
            if (newNum == 0 || newDen == 0)
            {
                continue;
            }
            
            var lhs = num * newDen;
            var rhs = newNum * den;
            
            if (lhs == rhs)
            {
                totalCount++;
                Console.WriteLine($"Cancel {digit}: {num} / {den} == {newNum} / {newDen}");
            }
        }
    }
}

Console.WriteLine($"TotalCount: {totalCount}");


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Update
// 
// This will consider any substring as a factor. For example "1234" will have factors {1, 2, 3, 4, 12, 23, 34, 123, 234}.
// 
// However, I think this is not enough, which is why this count is still wrong. From the mathworld page,
// there is 1019 / 5095 = 11 / 55 which I can not consider at all. 

public void CountAnomalousCancellation(int min, int max)
{
    int totalCount = 0;

    for (int num = min; num <= max; num++)
    {
        var numDivisors = GetDivisors(num);
        
        if (numDivisors.Contains(2) && numDivisors.Contains(5))
        {
            continue;
        }
        
        for (int den = num + 1; den < max; den++)
        {
            var numString = num.ToString();
            var denString = den.ToString();
                       
            foreach (var factor in ProperSubstrings(denString))
            {
                var f = int.Parse(factor);
                
                if (f == 0 || f % 10 == 0)
                {
                    continue;
                }
                
                var denDivisors = GetDivisors(den);
                
                if (numDivisors.Contains(f) || denDivisors.Contains(f))
                {
                    continue;
                }
                
                if (denDivisors.Contains(2) && denDivisors.Contains(5))
                {
                    continue;
                }
                
                // If there's nothing to factor.
                if (!numString.Contains(factor))
                {
                    continue;
                }
                
                var newNumString = numString.ReplaceFirst(factor, String.Empty);
                var newDenString = denString.ReplaceFirst(factor, String.Empty);
                
                // If the factor appears multiple times.
                if (newNumString.Contains(factor) || newDenString.Contains(factor))
                {
                    continue;
                }
                
                if (string.IsNullOrWhiteSpace(newNumString) || string.IsNullOrWhiteSpace(newDenString))
                {
                    continue;
                }
                
                var newNum = int.Parse(newNumString);
                var newDen = int.Parse(newDenString);
                
                // If the result got factored to nothing.
                if (newNum == 0 || newDen == 0)
                {
                    continue;
                }
                
                var lhs = num * newDen;
                var rhs = newNum * den;
                
                if (lhs == rhs)
                {
                    Console.WriteLine($"Cancel {factor}: {num} / {den} == {newNum} / {newDen}");
                    totalCount++;
                }
            }
        }
    }

    Console.WriteLine($"TotalCount: {totalCount}");
}

public IEnumerable<string> ProperSubstrings(string input)
{
    var ret = new List<string>();
    
    if (string.IsNullOrWhiteSpace(input))
    {
        yield break;
    }
    
    int len = input.Length;
    int maxLength = len - 1;
    
    if (maxLength < 1)
    {
        yield break;
    }
    
    int position = 0;
    int currentLength = 1; 
    
    while (true)
    {
        yield return input.Substring(position, currentLength);
        
        position++;
        
        if (position + currentLength > len)
        {
            position = 0;
            currentLength++;
        }
        
        if (currentLength > maxLength)
        {
            break;
        }
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

public static HashSet<int> Primes = new HashSet<int>();
public static List<int> PrimesList = new List<int>();
public static Dictionary<int, List<int>> DivisorsCash = new Dictionary<int, List<int>>();

public static void BuildPrimes()
{
    Primes.Clear();
    PrimesList.Clear();
    
    var max = 10000;
    var maxs = 100;
    
    var arr = Enumerable.Range(0, max).ToArray();
    arr[1] = 0;
    
    for (int i = 2; i < maxs; i++)
    {
        for (int j = i + i; j < max; j += i)
        {
            arr[j] = 0;
        }
    }
    
    foreach (var x in arr)
    {
        if (x > 0)
        {
            Primes.Add(x);
        }
    }
    
    PrimesList = Primes.ToList();
}

public static List<int> GetDivisors(int input)
{
    if (Primes.Count < 1)
    {
        BuildPrimes();
    }
    
    List<int> ret = null;
    
    if (!DivisorsCash.TryGetValue(input, out ret))
    {
        ret = new List<int>();
    }
    else
    {
        return ret;
    }
    
    if (Primes.Contains(input))
    {
        ret.Add(input);
        DivisorsCash[input] = ret;
        return ret;
    }
    
    var potentialDivisors = PrimesList.Where(x => x < input);
    
    foreach (var d in potentialDivisors)
    {
        int i;
        Math.DivRem(input, d, out i);
        
        if (i == 0)
        {
            ret.Add(d);
        }
    }
    
    DivisorsCash[input] = ret;
    return ret;
}
