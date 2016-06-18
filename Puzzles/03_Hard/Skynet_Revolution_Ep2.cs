using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Link
{
    public int node1;
    public int node2;
    public bool active = true;
    public Link(int n1, int n2)
    {
        node1 = n1; node2 = n2;
    }
    public void Cut()
    {
        active = false;
    }
}

public class Network
{
    public List<int> Nodes = new List<int>();
    public int[] Gateways;
    public List<Link> Links = new List<Link>();
    public void AddLink(Link l)
    {
        Links.Add(l);
        Nodes.Add(l.node1);
        Nodes.Add(l.node2);
    }
    public Link IsAdjGateway(int node)
    {
        foreach (int g in Gateways)
        {
            Link l = HasLink(g, node);
            if (l != null) return l;
        }
        return null;
    }
    public List<int> FindPath(int from, int to)
    {
        //
        Console.Error.Write("Finding path " + from + " - " + to + " ");
        Dictionary<int, int> Froms = new Dictionary<int, int>();
        Dictionary<int, int> Distances = new Dictionary<int, int>();
        //
        int Distance = 0;
        Distances[from] = Distance;
        while (true)
        {
            //Get current stage nodes
            List<int> Nodes = Distances.Where(d => d.Value == Distance).Select(d => d.Key).ToList();
            Distance++;
            foreach (int node in Nodes)
            {
                //
                List<Link> links = Links.Where(l => l.active && (l.node1 == node || l.node2 == node)).ToList();
                foreach (Link link in links)
                {
                    if (!Distances.ContainsKey(link.node1) && !Gateways.Contains(link.node1))
                    {
                        Distances[link.node1] = Distance;
                        Froms[link.node1] = node;
                    }
                    if (!Distances.ContainsKey(link.node2) && !Gateways.Contains(link.node2))
                    {
                        Distances[link.node2] = Distance;
                        Froms[link.node2] = node;
                    }
                }
            }
            //
            if (Froms.ContainsKey(to)) break;
        }

        //
        List<int> Path = new List<int>();
        Path.Add(to);
        int currNode = to;
        while (currNode != from)
        {
            Path.Add(Froms[currNode]);
            currNode = Froms[currNode];
        }
        //
        string debug = "";
        foreach (int n in Path)
        {
            debug += n + ">";
        }
        Console.Error.WriteLine(debug);
        //
        return Path;
    }
    public int CalcScore(List<int> Path, Dictionary<int, int> Hubs)
    {
        int sum = 0;
        foreach (int n in Path)
        {
            if (Hubs.ContainsKey(n)) sum += Hubs[n];
        }
        sum -= Path.Count();
        //
        string debug = "";
        foreach (int n in Path)
        {
            debug += n + ">";
        }
        Console.Error.WriteLine("Score for " + debug + "=" + sum);
        //
        return sum;
    }
    public Link GetLink(int SI)
    {
        //Identify all nodes leading to gateways
        Dictionary<int, int> Hubs = new Dictionary<int, int>();
        List<int> nodes = Nodes.Distinct().ToList();
        foreach (int n in nodes)
        {
            int count = 0;
            foreach (int g in Gateways)
            {
                Link l = HasLink(g, n);
                if (l != null) count++;
            }
            //
            if (count > 0) Hubs[n] = count;
        }
        string debug = "";
        foreach (int n in Hubs.Keys)
        {
            debug += "(" + n + "," + Hubs[n] + ") ";
        }
        Console.Error.WriteLine("Hubs are " + debug);
        //For all hub nodes (leading to 2 or more gateways)
        foreach (int n in Hubs.Where(d => d.Value > 1).Select(d => d.Key))
        {
            //Find the path from SI to the node
            List<int> path = FindPath(SI, n);
            //Identify at-risk nodes
            int score = CalcScore(path, Hubs);
            if (score >= 0)
            {
                Link link = IsAdjGateway(n);
                Console.Error.WriteLine("Cutting " + link.node1 + "-" + link.node2);
                return link;
            }
        }
        //No at-risk nodes found, return a random link
        foreach (Link link in Links)
        {
            if (link.active && (Gateways.Contains(link.node1) || Gateways.Contains(link.node2)))
            {
                Console.Error.WriteLine("Random cutting " + link.node1 + "-" + link.node2);
                return link;
            }
        }
        return null;
    }
    public Link HasLink(int node)
    {
        foreach (Link link in Links)
        {
            if (link.node1 == node || link.node2 == node)
            {
                if (link.active) return link;
            }
        }
        return null;
    }
    public Link HasLink(int node1, int node2)
    {
        foreach (Link link in Links)
        {
            if (link.node1 == node1 && link.node2 == node2 && link.active)
            {
                return link;
            }
            if (link.node2 == node1 && link.node1 == node2 && link.active)
            {
                return link;
            }
        }
        return null;
    }
}

class Player
{
    static void Main(string[] args)
    {
        //
        Network network = new Network();

        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            Link link = new Link(N1, N2);
            network.AddLink(link);
        }
        network.Gateways = new int[E];
        for (int i = 0; i < E; i++)
        {
            int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
            network.Gateways[i] = EI;
        }

        // game loop
        while (true)
        {
            int SI = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn

            //Cut the link if Skynet is next to a gateway
            Link link = network.IsAdjGateway(SI);
            //If not next to a gateway, get the most risky node
            if (link == null) link = network.GetLink(SI);
            //
            link.Cut();
            //
            Console.WriteLine(link.node1.ToString() + " " + link.node2.ToString());
        }
    }
}