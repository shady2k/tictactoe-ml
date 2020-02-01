using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace tictactoe_ml
{
    public partial class Form1 : Form
    {
        private Game game;
        private System.Timers.Timer mainTimer;
        public Form1()
        {
            InitializeComponent();
            game = new Game(pictureBox1.Width, pictureBox1.Height);

            tbChat.Text = "Введите сообщение";
            tbChat.GotFocus += new EventHandler(HidePlaceholder);
            tbChat.LostFocus += new EventHandler(ShowPlaceholder);
            GenerateBoard();
            UpdateChatUI();
            this.ActiveControl = tbChat;
        }
        private void SetTimer()
        {
            mainTimer = new System.Timers.Timer(250);
            mainTimer.Elapsed += OnTimedEvent;
            mainTimer.AutoReset = true;
            mainTimer.Enabled = true;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    UpdateChatUI();
                    UpdateBoardUI();
                }
                ));
            } catch { }
        }
        public void HidePlaceholder(object sender, EventArgs e)
        {
            if (tbChat.Text == "Введите сообщение")
            {
                tbChat.Text = "";
            }
        }
        private void UpdateChatUI()
        {
            if (game.IsNeedChatSync()) {
                tbLog.Text = String.Join(Environment.NewLine, game.GetChatLog());
                tbLog.SelectionStart = tbLog.Text.Length;
                tbLog.ScrollToCaret();
            }
        }
        private void UpdateBoardUI()
        {
            if(game.IsNeedBoardSync())
            {
                pictureBox1.Image = game.GetBoardImage();
            }
        }
        public void ShowPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbChat.Text))
            {
                tbChat.Text = "Введите сообщение";
            }
            tbChat.Focus();
        }
        private void GenerateBoard()
        {
            pictureBox1.Image = game.GetBoardImage();
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            game.HumanMakeMove(e.X, e.Y);
        }
        private void SendPlayerMessage()
        {
            if (!string.IsNullOrWhiteSpace(tbChat.Text) && tbChat.Text != "Введите сообщение")
            {
                var resp = game.SendPlayerMessage(tbChat.Text);
                var ga = resp.gameAction;
                switch (ga) {
                    case Utils.GameAction.HumanChooseX: 
                        game.HumanChoose("X");
                        break;
                    case Utils.GameAction.HumanChooseO:
                        game.HumanChoose("O");
                        break;
                    case Utils.GameAction.HumanChooseS:
                        game.HumanChoose("S");
                        break;
                    case Utils.GameAction.HumanChooseSM:
                        game.SetIterations(Int32.Parse(resp.payload));
                        game.HumanChoose("SM");
                        break;
                    case Utils.GameAction.Unknown:
                        game.SendUnknownAnswer();
                        break;
                }

                tbChat.Text = "";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SendPlayerMessage();
        }

        private void tbChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendPlayerMessage();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetTimer();
        }
    }
}
