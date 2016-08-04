using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


class Solution
{
    static void Main(string[] args)
    {
        string msg = Console.ReadLine();

        char t = msg.ToLower()[0];
        char e = msg[2];
        char s = msg[msg.IndexOf(":") - 1];
        Console.Error.WriteLine("set -> " + s + e + t);

        string numbers = msg.Substring(msg.IndexOf(":") + 2);
        string[] tokens = numbers.Split('-');
        string answer = "";
        foreach (string token in tokens)
        {
            //Console.Error.WriteLine(token);
            switch (token.Length)
            {
                case 5:
                    if (token[3] == token[4]) answer += "3";
                    else if (token[1] == token[3]) answer += "7";
                    else answer += "8";
                    break;
                case 4:
                    if (token[0] == token[2]) answer += "9";
                    else if (token[3] == e) answer += "5";
                    else if (token[1] == e) answer += "0";
                    else answer += "4";
                    break;
                case 3:
                    if (token[0] == t) answer += "2";
                    else if (token[0] == s) answer += "6";
                    else answer += "1";
                    break;
            }
        }

        Console.WriteLine(answer);
    }
}
