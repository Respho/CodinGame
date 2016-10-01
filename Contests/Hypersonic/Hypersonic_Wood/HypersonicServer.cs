using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypersonicWindow
{
    public class HypersonicServer
    {
        #region Basics
        public static List<Point> Directions = new List<Point>();
        public const int W = 13, H = 11;
        HypersonicPlayer player = null;
        HypersonicHuman human = null;
        public List<ServerGameState> States = new List<ServerGameState>();
        public int StatePointer;

        public HypersonicServer()
        {
            if (Directions.Count() == 0)
            {
                Directions.Add(new Point(1, 0));
                Directions.Add(new Point(-1, 0));
                Directions.Add(new Point(0, 1));
                Directions.Add(new Point(0, -1));
            }
            player = new HypersonicPlayer(0);
            human = new HypersonicHuman(1);
            States.Add(ServerGameState.Initial());
        }

        string Debug = "";
        void debug(string message)
        {
            Debug += message + "\n";
        }

        public static int Manhattan(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        /*
        http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
        OPEN = priority queue containing START
        CLOSED = empty set
        while lowest rank in OPEN is not the GOAL:
            current = remove lowest rank item from OPEN
            add current to CLOSED
            for neighbors of current:
            cost = g(current) + movementcost(current, neighbor)
            if neighbor in OPEN and cost less than g(neighbor):
                remove neighbor from OPEN, because new path is better
            if neighbor in CLOSED and cost less than g(neighbor): ⁽²⁾
                remove neighbor from CLOSED
            if neighbor not in OPEN and neighbor not in CLOSED:
                set g(neighbor) to cost
                add neighbor to OPEN
                set priority queue rank to g(neighbor) + h(neighbor)
                set neighbor's parent to current
        */
        public static List<Point> GetPath(ServerGameState state, Point startPoint, Point endPoint)
        {
            Dictionary<Point, Point> Froms = new Dictionary<Point, Point>();
            Dictionary<Point, int> Steps = new Dictionary<Point, int>();
            Dictionary<Point, int> Distances = new Dictionary<Point, int>();
            //
            HashSet<Point> OPEN = new HashSet<Point>() { startPoint };
            HashSet<Point> CLOSED = new HashSet<Point>();

            //
            int Step = 0;
            Froms[startPoint] = startPoint;
            Steps[startPoint] = Step;
            Distances[startPoint] = 0;
            //while lowest rank in OPEN is not the GOAL
            while ((OPEN.Count() > 0) && (OPEN.OrderBy(x => Distances[x]).First() != endPoint))
            {
                //current = remove lowest rank item from OPEN
                //add current to CLOSED
                Point current = OPEN.OrderBy(x => Distances[x]).First();
                OPEN.Remove(current);
                CLOSED.Add(current);
                //for neighbors of current
                List<Point> neighbors = new List<Point>();
                neighbors.Add(new Point(current.X + 1, current.Y));
                neighbors.Add(new Point(current.X - 1, current.Y));
                neighbors.Add(new Point(current.X, current.Y + 1));
                neighbors.Add(new Point(current.X, current.Y - 1));
                foreach (Point neighbor in neighbors)
                {
                    //Is valid neighbor?
                    if (neighbor.X >= W || neighbor.X < 0 || neighbor.Y >= H || neighbor.Y < 0) continue;
                    if (state.Board[neighbor.X, neighbor.Y] != '.') continue;
                    if (state.Entities.Exists(e => e.Position.Equals(neighbor) && e.Type == ServerGameState.EntityType.Bomb)) continue;
                    //cost = g(current) + movementcost(current, neighbor)
                    int cost = Distances[current] + 1;
                    //if neighbor in OPEN and cost less than g(neighbor)
                    if (OPEN.Contains(neighbor) && (cost < Distances[neighbor]))
                    {
                        //remove neighbor from OPEN, because new path is better
                        OPEN.Remove(neighbor);
                    }
                    //if neighbor in CLOSED and cost less than g(neighbor)
                    if (CLOSED.Contains(neighbor) && (cost < Distances[neighbor]))
                    {
                        //remove neighbor from CLOSED
                        CLOSED.Remove(neighbor);
                    }
                    //if neighbor not in OPEN and neighbor not in CLOSED:
                    if (!OPEN.Contains(neighbor) && !CLOSED.Contains(neighbor))
                    {
                        //set g(neighbor) to cost
                        Distances[neighbor] = cost;
                        //add neighbor to OPEN
                        OPEN.Add(neighbor);
                        //set neighbor's parent to current
                        Froms[neighbor] = current;
                        Steps[neighbor] = Steps[current] + 1;
                    }
                }
            }

            //
            List<Point> Result = new List<Point>();
            //Impossible
            if (!Froms.ContainsKey(endPoint))
            {
                return Result;
            }
            //
            Point currNode = endPoint;
            while (currNode != startPoint)
            {
                Result.Add(currNode);
                currNode = Froms[currNode];
            }
            //
            return Result;
        }

        public ServerGameState GetCurrent()
        {
            return States[StatePointer];
        }
        #endregion

        public void PlayOneMove()
        {
            ServerGameState latest = States[States.Count() - 1];
            ServerGameState state = new ServerGameState(latest);
            States.Add(state);
            StatePointer = States.Count() - 1;

            //
            Debug = "";
            debug("GetSerialization()");
            string serialization = state.GetSerialization();
            debug("player.GetMove()");
            string movePlayer = player.GetMove(serialization);
            debug("human.GetMove()");
            string moveHuman = human.GetMove(serialization);
            state.PlayerDebug = player.Debug;
            state.Moves = movePlayer + "\n" + moveHuman;

            //
            debug("updateGameState()");
            updateGameState(state);
            state.ServerDebug = Debug;
        }

        private void updateGameState(ServerGameState state)
        {
            //
            const int Exploded = 99;
            //Countdown
            foreach (ServerGameState.Entity bomb in state.Entities.Where(e => e.Type == ServerGameState.EntityType.Bomb))
            {
                bomb.Rounds--;
            }
            //Explode the bombs
            List<Point> boxes = new List<Point>();
            List<int> playersHit = new List<int>();
            while (state.Entities.Exists(e => e.Type == ServerGameState.EntityType.Bomb && e.Rounds == 0))
            {
                ServerGameState.Entity bomb = state.Entities.First(e => e.Type == ServerGameState.EntityType.Bomb && e.Rounds == 0);
                bomb.Rounds = Exploded;
                state.Blasts.Add(bomb.Position);
                //
                foreach (Point p in Directions)
                {
                    int checkRange = (bomb.Range - 1) / 2;
                    for (int i = 1; i <= checkRange; i++)
                    {
                        //
                        Point candidate = new Point(bomb.Position.X + p.X * i, bomb.Position.Y + p.Y * i);
                        if (candidate.X < 0 || candidate.X >= W || candidate.Y < 0 || candidate.Y >= H)
                            break;
                        state.Blasts.Add(candidate);
                        //Mark players
                        List<int> players = state.Entities
                            .Where(e => e.Type == ServerGameState.EntityType.Player && e.Position == candidate)
                            .Select(e => e.Team).ToList();
                        foreach (int player in players) playersHit.Add(player);
                        //Mark boxes and stop the blast
                        if ("012".Contains(state.Board[candidate.X, candidate.Y]))
                        {
                            if (bomb.Team == 0)
                            {
                                state.ScorePlayer++;
                            }
                            else
                            {
                                state.ScoreHuman++;
                            }
                            boxes.Add(candidate);
                            break;
                        }
                        //Trigger nearby bombs
                        if (state.Entities.Exists(e => e.Type == ServerGameState.EntityType.Bomb && e.Position == candidate && e.Rounds != Exploded))
                        {
                            state.Entities.Single(e => e.Type == ServerGameState.EntityType.Bomb && e.Position == candidate).Rounds = 0;
                            break;
                        }
                        //Stop at items
                        if (state.Entities.Exists(e => e.Type == ServerGameState.EntityType.Item && e.Position == candidate))
                        {
                            ServerGameState.Entity item = state.Entities.First(e => e.Type == ServerGameState.EntityType.Item && e.Position == candidate);
                            state.Entities.Remove(item);
                            break;
                        }
                        //Stop at walls
                        if (state.Board[candidate.X, candidate.Y] == 'X')
                        {
                            break;
                        }
                    }
                }
            }

            //Move the players
            Dictionary<int, Point> bombs = new Dictionary<int, Point>();
            //Cannot move to a box or bomb cell
            string[] moves = state.Moves.Split('\n');
            int team = 0;
            foreach (string move in moves)
            {
                ServerGameState.Entity player = state.Entities.Single(e => e.Team == team && e.Type == ServerGameState.EntityType.Player);
                string[] tokens = move.Split(' ');
                //
                if (tokens[0] == "BOMB")
                {
                    bombs[team] = player.Position;
                    state.Results += "Player " + team + " wants to plant a bomb\n";
                }
                //
                Point target = new Point(int.Parse(tokens[1]), int.Parse(tokens[2]));
                if (target == player.Position)
                {
                    state.Results += "Player " + team + " standing still\n";
                    team++;
                    continue;
                }
                //
                List<Tuple<Point, int>> distances = new List<Tuple<Point,int>>();
                for (int i = 0; i < W; i++)
                {
                    for (int j = 0; j < H; j++)
                    {
                        if (state.Board[i, j] == '.')
                        {
                            Point candidate = new Point(i, j);
                            if (!state.Entities.Exists(e => e.Type == ServerGameState.EntityType.Bomb && e.Position == candidate))
                            {
                                distances.Add(new Tuple<Point, int>(candidate, Manhattan(target, candidate)));
                            }
                        }
                    }
                }
                distances = distances.OrderBy(t => t.Item2).ToList();
                //
                bool found = false;
                foreach (Tuple<Point, int> candidate in distances)
                {
                    //
                    List<Point> path = new List<Point>();
                    path.Add(player.Position);
                    if (player.Position != candidate.Item1)
                    {
                        path = GetPath(state, player.Position, candidate.Item1);
                        if (path.Count() == 0) continue;
                    }
                    //
                    Point final = path[path.Count() - 1];
                    if (Manhattan(player.Position, final) > 1)
                    {
                        throw new Exception("Manhattan distance != 1");
                    }
                    state.Results += "Player " + team + " moving to (" + final.X + ", " + final.Y + ")\n";
                    player.Position = final;
                    found = true;
                    break;
                }
                //
                if (!found)
                {
                    state.Results += "Player " + team + " standing still\n";
                }
                team++;
            }

            //Account for powerups and pickup items
            team = 0;
            foreach (string move in moves)
            {
                ServerGameState.Entity player = state.Entities.First(e => e.Team == team && e.Type == ServerGameState.EntityType.Player);
                if (state.Entities.Exists(e => e.Type == ServerGameState.EntityType.Item && e.Position == player.Position))
                {
                    ServerGameState.Entity item = state.Entities.Single(e => e.Type == ServerGameState.EntityType.Item && e.Position == player.Position);
                    if (item.Rounds == 1)
                    {
                        player.Range += 2;
                        state.Results += "Player " + team + " got RANGE, now at " + player.Range + "\n";
                    }
                    else if (item.Rounds == 2)
                    {
                        player.Rounds += 1;
                        state.Results += "Player " + team + " got BOMB, now at " + player.Rounds + "\n";
                    }
                }
                team++;
            }
            List<Point> playerPositions = state.Entities.Where(e => e.Type == ServerGameState.EntityType.Player).Select(e => e.Position).ToList();
            state.Entities = state.Entities.Where(e => !(e.Type == ServerGameState.EntityType.Item && playerPositions.Contains(e.Position))).ToList();

            //Boxes and items
            foreach (Point box in boxes.Distinct())
            {
                //Drop items
                if (state.Board[box.X, box.Y] == '1' || state.Board[box.X, box.Y] == '2')
                {
                    state.Entities.Add(new ServerGameState.Entity()
                    {
                        Type = ServerGameState.EntityType.Item,
                        Team = 0,
                        Position = box,
                        Rounds = int.Parse("" + state.Board[box.X, box.Y]),
                        Range = 0
                    });
                }
                //Remove box
                state.Board[box.X, box.Y] = '.';
            }

            //Place bombs
            foreach (int teamNo in bombs.Keys)
            {
                int bombCount = state.Entities.Count(e => e.Team == teamNo && e.Type == ServerGameState.EntityType.Bomb);
                int allowed = state.Entities.Single(e => e.Team == teamNo && e.Type == ServerGameState.EntityType.Player).Rounds;
                if (bombCount >= allowed)
                {
                    state.Results += "Player " + teamNo + " limit reached " + allowed;
                }
                else if (state.Entities.Exists(e => e.Team == teamNo && e.Type == ServerGameState.EntityType.Bomb && e.Position == bombs[teamNo]))
                {
                    state.Results += "Player " + teamNo + " already planted";
                }
                else
                {
                    state.Entities.Add(new ServerGameState.Entity()
                    {
                        Team = teamNo,
                        Position = bombs[teamNo],
                        Type = ServerGameState.EntityType.Bomb,
                        Range = state.Entities.Single(e => e.Team == teamNo && e.Type == ServerGameState.EntityType.Player).Range,
                        Rounds = 8
                    });
                    state.Results += "Player " + teamNo + " planted (" + bombs[teamNo].X + ", " + bombs[teamNo].Y + ") " +
                        "ranged " + state.Entities.Single(e => e.Team == teamNo && e.Type == ServerGameState.EntityType.Player).Range + "\n";
                }
            }

            //Remove bombs
            state.Entities = state.Entities.Where(e => e.Rounds != Exploded).ToList();
        }

        public void Move(Point direction, bool bomb)
        {
            human.SetMove(direction, bomb);
            PlayOneMove();
        }

        public void Next()
        {
            StatePointer++;
            if (StatePointer >= States.Count()) StatePointer = States.Count() - 1;
        }

        public void Prev()
        {
            StatePointer--;
            if (StatePointer < 0) StatePointer = 0;
        }
    }

    public class ServerGameState
    {
        public enum EntityType { Player = 0, Bomb = 1, Item = 2 }
        public class Entity
        {
            public EntityType Type;
            public System.Drawing.Point Position;
            public int Team;   //ignored if item
            public int Rounds; //1 = extra range, 2 = extra bomb
            public int Range;  //explosion range of player's bombs / bomb's explosion range / ignored
        }

        public const int W = 13, H = 11;
        public char[,] Board = new char[W, H];
        public List<Entity> Entities = new List<Entity>();
        public List<Point> Blasts = new List<Point>();
        public string ServerDebug, PlayerDebug, Moves, Results;
        public int ScorePlayer, ScoreHuman;

        public static ServerGameState Initial()
        {
            //
            char[,] board = new char[W, H];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    board[i, j] = '.';
                }
            }
            //
            board[2, 1] = '1'; board[4, 1] = '2'; board[6, 1] = '1'; board[8, 1] = '2'; board[10, 1] = '1';
            board[0, 2] = '0'; board[12, 2] = '0';
            board[2, 3] = '2'; board[5, 3] = '1'; board[7, 3] = '1'; board[10, 3] = '2';
            board[0, 4] = '2'; board[12, 4] = '2';
            //
            board[2, 5] = '0'; board[4, 5] = '1'; board[8, 5] = '1'; board[10, 5] = '0';
            //
            board[0, 6] = '2'; board[12, 6] = '2';
            board[2, 7] = '2'; board[5, 7] = '1'; board[7, 7] = '1'; board[10, 7] = '2';
            board[0, 8] = '0'; board[12, 8] = '0';
            board[2, 9] = '1'; board[4, 9] = '2'; board[6, 9] = '1'; board[8, 9] = '2'; board[10, 9] = '1';
            //
            List<Entity> entities = new List<Entity>();
            entities.Add(new Entity() { Type = EntityType.Player, Position = new Point(0, 0), Team = 0, Rounds = 1, Range = 3 });
            entities.Add(new Entity() { Type = EntityType.Player, Position = new Point(W - 1, H - 1), Team = 1, Rounds = 1, Range = 3 });
            //
            return new ServerGameState(board, entities, "", "", "", "", 0, 0);
        }

        public ServerGameState(char[,] board, List<Entity> entities, string serverDebug, string playerDebug, string moves, string results, int scorePlayer, int scoreHuman)
        {
            Board = board; Entities = entities;
            ServerDebug = serverDebug; PlayerDebug = playerDebug; Moves = moves; Results = results;
            ScorePlayer = scorePlayer; ScoreHuman = scoreHuman;
        }

        public ServerGameState(ServerGameState state)
        {
            //
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Board[i, j] = state.Board[i, j];
                }
            }
            //
            foreach (Entity e in state.Entities)
            {
                Entities.Add(new Entity() { Type = e.Type, Team = e.Team, Position = e.Position, Rounds = e.Rounds, Range = e.Range });
            }
            //
            ScorePlayer = state.ScorePlayer; ScoreHuman = state.ScoreHuman;
        }

        #region Public
        public string getBoard()
        {
            string lines = "";
            for (int j = 0; j < H; j++)
            {
                string line = "";
                for (int i = 0; i < W; i++)
                {
                    line += Board[i, j];
                }
                lines += line + "\n";
            }
            return lines;
        }
        public string GetSerialization()
        {
            //
            string lines = getBoard();
            //
            lines += Entities.Count() + "\n";
            foreach (Entity e in Entities)
            {
                string line = ((int)e.Type).ToString() + " " + e.Team + " " + e.Position.X + " " + e.Position.Y + " " + e.Rounds + " " + e.Range;
                lines += line + "\n";
            }
            //
            return lines;
        }
        public string GetBoard()
        {
            //
            string board = getBoard();
            board = board.Replace("1", "R").Replace("2", "B");
            //
            StringBuilder sb = new StringBuilder(board);
            foreach (Point p in Blasts)
            {
                int index = p.X + p.Y * (W + 1);
                sb[index] = '*';
            }
            //
            foreach (Entity entity in Entities)
            {
                int index = entity.Position.X + entity.Position.Y * (W + 1);
                if (entity.Type == EntityType.Player)
                {
                    sb[index] = "XY"[entity.Team];
                }
                else if (entity.Type == EntityType.Bomb)
                {
                    sb[index] = "012345678"[entity.Rounds];
                }
                else if (entity.Type == EntityType.Item)
                {
                    sb[index] = "xrb"[entity.Rounds];
                }
            }
            //
            return sb.ToString();
        }
        #endregion
    }
}
