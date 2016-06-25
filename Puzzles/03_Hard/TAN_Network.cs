using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Stop
{
    public string Code, Name;
    public double Lat, Long;
    //
    public Stop(string input)
    {
        string[] tokens = input.Split(',');
        Code = tokens[0];
        Name = tokens[1].Substring(1, tokens[1].Length - 2);
        Lat = double.Parse(tokens[3]);
        Long = double.Parse(tokens[4]);
        //Console.Error.WriteLine("Stop() " + Code + " " + Name + " " + Lat + " " + Long);
    }
}

class Solution
{
    static Dictionary<string, Stop> Stops = new Dictionary<string, Stop>();
    static Dictionary<string, List<string>> Routes = new Dictionary<string,List<string>>();

    //
    static Dictionary<string, double> CacheDistances = new Dictionary<string,double>();
    static int CacheHits = 0;
    static double Distance(string stop1, string stop2)
    {
        //
        string cacheKey = stop1 + "-" + stop2;
        if (CacheDistances.ContainsKey(cacheKey))
        {
            //Console.Error.WriteLine("Cache hit " + cacheKey);
            CacheHits++;
            return CacheDistances[cacheKey];
        }
        //
        double x = (toRadians(Stops[stop2].Long) - toRadians(Stops[stop1].Long)) *
            Math.Cos((toRadians(Stops[stop1].Lat) + toRadians(Stops[stop2].Lat)) / 2);
        double y = toRadians(Stops[stop2].Lat) - toRadians(Stops[stop1].Lat);
        double inner = x * x + y * y;
        double answer = Math.Sqrt(inner) * 6371;
        CacheDistances[cacheKey] = answer;
        //
        return answer;
    }
    static double toRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    /*
    http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
    OPEN = priority queue containing START
    CLOSED = empty set
    while lowest rank in OPEN is not the GOAL:
      current = remove lowest rank item from OPEN
      add current to CLOSED
      for neighbors of current:
        cost = g(current) + movementcost(current, neighbor)
        if neighbor in OPEN and cost less than g(neighbor):
          remove neighbor from OPEN, because new path is better
        if neighbor in CLOSED and cost less than g(neighbor): ⁽²⁾
          remove neighbor from CLOSED
        if neighbor not in OPEN and neighbor not in CLOSED:
          set g(neighbor) to cost
          add neighbor to OPEN
          set priority queue rank to g(neighbor) + h(neighbor)
          set neighbor's parent to current
    */
    static void FindRoute(string startPoint, string endPoint)
    {
        Dictionary<string, string> Froms = new Dictionary<string, string>();
        Dictionary<string, int> Steps = new Dictionary<string, int>();
        Dictionary<string, double> Distances = new Dictionary<string, double>();
        //
        HashSet<string> OPEN = new HashSet<string>() { startPoint };
        HashSet<string> CLOSED = new HashSet<string>();
        //
        Dictionary<string, int> Times = new Dictionary<string, int>();

        //
        int Step = 0;
        Froms[startPoint] = startPoint;
        Steps[startPoint] = Step;
        Distances[startPoint] = 0;
        //while lowest rank in OPEN is not the GOAL
        while ((OPEN.Count() > 0) && (OPEN.OrderBy(x => Distances[x]).First() != endPoint))
        {
            //current = remove lowest rank item from OPEN
            //add current to CLOSED
            string current = OPEN.OrderBy(x => Distances[x]).First();
            OPEN.Remove(current);
            CLOSED.Add(current);
            //
            if (Times.ContainsKey(current)) Times[current]++; else Times[current] = 1;
            //for neighbors of current
            if (!Routes.ContainsKey(current)) continue;
            List<string> neighbors = Routes[current];
            foreach (string neighbor in neighbors)
            {
                //cost = g(current) + movementcost(current, neighbor)
                double cost = Distances[current] + Distance(current, neighbor);
                //if neighbor in OPEN and cost less than g(neighbor)
                if (OPEN.Contains(neighbor) && (cost < Distances[neighbor]))
                {
                    //remove neighbor from OPEN, because new path is better
                    OPEN.Remove(neighbor);
                }
                //if neighbor in CLOSED and cost less than g(neighbor)
                if (CLOSED.Contains(neighbor) && (cost < Distances[neighbor]))
                {
                    //remove neighbor from CLOSED
                    CLOSED.Remove(neighbor);
                }
                //if neighbor not in OPEN and neighbor not in CLOSED:
                if (!OPEN.Contains(neighbor) && !CLOSED.Contains(neighbor))
                {
                    //set g(neighbor) to cost
                    Distances[neighbor] = cost;
                    //add neighbor to OPEN
                    OPEN.Add(neighbor);
                    //set neighbor's parent to current
                    Froms[neighbor] = current;
                    Steps[neighbor] = Steps[current] + 1;
                }
            }
        }

        //
        if (!Froms.ContainsKey(endPoint))
        {
            Console.WriteLine("IMPOSSIBLE");
            return;
        }
        //
        List<string> Names = new List<string>();
        string currNode = endPoint;
        while (currNode != startPoint)
        {
            Names.Add(Stops[currNode].Name);
            currNode = Froms[currNode];
        }
        Names.Add(Stops[currNode].Name);
        //
        for (int i = Names.Count() - 1; i >= 0; i--)
        {
            Console.WriteLine(Names[i]);
        }
    }

    static void Main(string[] args)
    {
        string startPoint = Console.ReadLine();
        string endPoint = Console.ReadLine();
        //Console.Error.WriteLine("Calculate " + startPoint + " " + endPoint);
        //
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string stopName = Console.ReadLine();
            //
            Stop stop = new Stop(stopName);
            Stops.Add(stop.Code, stop);
        }
        //
        int M = int.Parse(Console.ReadLine());
        for (int i = 0; i < M; i++)
        {
            string route = Console.ReadLine();
            //
            string start = route.Substring(0, 13), end = route.Substring(14, 13);
            //Console.Error.WriteLine("Route " + start + " " + end);
            if (Routes.ContainsKey(start))
            {
                Routes[start].Add(end);
            }
            else
            {
                Routes[start] = new List<string>() { end };
            }
        }

        //
        FindRoute(startPoint, endPoint);
    }
}