namespace BoggleClient
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.gameMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.createGameMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.player1NameBox = new System.Windows.Forms.TextBox();
            this.player2NameBox = new System.Windows.Forms.TextBox();
            this.player1NameLabel = new System.Windows.Forms.Label();
            this.player2NameLabel = new System.Windows.Forms.Label();
            this.player1ScoreBox = new System.Windows.Forms.TextBox();
            this.player2ScoreBox = new System.Windows.Forms.TextBox();
            this.player1ScoreLabel = new System.Windows.Forms.Label();
            this.player2ScoreLabel = new System.Windows.Forms.Label();
            this.boardButton1 = new System.Windows.Forms.Button();
            this.boardButton2 = new System.Windows.Forms.Button();
            this.boardButton3 = new System.Windows.Forms.Button();
            this.boardButton0 = new System.Windows.Forms.Button();
            this.boardButton4 = new System.Windows.Forms.Button();
            this.boardButton7 = new System.Windows.Forms.Button();
            this.boardButton6 = new System.Windows.Forms.Button();
            this.boardButton5 = new System.Windows.Forms.Button();
            this.boardButton12 = new System.Windows.Forms.Button();
            this.boardButton15 = new System.Windows.Forms.Button();
            this.boardButton14 = new System.Windows.Forms.Button();
            this.boardButton13 = new System.Windows.Forms.Button();
            this.boardButton8 = new System.Windows.Forms.Button();
            this.boardButton11 = new System.Windows.Forms.Button();
            this.boardButton10 = new System.Windows.Forms.Button();
            this.boardButton9 = new System.Windows.Forms.Button();
            this.wordInputBox = new System.Windows.Forms.TextBox();
            this.wordSubmissionLabel = new System.Windows.Forms.Label();
            this.enterButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.timeBox1 = new System.Windows.Forms.TextBox();
            this.timeLabel = new System.Windows.Forms.Label();
            this.cancelGameRequestButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameMenuButton,
            this.helpMenuButton});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(478, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // gameMenuButton
            // 
            this.gameMenuButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createGameMenuButton,
            this.closeMenuButton});
            this.gameMenuButton.Name = "gameMenuButton";
            this.gameMenuButton.Size = new System.Drawing.Size(50, 20);
            this.gameMenuButton.Text = "Game";
            // 
            // createGameMenuButton
            // 
            this.createGameMenuButton.Name = "createGameMenuButton";
            this.createGameMenuButton.Size = new System.Drawing.Size(152, 22);
            this.createGameMenuButton.Text = "Play Game";
            this.createGameMenuButton.Click += new System.EventHandler(this.createGameMenuButton_Click);
            // 
            // closeMenuButton
            // 
            this.closeMenuButton.Name = "closeMenuButton";
            this.closeMenuButton.Size = new System.Drawing.Size(142, 22);
            this.closeMenuButton.Text = "Close";
            // 
            // helpMenuButton
            // 
            this.helpMenuButton.Name = "helpMenuButton";
            this.helpMenuButton.Size = new System.Drawing.Size(44, 20);
            this.helpMenuButton.Text = "Help";
            // 
            // player1NameBox
            // 
            this.player1NameBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player1NameBox.Location = new System.Drawing.Point(128, 79);
            this.player1NameBox.Name = "player1NameBox";
            this.player1NameBox.ReadOnly = true;
            this.player1NameBox.Size = new System.Drawing.Size(100, 20);
            this.player1NameBox.TabIndex = 1;
            // 
            // player2NameBox
            // 
            this.player2NameBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player2NameBox.Location = new System.Drawing.Point(253, 78);
            this.player2NameBox.Name = "player2NameBox";
            this.player2NameBox.ReadOnly = true;
            this.player2NameBox.Size = new System.Drawing.Size(100, 20);
            this.player2NameBox.TabIndex = 2;
            // 
            // player1NameLabel
            // 
            this.player1NameLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player1NameLabel.AutoSize = true;
            this.player1NameLabel.Location = new System.Drawing.Point(74, 81);
            this.player1NameLabel.Name = "player1NameLabel";
            this.player1NameLabel.Size = new System.Drawing.Size(48, 13);
            this.player1NameLabel.TabIndex = 3;
            this.player1NameLabel.Text = "Player 1 ";
            // 
            // player2NameLabel
            // 
            this.player2NameLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player2NameLabel.AutoSize = true;
            this.player2NameLabel.Location = new System.Drawing.Point(360, 79);
            this.player2NameLabel.Name = "player2NameLabel";
            this.player2NameLabel.Size = new System.Drawing.Size(45, 13);
            this.player2NameLabel.TabIndex = 4;
            this.player2NameLabel.Text = "Player 2";
            // 
            // player1ScoreBox
            // 
            this.player1ScoreBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player1ScoreBox.Location = new System.Drawing.Point(128, 106);
            this.player1ScoreBox.Name = "player1ScoreBox";
            this.player1ScoreBox.ReadOnly = true;
            this.player1ScoreBox.Size = new System.Drawing.Size(100, 20);
            this.player1ScoreBox.TabIndex = 5;
            // 
            // player2ScoreBox
            // 
            this.player2ScoreBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player2ScoreBox.Location = new System.Drawing.Point(253, 105);
            this.player2ScoreBox.Name = "player2ScoreBox";
            this.player2ScoreBox.ReadOnly = true;
            this.player2ScoreBox.Size = new System.Drawing.Size(100, 20);
            this.player2ScoreBox.TabIndex = 6;
            // 
            // player1ScoreLabel
            // 
            this.player1ScoreLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player1ScoreLabel.AutoSize = true;
            this.player1ScoreLabel.Location = new System.Drawing.Point(77, 112);
            this.player1ScoreLabel.Name = "player1ScoreLabel";
            this.player1ScoreLabel.Size = new System.Drawing.Size(35, 13);
            this.player1ScoreLabel.TabIndex = 7;
            this.player1ScoreLabel.Text = "Score";
            // 
            // player2ScoreLabel
            // 
            this.player2ScoreLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.player2ScoreLabel.AutoSize = true;
            this.player2ScoreLabel.Location = new System.Drawing.Point(360, 108);
            this.player2ScoreLabel.Name = "player2ScoreLabel";
            this.player2ScoreLabel.Size = new System.Drawing.Size(35, 13);
            this.player2ScoreLabel.TabIndex = 8;
            this.player2ScoreLabel.Text = "Score";
            // 
            // boardButton1
            // 
            this.boardButton1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton1.Location = new System.Drawing.Point(197, 141);
            this.boardButton1.Name = "boardButton1";
            this.boardButton1.Size = new System.Drawing.Size(39, 33);
            this.boardButton1.TabIndex = 9;
            this.boardButton1.UseVisualStyleBackColor = true;
            // 
            // boardButton2
            // 
            this.boardButton2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton2.Location = new System.Drawing.Point(242, 141);
            this.boardButton2.Name = "boardButton2";
            this.boardButton2.Size = new System.Drawing.Size(39, 33);
            this.boardButton2.TabIndex = 10;
            this.boardButton2.UseVisualStyleBackColor = true;
            // 
            // boardButton3
            // 
            this.boardButton3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton3.Location = new System.Drawing.Point(287, 141);
            this.boardButton3.Name = "boardButton3";
            this.boardButton3.Size = new System.Drawing.Size(39, 33);
            this.boardButton3.TabIndex = 11;
            this.boardButton3.UseVisualStyleBackColor = true;
            // 
            // boardButton0
            // 
            this.boardButton0.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton0.Location = new System.Drawing.Point(152, 141);
            this.boardButton0.Name = "boardButton0";
            this.boardButton0.Size = new System.Drawing.Size(39, 33);
            this.boardButton0.TabIndex = 12;
            this.boardButton0.UseVisualStyleBackColor = true;
            // 
            // boardButton4
            // 
            this.boardButton4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton4.Location = new System.Drawing.Point(152, 180);
            this.boardButton4.Name = "boardButton4";
            this.boardButton4.Size = new System.Drawing.Size(39, 33);
            this.boardButton4.TabIndex = 16;
            this.boardButton4.UseVisualStyleBackColor = true;
            // 
            // boardButton7
            // 
            this.boardButton7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton7.Location = new System.Drawing.Point(287, 180);
            this.boardButton7.Name = "boardButton7";
            this.boardButton7.Size = new System.Drawing.Size(39, 33);
            this.boardButton7.TabIndex = 15;
            this.boardButton7.UseVisualStyleBackColor = true;
            // 
            // boardButton6
            // 
            this.boardButton6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton6.Location = new System.Drawing.Point(242, 180);
            this.boardButton6.Name = "boardButton6";
            this.boardButton6.Size = new System.Drawing.Size(39, 33);
            this.boardButton6.TabIndex = 14;
            this.boardButton6.UseVisualStyleBackColor = true;
            // 
            // boardButton5
            // 
            this.boardButton5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton5.Location = new System.Drawing.Point(197, 180);
            this.boardButton5.Name = "boardButton5";
            this.boardButton5.Size = new System.Drawing.Size(39, 33);
            this.boardButton5.TabIndex = 13;
            this.boardButton5.UseVisualStyleBackColor = true;
            // 
            // boardButton12
            // 
            this.boardButton12.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton12.Location = new System.Drawing.Point(152, 258);
            this.boardButton12.Name = "boardButton12";
            this.boardButton12.Size = new System.Drawing.Size(39, 33);
            this.boardButton12.TabIndex = 24;
            this.boardButton12.UseVisualStyleBackColor = true;
            // 
            // boardButton15
            // 
            this.boardButton15.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton15.Location = new System.Drawing.Point(287, 258);
            this.boardButton15.Name = "boardButton15";
            this.boardButton15.Size = new System.Drawing.Size(39, 33);
            this.boardButton15.TabIndex = 23;
            this.boardButton15.UseVisualStyleBackColor = true;
            // 
            // boardButton14
            // 
            this.boardButton14.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton14.Location = new System.Drawing.Point(242, 258);
            this.boardButton14.Name = "boardButton14";
            this.boardButton14.Size = new System.Drawing.Size(39, 33);
            this.boardButton14.TabIndex = 22;
            this.boardButton14.UseVisualStyleBackColor = true;
            // 
            // boardButton13
            // 
            this.boardButton13.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton13.Location = new System.Drawing.Point(197, 258);
            this.boardButton13.Name = "boardButton13";
            this.boardButton13.Size = new System.Drawing.Size(39, 33);
            this.boardButton13.TabIndex = 21;
            this.boardButton13.UseVisualStyleBackColor = true;
            // 
            // boardButton8
            // 
            this.boardButton8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton8.Location = new System.Drawing.Point(152, 219);
            this.boardButton8.Name = "boardButton8";
            this.boardButton8.Size = new System.Drawing.Size(39, 33);
            this.boardButton8.TabIndex = 20;
            this.boardButton8.UseVisualStyleBackColor = true;
            // 
            // boardButton11
            // 
            this.boardButton11.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton11.Location = new System.Drawing.Point(287, 219);
            this.boardButton11.Name = "boardButton11";
            this.boardButton11.Size = new System.Drawing.Size(39, 33);
            this.boardButton11.TabIndex = 19;
            this.boardButton11.UseVisualStyleBackColor = true;
            // 
            // boardButton10
            // 
            this.boardButton10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton10.Location = new System.Drawing.Point(242, 219);
            this.boardButton10.Name = "boardButton10";
            this.boardButton10.Size = new System.Drawing.Size(39, 33);
            this.boardButton10.TabIndex = 18;
            this.boardButton10.UseVisualStyleBackColor = true;
            // 
            // boardButton9
            // 
            this.boardButton9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boardButton9.Location = new System.Drawing.Point(197, 219);
            this.boardButton9.Name = "boardButton9";
            this.boardButton9.Size = new System.Drawing.Size(39, 33);
            this.boardButton9.TabIndex = 17;
            this.boardButton9.UseVisualStyleBackColor = true;
            // 
            // wordInputBox
            // 
            this.wordInputBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.wordInputBox.Location = new System.Drawing.Point(152, 316);
            this.wordInputBox.Name = "wordInputBox";
            this.wordInputBox.ReadOnly = true;
            this.wordInputBox.Size = new System.Drawing.Size(174, 20);
            this.wordInputBox.TabIndex = 25;
            // 
            // wordSubmissionLabel
            // 
            this.wordSubmissionLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.wordSubmissionLabel.AutoSize = true;
            this.wordSubmissionLabel.Location = new System.Drawing.Point(57, 319);
            this.wordSubmissionLabel.Name = "wordSubmissionLabel";
            this.wordSubmissionLabel.Size = new System.Drawing.Size(89, 13);
            this.wordSubmissionLabel.TabIndex = 26;
            this.wordSubmissionLabel.Text = "Word Submission";
            // 
            // enterButton
            // 
            this.enterButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.enterButton.Location = new System.Drawing.Point(344, 314);
            this.enterButton.Name = "enterButton";
            this.enterButton.Size = new System.Drawing.Size(75, 23);
            this.enterButton.TabIndex = 27;
            this.enterButton.Text = "Enter";
            this.enterButton.UseVisualStyleBackColor = true;
            // 
            // clearButton
            // 
            this.clearButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.clearButton.Location = new System.Drawing.Point(344, 258);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 33);
            this.clearButton.TabIndex = 28;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            // 
            // timeBox1
            // 
            this.timeBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.timeBox1.Location = new System.Drawing.Point(197, 52);
            this.timeBox1.Name = "timeBox1";
            this.timeBox1.ReadOnly = true;
            this.timeBox1.Size = new System.Drawing.Size(84, 20);
            this.timeBox1.TabIndex = 29;
            // 
            // timeLabel
            // 
            this.timeLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(227, 36);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(30, 13);
            this.timeLabel.TabIndex = 31;
            this.timeLabel.Text = "Time";
            // 
            // cancelGameRequestButton
            // 
            this.cancelGameRequestButton.Location = new System.Drawing.Point(315, 378);
            this.cancelGameRequestButton.Name = "cancelGameRequestButton";
            this.cancelGameRequestButton.Size = new System.Drawing.Size(151, 23);
            this.cancelGameRequestButton.TabIndex = 32;
            this.cancelGameRequestButton.Text = "Cancel Pending Game...";
            this.cancelGameRequestButton.UseVisualStyleBackColor = true;
            this.cancelGameRequestButton.Click += new System.EventHandler(this.cancelGameRequestButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 413);
            this.Controls.Add(this.cancelGameRequestButton);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.timeBox1);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.enterButton);
            this.Controls.Add(this.wordSubmissionLabel);
            this.Controls.Add(this.wordInputBox);
            this.Controls.Add(this.boardButton12);
            this.Controls.Add(this.boardButton15);
            this.Controls.Add(this.boardButton14);
            this.Controls.Add(this.boardButton13);
            this.Controls.Add(this.boardButton8);
            this.Controls.Add(this.boardButton11);
            this.Controls.Add(this.boardButton10);
            this.Controls.Add(this.boardButton9);
            this.Controls.Add(this.boardButton4);
            this.Controls.Add(this.boardButton7);
            this.Controls.Add(this.boardButton6);
            this.Controls.Add(this.boardButton5);
            this.Controls.Add(this.boardButton0);
            this.Controls.Add(this.boardButton3);
            this.Controls.Add(this.boardButton2);
            this.Controls.Add(this.boardButton1);
            this.Controls.Add(this.player2ScoreLabel);
            this.Controls.Add(this.player1ScoreLabel);
            this.Controls.Add(this.player2ScoreBox);
            this.Controls.Add(this.player1ScoreBox);
            this.Controls.Add(this.player2NameLabel);
            this.Controls.Add(this.player1NameLabel);
            this.Controls.Add(this.player2NameBox);
            this.Controls.Add(this.player1NameBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Boggle";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem gameMenuButton;
        private System.Windows.Forms.ToolStripMenuItem helpMenuButton;
        private System.Windows.Forms.TextBox player1NameBox;
        private System.Windows.Forms.TextBox player2NameBox;
        private System.Windows.Forms.Label player1NameLabel;
        private System.Windows.Forms.Label player2NameLabel;
        private System.Windows.Forms.TextBox player1ScoreBox;
        private System.Windows.Forms.TextBox player2ScoreBox;
        private System.Windows.Forms.Label player1ScoreLabel;
        private System.Windows.Forms.Label player2ScoreLabel;
        private System.Windows.Forms.Button boardButton1;
        private System.Windows.Forms.Button boardButton2;
        private System.Windows.Forms.Button boardButton3;
        private System.Windows.Forms.Button boardButton0;
        private System.Windows.Forms.Button boardButton4;
        private System.Windows.Forms.Button boardButton7;
        private System.Windows.Forms.Button boardButton6;
        private System.Windows.Forms.Button boardButton5;
        private System.Windows.Forms.Button boardButton12;
        private System.Windows.Forms.Button boardButton15;
        private System.Windows.Forms.Button boardButton14;
        private System.Windows.Forms.Button boardButton13;
        private System.Windows.Forms.Button boardButton8;
        private System.Windows.Forms.Button boardButton11;
        private System.Windows.Forms.Button boardButton10;
        private System.Windows.Forms.Button boardButton9;
        private System.Windows.Forms.ToolStripMenuItem createGameMenuButton;
        private System.Windows.Forms.ToolStripMenuItem closeMenuButton;
        private System.Windows.Forms.TextBox wordInputBox;
        private System.Windows.Forms.Label wordSubmissionLabel;
        private System.Windows.Forms.Button enterButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.TextBox timeBox1;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Button cancelGameRequestButton;
    }
}

