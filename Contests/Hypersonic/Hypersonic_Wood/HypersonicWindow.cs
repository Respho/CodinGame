using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HypersonicWindow
{
    public partial class HypersonicWindow : Form
    {
        HypersonicServer Server;

        public HypersonicWindow()
        {
            InitializeComponent();
        }

        private void HypersonicWindow_Load(object sender, EventArgs e)
        {
            Server = new HypersonicServer();
            txtGameState.Text = Server.GetCurrent().GetBoard().Replace("\n", "\r\n");
        }

        private void update()
        {
            //
            int Round = Server.StatePointer;
            lblTurn.Text = (Round * 2).ToString() + "/400";
            //
            ServerGameState state = Server.GetCurrent();
            txtGameState.Text = state.GetBoard().Replace("\n", "\r\n");
            txtPlayer.Text = state.PlayerDebug.Replace("\n", "\r\n");
            txtMoves.Text = state.Moves.Replace("\n", "\r\n");
            txtServer.Text = state.ServerDebug.Replace("\n", "\r\n");
            txtResults.Text = state.Results.Replace("\n", "\r\n");
            //
            lblScore.Text = state.ScorePlayer + "/" + state.ScoreHuman;
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
        bool bomb;

        private void playMove()
        {
            Server.Move(direction, bomb);
            bomb = false;
            btnBomb.BackColor = default(Color);
            update();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            direction = new Point(0, -1);
            playMove();
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            direction = new Point(1, 0);
            playMove();
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            direction = new Point(0, 1);
            playMove();
        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            direction = new Point(-1, 0);
            playMove();
        }
        private void btnStay_Click(object sender, EventArgs e)
        {
            direction = new Point(0, 0);
            playMove();
        }
        private void btnBomb_Click(object sender, EventArgs e)
        {
            btnBomb.BackColor = Color.OrangeRed;
            bomb = true;
        }

    }
}
