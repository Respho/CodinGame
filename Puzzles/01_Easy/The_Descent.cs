using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        int slot = 0;
        int max = 0;

        // game loop
        while (true)
        {
            for (int i = 0; i < 8; i++)
            {
                int mountainH = int.Parse(Console.ReadLine()); // represents the height of one mountain, from 9 to 0.
                if (mountainH > max)
                {
                    max = mountainH;
                    slot = i;
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine(slot.ToString()); // The number of the mountain to fire on.
            slot = 0;
            max = 0;
        }
    }
}