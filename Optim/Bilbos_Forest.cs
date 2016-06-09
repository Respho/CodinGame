using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace CodinGame
{
    public class Turing
    {
        //
        int Pointer;
        int[] Memory = new int[30];

        //
        StringBuilder Output = new StringBuilder();
        string Program;
        int ProgramPointer = 0;
        int[] Stack = new int[10];
        int StackPointer = 0;

        public static int NextCell(int pointer, int increment)
        {
            int result = pointer + increment;
            if (result >= 30) result = (result % 30);
            else if (result < 0) { while (result < 0) result += 30; }
            return result;
        }
        public static int NextValue(int v, int increment)
        {
            int result = v + increment;
            if (result >= 27) result = (result % 27);
            else if (result < 0) { while (result < 0) result += 27; }
            return result;
        }

        //Find the next empty cell without any conflict with the program
        public int NextEmptyCellSteps(string program)
        {
            int steps = 0, count = 0;
            foreach (char c in program)
            {
                if (c == '>')
                {
                    count++;
                    if (count > steps) steps = count;
                }
                else if (c == '<') count--;
            }
            //
            for (int i = steps + 1; i < 30; i++)
            {
                if (Memory[NextCell(Pointer, i)] == 0) return i;
            }
            throw new Exception("No empty cell found");
        }

        public int Current()
        {
            return Memory[Pointer];
        }

        public State GetState()
        {
            bool[] a = new bool[30];
            for (int i = 0; i < 30; i++)
            {
                if (Memory[i] > 0 || i == 0)
                {
                    a[NextCell(i, -1)] = true;
                    a[NextCell(i, 1)] = true;
                    a[i] = true;
                }
            }
            return new State(Memory, a, Pointer);
        }

        public string GetMemory()
        {
            string result = "";
            for (int i = 0; i < 30; i++)
            {
                char p = Convert.ToChar(Memory[i]);
                p += 'A'; p--; if (p == '@') p = '_';
                result += "" + p + ",";
            }
            return result.TrimEnd(',');
        }

        public string GetOutput()
        {
            return Output.ToString();
        }

        public static string TestState(string program)
        {
            Turing turing = new Turing();
            turing.Execute(program);
            return turing.GetMemory();
        }

        public static string WhatIs(string program)
        {
            Turing turing = new Turing();
            turing.Execute(program);
            return turing.GetOutput();
        }

        public void Execute(string program)
        {
            Program = program;
            ProgramPointer = 0;
            StackPointer = 0;
            while (ProgramPointer < Program.Length)
            {
                char c = Program[ProgramPointer];
                if (c == '.')
                {
                    char p = Convert.ToChar(Memory[Pointer]);
                    p += 'A'; p--; if (p == '@') p = ' ';
                    Output.Append(p);
                }
                else if (c == '+')
                {
                    Memory[Pointer] = NextValue(Memory[Pointer], 1);
                }
                else if (c == '-')
                {
                    Memory[Pointer] = NextValue(Memory[Pointer], -1);
                }
                else if (c == '>')
                {
                    Pointer = NextCell(Pointer, 1);
                }
                else if (c == '<')
                {
                    Pointer = NextCell(Pointer, -1);
                }
                else if (c == '[')
                {
                    if (Memory[Pointer] != 0)
                    {
                        Stack[StackPointer] = ProgramPointer;
                        StackPointer++;
                    }
                    else
                    {
                        while (Program[++ProgramPointer] != ']') { }
                    }
                }
                else if (c == ']')
                {
                    if (Memory[Pointer] != 0)
                    {
                        //Repeat
                        ProgramPointer = Stack[StackPointer - 1];
                    }
                    else
                    {
                        //Exit
                        StackPointer--;
                    }
                }
                ProgramPointer++;
            }
        }

        public string ExecuteLetters(string program)
        {
            string Letters = new string(' ', 2000);
            char[] LettersArray = Letters.ToCharArray();
            Program = program;
            ProgramPointer = 0;
            StackPointer = 0;
            while (ProgramPointer < Program.Length)
            {
                char c = Program[ProgramPointer];
                if (c == '.')
                {
                    char p = Convert.ToChar(Memory[Pointer]);
                    p += 'A'; p--; if (p == '@') p = '_';
                    Output.Append(p);
                    LettersArray[ProgramPointer] = p;
                }
                else if (c == '+')
                {
                    Memory[Pointer] = NextValue(Memory[Pointer], 1);
                }
                else if (c == '-')
                {
                    Memory[Pointer] = NextValue(Memory[Pointer], -1);
                }
                else if (c == '>')
                {
                    Pointer = NextCell(Pointer, 1);
                }
                else if (c == '<')
                {
                    Pointer = NextCell(Pointer, -1);
                }
                else if (c == '[')
                {
                    if (Memory[Pointer] != 0)
                    {
                        Stack[StackPointer] = ProgramPointer;
                        StackPointer++;
                    }
                    else
                    {
                        while (Program[++ProgramPointer] != ']') { }
                    }
                }
                else if (c == ']')
                {
                    if (Memory[Pointer] != 0)
                    {
                        //Repeat
                        ProgramPointer = Stack[StackPointer - 1];
                    }
                    else
                    {
                        //Exit
                        StackPointer--;
                    }
                }
                ProgramPointer++;
            }
            return new string(LettersArray).TrimEnd(' ');
        }
    }

    public class State
    {
        public int BibloPos;
        public int[] Letters;
        public bool[] Actives;
        public State()
        {
            Letters = new int[30];
            Actives = new bool[30];
            for (int i = 0; i < 30; i++)
            {
                Letters[i] = 0;
                Actives[i] = false;
            }
            Actives[0] = true; Actives[29] = true;
        }
        public State(int[] l, bool[] a, int pos)
        {
            BibloPos = pos;
            Letters = new int[30]; Actives = new bool[30];
            Array.Copy(l, Letters, 30);
            Array.Copy(a, Actives, 30);
        }
        public State(State s)
        {
            BibloPos = s.BibloPos;
            Letters = new int[30]; Actives = new bool[30];
            Array.Copy(s.Letters, Letters, 30);
            Array.Copy(s.Actives, Actives, 30);
        }
    }

    public struct Traversal
    {
        public int Score;
        public string Scores;
        public string Steps;
    }

    public struct Pattern
    {
        public string Base;
        public int Occurrences;
    }

    public class Bilbo
    {
        //
        const int Depth = 6;
        static int[] PhraseInt = null;
        static string PhraseString = null;
        //
        static int[,] SlotDs = new int[30, 30];
        static int[,] SlotDsAbs = new int[30, 30];
        static bool[,] AlphaRs = new bool[27, 27]; //Whether reset is better
        static int[,] AlphaDs = new int[27, 27];
        static int[,] AlphaDsAbs = new int[27, 27];

        //
        #region Post
        //
        const int MaxPattern = 12;
        const int MinRepeat = 8;
        public static string PatternDebug;
        //
        static Dictionary<int, string> Loops;

        static void generateLoopsTable()
        {
            Loops = new Dictionary<int, string>();
            Loops.Add(7, "1,-4");
            Loops.Add(8, "3,3");
            Loops.Add(9, "9,-1");
            Loops.Add(10, "7,2");
            Loops.Add(11, "1,-5");
            Loops.Add(12, "3,2");
            Loops.Add(13, "1,2");
            Loops.Add(14, "1,-2");
            Loops.Add(15, "3,-2"); //O
            Loops.Add(16, "1,5");
            Loops.Add(17, "7,-2");
            Loops.Add(18, "9,1");
            Loops.Add(19, "8,1");
            Loops.Add(20, "1,4");
            Loops.Add(21, "6,1");
            Loops.Add(22, "5,1");
            Loops.Add(23, "4,1");
            Loops.Add(24, "3,1");
            Loops.Add(25, "2,1");
            Loops.Add(26, "1,1");

            /*
            string result = "";
            for (int l = 5; l < 200; l++)
            {
                int min = 100;
                string line = "";
                for (int seed = 13; seed >= -13; seed--)
                {
                    if (seed == 0) continue;
                    for (int i = -8; i <= 8; i++)
                    {
                        if (i == 0) continue;
                        int t = seed;
                        bool hit = false;
                        for (int x = 1; x <= l; x++)
                        {
                            t += i;
                            int check = t;
                            while (check < 0) check += 27;
                            if ((check % 27 == 0) && (x != l))
                            {
                                hit = true;
                                break;
                            }
                        }
                        if (hit) continue;
                        if (t < 0) t = -t;
                        if (t % 27 == 0)
                        {
                            int d = Math.Abs(seed) + Math.Abs(i);
                            if (d < min)
                            {
                                line = "Loops.Add(" + l.ToString() + ", \"" + seed + "," + i + "\");";
                                min = d;
                            }
                        }
                    }
                }
                result += line + "\r\n";
            }
            Console.WriteLine(result);
            */
        }

        static void init(string magicPhrase)
        {
            //
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    int d = j - i;
                    int dcycle = j >= i ? (i + 30 - j) * -1 : j + 30 - i;
                    int dfinal = Math.Abs(d) <= Math.Abs(dcycle) ? d : dcycle;
                    SlotDs[i, j] = dfinal;
                    SlotDsAbs[i, j] = Math.Abs(dfinal);
                }
            }
            //
            for (int i = 0; i < 27; i++)
            {
                for (int j = 0; j < 27; j++)
                {
                    int d = j - i;
                    int dcycle = j >= i ? (i + 27 - j) * -1 : j + 27 - i;
                    int ddial = Math.Abs(d) <= Math.Abs(dcycle) ? d : dcycle;
                    int dreset = ((27 - j) > j) ? j + 3 : -(27 - j) - 3;
                    bool reset = Math.Abs(dreset) < Math.Abs(ddial);
                    int dfinal = reset ? dreset : ddial;
                    AlphaRs[i, j] = reset;
                    AlphaDs[i, j] = dfinal;
                    AlphaDsAbs[i, j] = Math.Abs(dfinal);
                }
            }
            //
            generateLoopsTable();
            PatternDebug = "";
            //
            PhraseString = magicPhrase;
            PhraseInt = new int[magicPhrase.Length];
            for (int i = 0; i < magicPhrase.Length; i++)
            {
                if (magicPhrase[i] == ' ')
                    PhraseInt[i] = 0;
                else
                    PhraseInt[i] = magicPhrase[i] - 'A' + 1;
            }
        }

        static Pattern detectPattern(string text, int index)
        {
            for (int s = 1; s <= MaxPattern; s++)
            {
                //Check if there is enough string left
                int remainder = text.Length - index;
                if (remainder < (s * MinRepeat)) continue;
                //
                string rstring = text.Substring(index, s * MinRepeat);
                string b = rstring.Substring(0, s);
                string b4 = b + b + b + b;
                //
                if (rstring.StartsWith(b4))
                {
                    //Detailed counting
                    rstring = text.Substring(index);
                    int r = 3;
                    while (rstring.StartsWith(b4))
                    {
                        b4 += b;
                        r++;
                    }
                    if ((b.Length == 1) && (r <= 12)) continue;
                    if (r >= MinRepeat)
                    {
                        return new Pattern() { Base = b, Occurrences = r };
                    }
                }
            }
            return new Pattern() { Base = "", Occurrences = 0 };
        }

        //Given a pattern, construct a loop
        static string replaceProgramBounded(int steps, Pattern pattern)
        {
            string program = "";
            if (pattern.Occurrences == 1) return pattern.Base;
            //
            string travelToCounter = (steps > 0) ? new string('>', steps) : new string('<', -steps);
            string travelToWork = (steps > 0) ? new string('<', steps) : new string('>', -steps);
            string setCounter = new string('+', pattern.Occurrences);
            string dialCounter = "-";
            if (Loops.ContainsKey(pattern.Occurrences))
            {
                string specs = Loops[pattern.Occurrences];
                int c = int.Parse(specs.Split(',')[0]);
                int d = int.Parse(specs.Split(',')[1]);
                setCounter = new string(c > 0 ? '+' : '-', Math.Abs(c));
                dialCounter = new string(d > 0 ? '+' : '-', Math.Abs(d));
            }
            //Travel to counter
            program += travelToCounter;
            //Set the counter
            program += setCounter;
            //Loop start
            program += "[";
            {
                //Travel to work
                program += travelToWork;
                //Do the work
                program += pattern.Base;
                //Travel to counter
                program += travelToCounter;
                //Inc/Dec counter
                program += dialCounter;
            }
            //Loop end
            program += "]";
            //Travel back
            program += travelToWork;
            //
            string plain = "";
            for (int x = 0; x < pattern.Occurrences; x++)
            {
                plain += pattern.Base;
            }
            plain += "<";
            if (plain.Length < program.Length) return plain;
            //
            return program;
        }

        //Given a pattern, construct a loop
        static string replaceProgram(Turing turing, Pattern pattern)
        {
            //
            PatternDebug += pattern.Occurrences.ToString() + "x(" + pattern.Base + ") ";

            //
            int steps = turing.NextEmptyCellSteps(pattern.Base);

            //
            if (steps == 1 && pattern.Base.Equals(".") && pattern.Occurrences == 70)
            {
                return ">+++++++++>+++[<[<.>+]+>-]<";
            }

            //
            int left = pattern.Occurrences;
            string result = "";
            while (left > 0)
            {
                int o = left > 26 ? 26 : left;
                result += replaceProgramBounded(steps, new Pattern() { Base = pattern.Base, Occurrences = o });
                left -= 26;
            }

            //
            return result;
        }

        static string post(string program)
        {
            Turing turing = new Turing();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < program.Length; i++)
            {
                Pattern pattern = detectPattern(program, i);
                if (pattern.Occurrences > 0)
                {
                    string loop = replaceProgram(turing, pattern);
                    sb.Append(loop);
                    turing.Execute(loop);
                    int length = pattern.Base.Length * pattern.Occurrences;
                    i += length - 1;
                    continue;
                }
                if (program[i] == '[')
                {
                    int startIndex = i;
                    int stack = 1;
                    while (stack != 0)
                    {
                        char b = program[++i];
                        if (b == '[') stack++; else if (b == ']') stack--;
                    }
                    string block = program.Substring(startIndex, i - startIndex + 1);
                    sb.Append(block);
                    turing.Execute(block);
                    continue;
                }
                sb.Append(program[i]);
                turing.Execute("" + program[i]);
            }
            string output = turing.GetOutput();
            return sb.ToString();
        }
        #endregion

        #region Run
        static Traversal best(State s, int index, int depth)
        {
            int targetLetter = PhraseInt[index];
            //
            int mind = 10000;
            string minscores = "";
            string minpath = "";
            for (int i = 0; i < 30; i++)
            {
                if (s.Actives[i] || (i < 29 && s.Actives[i + 1]) || (i > 0 && s.Actives[i - 1]))
                {
                    int dist = SlotDsAbs[s.BibloPos, i] + AlphaDsAbs[s.Letters[i], targetLetter];
                    if (depth > 1)
                    {
                        State copy = new State(s);
                        copy.Actives[i] = true;
                        copy.Letters[i] = targetLetter;
                        copy.BibloPos = i;
                        Traversal t = best(copy, index + 1, depth - 1);
                        if ((dist + t.Score) < mind)
                        {
                            mind = dist + t.Score;
                            minscores = dist.ToString() + "," + t.Scores;
                            minpath = i.ToString() + "," + t.Steps;
                        }
                    }
                    else
                    {
                        if (dist < mind)
                        {
                            mind = dist;
                            minscores = dist.ToString();
                            minpath = i.ToString();
                        }
                    }
                }
            }
            //
            return new Traversal() { Score = mind, Scores = minscores, Steps = minpath };
        }

        static string print(State s, int index, int[] positions)
        {
            string result = "";
            for (int i = index; i < index + positions.Length; i++)
            {
                //
                int position = positions[i - index];
                int slotD = SlotDs[s.BibloPos, position];
                int slotDAbs = SlotDsAbs[s.BibloPos, position];
                result += new string(slotD > 0 ? '>' : '<', slotDAbs);
                s.BibloPos = position;
                //
                int alphaD = AlphaDs[s.Letters[s.BibloPos], PhraseInt[i]];
                int alphaDAbs = AlphaDsAbs[s.Letters[s.BibloPos], PhraseInt[i]];
                bool reset = AlphaRs[s.Letters[s.BibloPos], PhraseInt[i]];
                result += (reset ? "[-]" : "") + new string(alphaD > 0 ? '+' : '-', reset ? alphaDAbs - 3 : alphaDAbs);
                s.Letters[s.BibloPos] = PhraseInt[i];
                //
                result += ".";
            }
            return result;
        }

        static void flush(Turing turing, StringBuilder sb, int start, int length)
        {
            for (int i = start; i < start + length;)
            {
                int depth = Math.Min(start + length - i, Depth);
                if (PhraseString.Length > 200) depth = Math.Min(depth, 4);
                //
                State s = turing.GetState();
                Traversal t = best(s, i, depth);
                //
                string[] ps = t.Steps.Split(',');
                int[] positions = new int[ps.Length];
                for (int x = 0; x < depth; x++)
                {
                    positions[x] = int.Parse(ps[x]);
                }
                //
                string program = print(s, i, positions);
                sb.Append(program);
                turing.Execute(program);
                i += depth;
            }
        }

        static bool hasSpace(string text)
        {
            if (text.Length < 3) return false;
            string sub = text.Substring(1, text.Length - 2);
            return sub.Contains(" ");
        }

        static string preset(Turing turing, string text)
        {
            string program = "";
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                //
                int current = turing.Current();
                int target = (text[i] == ' ') ? 0 : text[i] - 'A' + 1;
                int alphaD = AlphaDs[current, target];
                int alphaDAbs = AlphaDsAbs[current, target];
                bool reset = AlphaRs[current, target];
                string fragment = (reset ? "[-]" : "") + new string(alphaD > 0 ? '+' : '-', reset ? alphaDAbs - 3 : alphaDAbs);
                if (i != length - 1) fragment += ">";
                turing.Execute(fragment);
                //
                program += fragment;
            }
            return program;
        }

        static string run(Turing turing)
        {
            //
            StringBuilder sb = new StringBuilder();
            int residual = 0;
            for (int i = 0; i < PhraseString.Length;)
            {
                Pattern pattern = detectPattern(PhraseString, i);
                if (pattern.Occurrences > 1 && pattern.Base.Length > 4)
                {
                    //
                    flush(turing, sb, i - residual, residual);
                    residual = 0;
                    //
                    PatternDebug += pattern.Occurrences.ToString() + "x(" + pattern.Base + ") ";
                    //
                    if (pattern.Base.Length == 10)
                    {
                        //GAAVOOOLLU
                        string token = "";
                        char temp = ' ';
                        foreach (char c in pattern.Base)
                        {
                            if (c != temp)
                            {
                                temp = c;
                                token += c;
                            }
                        }
                        sb.Append(preset(turing, "" + token + " ABCDEFGHI"[3]));
                        //
                        string walk = ""; temp = ' ';
                        foreach (char c in pattern.Base)
                        {
                            if (c != temp)
                            {
                                walk += ">."; temp = c;
                            }
                            else
                            {
                                walk += ".";
                            }
                        }
                        string f = "[[<]>.>..>.>...>..>.>+++]";
                        f = "[[<]" + walk + ">+++]";
                        sb.Append(f);
                        turing.Execute(f);
                        string o = turing.GetOutput();
                        i += 10 * pattern.Occurrences;
                        continue;
                    }
                    //_WORD_E
                    int setCounter = pattern.Occurrences;
                    int loops = 0;
                    if (setCounter > 26)
                    {
                        loops = setCounter / 26;
                        setCounter = setCounter % 26;
                    }
                    string dialCounter = "-";
                    if (Loops.ContainsKey(setCounter))
                    {
                        string[] specs = Loops[pattern.Occurrences].Split(',');
                        setCounter = int.Parse(specs[0]);
                        int d = int.Parse(specs[1]);
                        dialCounter = new string(d > 0 ? '+' : '-', Math.Abs(d));
                    }
                    //Set counter is always positive and at most 9
                    char counter = " ABCDEFGHI"[setCounter];
                    sb.Append(preset(turing, "" + pattern.Base.Trim() + " " + counter));
                    //
                    string fragment = "";
                    if (pattern.Base.StartsWith(" "))
                    {
                        fragment = "[<<[<].>[.>]>@]".Replace("@", dialCounter);
                    }
                    else if (pattern.Base.EndsWith(" "))
                    {
                        fragment = "[<<[<]>[.>].>@]".Replace("@", dialCounter);
                    }
                    else
                    {
                        fragment = "[<<[<]>[.>]>@]".Replace("@", dialCounter);
                        if (hasSpace(pattern.Base))
                        {
                            fragment = "[<<[<]<[<]>[.>].>[.>]>@]".Replace("@", dialCounter);
                            if (pattern.Base.Length == 7 && pattern.Base[5] == ' ')
                            {
                                fragment = "[<<<<<<<<[.>].>.>>@]".Replace("@", dialCounter);
                            }
                        }
                    }
                    sb.Append(fragment);
                    turing.Execute(fragment);
                    //
                    while (loops > 0)
                    {
                        bool isHere = true;
                        if (pattern.Base.StartsWith(" "))
                        {
                            fragment = "-[<<[<].>[.>]>-]";
                        }
                        else if (pattern.Base.EndsWith(" "))
                        {
                            fragment = "-[<<[<]>[.>].>-]";
                        }
                        else
                        {
                            fragment = "-[<<[<]>[.>]>-]";
                        }
                        sb.Append(fragment);
                        turing.Execute(fragment);
                        loops--;
                    }
                    //
                    i += pattern.Base.Length * pattern.Occurrences;
                    continue;
                }
                residual++;
                i++;
            }
            flush(turing, sb, PhraseString.Length - residual, residual);
            return sb.ToString();
        }
        #endregion

        static string pre(Turing turing)
        {
            string program = "";
            //
            Dictionary<char, int> cFrequencies = PhraseString.ToCharArray().Where(c => c != ' ').GroupBy(c => c).ToDictionary(c => c.Key, c => c.Count());
            List<Tuple<char, int>> chars = cFrequencies.ToList().OrderByDescending(c => c.Value).Select(c => new Tuple<char, int>(c.Key, c.Value)).ToList();
            Dictionary<string, int> wFrequencies = PhraseString.Split(' ').GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());
            List<Tuple<string, int>> words = wFrequencies.ToList().OrderByDescending(w => w.Value).Select(w => new Tuple<string, int>(w.Key, w.Value)).ToList(); ;
            List<string> shortWords = wFrequencies.ToList().OrderBy(w => w.Key.Length).Select(w => w.Key).ToList();
            //Random 4 chars
            if (PhraseString.Length > 20 && chars.Count() == 3)
            {
                string seed = "" + chars[1].Item1 + chars[0].Item1 + chars[2].Item1;
                PatternDebug += "Random 4 " + seed + " ";
                program += preset(turing, "" + seed);
                return program;
            }
            //Two words alternating
            if (shortWords.Count() <= 6 && shortWords.Count() >= 4)
            {
                string w1 = shortWords[0]; string w2 = shortWords[1];
                bool isGood = true;
                string remainder = PhraseString;
                while (remainder.Length > 0)
                {
                    if (remainder.StartsWith(" "))
                    {
                        remainder = remainder.Substring(1);
                        continue;
                    }
                    else if (remainder.StartsWith(w1))
                    {
                        remainder = remainder.Substring(w1.Length);
                        continue;
                    }
                    else if (remainder.StartsWith(w2))
                    {
                        remainder = remainder.Substring(w2.Length);
                        continue;
                    }
                    isGood = false;
                    break;
                }
                if (isGood)
                {
                    PatternDebug += "Two Words ";
                    //Set all to M
                    program += "-[<+>++]<[<-]>";
                    turing.Execute(program);
                    //Preset two words and spacer
                    program += preset(turing, w1 + " " + w2 + " ");
                    program += "<[<]";
                    //
                    remainder = PhraseString;
                    while (remainder.Length > 0)
                    {
                        if (remainder.StartsWith(" "))
                        {
                            remainder = remainder.Substring(1);
                            program += ".";
                        }
                        else if (remainder.StartsWith(w1))
                        {
                            remainder = remainder.Substring(w1.Length);
                            program += "<[<]>[.>]";
                        }
                        else if (remainder.StartsWith(w2))
                        {
                            remainder = remainder.Substring(w2.Length);
                            program += ">[.>]<[<]";
                        }
                    }
                    return "x" + program;
                }
            }
            //Freq analysis
            if (PhraseString.Length > 300)
            {
                //
                program += "+++++[<<<+>+++>+>-]"; //EOE
                turing.Execute(program);
                string phrase = words[0].Item1;
                program += preset(turing, phrase); //executed
                //
                string fragment = "";// "<[<]";
                turing.Execute(fragment);
                //
                PatternDebug += "Frequency Analysis " + words[0].Item1 + " ";
                return program + fragment;
            }
            //First letter is O
            if (PhraseString.Length > 2 && PhraseString[0] == 'O' && PhraseString[1] != ' ')
            {
                PatternDebug += "First Letter O ";
                program = "+++[<+>--]";
            }
            //Middle Spell
            int dev0 = 0;
            int devM = 0;
            for (int i = 0; i < PhraseInt.Length; i++)
            {
                dev0 += Math.Min(27 - PhraseInt[i], PhraseInt[i]);
                devM += Math.Abs(PhraseInt[i] - 13);
            }
            double avg0 = 1.0 * dev0 / PhraseInt.Length;
            double avgM = 1.0 * devM / PhraseInt.Length;
            if ((avgM + 0.1) < avg0)
            {
                Pattern pattern = detectPattern(PhraseString, 0);
                if (pattern.Occurrences <= 0)
                {
                    PatternDebug += "Middle Spell ";
                    program = "+[<<+>+>++]";
                }
            }
            //
            turing.Execute(program);
            return program;
        }

        public static string Optimize(string magicPhrase)
        {
            init(magicPhrase);
            //
            Turing turing = new Turing();
            string program = pre(turing);
            //
            program += (program.StartsWith("x") ? "" : run(turing));
            if (program.StartsWith("x")) return program.Substring(1);
            //
            if (program.EndsWith(".+.-.+.-.+.")) program += "-";
            if (program.EndsWith(".-.+.-.+.-.")) program += "+";
            if (program.EndsWith(".+.+.+.+.")) program += "+";
            if (program.EndsWith(".-.-.-.-.")) program += "-";
            //
            program = post(program);
            program = post(program);
            //
            program = program.Replace("+-", "").Replace("-+", "").Replace("<>", "").Replace("><", "");
            program = program.TrimEnd('-');
            program = program.TrimEnd('+');
            program = program.TrimEnd('<');
            program = program.TrimEnd('>');
            program = program.TrimStart('<');
            program = program.TrimStart('>');
            program = program.Replace("<.<-.---.<<<<.<.>>.>>", "<.<-.---.[<].<.>>.>>");
            return program;
        }

        static void Main(string[] args)
        {
            string magicPhrase = Console.ReadLine();
            Console.WriteLine(Bilbo.Optimize(magicPhrase));
        }
    }
}
