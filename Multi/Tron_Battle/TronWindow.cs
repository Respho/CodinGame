using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TronWindow
{
    public partial class TronWindow : Form
    {
        TronServer Server;

        public TronWindow()
        {
            InitializeComponent();
        }

        private void TronWindow_Load(object sender, EventArgs e)
        {
            Server = new TronServer();

            picState.Image = Server.States[0].GetState();
        }

        private void update()
        {
            //
            int Round = Server.StatePointer;
            lblTurn.Text = (Round * 2).ToString() + "/400";
            //
            ServerGameState state = Server.GetCurrent();
            picState.Image = state.GetState();
            picState.Invalidate();
            txtGameState.Text = state.GetBoard().Replace("\n", "\r\n");
            txtPlayer.Text = state.PlayerDebug.Replace("\n", "\r\n");
            txtServer.Text = state.ServerDebug.Replace("\n", "\r\n");
            txtMoves.Text = state.Moves.Replace("\n", "\r\n");
            txtResults.Text = state.Results.Replace("\n", "\r\n");
            //
            lblScore.Text = "--";
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            Server.Prev();
            update();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Server.Next();
            update();
        }

        Point direction;

        private void playMove()
        {
            Server.Move(direction);
            update();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            direction = new Point(0, -1);
            playMove();
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            direction = new Point(0, 1);
            playMove();
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            direction = new Point(1, 0);
            playMove();
        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            direction = new Point(-1, 0);
            playMove();
        }
    }
}
