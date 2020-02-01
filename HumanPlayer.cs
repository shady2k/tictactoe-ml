namespace tictactoe_ml
{
    class HumanPlayer : Player
    {
        public HumanPlayer(Utils.PlayerSide side) : base(side)
        {

        }
        public override bool MakeMove(Game game)
        {
            return false;
        }
        public override bool MakeMove(int x, int y, Game game)
        {
            int cell = game.GetBoardCellFromXY(x, y);
            if (game.CheckMove(cell))
            {
                game.PlaceSign(cell, this.GetSideSign());
                game.AddPlayerMessage("Я делаю ход в ячейку " + (cell + 1).ToString());
                return true;
            }
            return false;
        }
        public override void Win()
        {
            return;
        }
        public override void Loose()
        {
            return;
        }
        public override void Draw()
        {
            return;
        }
    }
}
