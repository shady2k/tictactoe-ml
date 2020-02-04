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
        public bool NeedSync = false;
        Random random = new Random();
        string[] unknownCommand = { "Я вас не понял, повторите еще раз.",
                                    "Хм, здесь что-то на эльфийском, объясните мне другими словами.",
                                    "Не могу понять, что от меня нужно."
        };
        public Chat()
        {
            AddBotMessage("Привет, сыграем?");
        }
        public void AddBotMessage(string text)
        {
            chatLog.Add("Бот: " + text);
            NeedSync = true;
        }
        public void AddPlayerMessage(string text)
        {
            chatLog.Add("Игрок: " + text);
            NeedSync = true;
        }
        public void AddDebugMessage(string text)
        {
            chatLog.Add("DEBUG: " + text);
            NeedSync = true;
        }
        public ChatResponse SendPlayerMessage(string text)
        {
            AddPlayerMessage(text);
            return AnalyzePlayerMessage(text);
        }
        private ChatResponse AnalyzePlayerMessage(string text)
        {
            bool isUnderstand = false;
            var matchX = Regex.Match(text, @"крест|^x$|^х$", RegexOptions.IgnoreCase);
            var matchO = Regex.Match(text, @"нолик|^o$|^о$|^0$", RegexOptions.IgnoreCase);
            var matchS = Regex.Match(text, @"комп|сам|^s", RegexOptions.IgnoreCase);
            var matchHelp = Regex.Match(text, @"помощ|помог|help|^\?$", RegexOptions.IgnoreCase);
            var matchInt = Regex.Match(text, @"\d+", RegexOptions.IgnoreCase);

            if (matchHelp.Success)
            {
                isUnderstand = true;
                AddBotMessage("крестиком, x - выбор крестика");
                AddBotMessage("ноликом, o, 0 - выбор нолика");
                AddBotMessage("компьютер, сам, s - буду играть сам с собой");
                AddBotMessage("помощь, help - отображение этой информации");
                return new ChatResponse(Utils.GameAction.HumanChooseO);
            } else if (matchX.Success)
            {
                isUnderstand = true;
                AddBotMessage("Отлично! Тогда я будут играть ноликами.");
                return new ChatResponse(Utils.GameAction.HumanChooseX);
            } else if (matchO.Success)
            {
                isUnderstand = true;
                AddBotMessage("Отлично! Тогда я будут играть крестиками.");
                return new ChatResponse(Utils.GameAction.HumanChooseO);
            } else if (matchS.Success)
            {
                int val = 1;
                if(matchInt.Success)
                {
                    val = Int32.Parse(matchInt.Value);
                    isUnderstand = true;
                    AddBotMessage("Ок, играю сам с собой " + val.ToString() + " раз.");
                    return new ChatResponse(Utils.GameAction.HumanChooseSM, val.ToString());
                } else
                {
                    isUnderstand = true;
                    AddBotMessage("Ок, играю сам с собой.");
                    return new ChatResponse(Utils.GameAction.HumanChooseS);
                }
            }

            if (!isUnderstand)
            {
                return new ChatResponse(Utils.GameAction.Unknown);
            }

            return new ChatResponse(Utils.GameAction.Unknown);
        }
        public void SendUnknownAnswer()
        {
            AddBotMessage(unknownCommand[random.Next(0, unknownCommand.Length)] + ". Если вам нужна помощь, наберите help или ?");
        }
        public List<string> GetChatLog()
        {
            NeedSync = false;
            return chatLog;
        }
    }
}
