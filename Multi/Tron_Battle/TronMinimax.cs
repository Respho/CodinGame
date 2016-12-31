using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronWindow
{
    public class Evaluation
    {
        public bool IsEndGame;
        public List<Point> TerritoryPlayer = new List<Point>();
        public List<Point> TerritoryOpponent = new List<Point>();
        public double Score;

        public static Evaluation Evaluate(GameState state, int playerNumber, int opponentNumber)
        {
            //
            Evaluation evaluation = new Evaluation();
            evaluation.IsEndGame = false;

            //Voronoi heuristic
            int territoryRound = 9999;
            List<Point> listPlayer = new List<Point>();
            listPlayer.Add(state.Players[playerNumber]);
            List<Point> listOpponent = new List<Point>();
            listOpponent.Add(state.Players[opponentNumber]);
            char player = (char)(playerNumber + 48); char opponent = (char)(opponentNumber + 48);
            while (territoryRound > 0)
            {
                territoryRound = 0;
                //
                List<Point> newListPlayer = new List<Point>();
                foreach (Point p in listPlayer)
                {
                    foreach (Point v in GameState.DirectionVectors)
                    {
                        Point next = new Point(p.X + v.X, p.Y + v.Y);
                        if (next.X < GameState.W && next.X >= 0 && next.Y < GameState.H && next.Y >= 0)
                        {
                            if (state.Board[next.X, next.Y] == GameState.BLANK)
                            {
                                state.Board[next.X, next.Y] = player;
                                newListPlayer.Add(next);
                                evaluation.TerritoryPlayer.Add(next);
                                territoryRound++;
                            }
                        }
                    }
                }
                listPlayer = newListPlayer;
                //
                List<Point> newListOpponent = new List<Point>();
                foreach (Point p in listOpponent)
                {
                    foreach (Point v in GameState.DirectionVectors)
                    {
                        Point next = new Point(p.X + v.X, p.Y + v.Y);
                        if (next.X < GameState.W && next.X >= 0 && next.Y < GameState.H && next.Y >= 0)
                        {
                            if (state.Board[next.X, next.Y] == GameState.BLANK)
                            {
                                state.Board[next.X, next.Y] = opponent;
                                newListOpponent.Add(next);
                                evaluation.TerritoryOpponent.Add(next);
                                territoryRound++;
                            }
                        }
                    }
                }
                listOpponent = newListOpponent;
            }
            //DOF
            int dof = state.GetDOF(playerNumber);
            //
            evaluation.Score = (10000 * evaluation.TerritoryPlayer.Count()) - (100 * evaluation.TerritoryOpponent.Count()) - dof;
            //
            return evaluation;
        }
    }

    public class GameTree
    {
        //Core
        public GameTree Ancestor = null;
        public List<GameTree> Children = new List<GameTree>();
        public GameState State = null;
        //Attributes
        public string Label;
        public int Ply; //Current node depth = 0, player's move (3 dof) 3 nodes = 1 ply
        public Evaluation Score = new Evaluation();

        //Initial - SpanTree(null, current, playerNumber, opponentNumber, 3 (me them me), 0 (current))
        public static GameTree SpanTree(GameTree ancestor, GameState state, int playerNumber, int opponentNumber, int depth, int ply)
        {
            GameTree gameTree = new GameTree();
            gameTree.Ancestor = ancestor;
            gameTree.State = state;
            gameTree.Label = ancestor == null ? "" : (ancestor.Label + ">" + GameState.GetDirection(
                ancestor.State.Players[(ply % 2 == 1) ? playerNumber : opponentNumber],
                state.Players[(ply % 2 == 1) ? playerNumber : opponentNumber]).ToString());
            gameTree.Ply = ply;
            if (depth > 0)
            {
                int currentPly = ply + 1;
                int currentPlayer = (currentPly % 2 == 1) ? playerNumber : opponentNumber; 
                List<GameState> childStates = state.GetChildren(currentPlayer);
                foreach (GameState childState in childStates)
                {
                    gameTree.Children.Add(SpanTree(gameTree, childState, playerNumber, opponentNumber, depth - 1, currentPly));
                }
            }
            return gameTree;
        }
    }

    //https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
    public class TronMinimax
    {
        public int PlayerNumber, OpponentNumber;
        public TronMinimax(int playerNumber, int opponentNumber)
        {
            PlayerNumber = playerNumber;
            OpponentNumber = opponentNumber;
        }

        //01 function alphabeta(node, depth, α, β, maximizingPlayer)
        //02      if depth = 0 or node is a terminal node
        //03          return the heuristic value of node
        //04      if maximizingPlayer
        //05          v := -∞
        //06          for each child of node
        //07              v := max(v, alphabeta(child, depth – 1, α, β, FALSE))
        //08              α := max(α, v)
        //09              if β ≤ α
        //10                  break (* β cut-off *)
        //11          return v
        //12      else
        //13          v := ∞
        //14          for each child of node
        //15              v := min(v, alphabeta(child, depth – 1, α, β, TRUE))
        //16              β := min(β, v)
        //17              if β ≤ α
        //18                  break (* α cut-off *)
        //19          return v
        //
        //Initial - alphabeta(origin, depth, -∞, +∞, TRUE)
        public GameTree MinimaxAlphaBeta(GameTree node, int depth, double alpha, double beta, bool maximizingPlayer)
        {
            bool terminal = node.Children.Count() == 0;
            if (depth == 0 || terminal)
            {
                node.Score = Evaluation.Evaluate(node.State, PlayerNumber, OpponentNumber);
                return node;
            }
            GameTree gameTreeScore = new GameTree();
            if (maximizingPlayer)
            {
                gameTreeScore.Score.Score = double.MinValue;
                foreach (GameTree child in node.Children)
                {
                    GameTree childScore = MinimaxAlphaBeta(child, depth - 1, alpha, beta, false);
                    if (childScore.Score.Score >= gameTreeScore.Score.Score) gameTreeScore = childScore;
                    alpha = Math.Max(alpha, gameTreeScore.Score.Score);
                    if (beta <= alpha) break;
                }
                return gameTreeScore;
            }
            else
            {
                gameTreeScore.Score.Score = double.MaxValue;
                foreach (GameTree child in node.Children)
                {
                    GameTree childScore = MinimaxAlphaBeta(child, depth - 1, alpha, beta, true);
                    if (childScore.Score.Score <= gameTreeScore.Score.Score) gameTreeScore = childScore;
                    beta = Math.Min(beta, gameTreeScore.Score.Score);
                    if (beta <= alpha) break;
                }
                return gameTreeScore;
            }
        }
    }
}
