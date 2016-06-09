using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace CodinGame
{
    class Test
    {
        const string NewLine = "\r\n";
        static string[] TestCases = new string[24];
        static string[] BestCases = new string[24];
        static void Start()
        {
            //
            TestCases[0] = "AZ";
            BestCases[0] = "+.--."; //5

            //
            TestCases[1] = "MINAS";
            //31 Middle Spell
            BestCases[1] = "+[<<+>+>++]<.----.<+.<+.>+++++.";

            //
            TestCases[2] = "UMNE TALMAR RAHTAINE NIXENEN UMIR";
            //132 Middle Spell
            BestCases[2] = "+[<<+>+>++]------.<.+.>>+++++.>.<<-.<[-]+.<-.+.>.>--.>>.<<.<.>>+++.<++.<.>>+.<<<+.>++++.<<.>.>>>.>---.<<<.<.>.<.<.>>>+.<<-.>>>.<---.";

            //One letter x15
            TestCases[3] = "OOOOOOOOOOOOOOO";
            //20
            BestCases[3] = "+++[<+>--]+++[<.>--]";

            //Close letters
            TestCases[4] = "BABCDEDCBABCDCB";
            //31
            BestCases[4] = "++.-.+.+.+.+.-.-.-.-.+.+.+.-.-.";

            //Edge of the alphabet
            TestCases[5] = "ZAZYA YAZ";
            //21
            BestCases[5] = "-.>+.<.-.>.-.<.>+.<+.";

            //One letter x31
            TestCases[6] = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
            //15
            BestCases[6] = "+>+[<.>+]<.....";

            //Two letter word x20
            TestCases[7] = "NONONONONONONONONONONONONONONONONONONONO";
            //21
            BestCases[7] = "+[<->++]+[<.+.->++++]";

            //Far away letters
            TestCases[8] = "GUZ MUG ZOG GUMMOG ZUMGUM ZUM MOZMOZ MOG ZOGMOG GUZMUGGUM";
            //181 Middle Spell
            BestCases[8] = "+[<<+>+>++]<------.<<------.<-.+.>>.<.>>.>.-.<<++.>.>+.<.<<.>--..++.>.>.-.<<<.>--.>.<<.>.<<.-.>.>.>>+.<<.<------.<.>>.<.<.+.>>.<.>>.>.-.<<<.>>.<.<.>>.>+.<.>------.>-.<<<.>>.<..>.<<.";

            //OL53 OL38
            TestCases[9] = "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE";//
            //49
            BestCases[9] = "-------->+[<.>+]+[<.>+]<.>+++++>+[<.>+]+++[<.>++]";

            //OL70
            TestCases[10] = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB";
            //28
            BestCases[10] = "++>+++++++++>+++[<[<.>+]+>-]";

            //Ten letter x8
            TestCases[11] = "GAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLU";
            //77
            BestCases[11] = "+++++++>+>----->------------>++++++++++++>------>+++[[<]>.>..>.>...>..>.>+++]";

            //Medium spell
            TestCases[12] = "O OROFARNE LASSEMISTA CARNIMIRIE O ROWAN FAIR UPON YOUR HAIR HOW WHITE THE BLOSSOM LAY";
            //312
            BestCases[12] = "------------.>.<.+++.---.>++++++.>+.<<+++.----.>-.<<.>--.>>.---------..<.<+.----.>>.+.>+.>.+++.<.<--.<<+++++.>++++.<-.>.>.<.----.[-].<++.>.>.---.<----.>>.<-.>-.>+++.>+.<+++.<<++++.>.<<--.<+.-.-.<.--.>+.>.>.>.>-.>.<+.<<.>.>-.<<<<.<--.<.>.>>>>>.+.<<++.>>----.<.<.>>+++.---.<.++.<<<---.+++.>--..<.--.<<.>>-.<<+.<--.";

            //Seven letter x26
            TestCases[13] = "ALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG B";
            //72
            BestCases[13] = "+>++++++++++++>--------->------------>+++++++>>++>>+[<<<<<<<<[.>].>.>>+]";

            //Long random 4
            TestCases[14] = "OYLO Y OOOYYY LLLYOOYY O YO YLOO O OLY YL OY L YY L YOO LYL YYYOOYLOL L Y O YYYLLOY O L YYYOOYLOL YOLOLOY";
            //254
            BestCases[14] = "------------>-->++++++++++++<<.>.>.<<.<.>>.<<.>...>...>>.<...<.<..>..++.<.>.--.<.<.>>.>.<<..<.>.<.>.>>.<.++.--.>.>.<<<.>.++.>.<.--..++.>.<.--.<..>>>.<.<.>.>.<<...<..>.>.<<.>>.>.<.>.<<.++.<.>.--...>..<<.>.++.<.>.>.<.--...<..>.>.<<.>>.>.<<.<.>>.<<.>>.<<.>.";

            //Incremental 18
            TestCases[15] = "TUVWXYZ ABCDEFGHIJ";
            //24
            BestCases[15] = "------->+++++++++[<.+>+]";

            //Alphabet x11 separated by one letter
            TestCases[16] = "ABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZA";
            //41
            BestCases[16] = "+[<+.>+]<+>>+++++++[<<+.>+[<.+>+]>++]<<+.";

            //One same + one random x32
            TestCases[17] = "OROZOLOKOTONOFOGOMOJOHOFOTOLOPO ODOYOWOAOZO OPOJOTO OROXOVOXO OC";
            //222 Middle Spell
            BestCases[17] = "+[<<+>+>++]<++.+++.---.>-.<.<-.>.<-.>.>------.<.-.+.<-----.>.<+.>.--.++.<+++.>.<--.>.<--.>.>.<.---.+++.+.-.>[-].<.<--.>.>--.<.>--.<.>>+.<<.<<-.>>.>[-].<.+.-.-----.+++++.<<------.>>.>.<.+++.---.>---.<.>--.<.>++.<.>+++.<.<-.";

            //Incremental space
            TestCases[18] = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z";
            //29
            BestCases[18] = "+.-.+>>++++[<<+.>.>+]<<+.>.-.";

            //Pattern followed by another pattern
            TestCases[19] = "FIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFO FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM";
            //96
            BestCases[19] = "++++++.+++.>++++++.<++++++>>+[<<.>.+++.---.>-----]<<.>>.<.>------.<<-->>>+[<<<.<.>>.>.<>>--]<<<.";

            //Pattern then random sequence
            TestCases[20] = "GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY HIJIHIJIJIJIHIHIHIJIHIJIHIJIJIJIHHHIJIJHIHH";
            //126
            BestCases[20] = "+++++++.>-->>+[<<.>.<<.>>>+]<<.>.<<.>.>.<<+.+.+.-.-.+.+.-.+.-.+.-.-.+.-.+.-.+.+.-.-.+.+.-.-.+.+.-.+.-.+.-.-...+.+.-.+.--.+.-..";

            //Two patterns separated randomly
            TestCases[21] = "MELLON MORIAMELLON MORIAMORIAMORIAMELLON MELLON MELLON MORIAMORIAMELLON MELLON MORIA";
            //197
            BestCases[21] = "-[<+>++]<[<-]>>++>+++++>---->[-]+>[-]>>-------->->->++>+>[-]<[<]>[.>]<[<].<[<]>[.>]>[.>]<[<].<[<]>[.>]<[<]>[.>]<[<]>[.>]>[.>]<[<].>[.>]<[<].>[.>]<[<].<[<]>[.>]<[<]>[.>]>[.>]<[<].>[.>]<[<].<[<]>[.>]";

            //Pattern followed by incremental seq
            TestCases[22] = "ZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZACEGIKMOQSUWY BDFHJLNPRTVXZACEGIKMOQSUWY BDFHJLNPRTVXZA";
            //115
            BestCases[22] = "-.>+>++++++++[<.<.>>+]+[<.++>-----]<.<-.>>+[<<<.++>>>-----]<<<.>->>+++[<<.++>>++]<<.>.++.>>+[<.++>-----]<.<-.++.++.";

            //Long spell
            TestCases[23] = "THREE RINGS FOR THE ELVEN KINGS UNDER THE SKY SEVEN FOR THE DWARF LORDS IN THEIR HALLS OF STONE NINE FOR MORTAL MEN DOOMED TO DIE ONE FOR THE DARK LORD ON HIS DARK THRONEIN THE LAND OF MORDOR WHERE THE SHADOWS LIE ONE RING TO RULE THEM ALL ONE RING TO FIND THEM ONE RING TO BRING THEM ALL AND IN THE DARKNESS BIND THEM IN THE LAND OF MORDOR WHERE THE SHADOWS LIE";
            //1281
            BestCases[23] = "+++++[<<<+>+++>+>-]------->++++++++>+++++<<.>.<--.<..[-].>.>+.+++++.>++.<<+.<.<<+.>.>>-.<.>++.>>+.---.>.<.<--.<++.>>.<<<<-.>.>>-.--.<<<.<+.>>>---.<.>++.<<.<---.+.>++++.>.>-.>-.>.>.<<<-.>+++.<<--.++.>.>>.<<+++.>>.<+++.>>.<+.<+.<<<.>.>--.>>++.---.>.<-.<<+++.<+.<.<+.<.>++++++.+++.>.>+++.<+.<<.>>>+++++.<<-.<.>>+.>-.---.++++.<--.[-].>-.<+.<--..>>>----.[-].>.>++.<<.>++++.+.-----.-.>-.>.<<.<<+.>>.>.>.<+.<+.+++.<.<<<+.++.+++.++.>.>+++.>.<+.<++++.>+.>.<<-.>+..--.<+.-.<<.>.>>++.>.<<.+++++.----.>>.<.-.<.>>.<<+.>+.>>.<.>++.>++.---.>.<-.>+.<<--.<<----.>.<+.+++.>>.>.<<.<.-.>.<<++.+.>>>+.<.>>.>.<<-.<<---.>.<<<.>-.>>>.---.-.>+.++++.<.<.<<<.>.---.>>.<+.>+.>.<+++.[-].>+.>---.<<.<+.>>.+++.<<<-.>++.>>.<.----.>>++.---.<.>.>-.<<++.>+++.---.>.<<-.>+++.>+.<<<<<.>.>.>.<[-].<---.---.<+.>>.>----.-.<<<.<<.>--.>>.>>.>-.<<.<<<++.>>>>+.<.>+++.+++.<<+++.<.<<.>.>+++.---.>+.>.+.<-..>-.<+++.-.<.>>.>---.>++.<<<.<++.<<.>.>>+.>.<<-.+++.>-.>++++.<<<<.>.>-.>>+.<-.>[-].<++.-.<---.>>.>.>.<<<.<++.<<.>.>>+.>.++.>.>.<<<-.<.<<.>.>+.---.>-.>--.+.<-..>-.+.<++.<-.>>-.>>.<<<.>.>++.>-.---.<<.>>-.>.<<--.<<---.+++.<+.<-..<.++.>>++++.>.>>>.<<.<<<+.>-.---.>-.>.<----.>>----.<.<<<.>>-.<.>>.>--.<+.>++.>.<<-.>+.>++.<<.>--.++.+++.>--.<---.+++.<.----.<.<.>>>.<<<.[-].<.>>.---.<.<-.>>+++.>>>>.<.<---.<.<<<.>.>++++.---.>>>+.";
        }

        static void Main(string[] args)
        {
            Start();
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            string result = "";
            int total = 0;
            bool ok = true;
            for (int i = 0; i < TestCases.Length; i++)
            {
                //
                stopWatch.Reset();
                stopWatch.Start();
                //
                string program = Bilbo.Optimize(TestCases[i]);
                //
                stopWatch.Stop();
                //
                string item = "";
                item += TestCases[i] + NewLine + program + NewLine;
                //
                Turing turing = new Turing();
                turing.Execute(program);
                string output = turing.GetOutput();
                bool correct = output.Equals(TestCases[i]);
                if (correct)
                {
                    //
                    item += new Turing().ExecuteLetters(program) + NewLine;
                    item += program.Length + " ";
                    if (program.Length > BestCases[i].Length) item += "***Suboptimal*** ";
                    if (program.Length < BestCases[i].Length) item += "***New Best Case*** ";
                    total += program.Length;
                }
                else
                {
                    item += "PROBLEM ";
                    ok = false;
                }
                //
                Turing t2 = new Turing();
                t2.Execute(BestCases[i]);
                if (!t2.GetOutput().Equals(TestCases[i])) throw new Exception("Best case is wrong.");
                //
                long time = stopWatch.ElapsedMilliseconds;
                item += time + "ms -- " + Bilbo.PatternDebug;
                //
                item += NewLine + NewLine;
                result += item;
            }

            logResult(result, total, ok);
        }

        static void logResult(string result, int total, bool ok)
        {
            string path = "Bilbos_Forest.cs";
            System.IO.StreamReader streamReader = new System.IO.StreamReader(path);
            string contents = streamReader.ReadToEnd();
            streamReader.Close();

            string timeStamp = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            string filePath = System.DateTime.Now.ToString("dd_HH_mm_ss") + " " + total + ".txt";
            if (!ok) filePath = filePath.Replace(".txt", " error.txt");
            bool append = true;
            TextWriter textWriter = new StreamWriter(filePath, append);
            textWriter.Write(result);
            textWriter.Write(contents);
            textWriter.Close();
        }
    }
}
