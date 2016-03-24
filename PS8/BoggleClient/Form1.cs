﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    /// <summary>
    /// Main game window form
    /// </summary>
    public partial class Form1 : Form, GameInterface
    {
        /// <summary>
        /// Constructs the form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            cancelGameRequestButton.Visible = false;
        }
        /// <summary>
        /// Fires the CreateGameEvent
        /// </summary>
        public event Action<string, string, string> CreateGameEvent;
        /// <summary>
        /// Fires cancelGameEvent
        /// </summary>
        public event Action CancelGameEvent;
        /// <summary>
        /// Fires WordEnteredEvent
        /// </summary>
        public event Action<string> WordEnteredEvent;
        /// <summary>
        /// Fires EndCancelEvent
        /// </summary>
        public event Action EndCancelEvent;
        /// <summary>
        /// Player1's name string
        /// </summary>
        public string Player1Name
        {
            get
            {
                return player1NameBox.Text;
            }
            set
            {
                player1NameBox.Text = value;
            }
        }
        /// <summary>
        /// Player2's name string
        /// </summary>
        public string Player2Name
        {
            get
            {
                return player2NameBox.Text;
            }
            set
            {
                player2NameBox.Text = value;
            }
        }
        /// <summary>
        /// Player1's score string
        /// </summary>
        public string Player1Score
        {
            get
            {
                return player1ScoreBox.Text;
            }
            set
            {
                player1ScoreBox.Text = value;
            }
        }
        /// <summary>
        /// Player2's scrore string
        /// </summary>
        public string Player2Score
        {
            get
            {
                return player2ScoreBox.Text;
            }
            set
            {
                player2ScoreBox.Text = value;
            }
        }
        /// <summary>
        /// Message to be display in Message boxes, string
        /// </summary>
        public string Message
        {
            set
            {
                MessageBox.Show(value);
            }
        }
        /// <summary>
        /// Cancel button boolean, true is canelButton is pressed
        /// </summary>
        public bool cancelbutton
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                cancelGameRequestButton.Visible = value;
            }
        }
        /// <summary>
        /// char array of the board, holds the 16 character long string that is broken up and
        /// represents the letters of each of the tiles. Adjusts the Q tile to be QI, per specification
        /// </summary>
        public char[] Board
        {
            set
            {
                int i = 0;
                Button[] buttons = new Button[] { boardButton0, boardButton1, boardButton2, boardButton3, boardButton4, boardButton5, boardButton6, boardButton7, boardButton8,
                    boardButton9,boardButton10,boardButton11,boardButton12,boardButton13,boardButton14,boardButton15};
                foreach (char letter in value)
                {
                    if (letter == 'Q')
                    {
                        buttons[i].Text = letter.ToString() + "U";
                    }
                    else {
                        buttons[i].Text = letter.ToString();
                    }
                    i++;
                }
            }
        }
        /// <summary>
        /// Timer string
        /// </summary>
        public string Timer
        {
            set
            {
                timeBox1.Text = value;
            }
        }
        /// <summary>
        /// End Request button bool, true if endRequestButton is depressed
        /// </summary>
        public bool EndRequestButton
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                CancelRequestlButton.Visible = value;
            }
        }
        /// <summary>
        /// Cancellation button Text
        /// </summary>
        public string cancelbuttonText
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                cancelGameRequestButton.Text = value;
            }
        }
        /// <summary>
        /// Resets focus to the word input box (after a game starts for example)
        /// </summary>
        public void WordFocus()
        {
            wordInputBox.Focus();
        }
        /// <summary>
        /// Prompts user for nickname, timelimit, and server address they wish to connect to,
        /// passes those three strings are parameters through CreateGameEvent, which calls CreateGameHandler
        /// in the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createGameMenuButton_Click(object sender, EventArgs e)
        {
            string nickname, address, clock;
            bool enterGame = true;

            DialogResult result = Prompt.GameCreateDialogue(out nickname, out clock, out address);

            if(clock == "")
            {
                clock = "0";
            }

            if((Int32.Parse(clock) < 5 || Int32.Parse(clock) > 120) && result == DialogResult.OK)
            {
                MessageBox.Show("Parameters are empty or incorrect");
                enterGame = false;
            }

            if(CreateGameEvent != null && result == DialogResult.OK && enterGame == true)
            {
                CreateGameEvent(nickname, clock, address);
                cancelGameRequestButton.Visible = true;
            }
        }
        /// <summary>
        /// Calls the CancelGameEvent, attempts to close down a pending game request task in the BoggleAPI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelGameRequestButton_Click(object sender, EventArgs e)
        {
            if(CancelGameEvent != null)
            {
                CancelGameEvent();
            }
        }
        /// <summary>
        /// Closes the current form/game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeMenuButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Method to gracefully end the currently running game w/o closing the form. 
        /// </summary>
        /// <param name="Player1List"></param>
        /// <param name="Player2List"></param>
        public void EndGame(List<string> Player1List, List<string> Player2List)
        {
            Prompt.EndGameWindow(Player1Name, Player2Name, Player1Score, Player2Score, Player1List, Player2List);
            ResetBoard();
        }
        /// <summary>
        /// Submits the currently typed in word in the submission box. Fires the WordEnteredEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterButton_Click(object sender, EventArgs e)
        {
            if(WordEnteredEvent != null)
            {
                WordEnteredEvent(wordInputBox.Text);
                wordInputBox.Clear();

            }
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = "";
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Allows user to submit a word through the text box by pressing Enter on their keyboard, rather than the enter button in the GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wordInputBox_TextChanged(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (WordEnteredEvent != null)
                {
                    WordEnteredEvent(wordInputBox.Text);
                    wordInputBox.Clear();
                }
            }
        }
        /// <summary>
        /// Clears the board after a game
        /// </summary>
        public void ResetBoard()
        {
            player1NameBox.Clear();
            player2NameBox.Clear();
            player1ScoreBox.Clear();
            player2ScoreBox.Clear();
            timeBox1.Clear();
            Board = new Char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        }
        /// <summary>
        /// Fires cancellation request to controller to attempt to end pending game request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelRequestlButton_Click(object sender, EventArgs e)
        {
            if (EndCancelEvent != null)
            {
                EndCancelEvent();
            }
        }
        /// <summary>
        /// Updates the players' scores and game time via a call from the controller, based on pings
        /// to the server/API
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        public void boardScoreUpdate(string v1, string v2, string v3)
        {
            player1ScoreBox.Text = v1;
            player2ScoreBox.Text = v2;
            timeBox1.Text = v3;
        }
        /// <summary>
        /// *********************************NEXT 16 FUNCTIONS CORRESPOND TO THE 16 TILES ON THE BOARD**********************
        /// Sets the tile 16 tiles be to be their corresponding letter in the char array of the boardState.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton0_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton0.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton1_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton1.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton2_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton2.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton3_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton3.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton4_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton4.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton5_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton5.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton6_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton6.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton7_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton7.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton8_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton8.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton9_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton9.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton10_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton10.Text;
            wordInputBox.Refresh();
        } 
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton11_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton11.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton12_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton12.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton13_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton13.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton14_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton14.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Sets tile to corresponding letter in char array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardButton15_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton15.Text;
            wordInputBox.Refresh();
        }
        /// <summary>
        /// Help Menu Text for end user 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpMenuButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Boggle\n"
                + "==============================================\n"
                + "How to Play\n"
                + "Press Game > Play Game\n"
                + "Enter in a nickname, desired game length in seconds, and server\n"
                + "Wait for a game to begin (you can cancel waiting for a game from the bottom right of the client)\n"
                + "==============================================\n"
                + "Rules\n"
                + "Either click on the tiles or type in a word that can be made from the tiles\n"
                + "Each tile can only be used once per word, and each next tile must 'touch' the previous one\n"
                + "Two tiles touch when they are above, below, left, right, or diagonally from one another\n"
                + "When time runs out, the player with the highest score wins!\n"
                + "Invalid words cause you to lose a point, so be careful!\n"
                + "Created by Ryan Steele and Henry Kucab\n");
        }
    }
    /// <summary>
    /// Prompt class allows for custom dialogue boxes to be made, such as the End of Game results dialog and Create Game Dialogue. Most of them
    /// pass their outputs via outs back to the GUI calls that create the prompt. Useful for passing multiple pieces of data, such as the username, clocks, 
    /// and addresses.
    /// </summary>
    public static class Prompt
    {
        /// <summary>
        /// Creates the "Create a Game" prompt. Asks for the current player's nickname, desired gameLength, and Server address. Returns
        /// DialogResult.OK if fields are not empty and time is positive. Returns DialogResult.Cancel otherwise.
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="gameLength"></param>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        public static DialogResult GameCreateDialogue(out string nickname, out string gameLength, out string serverAddress)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 300,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 30, Text = "Nickname" };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };

            Label textLabel1 = new Label() { Left = 50, Top = 80, Width = 200, Text = "Game Length, between 5-120 seconds" };
            TextBox textBox1 = new TextBox() { Left = 50, Top = 100, Width = 400 };

            Label textLabel2 = new Label() { Left = 50, Top = 130, Text = "Server" };
            Label defaultAddress = new Label() { Left = 50, Top = 170, Width = 500, Text = "(Professor's: http://bogglecs3500s16.azurewebsites.net/BoggleService.svc/)" };
            TextBox textBox2 = new TextBox() { Left = 50, Top = 150, Width = 400 };

            Button confirmation = new Button() { Text = "Search", Left = 350, Width = 100, Top = 210, DialogResult = DialogResult.OK };

            confirmation.Click += (sender, e) => {prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox1);
            prompt.Controls.Add(textLabel1);
            prompt.Controls.Add(textBox2);
            prompt.Controls.Add(textLabel2);
            prompt.Controls.Add(defaultAddress);
            prompt.Controls.Add(confirmation);

            prompt.AcceptButton = confirmation;

            prompt.ShowDialog();

            nickname = textBox.Text;
            gameLength = textBox1.Text;
            serverAddress = textBox2.Text;

            
            double test;
            if (!string.IsNullOrWhiteSpace(nickname) && gameLength != "" && !string.IsNullOrWhiteSpace(serverAddress) && (double.TryParse(gameLength, out test) && test > 0))
            {
                return DialogResult.OK;
            }
            {
                return DialogResult.Cancel;
            }
        }
        /// <summary>
        /// End of Game dialog window. Takes in both players names, their scores, and the list of words they played, which have been modified
        /// to also include the scores of each individual word appended. Displays the results of the game, the final score, and the
        /// lists of words with their respective score breakdowns to both players at the game's end.
        /// </summary>
        /// <param name="Player1Name"></param>
        /// <param name="Player2Name"></param>
        /// <param name="Player1Score"></param>
        /// <param name="Player2Score"></param>
        /// <param name="Player1Words"></param>
        /// <param name="Player2Words"></param>
        public static void EndGameWindow(string Player1Name, string Player2Name, string Player1Score, string Player2Score, List<string> Player1Words, List<string> Player2Words)
        {
            Form prompt = new Form()
            {
                Width = 525,
                Height = 450,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label player1BoxLabel = new Label() { Left = 85, Top = 70 };
            Label player2BoxLabel = new Label() { Left = 340, Top = 70 };
            ListBox player1WordBox = new ListBox() { Left = 50, Top = 90, Width = 150, Height = 250 };
            ListBox player2WordBox = new ListBox() { Left = 300, Top = 90, Width = 150, Height = 250 };

            Label header = new Label() { Left = 210, Top = 10 };
            header.Text = "GAME RESULTS";

            TextBox result = new TextBox() { Left = 50, Top = 40, Width = 400 };
            result.ReadOnly = true;

            if (Int32.Parse(Player1Score) > Int32.Parse(Player2Score))
            {
                result.Text = Player1Name + " beat " + Player2Name + ", " + Player1Score + " to " + Player2Score;
                result.Refresh();
            }
            if (Int32.Parse(Player1Score) < Int32.Parse(Player2Score))
            {
                result.Text = Player2Name + " beat " + Player1Name + ", " + Player2Score + " to " + Player1Score;
                result.Refresh();
            }
            if (Int32.Parse(Player1Score) == Int32.Parse(Player2Score))
            {
                result.Text = Player1Name + " tied " + Player2Name + ", " + Player1Score + " to " + Player2Score;
                result.Refresh();
            }

            player1BoxLabel.Text = "Player 1's Words";
            player2BoxLabel.Text = "Player 2's Words";
            foreach (string i in Player1Words)
            {
                player1WordBox.Items.Add(i);
            }
            foreach(string j in Player2Words)
            {
                player2WordBox.Items.Add(j);
            }

            Button confirmation = new Button() { Text = "Continue", Left = 175, Width = 150, Top = 350 };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(player1WordBox);
            prompt.Controls.Add(player2WordBox);
            prompt.Controls.Add(player1BoxLabel);
            prompt.Controls.Add(player2BoxLabel);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(header);
            prompt.Controls.Add(result);

            prompt.ShowDialog();

        }
    }
}
