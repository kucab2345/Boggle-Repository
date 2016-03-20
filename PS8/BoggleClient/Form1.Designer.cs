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
            this.findGameMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.createUserMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.player1NameBox = new System.Windows.Forms.TextBox();
            this.player2NameBox = new System.Windows.Forms.TextBox();
            this.player1NameLabel = new System.Windows.Forms.Label();
            this.player2NameLabel = new System.Windows.Forms.Label();
            this.player1ScoreBox = new System.Windows.Forms.TextBox();
            this.player2ScoreBox = new System.Windows.Forms.TextBox();
            this.player1ScoreLabel = new System.Windows.Forms.Label();
            this.player2ScoreLabel = new System.Windows.Forms.Label();
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
            this.menuStrip1.Size = new System.Drawing.Size(618, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // gameMenuButton
            // 
            this.gameMenuButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findGameMenuButton,
            this.createUserMenuButton});
            this.gameMenuButton.Name = "gameMenuButton";
            this.gameMenuButton.Size = new System.Drawing.Size(50, 20);
            this.gameMenuButton.Text = "Game";
            // 
            // findGameMenuButton
            // 
            this.findGameMenuButton.Name = "findGameMenuButton";
            this.findGameMenuButton.Size = new System.Drawing.Size(152, 22);
            this.findGameMenuButton.Text = "Find Game";
            // 
            // createUserMenuButton
            // 
            this.createUserMenuButton.Name = "createUserMenuButton";
            this.createUserMenuButton.Size = new System.Drawing.Size(152, 22);
            this.createUserMenuButton.Text = "Create User";
            // 
            // helpMenuButton
            // 
            this.helpMenuButton.Name = "helpMenuButton";
            this.helpMenuButton.Size = new System.Drawing.Size(44, 20);
            this.helpMenuButton.Text = "Help";
            // 
            // player1NameBox
            // 
            this.player1NameBox.Location = new System.Drawing.Point(171, 45);
            this.player1NameBox.Name = "player1NameBox";
            this.player1NameBox.Size = new System.Drawing.Size(100, 20);
            this.player1NameBox.TabIndex = 1;
            // 
            // player2NameBox
            // 
            this.player2NameBox.Location = new System.Drawing.Point(296, 44);
            this.player2NameBox.Name = "player2NameBox";
            this.player2NameBox.Size = new System.Drawing.Size(100, 20);
            this.player2NameBox.TabIndex = 2;
            // 
            // player1NameLabel
            // 
            this.player1NameLabel.AutoSize = true;
            this.player1NameLabel.Location = new System.Drawing.Point(117, 47);
            this.player1NameLabel.Name = "player1NameLabel";
            this.player1NameLabel.Size = new System.Drawing.Size(48, 13);
            this.player1NameLabel.TabIndex = 3;
            this.player1NameLabel.Text = "Player 1 ";
            // 
            // player2NameLabel
            // 
            this.player2NameLabel.AutoSize = true;
            this.player2NameLabel.Location = new System.Drawing.Point(403, 45);
            this.player2NameLabel.Name = "player2NameLabel";
            this.player2NameLabel.Size = new System.Drawing.Size(45, 13);
            this.player2NameLabel.TabIndex = 4;
            this.player2NameLabel.Text = "Player 2";
            // 
            // player1ScoreBox
            // 
            this.player1ScoreBox.Location = new System.Drawing.Point(171, 72);
            this.player1ScoreBox.Name = "player1ScoreBox";
            this.player1ScoreBox.Size = new System.Drawing.Size(100, 20);
            this.player1ScoreBox.TabIndex = 5;
            // 
            // player2ScoreBox
            // 
            this.player2ScoreBox.Location = new System.Drawing.Point(296, 71);
            this.player2ScoreBox.Name = "player2ScoreBox";
            this.player2ScoreBox.Size = new System.Drawing.Size(100, 20);
            this.player2ScoreBox.TabIndex = 6;
            // 
            // player1ScoreLabel
            // 
            this.player1ScoreLabel.AutoSize = true;
            this.player1ScoreLabel.Location = new System.Drawing.Point(120, 78);
            this.player1ScoreLabel.Name = "player1ScoreLabel";
            this.player1ScoreLabel.Size = new System.Drawing.Size(35, 13);
            this.player1ScoreLabel.TabIndex = 7;
            this.player1ScoreLabel.Text = "Score";
            // 
            // player2ScoreLabel
            // 
            this.player2ScoreLabel.AutoSize = true;
            this.player2ScoreLabel.Location = new System.Drawing.Point(403, 74);
            this.player2ScoreLabel.Name = "player2ScoreLabel";
            this.player2ScoreLabel.Size = new System.Drawing.Size(35, 13);
            this.player2ScoreLabel.TabIndex = 8;
            this.player2ScoreLabel.Text = "Score";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 413);
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
        private System.Windows.Forms.ToolStripMenuItem findGameMenuButton;
        private System.Windows.Forms.ToolStripMenuItem createUserMenuButton;
        private System.Windows.Forms.ToolStripMenuItem helpMenuButton;
        private System.Windows.Forms.TextBox player1NameBox;
        private System.Windows.Forms.TextBox player2NameBox;
        private System.Windows.Forms.Label player1NameLabel;
        private System.Windows.Forms.Label player2NameLabel;
        private System.Windows.Forms.TextBox player1ScoreBox;
        private System.Windows.Forms.TextBox player2ScoreBox;
        private System.Windows.Forms.Label player1ScoreLabel;
        private System.Windows.Forms.Label player2ScoreLabel;
    }
}

