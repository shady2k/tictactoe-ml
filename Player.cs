﻿using System;
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
        public Utils.PlayerSide GetSide()
        {
            return side;
        }
        public string GetSideSign()
        {
            if (side == Utils.PlayerSide.O) return "O";
            if (side == Utils.PlayerSide.X) return "X";
            return null;
        }
        public string GetSign()
        {
            return "X";
        }
        public abstract bool MakeMove(Game game);
        public abstract bool MakeMove(int x, int y, Game game);
    }
}