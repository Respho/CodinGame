using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static Dictionary<int, Tuple<int, int, int>> Rooms = new Dictionary<int, Tuple<int, int, int>>();

    //
    static Dictionary<int, int> Cache = new Dictionary<int, int>();
    static int Lookups = 0, Calculations = 0;

    static int max(int roomNo)
    {
        //
        Lookups++;
        if (Cache.ContainsKey(roomNo)) return Cache[roomNo];

        //
        Calculations++;
        Tuple<int, int, int> room = Rooms[roomNo];
        int answer = room.Item1 + Math.Max(room.Item2 == -1 ? 0 : max(room.Item2), room.Item3 == -1 ? 0 : max(room.Item3));

        //
        Cache.Add(roomNo, answer);
        return answer;
    }

    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string room = Console.ReadLine();
            string[] tokens = room.Split(' ');
            int roomNo = int.Parse(tokens[0]);
            int money = int.Parse(tokens[1]);
            int exit1 = tokens[2] == "E" ? -1 : int.Parse(tokens[2]);
            int exit2 = tokens[3] == "E" ? -1 : int.Parse(tokens[3]);
            Rooms.Add(roomNo, new Tuple<int, int, int>(money, exit1, exit2));
        }

        int result = max(0);

        Console.Error.WriteLine("Lookups: " + Lookups + ", Calculations: " + Calculations);

        Console.WriteLine(result);
    }
}
