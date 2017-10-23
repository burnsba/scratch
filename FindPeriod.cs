public void FindPeriod(int digits, int n)
{
    int working = n;
    int start = -1;
    int startPeriod = 0;
    int ilimit = 10;

    for (int i=0; i<digits; i++)
    {
        ilimit *= 10;
    }

    for (int i=1; i<ilimit; i++)
    {
        if (start == working)
        {
            string msg = String.Format("Repeat at i={0}, working={1}, period={2}", i, working, i-startPeriod);
            Console.WriteLine(msg);
            break;
        }
        
        if (working.ToString().Length == digits && start == -1)
        {
            start = working;
            startPeriod = i;
            string msg = String.Format("Start at i={0}, start={1}", startPeriod, start);
            Console.WriteLine(msg);
        }
        
        working *= n;
        string t = working.ToString().PadLeft(digits);
        working = int.Parse(t.Substring(t.Length - digits, digits));
    }
}
