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
        int N = int.Parse(Console.ReadLine());
        //
        string[] Lines = new string[N];
        List<int> IndH = null;
        List<int> IndV = new List<int>();
        //
        for (int i = 0; i < N; i++)
        {
            string s = Console.ReadLine();
            Lines[i] = s;
            //
            if (i == 0)
                IndH = Enumerable.Range(0, s.Length).Where(x => s[x] == '+').ToList();
            //
            if (s[0] == '+')
                IndV.Add(i);
        }
        Console.Error.WriteLine("IndH " + IndH.Count() + " IndV " + IndV.Count());
 
        //
        Console.WriteLine("<table>");
        for (int y = 1; y < IndV.Count(); y++)
        {
            string row = "";
            for (int x = 1; x < IndH.Count(); x++)
            {
                int x0 = IndH[x - 1] + 1;
                int x1 = IndH[x] - 1;
                int y0 = IndV[y - 1] + 1;
                int y1 = IndV[y] - 1;
                string cell = "";
                for (int m = y0; m <= y1; m++)
                {
                    cell += Lines[m].Substring(x0, x1 - x0 + 1).Trim() + " ";
                }
                row += "<td>" + cell.Trim() + "</td>";
            }
            Console.WriteLine("<tr>" + row + "</tr>");
        }
        Console.WriteLine("</table>");
    }
}