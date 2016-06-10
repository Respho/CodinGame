using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Save humans, destroy zombies!
 **/
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            int humanCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < humanCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int humanId = int.Parse(inputs[0]);
                int humanX = int.Parse(inputs[1]);
                int humanY = int.Parse(inputs[2]);
            }
            int zombieCount = int.Parse(Console.ReadLine());

            int nextX = 0, nextY = 0;
            long best = 10000000000;
            for (int i = 0; i < zombieCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int zombieId = int.Parse(inputs[0]);
                int zombieX = int.Parse(inputs[1]);
                int zombieY = int.Parse(inputs[2]);
                int zombieXNext = int.Parse(inputs[3]);
                int zombieYNext = int.Parse(inputs[4]);

                long dist = (long)(x - zombieXNext)*(x - zombieXNext) + (long)(y - zombieYNext)*(y - zombieYNext);
                if (dist < best)
                {
                    nextX = zombieXNext; nextY = zombieYNext; best = dist;
                }
            }

            Console.WriteLine(nextX.ToString() + " " + nextY.ToString()); // Your destination coordinates
        }
    }
}