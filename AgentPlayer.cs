using System;
using System.Collections.Generic;
using System.Linq;

namespace tictactoe_ml
{
    class State
    {
        public string[] board;
        public double value;

        public State(string[] board, double value) {
            this.board = board;
            this.value = value;
        }
    }
    public class AgentPlayer : Player
    {
        List<State> states = new List<State>();
        Random random = new Random();
        double alpha = 0.1;
        double defaultValue = 0.5;
        double winValue = 1;
        double looseValue = 0;
        double drawValue = 0.5;
        string modelFile = AppDomain.CurrentDomain.BaseDirectory + "model.json";
        string[] lastBoard;

        public AgentPlayer(Utils.PlayerSide side) : base(side)
        {
            LoadStates();
            return;
        }
        public override bool MakeMove(Game game)
        {
            AddNewState(game.GetBoard());
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
        private void AddNewState(string[] board)
        {
            lastBoard = board;
            bool isExists = states.Any(state => state.board.SequenceEqual(board));
            if (!isExists)
            {
                states.Add(new State(board, defaultValue));
            }
        }
        public override void Win()
        {
            CorrectLastStateValue(winValue);
            return;
        }
        public override void Loose()
        {
            CorrectLastStateValue(looseValue);
            return;
        }
        public override void Draw()
        {
            CorrectLastStateValue(drawValue);
            return;
        }
        public override bool MakeMove(int x, int y, Game game)
        {
            return false;
        }
        private void SaveStates()
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(states);
            System.IO.File.WriteAllText(modelFile, jsonString);
        }
        private void LoadStates()
        {
            if (System.IO.File.Exists(modelFile))
            {
                string jsonString = System.IO.File.ReadAllText(modelFile);
                states = Newtonsoft.Json.JsonConvert.DeserializeObject<List<State>>(jsonString);
            }
        }
        private void CorrectLastStateValue(double value)
        {
            var lastState = states.LastOrDefault(state => state.board.SequenceEqual(lastBoard));
            lastState.value = lastState.value + alpha * (value - lastState.value);
            SaveStates();
            return;
        }
        private void GetReward(Utils.GameEnd result)
        {
            if(result == Utils.GameEnd.PlayerXWin && this.GetPlayerSide() == Utils.PlayerSide.X)
            {

            }
            return;
        }
    }
}
