using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoggleClient;

namespace BoggleClient
{
    public partial class Form1 : Form , GameInterface
    {
        /// <summary>
        /// Constructs the form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Fires the CreateGameEvent
        /// </summary>
        public event Action<string, string, string> CreateGameEvent;

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
            Prompt.GameCreateDialogue(out nickname, out clock, out address);

            if(CreateGameEvent != null)
            {
                CreateGameEvent(nickname, clock, address);
            }
        }
    }
    public static class Prompt
    {
        public static bool GameCreateDialogue(out string nickname, out string gameLength, out string serverAddress)
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

            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox1);
            prompt.Controls.Add(textLabel1);
            prompt.Controls.Add(textBox2);
            prompt.Controls.Add(textLabel2);

            prompt.AcceptButton = confirmation;

            prompt.ShowDialog();
            nickname = textBox.Text;
            gameLength = textBox1.Text;
            serverAddress = textBox2.Text;

            return true;
        }
    }
}
