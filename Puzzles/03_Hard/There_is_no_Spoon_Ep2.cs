//
//http://www.conceptispuzzles.com/de/index.aspx?uri=puzzle/hashi/techniques
//
//https://en.wikipedia.org/wiki/Hashiwokakero
//
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

/**
 * The machines are gaining ground. Time to show them what we're really made of...
 **/
class Solution
{
    //Static list of node neighbours
    static Dictionary<Point, List<Point>> Neighbours = new Dictionary<Point, List<Point>>();

    public class PuzzleState
    {
        //Node position and remaining value
        public Dictionary<Point, int> Nodes;
        //Link endpoints and value (1 or 2)
        public Dictionary<Tuple<Point, Point>, int> Links;
        //Cache intersection
        public HashSet<Point> Intersections;
        //
        public PuzzleState()
        {
            Nodes = new Dictionary<Point, int>();
            Links = new Dictionary<Tuple<Point, Point>, int>();
            Intersections = new HashSet<Point>();
        }
        //
        public PuzzleState(PuzzleState puzzleState)
        {
            Nodes = new Dictionary<Point, int>(puzzleState.Nodes);
            Links = new Dictionary<Tuple<Point, Point>, int>(puzzleState.Links);
            Intersections = new HashSet<Point>(puzzleState.Intersections);
        }

        //
        public static int GetNodeDegreesRequired(int value)
        {
            return (value / 2) + (value % 2);
        }
        public static Tuple<Point, Point> Canonical(Point a, Point b)
        {
            //Links are either vertical or horizontal, non-zero length
            if (a.X == b.X)
            {
                //Vertical
                if (a.Y < b.Y) return new Tuple<Point, Point>(a, b);
                return new Tuple<Point, Point>(b, a);
            }
            //Horizontal
            if (a.X < b.X) return new Tuple<Point, Point>(a, b);
            return new Tuple<Point, Point>(b, a);
        }
        public int LinkValue(Point a, Point b)
        {
            Tuple<Point, Point> canonical = Canonical(a, b);
            if (Links.ContainsKey(canonical))
            {
                return Links[canonical];
            }
            return 0;
        }
        public int GetNodeLinksAvailable(Point n)
        {
            int links = 0;
            foreach (Point neighbour in Neighbours[n])
            {
                links += CanLink(n, neighbour);
            }
            //
            return links;
        }
        public int GetNodeDegreesAvailable(Point n)
        {
            int degree = 0;
            foreach (Point neighbour in Neighbours[n])
            {
                if (CanLink(n, neighbour) > 0) degree++;
            }
            //
            return degree;
        }
        public int CanLink(Point a, Point b)
        {
            //Cannot link nodes if one value is saturated
            if (Nodes[a] == 0 || Nodes[b] == 0) return 0;
            //Cannot link if the link is saturated
            int linkValue = LinkValue(a, b);
            if (linkValue == 2) return 0;
            if (linkValue == 1) return 1; //Knowing that a,b both cannot be 0
            //Check for intersections
            int length = Math.Abs((b.X - a.X) + (b.Y - a.Y));
            Point vector = new Point((b.X - a.X) / length, (b.Y - a.Y) / length);
            for (int i = 1; i < length; i++)
            {
                Point current = new Point(a.X + vector.X * i, a.Y + vector.Y * i);
                if (Intersections.Contains(current)) return 0;
            }
            //
            return Math.Min(Math.Min(Nodes[a], Nodes[b]), 2);
        }
        public void AddLink(Point a, Point b)
        {
            //
            Console.Error.WriteLine(new string(' ', level * 2) + " -linking (" + a.X + "," + a.Y + ")->(" + b.X + "," + b.Y + ")");
            //
            string answer = a.X + " " + a.Y + " " + b.X + " " + b.Y + " " + 1;
            //Console.WriteLine(answer);
            //
            //Decrement node values
            Nodes[a]--; Nodes[b]--;
            //If the link exists, increment the value
            Tuple<Point, Point> canonical = Canonical(a, b);
            if (Links.ContainsKey(canonical))
            {
                Links[canonical]++;
                return;
            }
            //Create the link and populate the intersections
            Links[canonical] = 1;
            int length = Math.Abs((b.X - a.X) + (b.Y - a.Y));
            Point vector = new Point((b.X - a.X) / length, (b.Y - a.Y) / length);
            for (int i = 1; i < length; i++)
            {
                Point current = new Point(a.X + vector.X * i, a.Y + vector.Y * i);
                Intersections.Add(current);
            }
        }
        public void LinkNeighbours(Point n)
        {
            foreach (Point neighbour in Neighbours[n])
            {
                if (CanLink(n, neighbour) > 0) AddLink(n, neighbour);
            }
        }
        //
        public bool IsConnected()
        {
            HashSet<Point> Past = new HashSet<Point>();
            HashSet<Point> Current = new HashSet<Point>();
            Current.Add(Nodes.Keys.First());
            while (Current.Count() > 0)
            {
                List<Point> current = Current.ToList();
                foreach (Point n in current)
                {
                    Past.Add(n);
                    foreach (Point neighbour in Neighbours[n])
                    {
                        if (LinkValue(n, neighbour) > 0)
                        {
                            if (!Past.Contains(neighbour) && !Current.Contains(neighbour)) Current.Add(neighbour);
                        }
                    }
                }
                foreach (Point n in Past)
                {
                    if (Current.Contains(n)) Current.Remove(n);
                }
            }
            bool isConnected = Nodes.Where(d => !Past.Contains(d.Key)).Count() == 0;
            return isConnected;
        }
    }

    static void solveLogic(PuzzleState puzzleState)
    {
        bool makeLinks = true;
        int iterations = 0;
        while (makeLinks)
        {
            makeLinks = false;
            List<Point> nodes = puzzleState.Nodes.Keys.ToList();
            foreach (Point n in nodes)
            {
                //Skip a saturated node
                int remaining = puzzleState.Nodes[n];
                if (remaining == 0) continue;
                //Calculate links
                int requiredLinks = remaining;
                int availableLinks = puzzleState.GetNodeLinksAvailable(n);
                if (requiredLinks == availableLinks)
                {
                    //Populate links if required links equals available links
                    //Console.Error.WriteLine("LinkAll n=(" + n.X + "," + n.Y + ") links=" + requiredLinks);
                    makeLinks = true;
                    puzzleState.LinkNeighbours(n);
                    puzzleState.LinkNeighbours(n);
                    continue;
                }
                //Calculate requirements
                int requiredDegrees = PuzzleState.GetNodeDegreesRequired(remaining);
                int availableDegrees = puzzleState.GetNodeDegreesAvailable(n);
                if (requiredDegrees == availableDegrees)
                {
                    //Populate links if the available degrees of freedom equals required
                    //Console.Error.WriteLine("LinkNeighbours n=(" + n.X + "," + n.Y + ") links=" + requiredLinks + "," + availableLinks + " degrees=" + requiredDegrees);
                    makeLinks = true;
                    puzzleState.LinkNeighbours(n);
                }
                else if (availableDegrees - requiredDegrees == 1)
                {
                    //Special case for 2->1, 4->1, 6->1
                    if (puzzleState.Nodes[n] % 2 == 0)
                    {
                        foreach (Point neighbour in Neighbours[n])
                        {
                            if (puzzleState.CanLink(n, neighbour) == 1)
                            {
                                //Console.Error.WriteLine("LinkNeighbours -1 n=(" + n.X + "," + n.Y + ") links=" + requiredLinks + "," + availableLinks + " degrees=" + requiredDegrees);
                                //Link others except neighbour
                                foreach (Point candidate in Neighbours[n])
                                {
                                    if (!candidate.Equals(neighbour) && (puzzleState.CanLink(n, candidate) > 0))
                                    {
                                        makeLinks = true;
                                        puzzleState.AddLink(n, candidate);
                                    }
                                }
                                //One iteration
                                break;
                            }
                        }
                    }
                }
            }
            iterations++;
            //Console.Error.WriteLine(new string(' ', level) + " Finished " + iterations + " iterations");
        }
    }

    static int level = 0;
    static bool solve(PuzzleState puzzleState)
    {
        //Make links with logic
        solveLogic(puzzleState);

        //Make links with guess
        HashSet<Tuple<Point, Point>> links = new HashSet<Tuple<Point, Point>>();
        List<Point> nodes = puzzleState.Nodes.Keys.ToList();
        foreach (Point n in nodes)
        {
            foreach (Point neighbour in Neighbours[n])
            {
                if (puzzleState.CanLink(n, neighbour) > 0)
                {
                    Tuple<Point, Point> canonical = PuzzleState.Canonical(n, neighbour);
                    if (!links.Contains(canonical)) links.Add(canonical);
                }
            }
        }
        //Hack - try the shortest links first
        List<Tuple<Point, Point>> sortedLinks = links.ToList().OrderBy(l => Math.Abs(l.Item1.X - l.Item2.X) + Math.Abs(l.Item1.Y - l.Item2.Y)).ToList();
        //Choose a random link
        foreach (Tuple<Point, Point> link in sortedLinks)
        {
            //Duplicate the state
            PuzzleState duplicate = new PuzzleState(puzzleState);
            //Make the link
            level++;
            Console.Error.WriteLine(new string(' ', level * 2) + "test linking (" + link.Item1.X + "," + link.Item1.Y + ")->(" + link.Item2.X + "," + link.Item2.Y + ")");
            duplicate.AddLink(link.Item1, link.Item2);
            //If exhausted, check for connectedness
            if (duplicate.Nodes[link.Item1] == 0 && duplicate.Nodes[link.Item2] == 0)
            {
                int degree1 = 0;
                foreach (Point neighbour in Neighbours[link.Item1])
                {
                    if (duplicate.LinkValue(link.Item1, neighbour) > 0) degree1++;
                }
                int degree2 = 0;
                foreach (Point neighbour in Neighbours[link.Item2])
                {
                    if (duplicate.LinkValue(link.Item2, neighbour) > 0) degree2++;
                }
                //
                if (degree1 == 1 && degree2 == 1)
                {
                    level--;
                    continue;
                }
            }
            //Solve it
            bool result = solve(duplicate);
            level--;
            if (result)
            {
                puzzleState.Nodes = new Dictionary<Point, int>(duplicate.Nodes);
                puzzleState.Links = new Dictionary<Tuple<Point, Point>, int>(duplicate.Links);
                puzzleState.Intersections = new HashSet<Point>(duplicate.Intersections);
                break;
            }
        }

        //Solved, check for completness
        if (puzzleState.Nodes.Count(d => d.Value > 0) > 0) return false;
        if (!puzzleState.IsConnected()) return false;
        //
        return true;
    }

    static void Solve(int width, int height, string[] lines)
    {
        //Initialize nodes
        PuzzleState puzzleState = new PuzzleState();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                char cell = lines[j][i];
                if (cell != '.')
                {
                    int value = int.Parse("" + cell);
                    puzzleState.Nodes[new Point(i, j)] = value;
                }
            }
        }

        //Calculate neighbours
        //Going right, down, left, up
        List<Point> Directions = new List<Point>() { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
        foreach (Point n in puzzleState.Nodes.Keys)
        {
            List<Point> neighbours = new List<Point>();
            //Iterate 4 directions
            for (int dir = 0; dir < 4; dir++)
            {
                Point vector = Directions[dir];
                Point current = n;
                while (current.X >= 0 && current.X < width && current.Y >= 0 && current.Y < height)
                {
                    current = new Point(current.X + vector.X, current.Y + vector.Y);
                    if (puzzleState.Nodes.ContainsKey(current))
                    {
                        //Found a neighbour in the direction
                        neighbours.Add(current);
                        break;
                    }
                }
            }
            //Every node must have a neighbour
            Neighbours[n] = neighbours;
        }

        //Solve the puzzle
        solve(puzzleState);

        //Output links
        List<string> answers = new List<string>();
        foreach (Tuple<Point, Point> link in puzzleState.Links.Keys)
        {
            answers.Add(link.Item1.X + " " + link.Item1.Y + " " + link.Item2.X + " " + link.Item2.Y + " " + puzzleState.Links[link]);
        }
        foreach (string answer in answers)
        {
            Console.WriteLine(answer);
        }
    }

    static void Main(string[] args)
    {
        /*
        Input
        Line 1: one integer width for the number of cells along the x axis.
        Line 2: one integer height for the number of cells along the y axis.
        Next height lines: A string line containing width characters. A dot . represents an empty cell. A number from 1 to 8 represents a cell containing a node. The number is the amount of links the node must have.
        Output
        One line per link or couple of links. Each line is comprised of five integers: x1 y1 x2 y2 amount to add amount links between two nodes at coordinates (x1,y1) and (x2,y2).
        Alloted response time to first output line ≤ 1s.
        Response time between two output lines ≤ 100ms
        */
        int width = int.Parse(Console.ReadLine()); // the number of cells on the X axis
        int height = int.Parse(Console.ReadLine()); // the number of cells on the Y axis
        string[] lines = new string[height];
        for (int i = 0; i < height; i++)
        {
            string line = Console.ReadLine(); // width characters, each either a number or a '.'
            lines[i] = line;
        }

        //
        Solve(width, height, lines);
        // Two coordinates and one integer: a node, one of its neighbors, the number of links connecting them.
        //Console.WriteLine("0 0 2 0 1");
    }

    static void Test(string[] args)
    {
        string[] lines = new string[14];
        lines[0] = "22221";
        lines[1] = "2....";
        lines[2] = "2....";
        lines[3] = "2....";
        lines[4] = "2....";
        lines[5] = "22321";
        lines[6] = ".....";
        lines[7] = ".....";
        lines[8] = "22321";
        lines[9] = "2....";
        lines[10] = "2....";
        lines[11] = "2.131";
        lines[12] = "2..2.";
        lines[13] = "2222.";

        Solve(lines[0].Length, lines.Length, lines);
    }
}

/*
string[] lines = new string[3];
lines[0] = "1.2";
lines[1] = "...";
lines[2] = "..1";

string[] lines = new string[2];
lines[0] = "2.";
lines[1] = "42";

string[] lines = new string[2];
lines[0] = "33";
lines[1] = "33";

string[] lines = new string[3];
lines[0] = "1.3";
lines[1] = "...";
lines[2] = "123";

string[] lines = new string[5];
lines[0] = "2..2.1.";
lines[1] = ".3..5.3";
lines[2] = ".2.1...";
lines[3] = "2...2..";
lines[4] = ".1....2";

string[] lines = new string[14];
lines[0] = "22221";
lines[1] = "2....";
lines[2] = "2....";
lines[3] = "2....";
lines[4] = "2....";
lines[5] = "22321";
lines[6] = ".....";
lines[7] = ".....";
lines[8] = "22321";
lines[9] = "2....";
lines[10] = "2....";
lines[11] = "2.131";
lines[12] = "2..2.";
lines[13] = "2222.";
*/

/*
22221
2....
2....
2....
2....
22321
.....
.....
22321
2....
2....
2.131
2..2.
2222.

.12..
.2421
24442
1242.
..21.

3..2.2..1....3........4
.2..1....2.6.........2.
..3..6....3............
.......2........1..3.3.
..1.............3..3...
.......3..3............
.3...8.....8.........3.
6.5.1...........1..3...
............2..6.31..2.
..4..4.................
5..........7...7...3.3.
.2..3..3..3............
......2..2...1.6...3...
....2..................
.4....5...3............
.................2.3...
.......3.3..2.44....1..
3...1.3.2.3............
.2.....3...6.........5.
................1......
.1.......3.6.2...2...4.
5...............3.....3
4...................4.2
*/
