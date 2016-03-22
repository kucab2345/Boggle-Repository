using System;
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

        public event Action<string> WordEnteredEvent;

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

        public string Message
        {
            set
            {
                MessageBox.Show(value);
            }
        }

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

        public string Timer
        {
            set
            {
                timeBox1.Text = value;
            }
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
            string nickname, clock, address;

            DialogResult result = Prompt.GameCreateDialogue(out nickname, out clock, out address);

            if(CreateGameEvent != null && result == DialogResult.OK)
            {
                CreateGameEvent(nickname, clock, address);
                cancelGameRequestButton.Visible = true;
            }
        }

        private void cancelGameRequestButton_Click(object sender, EventArgs e)
        {
            if(CancelGameEvent != null)
            {
                CancelGameEvent();
            }
        }
        private void closeMenuButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void enterButton_Click(object sender, EventArgs e)
        {
            if(WordEnteredEvent != null)
            {
                WordEnteredEvent(wordInputBox.Text);
                wordInputBox.Text = "";
            }
        }

        private void boardButton0_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton0.Text;
            wordInputBox.Refresh();
        }

        private void boardButton1_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton1.Text;
            wordInputBox.Refresh();
        }

        private void boardButton2_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton2.Text;
            wordInputBox.Refresh();
        }

        private void boardButton3_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton3.Text;
            wordInputBox.Refresh();
        }

        private void boardButton4_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton4.Text;
            wordInputBox.Refresh();
        }

        private void boardButton5_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton5.Text;
            wordInputBox.Refresh();
        }

        private void boardButton6_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton6.Text;
            wordInputBox.Refresh();
        }

        private void boardButton7_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton7.Text;
            wordInputBox.Refresh();
        }

        private void boardButton8_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton8.Text;
            wordInputBox.Refresh();
        }

        private void boardButton9_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton9.Text;
            wordInputBox.Refresh();
        }

        private void boardButton10_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton10.Text;
            wordInputBox.Refresh();
        }

        private void boardButton11_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton11.Text;
            wordInputBox.Refresh();
        }

        private void boardButton12_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton12.Text;
            wordInputBox.Refresh();
        }

        private void boardButton13_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton13.Text;
            wordInputBox.Refresh();
        }

        private void boardButton14_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton14.Text;
            wordInputBox.Refresh();
        }

        private void boardButton15_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = wordInputBox.Text + boardButton15.Text;
            wordInputBox.Refresh();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            wordInputBox.Text = "";
            wordInputBox.Refresh();
        }
    }
    public static class Prompt
    {
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

            Label textLabel1 = new Label() { Left = 50, Top = 80, Text = "Game Length" };
            TextBox textBox1 = new TextBox() { Left = 50, Top = 100, Width = 400 };

            Label textLabel2 = new Label() { Left = 50, Top = 130, Text = "Server" };
            TextBox textBox2 = new TextBox() { Left = 50, Top = 150, Width = 400 };

            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 200, DialogResult = DialogResult.OK };

            confirmation.Click += (sender, e) => {prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox1);
            prompt.Controls.Add(textLabel1);
            prompt.Controls.Add(textBox2);
            prompt.Controls.Add(textLabel2);
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
    }
}
