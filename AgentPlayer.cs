using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    public class AgentPlayer : Player
    {
        Random random = new Random();
        public AgentPlayer(Utils.PlayerSide side) : base(side)
        {
            
        }
        public override bool MakeMove(Game game)
        {
            List<int> res = game.GetPossibleMoves();
            if (res.Count > 0)
            {
                int cell = random.Next(0, res.Count() - 1);
                game.PlaceSign(res[cell], this.GetSideSign());
                game.AddBotMessage("Делаю ход в ячейку " + (res[cell] + 1).ToString());
                return true;
            }

            return false;
        }
        public override bool MakeMove(int x, int y, Game game)
        {
            return false;
        }
    }
}
