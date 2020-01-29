using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    class Chat
    {
        private List<string> chatLog = new List<string>();
        public Chat()
        {
            AddBotMessage("Привет, сыграем? Будешь ходить крестиками или ноликами?");
        }
        private void AddBotMessage(string text)
        {
            chatLog.Add("Бот: " + text);
        }
        public void SendPlayerMessage(string text)
        {
            chatLog.Add("Игрок: " + text);
            AnalyzePlayerMessage(text);
        }
        private void AnalyzePlayerMessage(string text)
        {
            bool isUnderstand = false;
            var matchX = Regex.Match(text, @"крест|^x$|^х$", RegexOptions.IgnoreCase);
            if(matchX.Success)
            {
                isUnderstand = true;
                AddBotMessage("Отлично! Тогда я будут играть ноликами.");
                AddBotMessage("Ваш ход.");
            }
            var matchO = Regex.Match(text, @"нолик|^o$|^о$", RegexOptions.IgnoreCase);
            if (matchO.Success)
            {
                isUnderstand = true;
                AddBotMessage("Отлично! Тогда я будут играть крестиками.");
                AddBotMessage("Мой ход.");
            }
            if(!isUnderstand)
            {
                AddBotMessage("Я вас не понял, повторите еще раз.");
            }
        }
        public List<string> GetChatLog()
        {
            return chatLog;
        }
    }
}
