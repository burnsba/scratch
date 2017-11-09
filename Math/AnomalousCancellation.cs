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
