using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    public class ChatResponse
    {
        public Utils.GameAction gameAction;
        public string payload;

        public ChatResponse(Utils.GameAction gameAction, string payload)
        {
            this.gameAction = gameAction;
            this.payload = payload;
        }
        public ChatResponse(Utils.GameAction gameAction)
        {
            this.gameAction = gameAction;
            this.payload = "";
        }
    }
    public static class Utils
    {
        public enum GameAction
        {
            HumanChooseX,
            HumanChooseO,
            HumanChooseS,
            HumanChooseSM,
            None,
            Unknown
        }
        public enum GameEnd
        {
            None,
            PlayerXWin,
            PlayerOWin,
            Draw,
            Unknown
        }
        public enum PlayerSide
        {
            X,
            O
        }
    }
}
