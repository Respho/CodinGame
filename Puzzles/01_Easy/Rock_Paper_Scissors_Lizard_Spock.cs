using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
    static Dictionary<int, List<int>> WhoPlaysWhom = new Dictionary<int, List<int>>();

    //
    const string WinningPairs = "CP|PR|RL|LS|SC|CL|LP|PS|SR|RC";
    //Returns "1 P" if given "4 R", "1 P"
    private static string findWinner(string a, string b)
    {
        int playerA = Convert.ToInt32(a.Split(' ')[0]);
        int playerB = Convert.ToInt32(b.Split(' ')[0]);
        if (WhoPlaysWhom.ContainsKey(playerA))
        {
            //Second round
            WhoPlaysWhom[playerA].Add(playerB);
            WhoPlaysWhom[playerB].Add(playerA);
        }
        else
        {
            //First round
            WhoPlaysWhom.Add(playerA, new List<int>(){ playerB });
            WhoPlaysWhom.Add(playerB, new List<int>(){ playerA });
        }

        //
        string pairing1 = "" + a[a.Length - 1] + b[b.Length - 1];
        string pairing2 = "" + b[b.Length - 1] + a[a.Length - 1];
        if (WinningPairs.Contains(pairing1))
        {
            return a;
        }
        if (WinningPairs.Contains(pairing2))
        {
            return b;
        }
        //Compare numbers
        return playerA < playerB ? a : b;
    }

    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        var players = new List<string>();
        for (int i = 0; i < N; i++)
        {
            string line = Console.ReadLine();
            //Console.Error.WriteLine(line);
            players.Add(line);
            //string[] inputs = line.Split(' ');
            //int NUMPLAYER = int.Parse(inputs[0]);
            //string SIGNPLAYER = inputs[1];
        }

        while (players.Count() > 1)
        {
            var newPlayers = new List<string>();
            int rounds = players.Count() / 2;
            for (int i = 0; i < rounds; i++)
            {
                string winner = findWinner(players[i * 2], players[i * 2 + 1]);
                newPlayers.Add(winner);
            }
            players = newPlayers;
        }

        int index = Convert.ToInt32(players[0].Split(' ')[0]);
        Console.WriteLine(index);
        string answer = "";
        foreach (int p in WhoPlaysWhom[index])
        {
            answer += "" + p + " ";
        }
        answer = answer.Trim();
        Console.WriteLine(answer);
    }
}