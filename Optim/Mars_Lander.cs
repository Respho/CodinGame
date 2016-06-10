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
        string[] inputs;
        int surfaceN = int.Parse(Console.ReadLine()); // the number of points used to draw the surface of Mars.
        Tuple<int, int>[] points = new Tuple<int, int>[surfaceN];
        for (int i = 0; i < surfaceN; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int landX = int.Parse(inputs[0]); // X coordinate of a surface point. (0 to 6999)
            int landY = int.Parse(inputs[1]); // Y coordinate of a surface point. By linking all the points together in a sequential fashion, you form the surface of Mars.
            points[i] = new Tuple<int, int>(landX, landY);
        }

        bool isWrongCase = false;
        bool isCanyon = false;

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int X = int.Parse(inputs[0]);
            int Y = int.Parse(inputs[1]);
            int hSpeed = int.Parse(inputs[2]); // the horizontal speed (in m/s), can be negative.
            int vSpeed = int.Parse(inputs[3]); // the vertical speed (in m/s), can be negative.
            int fuel = int.Parse(inputs[4]); // the quantity of remaining fuel in liters.
            int rotate = int.Parse(inputs[5]); // the rotation angle in degrees (-90 to 90).
            int power = int.Parse(inputs[6]); // the thrust power (0 to 4).

            int x1 = 0, x2 = 0, xmid = 0, elev = 0;
            for (int i = 0; i < surfaceN - 1; i++)
            {
                if (points[i].Item2 == points[i+1].Item2)
                {
                    x1 = points[i].Item1; x2 = points[i+1].Item1;
                    xmid = (x2 + x1) / 2;
                    elev = points[i].Item2;
                }
            }
            Console.Error.WriteLine("Flat " + x1 + "-" + x2 + " (" + xmid + ") " + elev);

            int alt = Y - elev;

            if (X > xmid && Math.Abs(X - xmid) < 2000)
            {
                isWrongCase = true;
                Console.Error.WriteLine("Is Wrong Side Case");
            }
            if (hSpeed == 100 && points.Count(p => p.Item2 > 1500) > 3)
            {
                isCanyon = true;
                Console.Error.WriteLine("Is Canyon Case");
            }
            
            //Calc horizontal
            int MaxTilt = 30;
            if (alt < 700) MaxTilt = 17;
            double SafeFactorH = 0.85;
            double MaxHAccel = 4.0 * Math.Sin(Math.PI / 180 * MaxTilt) * SafeFactorH;
            Console.Error.WriteLine("MaxHAccel " + MaxHAccel);
            //v = sqrt(2ad)
            double hTarget = 0;
            if (X < x1)
            {
                hTarget = Math.Sign(x1 - X) * Math.Sqrt(2.0 * MaxHAccel * Math.Abs(x1 + 100 - X));
            }
            if (X > x2)
            {
                hTarget = Math.Sign(x2 - X) * Math.Sqrt(2.0 * MaxHAccel * Math.Abs(X - x2 + 100));
            }
            if (x1 < X && X < x2 && Math.Abs(hSpeed) < 40)
            {
                hTarget = 0;
            }
            
            if (isCanyon)
            {
                hTarget = Math.Sign(xmid - X) * Math.Sqrt(2.0 * MaxHAccel * Math.Abs(X - xmid));
            }
            
            string re = "";
            for (int i = 4800; i > 4700; i--)
            {
                re += i.ToString() + "," + Math.Sign(xmid - i) * Math.Sqrt(2.0 * MaxHAccel * Math.Abs(i - xmid)) + " . ";
            }
            //Console.Error.WriteLine(re);

            //Calc vertical
            double NetAccelV = 4.0 - 3.711;
            double SafeFactorV = 0.95;
            double a = NetAccelV * SafeFactorV;
            double vTarget = Math.Sqrt(40.0 * 40.0 + 2 * a * alt);
            Console.Error.WriteLine(X + "," + Y + " | " + hSpeed + "h" + "/" + hTarget.ToString("#.##") + "ht " + rotate + "d | " + alt + " " + vSpeed + "v/" + vTarget.ToString("#.##") + "max");

            //
            int r = 0;

            //
            if (hTarget < hSpeed)
            {
                r = Convert.ToInt32(Math.Floor(1.3 * MaxTilt));
            }
            else
            {
                r = -Convert.ToInt32(Math.Floor(1.3 * MaxTilt));
            }
            //
            if (x1 < X && X < x2 && Math.Abs(hSpeed) < 40)
            {
                r = Convert.ToInt32(Math.Floor(0.9 * r));
            }
            //
            if (alt < 70) r = 0;
            if (Math.Abs(hSpeed) < 8 && x1 < X && X < x2) r = 0;

            int t = 3;
            if (vSpeed < -35 || Math.Abs(r) > 0)
            {
                if (alt < 2400)
                {
                    if (!(elev > 2000 && x1 < X && X < x2))
                    {
                        t = 4;
                    }
                }
            }
            
            if (vSpeed > -45 && r == 0)
            {
                if (Math.Abs(vSpeed) < vTarget)
                {
                    t = 3;
                }
            }
            

            //Emergency, case 2
            if (vSpeed < -38)
            {
                r = 0; t = 4;
            }
 
            //
            if (isWrongCase)
            {
                MaxTilt = 35;
                if (hTarget < hSpeed)
                {
                    r = MaxTilt;
                }
                else
                {
                    r = -MaxTilt;
                }
                //
                if (alt < 70) r = 0;
                //Emergency, case 2
                if (vSpeed < -30 && alt < 1000)
                {
                    r = 0; t = 4;
                }
            }
 
            Console.WriteLine(r + " " + t);
        }
    }
}