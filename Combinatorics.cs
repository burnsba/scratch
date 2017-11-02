public IEnumerable<List<T>> ChooseWithDuplicatesWithOrder<T>(List<T> source)
{
    var digits = source.Count;
    var selection = new int[digits];
    
    for (int i=0; i<digits; i++)
    {
        selection[i] = 0;
    }
    
    int currentSelectionIndex = 0;
    int maxItems = 1;
    int overflowCount;
    
    while (currentSelectionIndex < digits && maxItems <= digits)
    {
        var results = new List<T>();
        for (int i=0; i<digits && i<maxItems; i++)
        {
            results.Add(source[selection[i]]);
        }
        
        overflowCount = 0;
        for (int i=currentSelectionIndex; i>=0; i--)
        {
            selection[i]++;
            
            if (selection[i] >= digits)
            {
                selection[i] = 0;
                overflowCount++;
            }
            else
            {
                break;
            }
        }
        
        if (overflowCount > currentSelectionIndex)
        {
            currentSelectionIndex++;
            maxItems++;
        }
        
        yield return results;
    }
}

public IEnumerable<List<T>> ChooseWithDuplicatesWithOrder<T>(List<T> source)
{
    var digits = source.Count;
    var selection = new int[digits];
    
    for (int i=0; i<digits; i++)
    {
        selection[i] = 0;
    }
    
    int currentSelectionIndex = 0;
    int maxItems = 1;
    int overflowCount;
    
    var seen = new HashSet<int>();
    bool duplicates = false;
    
    while (currentSelectionIndex < digits && maxItems <= digits)
    {
        var results = new List<T>();
        duplicates = false;
        seen.Clear();
        for (int i=0; i<digits && i<maxItems; i++)
        {
            results.Add(source[selection[i]]);
            
            duplicates |= seen.Contains(selection[i]);
            seen.Add(selection[i]);
        }
        
        overflowCount = 0;
        for (int i=currentSelectionIndex; i>=0; i--)
        {
            selection[i]++;
            
            if (selection[i] >= digits)
            {
                selection[i] = 0;
                overflowCount++;
            }
            else
            {
                break;
            }
        }
        
        if (overflowCount > currentSelectionIndex)
        {
            currentSelectionIndex++;
            maxItems++;
        }
        
        if (!duplicates)
        {
            yield return results;
        }
    }
}
