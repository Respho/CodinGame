using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static Point[] Directions = new Point[] {
        new Point(1,0), new Point(-1, 0), new Point( 0, 1), new Point( 0,-1),
        new Point(1,1), new Point(-1,-1), new Point( 1,-1), new Point(-1, 1)
    };

    static void Main(string[] args)
    {
        //Read the board
        var lines = new List<string>();
        for (int i = 0; i < 8; i++)
        {
            string row = Console.ReadLine();
            Console.Error.WriteLine(row);
            lines.Add(row);
        }
        //Read my position
        string line = Console.ReadLine();
        Console.Error.WriteLine(line);
        string[] inputs = line.Split(' ');
        char myColour = inputs[0][0];
        char opColour = (myColour == 'B') ? 'W' : 'B';
        string move = inputs[1];
        int moveCol = (int)move[0] - 97;
        int moveRow = int.Parse("" + move[1]) - 1;
        Console.Error.WriteLine("Move at col " + moveCol + " row " + moveRow);

        //Start
        if (lines[moveRow][moveCol] != '-')
        {
            Console.WriteLine("NOPE");
            return;
        }

        //Check directions
        var validDirections = new List<Point>();
        foreach (Point direction in Directions)
        {
            //
            int i = moveRow, j = moveCol;
            for (int step = 1; step < 7; step++)
            {
                i += direction.X;
                j += direction.Y;
                if ((i < 0) || (i >= 8) ||
                    (j < 0) || (j >= 8)) continue;
                //Require next to be opponent colour
                if (step == 1)
                {
                    if (lines[i][j] != opColour)
                        break;
                }
                //Require continuity and my color
                else
                {
                    if (lines[i][j] == '-')
                        break;
                    if (lines[i][j] == myColour)
                    {
                        validDirections.Add(direction);
                        break;
                    }
                }
            }
        }

        //Invalid move
        if (validDirections.Count() == 0)
        {
            Console.WriteLine("NULL");
            return;
        }

        //Count
        int countW = 0, countB = 0;
        for (int i = 0; i < 8; i++)
        {
            countW += lines[i].Count(c => c == 'W');
            countB += lines[i].Count(c => c == 'B');
        }

        //Flip
        if (myColour == 'W') countW++; else countB++;
        foreach (Point direction in validDirections)
        {
            int i = moveRow, j = moveCol;
            for (int step = 1; step < 7; step++)
            {
                i += direction.X;
                j += direction.Y;
                if ((i < 0) || (i >= 8) ||
                    (j < 0) || (j >= 8)) continue;
                //
                if (lines[i][j] != opColour)
                    break;
                else
                {
                    if (myColour == 'W')
                    {
                        countW++;  
                        countB--;
                    }
                    else
                    {
                        countW--;
                        countB++;
                    }
                }
            }
        }

        //
        Console.WriteLine("" + countW + " " + countB);
    }
}
