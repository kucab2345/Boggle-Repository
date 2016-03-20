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
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
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
    }
}

