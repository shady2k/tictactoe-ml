using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    public static class Utils
    {
        public enum GameAction
        {
            HumanChooseX,
            HumanChooseO,
            HumanChooseS,
            Unknown
        }
        public enum GameEnd
        {
            None,
            Player1Win,
            Player2Win,
            Draw,
            Unknown
        }
    }
}
