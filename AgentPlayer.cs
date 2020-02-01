using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace tictactoe_ml
{
    [SerializableAttribute]
    class State
    {
        public string[] board;
        public double value;

        public State(string[] board, double value) {
            this.board = board;
            this.value = value;
        }
    }
    class Reward
    {
        public int cell;
        public double value;

        public Reward(int cell, double value)
        {
            this.cell = cell;
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
        double probingProbability = 0.3;
        string modelFile = AppDomain.CurrentDomain.BaseDirectory + "model.json";
        string[] lastBoard;

        public AgentPlayer(Utils.PlayerSide side) : base(side)
        {
            LoadStates();
            return;
        }
        public override bool MakeMove(Game game)
        {
            List<int> res = game.GetPossibleMoves();
            if (res.Count > 0)
            {
                int cell = -1;
                double rn = random.NextDouble();
                if (rn <= probingProbability)
                {
                    game.AddDebugMessage("Зондирующий ход, вероятность " + rn.ToString());
                    cell = RandomMove(game);
                } else
                {
                    game.AddDebugMessage("Жадный ход, вероятность " + rn.ToString());
                    cell = TDLMove(game);
                }               

                game.PlaceSign(cell, this.GetSideSign());
                game.AddBotMessage("Делаю ход в ячейку " + (cell + 1).ToString());

                AddNewState(game.GetBoard());
                lastBoard = game.GetBoard();
                return true;
            }

            return false;
        }
        private int RandomMove(Game game)
        {
            List<int> res = game.GetPossibleMoves();
            int cell = random.Next(0, res.Count() - 1);
            return res[cell];
        }
        //Temporal difference learning
        private int TDLMove(Game game)
        {
            List<Reward> rewards = new List<Reward>();
            List<int> possibleMoves = game.GetPossibleMoves();
            foreach (int cell in possibleMoves)
            {
                string[] board = game.GetBoard();
                board[cell] = this.GetSideSign();
                double rewardValue = GetReward(board);
                rewards.Add(new Reward(cell, rewardValue));
            }
            var maxRewards = rewards.OrderByDescending(item => item.value).GroupBy(foo => foo.value).Take(1).SelectMany(foo => foo);

            string txt = "У меня на выбор:";
            foreach (var r in maxRewards)
            {
                txt += "\r\n" + "Ячейка " + (r.cell+1).ToString() + " = " + r.value;
            }
            game.AddDebugMessage(txt);

            Reward reward = maxRewards.OrderBy(i => random.Next()).Take(1).FirstOrDefault();

            CorrectLastStateValue(reward.value);

            return reward.cell;
        }
        private double GetReward(string[] board)
        {
            State state = null;
            state = states.FirstOrDefault(s => s.board.SequenceEqual(board));
            if(state != null)
            {
                return Math.Round(state.value, 2);
            }
            return defaultValue;
        }
        private void AddNewState(string[] board)
        {
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
        public static T DeepClone<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
        private void SaveStates()
        {
            List<State> tempStates = DeepClone(states);
            if (this.GetPlayerSide() == Utils.PlayerSide.O)
            {
                foreach(var item in tempStates)
                {
                    for(int i = 0; i < item.board.Length; i++)
                    {
                        if (item.board[i] == "X")
                        {
                            item.board[i] = "O";
                        }
                        else if(item.board[i] == "O")
                        {
                            item.board[i] = "X";
                        }
                    }
                }
            }

            if (System.IO.File.Exists(modelFile))
            {
                string jsonStringSync = System.IO.File.ReadAllText(modelFile);
                List<State> syncStates = Newtonsoft.Json.JsonConvert.DeserializeObject<List<State>>(jsonStringSync);

                foreach (var ss in syncStates) {
                    bool isExists = tempStates.Any(state => state.board.SequenceEqual(ss.board));
                    if (!isExists)
                    {
                        tempStates.Add(new State(ss.board, ss.value));
                    }
                }
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(tempStates);
            System.IO.File.WriteAllText(modelFile, jsonString);
        }
        private void LoadStates()
        {
            if (System.IO.File.Exists(modelFile))
            {
                string jsonString = System.IO.File.ReadAllText(modelFile);
                List<State> tempStates = Newtonsoft.Json.JsonConvert.DeserializeObject<List<State>>(jsonString);

                if (this.GetPlayerSide() == Utils.PlayerSide.O)
                {
                    foreach (var item in tempStates)
                    {
                        for (int i = 0; i < item.board.Length; i++)
                        {
                            if (item.board[i] == "X")
                            {
                                item.board[i] = "O";
                            }
                            else if (item.board[i] == "O")
                            {
                                item.board[i] = "X";
                            }
                        }
                    }
                }
                
                states = DeepClone(tempStates);
            }
        }
        private void CorrectLastStateValue(double value)
        {
            if (lastBoard != null)
            {
                var lastState = states.FirstOrDefault(state => state.board.SequenceEqual(lastBoard));
                if (lastState != null)
                {
                    var newValue = Math.Round(lastState.value + alpha * (value - lastState.value), 2);
                    lastState.value = newValue;
                    if(newValue != 0.5) Console.WriteLine(newValue);
                    SaveStates();
                }
            }
            return;
        }
    }
}
