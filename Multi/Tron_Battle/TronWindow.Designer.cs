﻿namespace TronWindow
{
    partial class TronWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnPrev = new System.Windows.Forms.Button();
            this.picState = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtPlayer = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.txtMoves = new System.Windows.Forms.TextBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.lblTurn = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.txtGameState = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picState)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(656, 607);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(75, 23);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.Text = "Prev";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // picState
            // 
            this.picState.Location = new System.Drawing.Point(12, 12);
            this.picState.Name = "picState";
            this.picState.Size = new System.Drawing.Size(480, 320);
            this.picState.TabIndex = 3;
            this.picState.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // txtServer
            // 
            this.txtServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServer.Enabled = false;
            this.txtServer.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServer.Location = new System.Drawing.Point(498, 12);
            this.txtServer.Multiline = true;
            this.txtServer.Name = "txtServer";
            this.txtServer.ReadOnly = true;
            this.txtServer.Size = new System.Drawing.Size(320, 385);
            this.txtServer.TabIndex = 4;
            // 
            // txtPlayer
            // 
            this.txtPlayer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPlayer.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayer.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtPlayer.Location = new System.Drawing.Point(824, 12);
            this.txtPlayer.MaxLength = 999999;
            this.txtPlayer.Multiline = true;
            this.txtPlayer.Name = "txtPlayer";
            this.txtPlayer.ReadOnly = true;
            this.txtPlayer.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPlayer.ShortcutsEnabled = false;
            this.txtPlayer.Size = new System.Drawing.Size(490, 719);
            this.txtPlayer.TabIndex = 5;
            this.txtPlayer.TabStop = false;
            this.txtPlayer.WordWrap = false;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(737, 607);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // txtMoves
            // 
            this.txtMoves.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMoves.Enabled = false;
            this.txtMoves.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoves.Location = new System.Drawing.Point(497, 403);
            this.txtMoves.Multiline = true;
            this.txtMoves.Name = "txtMoves";
            this.txtMoves.ReadOnly = true;
            this.txtMoves.Size = new System.Drawing.Size(320, 78);
            this.txtMoves.TabIndex = 7;
            // 
            // txtResults
            // 
            this.txtResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResults.Enabled = false;
            this.txtResults.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResults.Location = new System.Drawing.Point(498, 487);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.Size = new System.Drawing.Size(320, 78);
            this.txtResults.TabIndex = 8;
            // 
            // lblTurn
            // 
            this.lblTurn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTurn.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTurn.Location = new System.Drawing.Point(650, 568);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(168, 36);
            this.lblTurn.TabIndex = 9;
            this.lblTurn.Text = "0/400";
            this.lblTurn.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.Location = new System.Drawing.Point(492, 568);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(72, 36);
            this.lblScore.TabIndex = 10;
            this.lblScore.Text = "0/0";
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(698, 645);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(58, 40);
            this.btnUp.TabIndex = 11;
            this.btnUp.Text = "Up &I";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(698, 691);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(58, 40);
            this.btnDown.TabIndex = 12;
            this.btnDown.Text = "Down &K";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(639, 691);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(53, 40);
            this.btnLeft.TabIndex = 13;
            this.btnLeft.Text = "Left &J";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(762, 691);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(55, 40);
            this.btnRight.TabIndex = 14;
            this.btnRight.Text = "Right &L";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // txtGameState
            // 
            this.txtGameState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGameState.Enabled = false;
            this.txtGameState.Font = new System.Drawing.Font("Courier New", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGameState.Location = new System.Drawing.Point(498, 614);
            this.txtGameState.Multiline = true;
            this.txtGameState.Name = "txtGameState";
            this.txtGameState.ReadOnly = true;
            this.txtGameState.Size = new System.Drawing.Size(135, 117);
            this.txtGameState.TabIndex = 2;
            // 
            // TronWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 775);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblTurn);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.txtMoves);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.txtPlayer);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.picState);
            this.Controls.Add(this.txtGameState);
            this.Controls.Add(this.btnPrev);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1376, 814);
            this.MinimumSize = new System.Drawing.Size(1376, 814);
            this.Name = "TronWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Tron";
            this.Load += new System.EventHandler(this.TronWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picState)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.PictureBox picState;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtPlayer;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.TextBox txtMoves;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.TextBox txtGameState;
    }
}

