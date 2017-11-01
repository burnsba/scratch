public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Point() { }
    public Point(int x, int y)
    {
        X = x;
        Y = y;
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
    
    public override bool Equals(Object obj)
    {
        var p = obj as Point;
        
        if (Object.ReferenceEquals(null, p) || Object.ReferenceEquals(this, obj))
        {
            return Object.ReferenceEquals(this, obj);
        }
        
        return X == p.X && Y == p.Y;
    }
    
    public override int GetHashCode()
    {
        unchecked { return X + Y; }
    }
    
    public int GetHashCode(int size)
    {
        return X * size + Y;
    }
    
    public override string ToString()
    {
        return String.Format("{0}, {1}", X, Y);
    }
}

public class Board
{
    public int BoardX { get; set; }
    public int BoardY { get; set; }

    public bool IsWithinBounds(Point p)
    {
        return p.X >= 0 && p.X < BoardX &&
                p.Y >= 0 && p.Y < BoardY;
    }

    public List<Point> GetKnightMoves(Point startingPoint)
    {
        var results = new List<Point>();
        
        Point p;
        
        p = new Point(startingPoint.X + 2, startingPoint.Y + 1);
        if (IsWithinBounds(p)) { results.Add(p); }
        p = new Point(startingPoint.X + 2, startingPoint.Y - 1);
        if (IsWithinBounds(p)) { results.Add(p); }
        p = new Point(startingPoint.X - 2, startingPoint.Y + 1);
        if (IsWithinBounds(p)) { results.Add(p); }
        p = new Point(startingPoint.X - 2, startingPoint.Y - 1);
        if (IsWithinBounds(p)) { results.Add(p); }
        p = new Point(startingPoint.X + 1, startingPoint.Y + 2);
        if (IsWithinBounds(p)) { results.Add(p); }
        p = new Point(startingPoint.X + 1, startingPoint.Y - 2);
        if (IsWithinBounds(p)) { results.Add(p); }
        p = new Point(startingPoint.X - 1, startingPoint.Y + 2);
        if (IsWithinBounds(p)) { results.Add(p); }
        p = new Point(startingPoint.X - 1, startingPoint.Y - 2);
        if (IsWithinBounds(p)) { results.Add(p); }
        
        return results;
    }

    public int GetDistance(Point startingPoint, Point destination)
    {
        int moveCount = 0;
        
        var considerations = new List<Point>();
        considerations.Add(startingPoint);
        Point current = null;
        
        var seen = new HashSet<int>();

        while(true)
        {
            var nextIteration = new List<Point>();
            
            if (considerations.Count == 0)
            {
                break;
            }
            
            foreach (var node in considerations)
            {
                if (node == destination)
                {
                    return moveCount;
                }
                
                seen.Add(node.GetHashCode(BoardX));
                
                var possibleMoves = GetKnightMoves(node);
                foreach (var possibleMove in possibleMoves)
                {
                    int hash = possibleMove.GetHashCode(BoardX);
                    
                    if (!seen.Contains(hash))
                    {
                        seen.Add(hash);
                        nextIteration.Add(possibleMove);
                    }
                }
            }
            
            considerations = nextIteration;
            
            moveCount++;
        }

        return -1;
    }
}

/*
// example:
Board b = new Board() { BoardX = 1024, BoardY = 1024 };
b.GetDistance(new Point(1,1), new Point(1000,1000))
*/
