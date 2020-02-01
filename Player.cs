using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    public abstract class Player
    {
        public Utils.PlayerSide side;
        public Player(Utils.PlayerSide side)
        {
            this.side = side;
        }
        public Utils.PlayerSide GetPlayerSide()
        {
            return side;
        }
        public string GetSideSign()
        {
            if (side == Utils.PlayerSide.O) return "O";
            if (side == Utils.PlayerSide.X) return "X";
            return null;
        }
        public bool isHuman()
        {
            return this.GetType().Name == "HumanPlayer";
        }
        
        public abstract bool MakeMove(Game game);
        public abstract bool MakeMove(int x, int y, Game game);
        public abstract void Win();
        public abstract void Loose();
        public abstract void Draw();
    }
}
