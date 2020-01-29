using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tictactoe_ml
{
    public partial class Form1 : Form
    {
        Board board;
        Chat chat;
        public Form1()
        {
            InitializeComponent();
            tbChat.Text = "Введите сообщение";
            tbChat.GotFocus += new EventHandler(HidePlaceholder);
            tbChat.LostFocus += new EventHandler(ShowPlaceholder);
            GenerateBoard();
            chat = new Chat();
            UpdateChatUI();
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
            tbLog.Text = String.Join(Environment.NewLine, chat.GetChatLog());
        }
        public void ShowPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbChat.Text))
            {
                tbChat.Text = "Введите сообщение";
            }
        }
        private void GenerateBoard()
        {
            board = new Board(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = board.GetBoardImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            board.PlaceO(e.X, e.Y);
            pictureBox1.Image = board.GetBoardImage();
        }
        private void SendPlayerMessage()
        {
            if (!string.IsNullOrWhiteSpace(tbChat.Text) && tbChat.Text != "Введите сообщение")
            {
                chat.SendPlayerMessage(tbChat.Text);
                tbChat.Text = "";
                UpdateChatUI();
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
    }
}
