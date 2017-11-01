public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public override bool Equals(Object obj)
    {
        Point p = obj as Point;
        
        if (Object.ReferenceEquals(null, p) || Object.ReferenceEquals(this, obj))
        {
            return Object.ReferenceEquals(this, obj);
        }
        
        return X == p.X && Y == p.Y;
    }
    
    public static bool operator ==(Point p1, Point p2)
    {
        if (Object.ReferenceEquals(null, p1))
        {
            return Object.ReferenceEquals(p1, p2);
        }
        
        return p1.Equals(p2);
    }
    
    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1 == p2);
    }
    
    public override int GetHashCode()
    {
        return X ^ ~Y;
    }
    
    public override string ToString()
    {
        return  $"{{{X}, {Y}}}";
    }
}

public static class ApplicationSettings
{
    public const int BoardX = 8;
    public const int BoardY = 8;
}

public class PotentialMove
{
    public Point Position { get; set; }
    public List<Point> Available { get; set; }
}

public static bool IsWithinBoard(Point p)
{
    if (Object.ReferenceEquals(null, p))
    {
        return false;
    }
    
    return p.X >= 0 && p.X < ApplicationSettings.BoardX &&
            p.Y >= 0 && p.Y < ApplicationSettings.BoardY;
}

public static List<Point> GetKnightMoves(Point startingPoint)
{
    var results = new List<Point>();
    
    if (Object.ReferenceEquals(null, startingPoint))
    {
        return results;
    }
    
    Point p = null;
    p = new Point() { X = startingPoint.X + 2, Y = startingPoint.Y + 1 };
    if (IsWithinBoard(p)) { results.Add(p); }
    p = new Point() { X = startingPoint.X + 2, Y = startingPoint.Y - 1 };
    if (IsWithinBoard(p)) { results.Add(p); }
    p = new Point() { X = startingPoint.X - 2, Y = startingPoint.Y + 1 };
    if (IsWithinBoard(p)) { results.Add(p); }
    p = new Point() { X = startingPoint.X - 2, Y = startingPoint.Y - 1 };
    if (IsWithinBoard(p)) { results.Add(p); }
    p = new Point() { X = startingPoint.X + 1, Y = startingPoint.Y + 2 };
    if (IsWithinBoard(p)) { results.Add(p); }
    p = new Point() { X = startingPoint.X + 1, Y = startingPoint.Y - 2 };
    if (IsWithinBoard(p)) { results.Add(p); }
    p = new Point() { X = startingPoint.X - 1, Y = startingPoint.Y + 2 };
    if (IsWithinBoard(p)) { results.Add(p); }
    p = new Point() { X = startingPoint.X - 1, Y = startingPoint.Y - 2 };
    if (IsWithinBoard(p)) { results.Add(p); }
    
    return results;
}

public static void Visit()
{
    var startingPoint = new Point() { X = 1, Y = 1 };
    var startingMoves = GetKnightMoves(startingPoint);
    var currentPotentialMove = new PotentialMove() { Position = startingPoint, Available = startingMoves };
    
    var visited = new List<Point>();
    var travelPath = new List<PotentialMove>();
    
    travelPath.Add(currentPotentialMove);
    visited.Add(startingPoint);
    
    while(true)
    {
        if (travelPath.Count < 1 || travelPath.Count > 63)
        {
            break;
        }
        
        var potential = travelPath.Last();
        Point nextMove = null;
        while (Object.ReferenceEquals(null, nextMove) && potential.Available.Count > 0)
        {
            nextMove = potential.Available.First();
            potential.Available.RemoveAt(0);
            if (visited.Contains(nextMove))
            {
                nextMove = null;
            }
        }
        
        if (Object.ReferenceEquals(null, nextMove))
        {
            visited.Remove(potential.Position);
            travelPath.RemoveAt(travelPath.Count - 1);
            continue;
        }
        
        var nextAvailable = GetKnightMoves(nextMove);
        var nextPotential = new PotentialMove() { Position = nextMove, Available = nextAvailable };
        
        visited.Add(nextMove);
        
        travelPath.Add(nextPotential);
    }
    
    Console.WriteLine(String.Join(",", visited.Select(x => x.ToString())));
}
