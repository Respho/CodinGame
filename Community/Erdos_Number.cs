using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Link
{
    public string node1, node2, name;
    public Link(string n1, string n2, string n)
    {
        node1 = n1; node2 = n2; name = n;
    }
    public bool HasNode(string n)
    {
        return node1.Equals(n) || node2.Equals(n);
    }
    public bool HasNode(string n1, string n2)
    {
        return (node1.Equals(n1) && node2.Equals(n2)) || (node1.Equals(n2) && node2.Equals(n1));
    }
    public string ConnectsTo(string n)
    {
        if (node1.Equals(n)) return node2;
        return node1;
    }
}

public class Network
{
    public List<string> Nodes = new List<string>();
    public List<Link> Links = new List<Link>();
    public void AddLink(Link l)
    {
        Links.Add(l);
        Nodes.Add(l.node1);
        Nodes.Add(l.node2);
    }
    public List<string> FindPath(string from, string to)
    {
        //
        Dictionary<string, string> Froms = new Dictionary<string, string>();
        Dictionary<string, int> Distances = new Dictionary<string, int>();

        //
        int Distance = 0;
        Distances[from] = Distance;
        while (true)
        {
            //Get current stage nodes
            List<string> Nodes = Distances.Where(d => d.Value == Distance).Select(d => d.Key).ToList();
            Distance++;
            // Check for infinite
            if (Distance > 50) return null;
            //
            foreach (string node in Nodes)
            {
                List<Link> links = Links.Where(l => l.HasNode(node)).ToList();
                foreach (Link link in links)
                {
                    string target = link.ConnectsTo(node);
                    if (!Distances.ContainsKey(target))
                    {
                        Distances[target] = Distance;
                        Froms[target] = node;
                    }
                }
            }
            //
            if (Froms.ContainsKey(to)) break;
        }

        //
        List<string> Path = new List<string>();
        string currNode = to;
        while (currNode != from)
        {
            string ancestor = Froms[currNode];
            Link link = Links.First(l => l.HasNode(currNode, ancestor));
            Path.Add(link.name);
            currNode = Froms[currNode];
        }
        //
        string debug = "";
        foreach (string n in Path)
        {
            debug += n + ">";
        }
        //Console.Error.WriteLine(debug);
        //
        return Path;
    }

    public Link HasLink(string n)
    {
        foreach (Link link in Links)
        {
            if (link.HasNode(n)) return link;
        }
        return null;
    }
}

class Solution
{
    const string Erdos = "Erdős";
    static List<string> Titles = new List<string>();
    static List<string> Authors = new List<string>();
    
    static void Main(string[] args)
    {
        string scientist = Console.ReadLine();
        if (scientist.Equals(Erdos))
        {
            Console.WriteLine(0);
            return;
        }
        //
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string title = Console.ReadLine();
            Titles.Add(title);
        }
        for (int i = 0; i < N; i++)
        {
            string authors = Console.ReadLine();
            Authors.Add(authors);
        }

        //
        Network network = new Network();
        //For each title
        for (int i = 0; i < N; i++)
        {
            //Console.Error.WriteLine(Titles[i]);
            string[] names = Authors[i].Split(' ');
            int numAuthors = names.Length;
            for (int j = 0; j < numAuthors; j++)
            {
                for (int k = j + 1; k < numAuthors; k++)
                {
                    //Console.Error.WriteLine(j + " " + k);
                    network.AddLink(
                        new Link(names[j], names[k], Titles[i]));
                }
            }
        }
 
        //
        List<string> works = network.FindPath(Erdos, scientist);
        if (works == null)
        {
            Console.WriteLine("infinite");
            return;
        }
        Console.WriteLine(works.Count());
        foreach (string work in works)
        {
            Console.WriteLine(work);
        }
    }
}