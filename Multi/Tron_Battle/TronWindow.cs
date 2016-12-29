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
            txtGameState.Text = Server.GetCurrent().GetBoard().Replace("\n", "\r\n");

            ////
            //if (picState.Image == null)
            //{
            //    Bitmap bitmap = new Bitmap(picState.Width, picState.Height);
            //    using (Graphics g = Graphics.FromImage(bitmap))
            //    {
            //        g.Clear(Color.FromArgb(255, 20, 80, 20));
            //    }
            //    picState.Image = bitmap;
            //}
            ////
            //using (Graphics g = Graphics.FromImage(picState.Image))
            //{
            //    Pen greenPen = new Pen(Color.FromArgb(255, 0, 180, 0), 10);
            //    greenPen.Alignment = PenAlignment.Center;
            //    g.DrawLine(greenPen, 10, 100, 100, 50);
            //    //g.DrawImage(new Bitmap(@"C:\Users\Mena\Desktop\1.png"), new Point(182, 213));
            //}
            ////
            //picState.Invalidate();
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
