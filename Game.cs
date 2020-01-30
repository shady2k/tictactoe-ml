using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    public class Game
    {
        public enum GameState
        {
            ChooseTurn,
            HumanTurn,
            AgentTurn,
            Agent2Turn,
            CheckAndTransferToHuman,
            CheckAndTransferToAgent,
            CheckAndTransferToAgent2,
            HumanWin,
            AgentWin,
            Agent2Win,
            Draw,
            GameEnd,
            Unknown
        }

        Board board;
        Chat chat;
        GameState gameState;
        Random random = new Random();

        public Game(int frameWidth, int frameHeight)
        {
            board = new Board(frameWidth, frameHeight);
            chat = new Chat();
            ChangeGameState(GameState.ChooseTurn);
        }
        private void ChangeGameState(GameState _gameState)
        {
            gameState = _gameState;
            chat.AddDebugMessage(gameState.ToString());

            switch (gameState)
            {
                case GameState.AgentTurn:
                    chat.AddBotMessage("Мой ход.");
                    MakeAgentMove();
                    ChangeGameState(GameState.CheckAndTransferToHuman);
                    break;
                case GameState.HumanTurn:
                    chat.AddBotMessage("Ваша очередь.");
                    break;
                case GameState.CheckAndTransferToHuman:
                    CheckEnd(GameState.HumanTurn);
                    break;
                case GameState.CheckAndTransferToAgent:
                    CheckEnd(GameState.AgentTurn);
                    break;
                case GameState.ChooseTurn:
                    chat.AddBotMessage("Будешь ходить крестиками или ноликами? А еще я могу поиграть сам с собой.");
                    break;
                case GameState.GameEnd:
                    chat.AddBotMessage("Игра окончена! Еще разок?");
                    ChangeGameState(GameState.ChooseTurn);
                    break;
            }
        }
        private void CheckEnd(GameState _gameState)
        {
            var res = board.CheckEnd();
            if (res == Utils.GameEnd.None)
            {
                ChangeGameState(_gameState);
            }
            else
            {
                if (res == Utils.GameEnd.Player1Win)
                {
                    chat.AddBotMessage("Поздравляю! Вы выиграли!");
                } else if(res == Utils.GameEnd.Draw)
                {
                    chat.AddBotMessage("Отлично сыграли, у нас ничья.");
                } else if (res == Utils.GameEnd.Player2Win)
                {
                    chat.AddBotMessage("Ну ничего, в следующий раз вам повезет.");
                } else
                {
                    chat.AddBotMessage("Произошла какая-то ошибка :(");
                }
                ChangeGameState(GameState.GameEnd);
            }
        }
        private void MakeAgentMove()
        {
            List<int> res = GetPossibleMoves();
            if (res.Count > 0)
            {
                int cell = random.Next(0, res.Count() - 1);
                if (placeAgentSign(res[cell]))
                {
                    chat.AddBotMessage("Делаю ход в ячейку " + (res[cell] + 1).ToString());
                } else
                {
                    chat.AddBotMessage("Я пытаюсь выполнить недопустимый ход в ячейку " + (res[cell] + 1).ToString());
                    chat.AddBotMessage("Что-то пошло не так...");
                    ChangeGameState(GameState.GameEnd);
                }
            } else
            {
                ChangeGameState(GameState.CheckAndTransferToHuman);
            }
        }
        private List<int> GetPossibleMoves()
        {
            return board.GetFreeCells();
        }
        public void HumanChoose(string sign)
        {
            board.GenerateBoard();
            if (sign == "X" && sign == "S")
            {
                board.Player1Sign = "X";
                board.Player2Sign = "O";
                ChangeGameState(GameState.HumanTurn);
            } else if (sign == "O")
            {
                board.Player1Sign = "O";
                board.Player2Sign = "X";
                ChangeGameState(GameState.AgentTurn);
            }
        }
        public bool placeHumanSign(int x, int y)
        {
            if (gameState == GameState.HumanTurn)
            {
                int cell = board.GetCellFromXY(x, y);
                if (board.CheckMove(cell))
                {
                    board.PlaceSign(cell, board.Player1Sign);
                    chat.AddPlayerMessage("Я делаю ход в ячейку " + (cell + 1).ToString());
                    ChangeGameState(GameState.CheckAndTransferToAgent);
                    return true;
                }
            }
            return false;
        }
        private bool placeAgentSign(int cell)
        {
            if (gameState == GameState.AgentTurn)
            {
                if (board.CheckMove(cell))
                {
                    board.PlaceSign(cell, board.Player2Sign);
                    return true;
                } else
                {
                    return false;
                }
            }
            return false;
        }
        public Utils.GameAction SendPlayerMessage(string text)
        {
            if (gameState == GameState.ChooseTurn)
            {
                return chat.SendPlayerMessage(text);
            } else
            {
                return Utils.GameAction.Unknown;
            }
        }
        public void AddUnknownMessage()
        {
            chat.SendUnknownAnswer();
        }
        public Bitmap GetBoardImage()
        {
            return board.GetBoardImage();
        }
        public bool IsNeedBoardSync()
        {
            return board.NeedSync;
        }
        public List<string> GetChatLog()
        {
            return chat.GetChatLog();
        }
        public bool IsNeedChatSync()
        {
            return chat.NeedSync;
        }
    }
}
